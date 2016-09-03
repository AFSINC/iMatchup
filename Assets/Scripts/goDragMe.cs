using UnityEngine; 
using UnityEngine.EventSystems; 
using UnityEngine.UI; 

//------------------------------------------------------------- 
//! ドラッグ＆ドロップ規定クラス 
//------------------------------------------------------------- 
public class goDragMeBase : MonoBehaviour 
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
//! ドラッグ＆ドロップ 
//------------------------------------------------------------- 
public class goDragMe : goDragMeBase, IBeginDragHandler, IDragHandler, IEndDragHandler 
{ 
	//------------------------------------------------------------- 
	//! ドラッグ開始 
	//------------------------------------------------------------- 
	public void OnBeginDrag(PointerEventData _eventData) 
	{ 
		var canvas = FindInParents<Canvas>(gameObject); 
		if (canvas == null) 
		{ 
			return; 
		} 

		Image image = createImage(canvas, _eventData); 
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

			SetDraggedPosition(_eventData); 
		} 

		if (m_OnEventPointerBegin != null) 
		{ 
			m_OnEventPointerBegin(_eventData); 
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
		if (m_OnEventPointerEnd != null) 
		{ 
			m_OnEventPointerEnd(_eventData); 
		} 

		if (m_draggingIcon != null) 
		{ 
			Destroy(m_draggingIcon); 
			m_draggingIcon = null; 
		} 
	} 
}