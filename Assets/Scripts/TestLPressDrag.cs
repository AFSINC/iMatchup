using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class TestLPressDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
	ScrollRect scrRct;					//  EventMessage を通過させる hierarchyでの親となる ScrollRectComponent
	EventLongPress eventLP;
	PointerEventData _e;
	private Vector2 offsetPos;					// Item起点のDragポイントまでの距離(左移動マイナス値、右移動プラス値)
	private bool longPressFlg = false;

	public void OnLongPress() {
//		Debug.Log ("OnLongPress");
	}
	public void OnLongPressPara(PointerEventData e) {
		Debug.Log ("OnLongPress:" + e.position.x);
		longPressFlg = true;
	}

	public void OnBeginDrag(PointerEventData e) {
		eventLP.enableLongPress = false;
		if (longPressFlg) {
			scrRct.vertical = false;		//　scrollRsctの停止
			offsetPos.x = this.transform.position.x - e.position.x;
			offsetPos.y = this.transform.position.y - e.position.y;
		} else
		transform.DoParentEventSystemHandler<IBeginDragHandler> ((parent) => {scrRct.OnBeginDrag (e);});
	}
	public void OnDrag(PointerEventData e) {
		if (longPressFlg) {
			this.transform.position = e.position + offsetPos;
		} else
		transform.DoParentEventSystemHandler<IDragHandler> ((parent) => {scrRct.OnDrag (e);});
	}
	public void OnEndDrag(PointerEventData e) {
		eventLP.enableLongPress = true;
		scrRct.vertical = true;	//　scrollRsctの再開
		longPressFlg = false;
		transform.DoParentEventSystemHandler<IEndDragHandler>((parent) => { scrRct.OnEndDrag(e); });
	}



	void Start () {
		scrRct = this.transform.parent.parent.parent.GetComponent<ScrollRect>();	// 不要Eventを送る親
		this.gameObject.AddComponent (typeof(EventLongPress));
		eventLP = GetComponent<EventLongPress> ();
		eventLP.onLongPress.AddListener(OnLongPress);
		eventLP.onLongPressPara.AddListener(OnLongPressPara);
		eventLP.intervalAction = 0.5f;		// LongPress判定時間
	}
	void Update () {	
	}
}
