using UnityEngine; 
using UnityEngine.EventSystems; 
using UnityEngine.UI; 

//------------------------------------------------------------- 
//! ドラッグ＆ドロップ規定クラス 
//------------------------------------------------------------- 
public class goDragBase : MonoBehaviour 
{ 
	public delegate void CallbackFuncPointer( PointerEventData _eventData ); 
	public CallbackFuncPointer m_OnEventPointerBegin; 
	public CallbackFuncPointer m_OnEventPointerEnd; 

	protected Canvas m_canvas; 
	protected bool m_dragOnSurfaces = true; 
	protected GameObject m_draggingIcon = null; 
	protected RectTransform m_draggingPlane = null; 

	//------------------------------------------------------------- 
	//! ドラッグ処理中か？ 
	//------------------------------------------------------------- 
	public bool IsDragging() 
	{ 
		return (m_draggingIcon != null) ? true : false; 
	} 

	//------------------------------------------------------------- 
	//! 親を探す。 
	//------------------------------------------------------------- 
	protected T FindInParents<T>(GameObject _go) where T : Component 
	{ 
		if (_go == null) 
		{ 
			return null; 
		} 
		var comp = _go.GetComponent<T>(); 

		if (comp != null) 
		{ 
			return comp; 
		} 

		Transform t = _go.transform.parent; 
		while (t != null && comp == null) 
		{ 
			comp = t.gameObject.GetComponent<T>(); 
			t = t.parent; 
		} 
		return comp; 
	} 

	//------------------------------------------------------------- 
	//! イメージ作成 
	//------------------------------------------------------------- 
	protected Image createImage(Canvas _canvas, PointerEventData _eventData) 
	{ 
		m_canvas = _canvas; 

		// GameObjectの作成。 
		m_draggingIcon = new GameObject(_eventData.selectedObject.name); 

		// 親子関係設定 
		m_draggingIcon.transform.SetParent(_canvas.transform, false); 

		// 最前面に表示する。 
		m_draggingIcon.transform.SetAsLastSibling(); 

		// イメージ追加 
		Image image = m_draggingIcon.AddComponent<Image>(); 

		// 当たり無効。 
//		m_draggingIcon.AddComponent<IgnoreRaycast>(); 

		// スプライト設定。 
		image.sprite = gameObject.GetComponent<Image>().sprite; 
		image.SetNativeSize(); 

		return image; 
	} 

	//------------------------------------------------------------- 
	//! 座標更新 
	//------------------------------------------------------------- 
	protected void SetDraggedPosition(PointerEventData _eventData) 
	{ 
		if (m_dragOnSurfaces && _eventData.pointerEnter != null && _eventData.pointerEnter.transform as RectTransform != null) 
		{ 
			m_draggingPlane = _eventData.pointerEnter.transform as RectTransform; 
		} 

		RectTransform rt = m_draggingIcon.GetComponent<RectTransform>(); 
		Vector3 globalMousePos; 
		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_draggingPlane, 
			_eventData.position, _eventData.pressEventCamera, out globalMousePos)) 
		{ 
			rt.position = globalMousePos; 
			rt.rotation = m_draggingPlane.rotation; 
		} 
	} 
} 




//------------------------------------------------------------- 
//! 長押しからのドラッグ＆ドロップ 
//------------------------------------------------------------- 
public class goDragMeForLongPress : goDragBase, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler 
{ 
	private float m_longPressTime = 0.16f; 
	private float m_timer = 0.0f; 
	private bool m_isPress = false; 
	private PointerEventData m_eventData = null; 

	//------------------------------------------------------------- 
	//! 長押し判定時間の設定。 
	//------------------------------------------------------------- 
	public void SetLongPressTime(float _time) 
	{ 
		m_longPressTime = _time; 
	} 

	//------------------------------------------------------------- 
	//! 状態のクリア。 
	//------------------------------------------------------------- 
	private void clearStatus() 
	{ 
		m_isPress = false; 
		m_timer = 0.0f; 
		m_eventData = null; 
	} 

	public void OnPointerDown(PointerEventData _eventData) 
	{ 
		m_isPress = true; 
		m_timer = 0.0f; 
		m_eventData = _eventData; 

		if (m_OnEventPointerBegin != null) 
		{ 
			m_OnEventPointerBegin(m_eventData); 
		} 
	} 

	public void OnPointerUp(PointerEventData _eventData) 
	{ 
		// ドラッグせずに指を離した。 
		GameObject go = _eventData.pointerCurrentRaycast.gameObject; 
		if (_eventData.selectedObject == go) 
		{ 
			if (m_OnEventPointerEnd != null) 
			{ 
				m_OnEventPointerEnd(_eventData); 
			} 

			if (m_draggingIcon != null) 
			{ 
				Destroy(m_draggingIcon); 
				m_draggingIcon = null; 
			} 
			clearStatus(); 
		} 
	} 

	public void OnPointerExit(PointerEventData _eventData) 
	{ 
		clearStatus(); 
	} 

	public void Update() 
	{ 
		if (m_isPress) 
		{ 
			m_timer += Time.deltaTime; 

			if (m_timer >= m_longPressTime) 
			{ 
				// ドラッグ開始。 
				var canvas = FindInParents<Canvas>(gameObject); 
				if (canvas == null) 
				{ 
					return; 
				} 

				Image image = createImage(canvas, m_eventData); 
				if (image != null) 
				{ 
					if (m_dragOnSurfaces) 
					{ 
						m_draggingPlane = transform as RectTransform; 
					} 
					else 
					{ 
						m_draggingPlane = canvas.transform as RectTransform; 
					} 

					SetDraggedPosition(m_eventData); 
				} 
				clearStatus(); 
			} 
		} 
	} 

	//------------------------------------------------------------- 
	//! ドラッグ中 
	//------------------------------------------------------------- 
	public void OnDrag(PointerEventData _eventData) 
	{ 
		if (m_draggingIcon != null) 
		{ 
			SetDraggedPosition(_eventData); 
		} 
	} 

	//------------------------------------------------------------- 
	//! ドラッグ終了 
	//------------------------------------------------------------- 
	public void OnEndDrag(PointerEventData _eventData) 
	{ 
		if (m_draggingIcon != null) 
		{ 
			if (m_OnEventPointerEnd != null) 
			{ 
				m_OnEventPointerEnd(_eventData); 
			} 

			Destroy(m_draggingIcon); 
			m_draggingIcon = null; 
		} 
		clearStatus(); 
	}
}