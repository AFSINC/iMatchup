using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;

public class PairMember : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
	private ViewManager viewManager;
//	private ScriptPairView scripParetView;
	private Transform tParent;
	public string myRegDate;	// prefab作成時にScriptPairViewから設定される
	private Vector2 viewPanel;					// viewが表示されているか隠れているか判断に使用
	private RectTransform viewRectTfm;	// viewが表示されているか隠れているか判断に使用
	private ScrollRect scrRct;					//  EventMessage を通過させる hierarchyでの親となる ScrollRectComponent
	private Transform trsOfScrRect;			// Itemの横サイズを決定している親(ScrollRect)
	private float  ItemPosInitX;				// 初期のItemのworld座標
	private Vector2 ItemPos;					// 現在のItemのworld座標
	private Vector2 offsetPos;					// Item起点のDragポイントまでの距離(左移動マイナス値、右移動プラス値)
	private float DragSatrtX;					// Dragの開始位置
	private float ItemWidth;						// Itemの幅(固定)
//	private float ItemRightEndPosX;			// Itemの右端のX座標(固定)
	private int dragStat = 0;					// Dragステータス 0:NoDrag  1:Drag中  2:delete表示連動中
	private float moveX;							// Drag起点からの移動距離(左移動マイナス値、右移動プラス値)
	private float dragJudgeValue = 10f;		// Drag開始と判断する moveXの移動量
	private float itemDelValue;					// Item削除と判断する moveXの移動量
	private float delWidth;						// Delete文字表示幅
	private Transform itemTfm;				// Drag対象となるItemTransform(ItemPosのtarnsform)
	private Transform delTfm;					// DeleteTransform
	private PairDeleteMember PDM;			// 2箇所からくる移動メッセージを受け取り、Itemの移動を1か所で制御するscript

	public void _OnTap() {
		if (PDM.dragStat != 0)
			return;
		
		PairManager.tempCurrentRow = tParent.GetSiblingIndex ();
		PairManager.tempCurrentLR = this.transform.parent.name == "LeftMember" ? PairManager.LEFT : PairManager.RIGHT;
		viewManager.scriptPairView._toPareSelectView (myRegDate);
	}
	void onDoubleTap() {
	}
	void OnLongPress() {
	}

	public void OnBeginDrag(PointerEventData e) {
		if (this.transform.parent.Find ("MemberName").GetComponent<Text> ().text != "選択...") { // 片方でも「選択...」は対象外
			PDM.dragStat = 1;
	
			ItemPos = itemTfm.position;
			offsetPos.x = ItemPos.x - e.position.x;
			DragSatrtX = e.position.x;
		}
		transform.DoParentEventSystemHandler<IBeginDragHandler>((parent) => { scrRct.OnBeginDrag(e); });
	}
	public void OnDrag(PointerEventData e) {
		if (PDM.dragStat == 0)
			return;
		
		ItemPos.x = offsetPos.x + e.position.x;
		moveX = e.position.x - DragSatrtX;
		PDM.moveX = moveX;
        if (moveX < itemDelValue && moveX != 0) {     // Item幅の85%以上のDragがあったら即削除 (ミスで消えないよう少し多め)
            delTfm.position = new Vector2(ItemPosInitX, delTfm.position.y);
        } else if (moveX < dragJudgeValue || dragStat == 2)  {	// Drag開始判定移動量、またはDrag中(Drag中にDrag開始判定移動量より減少することがある)
			PDM.dragStat = 2;
			scrRct.vertical = false;		//　scrollRsctの停止
			ItemPos.x = Mathf.Clamp (ItemPos.x, ItemPosInitX - delWidth,  ItemPosInitX);	// 最小:DELETE表示　最大:初期位置
			ItemPos.y = Mathf.Clamp (ItemPos.y, itemTfm.position.y, itemTfm.position.y);
			itemTfm.position = ItemPos;

			// delete,nodifyImageを左から引き出す
			delTfm.position = new Vector2 (ItemPos.x+ItemWidth, delTfm.position.y);
		}
		transform.DoParentEventSystemHandler<IDragHandler>((parent) => { scrRct.OnDrag(e); });
	}
	public void OnEndDrag(PointerEventData e) {
		PDM.dragStat = 0;
		scrRct.vertical = true;	//　scrollRsctの再開
		transform.DoParentEventSystemHandler<IEndDragHandler>((parent) => { scrRct.OnEndDrag(e); });
	}

	void Start() {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
		tParent = this.transform.parent.parent.parent;			//PairのTransform
		this.gameObject.AddComponent (typeof(EventDoubleTap));
		GetComponent<EventDoubleTap>().onDoubleTap.AddListener(onDoubleTap);
		this.gameObject.AddComponent (typeof(EventLongPress));
		GetComponent<EventLongPress>().onLongPress.AddListener(OnLongPress);
		 
		// DragDeleteのData収集
		trsOfScrRect = GameObject.Find("PairView/PairMemberPanel/ListScroll").transform;
		scrRct = trsOfScrRect.GetComponent<ScrollRect>();	// 不要Eventを送る親
		delTfm = this.transform.parent.parent.parent.FindChild("ImgDelete");	// 自分の属するPrefabのImgDelete
		viewRectTfm = viewManager.scriptPairView.transform as RectTransform;
		viewPanel = viewRectTfm.TransformVector (new Vector3(viewRectTfm.rect.width, viewRectTfm.rect.height));
 		RectTransform rectTrsOfScrRect = trsOfScrRect as RectTransform;		// 親からItemの横サイズ(World)を得る
		Vector3 wh  = rectTrsOfScrRect.TransformVector (new Vector3(rectTrsOfScrRect.rect.width, rectTrsOfScrRect.rect.height));
		Vector3 awh  = rectTrsOfScrRect.TransformVector (new Vector3(rectTrsOfScrRect.offsetMin.x, rectTrsOfScrRect.offsetMin.y));
		ItemWidth = wh.x;
		ItemPosInitX  = awh.x + ItemWidth / 2;
//		ItemRightEndPosX = awh.x + ItemWidth; 
		itemDelValue = Mathf.Abs(ItemWidth) * -0.7f;
		itemTfm = this.transform.parent.parent;
//		(delTfm as RectTransform).sizeDelta = new Vector2(rectTrsOfScrRect.rect.width, (delTfm as RectTransform).sizeDelta.y);		//ipad画面だとDELETEが伸びないので引き延ばす
		delWidth = ItemWidth * 0.25f;

		// ImgItemBase へデータ移送
		PDM = this.transform.parent.parent.GetComponent<PairDeleteMember> ();
		PDM.viewPanel = viewPanel;
		PDM.viewRectTfm = viewRectTfm;
		PDM.itemTfm = itemTfm;
		PDM.ItemWidth = ItemWidth;
		PDM.delTfm = delTfm;
		PDM.itemDelValue = itemDelValue;
		PDM.ItemPosInitX = ItemPosInitX;
		PDM.moveX = moveX;	
		PDM.delWidth = delWidth;
	}
}
