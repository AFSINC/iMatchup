using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;


public class ListItemMainView : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
	ViewManager viewManager;
	public string myRegDate;		// prefab作成時にmainViewからセットされる
	ScrollRect scrRct;					//  EventMessage を通過させる hierarchyでの親となる ScrollRectComponent
	EventLongPress eventLP;
	EventDoubleTap eventDT;
	float itemHalfHight;
	float viewPointTop, viewPointBtm;
	PointerEventData _e;
	private Vector2 offsetPos;					// Item起点のDragポイントまでの距離(左移動マイナス値、右移動プラス値)
	private bool longPressFlg = false;
	Vector3 parentPos, posOffset, p;
	GameObject placeHolder = null;
	Shadow sd = null;
	Outline ol = null;
	Transform placeholderParent = null;
	int sousceItemIndex, destItemIndex;

	public void OnLongPressPara(PointerEventData e) {
		if (!longPressFlg) {
			longPressFlg = true;

			Transform pp = this.transform;
			pp.gameObject.AddComponent<Shadow> ();
			sd = pp.GetComponent<Shadow> ();
			sd.effectColor = new Vector4 (0f, 0f, 0f, 20f/255f);
			sd.effectDistance = new Vector2 (20f, -30f);
			pp.gameObject.AddComponent<Outline> ();
			ol = pp.GetComponent<Outline> ();
			ol.effectColor = new Vector4 (0f, 0f, 0f, 60f/255f);
			ol.effectDistance = new Vector2 (1.0f, -1.0f);
			ol.useGraphicAlpha = true;

			sousceItemIndex = this.transform.parent.GetSiblingIndex ();
			parentPos.x = this.transform.parent.position.x;
			parentPos.y = this.transform.parent.position.y;
			this.transform.parent.SetAsLastSibling ();

			placeHolder = new GameObject("placeHolder");
			placeHolder.transform.SetParent (this.transform.parent.parent);
			LayoutElement le = placeHolder.AddComponent<LayoutElement> ();
			le.preferredHeight = this.transform.parent.GetComponent<LayoutElement> ().preferredHeight;
			placeholderParent = this.transform.parent.parent;
			placeHolder.transform.SetSiblingIndex (sousceItemIndex);

			this.transform.parent.SetParent (this.transform.parent.parent.parent);

			RectTransform recTrs = this.transform.parent.GetComponent<RectTransform>();		// 親からItemの横サイズ(World)を得る
			Vector3 wh  = recTrs.TransformVector (new Vector3(recTrs.rect.width, recTrs.rect.height));
			itemHalfHight = wh.y / 2;
		}
	}
	public void OnLongPressExit(PointerEventData e) {
		if (longPressFlg)
			OnEndDrag(e);
	}

	public void OnBeginDrag(PointerEventData e) {
		if (longPressFlg) {
			eventLP.enableLongPress = false;
			scrRct.vertical = false;		//　scrollRsctの停止

			posOffset.x = this.transform.parent.position.x - e.position.x;
			posOffset.y = this.transform.parent.position.y - e.position.y;

			placeHolder.transform.SetSiblingIndex (this.transform.parent.GetSiblingIndex ());

//			this.transform.parent.SetParent (this.transform.parent.parent.parent);
		} else
			transform.DoParentEventSystemHandler<IBeginDragHandler> ((parent) => {scrRct.OnBeginDrag (e);});
	}
	public void OnDrag(PointerEventData e) {
		if (longPressFlg) {
			// clamp 縦を計算
			RectTransform recListTrs = this.transform.parent.parent.GetComponent<RectTransform>();		// listScroll
			RectTransform rctTrsActPanel = GameObject.Find ("MainView/ActiveMemberListPanel").transform.GetComponent<RectTransform> ();
			RectTransform rctTrsFncPanel = GameObject.Find ("MainView/FuntionPanel").transform.GetComponent<RectTransform> ();
			RectTransform rctTrsItem = this.transform.parent.GetComponent<RectTransform> ();
			Vector3 listTwh  = recListTrs.TransformVector (new Vector3(recListTrs.anchoredPosition.x, recListTrs.anchoredPosition.y));
			Vector3 listWh  = recListTrs.TransformVector (new Vector3(recListTrs.rect.width, recListTrs.sizeDelta.y));
			Vector3 actWh  = rctTrsActPanel.TransformVector (new Vector3(rctTrsActPanel.rect.width, rctTrsActPanel.sizeDelta.y));
			Vector3 fncWh  = rctTrsFncPanel.TransformVector (new Vector3(rctTrsFncPanel.rect.width, rctTrsFncPanel.sizeDelta.y));
			Vector3 itemWh  = rctTrsFncPanel.TransformVector (new Vector3(rctTrsItem.rect.width, rctTrsItem.sizeDelta.y));
			viewPointTop = listTwh.y + actWh.y + fncWh.y - itemWh.y/2 + 1;
			viewPointBtm =  listTwh.y + actWh.y + fncWh.y - listWh.y + itemWh.y/2 +1;

			p.x = posOffset.x + e.position.x;
			p.y = posOffset.y + e.position.y;
			p.x = Mathf.Clamp (p.x, parentPos.x, parentPos.x);
			p.y = Mathf.Clamp (p.y, viewPointBtm, viewPointTop);
			this.transform.parent.position = p;

			int newSiblingIndex = placeholderParent.childCount;
			for (int i = 0; i < placeholderParent.childCount; i++) {
				if (this.transform.parent.position.y > placeholderParent.GetChild (i).position.y + itemHalfHight) {
					newSiblingIndex = i;
					if (placeholderParent.transform.GetSiblingIndex () < newSiblingIndex)
						newSiblingIndex--;

					break;
				}
			}
			placeHolder.transform.SetSiblingIndex (newSiblingIndex);

			RectTransform rctTrsVtc = GameObject.Find ("MainView/ActiveMemberListPanel/ListScroll/LayoutVertical").transform as RectTransform;
			float mgn = rctTrsVtc.sizeDelta.y / rctTrsVtc.childCount / rctTrsVtc.sizeDelta.y;
			ScrollRect scrRect = this.transform.parent.parent.GetComponent<ScrollRect>();
			if (p.y > viewPointTop-5 && placeHolder.transform.GetSiblingIndex() > 0) {
				scrRect.verticalNormalizedPosition += mgn;
			}
			if (p.y < viewPointBtm+5 && placeHolder.transform.GetSiblingIndex() < rctTrsVtc.childCount-1) {
				scrRect.verticalNormalizedPosition -= mgn;
			}
		} else {
			eventLP.enableLongPress = false;
			transform.DoParentEventSystemHandler<IDragHandler> ((parent) => {scrRct.OnDrag (e);});
		}
	}
	public void OnEndDrag(PointerEventData e) {
		if (longPressFlg) {
			Object.Destroy (sd);
			sd = null;
			Object.Destroy (ol);
			ol = null;

			destItemIndex = placeHolder.transform.GetSiblingIndex ();
			MemberManager.movMember (sousceItemIndex, destItemIndex);

			this.transform.parent.SetParent (placeholderParent);
			this.transform.parent.SetSiblingIndex (placeHolder.transform.GetSiblingIndex ());
			Destroy (placeHolder);
			scrRct.vertical = true;	//　scrollRsctの再開
			longPressFlg = false;
		}
		eventLP.enableLongPress = true;
		transform.DoParentEventSystemHandler<IEndDragHandler>((parent) => { scrRct.OnEndDrag(e); });
	}

	// MainViewに試合補正値設定Viewを呼ばせる
	private void onDoubleTap() {}


	void Start () {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
		scrRct = this.transform.parent.parent.parent.GetComponent<ScrollRect>();	// 不要Eventを送る親
		this.gameObject.AddComponent (typeof(EventLongPress));
		eventLP = GetComponent<EventLongPress> ();
		eventLP.onLongPressPara.AddListener(OnLongPressPara);
		eventLP.onLongPressParaExit.AddListener(OnLongPressExit);
		eventLP.intervalAction = 0.4f;		// LongPress判定時間

		this.gameObject.AddComponent (typeof(EventDoubleTap));
		eventDT = GetComponent<EventDoubleTap> ();
		eventDT.onDoubleTap.AddListener(onDoubleTap);
		}
	void Update () {	
	}
}
