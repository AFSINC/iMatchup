using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;

public class DeleteMember : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
	ViewManager viewManager;
	ScriptSelectView scriptSelectView;
	public string myRegDate;					// scriptSelectView からprefab作成時に設定される
	private Vector2 viewPanel;					// viewが表示されているか隠れているか判断に使用
	private RectTransform viewRectTfm;	// viewが表示されているか隠れているか判断に使用
	private ScrollRect scrRct;					//  EventMessage を通過させる hierarchyでの親となる ScrollRectComponent
	private Transform trsOfScrRect;			// Itemの横サイズを決定している親(ScrollRect)
	private float  ItemPosInitX;				// 初期のItemのworld座標
	private Vector2 ItemPos;					// 現在のItemのworld座標
	private Vector2 offsetPos;					// Item起点のDragポイントまでの距離(左移動マイナス値、右移動プラス値)
	private float DragSatrtX;					// Dragの開始位置
	private float ItemWidth;						// Itemの幅(固定)
	private float ItemRightEndPosX;			// Itemの右端のX座標(固定)
	private int dragStat = 0;					// Dragステータス 0:NoDrag  1:Drag中  2:delete表示連動中
	private float moveX;							// Drag起点からの移動距離(左移動マイナス値、右移動プラス値)
	private float dragJudgeValue = -10f;	// Drag開始と判断する moveXの移動量
	private float itemDelValue;					// Item削除と判断する moveXの移動量
	private float delWidth;						// Delete文字表示幅
	private Transform delTfm;					// DeleteTransform
	private Transform modTfm;				// ModifyTransform

	// Event CallBack
	public void _OnDeleteMember() {
		viewManager.scriptSelectView.deleteMember(this.transform.parent.gameObject, this.transform.parent.GetSiblingIndex ());
	}
	public void _OnModifyMember() {
		viewManager.scriptSelectView._CallBackBtnEditMember (myRegDate);
	}
	void OnLongPress() {
/*		string title = "メンバー削除の確認";
		string txt = this.transform.parent.Find ("MemberName").GetComponent<Text>().text;
		string message = "[ " + txt + " ]さんを削除しますか？\nペア固定がある場合は解除されます。";
		DialogViewController.Show(title, message, new DialogViewOptions {
			btnCancelTitle = "キャンセル", btnCancelDelegate = ()=>{
				return;
			},
			btnOkTitle = "削除", btnOkDelegate = ()=>{
				viewManager.scriptSelectView.deleteMember(this.transform.parent.gameObject, this.transform.parent.GetSiblingIndex ());
			},
		});*/
	}

	public void OnBeginDrag(PointerEventData e) {
		dragStat = 1;
		ItemPos = this.transform.position;
		offsetPos.x =ItemPos.x - e.position.x;
		DragSatrtX = e.position.x;

		transform.DoParentEventSystemHandler<IBeginDragHandler>((parent) => { scrRct.OnBeginDrag(e); });
	}

	public void OnDrag(PointerEventData e) {
		ItemPos.x = offsetPos.x + e.position.x;
		moveX =  e.position.x - DragSatrtX;
        if (moveX < itemDelValue && moveX != 0) {     // Item幅の85%以上のDragがあったら即削除 (ミスで消えないよう少し多め)
            delTfm.position = new Vector2(ItemPosInitX, delTfm.position.y);
        } else if(moveX < dragJudgeValue || dragStat == 2) {  // Drag開始判定移動量、またはDrag中(Drag中にDrag開始判定移動量より減少することがある)
            dragStat = 2;
            scrRct.vertical = false;        //　scrollRsctの停止
            ItemPos.x = Mathf.Clamp(ItemPos.x, ItemPosInitX - delWidth, ItemPosInitX);  // 最小:DELETE表示　最大:初期位置
            ItemPos.y = Mathf.Clamp(ItemPos.y, this.transform.position.y, this.transform.position.y);
            this.transform.position = ItemPos;

            // delete,nodifyImageを左から引き出す
            modTfm.position = new Vector2(ItemPos.x + ItemWidth, modTfm.position.y);
            delTfm.position = new Vector2(ItemRightEndPosX + ItemWidth / 2 - (ItemPosInitX - ItemPos.x) / 2, delTfm.position.y);
        } else  {
            transform.DoParentEventSystemHandler<IDragHandler>((parent) => { scrRct.OnDrag(e); });
		}
	}

	public void OnEndDrag(PointerEventData e) {
		dragStat = 0;
		scrRct.vertical = true;	//　scrollRsctの再開


		transform.DoParentEventSystemHandler<IEndDragHandler>((parent) => { scrRct.OnEndDrag(e); });
	}

	void Start () {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
		this.gameObject.AddComponent (typeof(EventLongPress));
		GetComponent<EventLongPress>().onLongPress.AddListener(OnLongPress);

		delTfm = this.transform.parent.FindChild ("ImgDelete").transform;
		modTfm = this.transform.parent.FindChild ("ImgModify").transform;

		viewRectTfm = viewManager.scriptSelectView.transform as RectTransform;
		viewPanel = viewRectTfm.TransformVector (new Vector3(viewRectTfm.rect.width, viewRectTfm.rect.height));
		scrRct = this.transform.parent.parent.parent.GetComponent<ScrollRect>();	// 不要Eventを送る親
		trsOfScrRect = this.transform.parent.parent.parent;
		RectTransform rectTrsOfScrRect = trsOfScrRect.GetComponent<RectTransform>();		// 親からItemの横サイズ(World)を得る
		Vector3 wh  = rectTrsOfScrRect.TransformVector (new Vector3(rectTrsOfScrRect.rect.width, rectTrsOfScrRect.rect.height));
		Vector3 awh  = rectTrsOfScrRect.TransformVector (new Vector3(rectTrsOfScrRect.offsetMin.x, rectTrsOfScrRect.offsetMin.y));
		ItemWidth = wh.x;
		ItemPosInitX  = awh.x + ItemWidth / 2;
		ItemRightEndPosX = awh.x + ItemWidth;
		itemDelValue = ItemWidth * (-1) * 0.7f;     // 即削除と判断するDrag量
        delWidth = ItemWidth * 0.5f;
	}
	void Update () {
		if (dragStat == 0 && this.transform.position.x != ItemPosInitX) {
			if (moveX < itemDelValue && moveX != 0) {		// Item幅がitmeDelValueno幅以上のDragがあったら即削除
//				if (GameManager.chkLock(myRegDate) == GameManager.UNLOCK) {
                   _OnDeleteMember();
//				}
			} else if (moveX > (delWidth * -1)) {	// 
				if (viewPanel.x / 2 == viewRectTfm.position.x) {	// Viewが完全に表示されているかを中心位置で確認(中心にあるときだけLeap)
					float newx = Mathf.Lerp (this.transform.position.x, ItemPosInitX, Time.deltaTime * 10f);
					this.transform.position = new Vector2 (newx, this.transform.position.y);
					modTfm.position = new Vector2 (this.transform.position.x+ItemWidth, modTfm.position.y);
					delTfm.position = new Vector2 (ItemRightEndPosX+ItemWidth/2-(ItemPosInitX-this.transform.position.x)/2, delTfm.position.y);
				} else {
//					(this.transform as RectTransform).localPosition = Vector2.zero;
				}
			}
		}
	}
}
