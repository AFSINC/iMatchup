using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class NoticeElement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
	public GameObject myView;
	ViewManager viewManager;
	public int readStat;					// prefab作成時にNoticeviewからセット 共通  0:未読  1:既読
	public string category;				// prefab作成時にNoticeviewからセット 共通
	public string noticeDate;				// prefab作成時にNoticeviewからセット 共通
	public string title;						// prefab作成時にNoticeviewからセット SYS
	public string message;				// prefab作成時にNoticeviewからセット SYS
	public string sumDate;				// prefab作成時にNoticeviewからセット RES
	public string sumEndDate;			// prefab作成時にNoticeviewからセット RES
	private string para1 = "";
	private string para2 = "";
	private Vector2 viewPanel;					// viewが表示されているか隠れているか判断に使用
	private RectTransform viewRectTfm;	// viewが表示されているか隠れているか判断に使用
	ScrollRect scrRct;								// EventMessage を通過させる hierarchyでの親となる ScrollRectComponent
	Transform trsOfScrRect;						// Itemの横サイズを決定している親(ScrollRect)
	private Vector2 ItemPos;					// Itemのworld座標
	private Vector2 offsetPos;					// Item起点のDragポイントまでの距離(左移動マイナス値、右移動プラス値)
	private float DragSatrtX;						// Dragの開始位置
	private float ItemWidth;						// Itemの幅
	private float ItemRightEndPosX;			// Itemの右端のX座標となるItemの中心点(ピボット)
	private int dragStat = 0;						// Dragステータス 0:NoDrag  1:Drag中  2:delete表示連動中
	private float moveX;							// Drag起点からの移動距離(左移動マイナス値、右移動プラス値)
	private float dragJudgeValue = -10f;	// Drag開始と判断する moveXの移動量
	private float itemDelValue;					// Item削除と判断する moveXの移動量
	private float delWidth = 70f;				// Delete文字表示幅
	private Transform delTfm;

	public void _CallBackBtnElement() {
		if (dragStat != 0)
			return;
		
		if (category == NoticeManager.CATE_SYSTEM) {
			para1 = title;
			para2 = message;
		} else if (category == NoticeManager.CATE_RESULT) {
			para1 = sumDate;
			para2 = sumEndDate;
		} else if (category == NoticeManager.CATE_NETWK) {
		} else {
		}
		viewManager.scriptNoticeView._CallBackBtnDispNoticeinfoElement (this.transform, category, noticeDate, para1, para2);
	}

	public void _CallBackBtnElementDelete() {
		if (category == NoticeManager.CATE_SYSTEM) {
			para1 = noticeDate;
			para2 = title;
		} else if (category == NoticeManager.CATE_RESULT) {
			para1 = sumDate;
			para2 = sumEndDate;
		} else if (category == NoticeManager.CATE_NETWK) {
		} else {
		}
		viewManager.scriptNoticeView.deleteNoticeItem (this.transform.parent, category, para1, para2);
	}

	void OnLongPress() {
	}
	public void loadView () {
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
        //		Debug.Log ("moveX:" + moveX);
        if (moveX < itemDelValue && moveX != 0) {     // Item幅の85%以上のDragがあったら即削除 (ミスで消えないよう少し多め)
            delTfm.position = new Vector2(ItemRightEndPosX, delTfm.position.y);
        } else if (moveX < dragJudgeValue || dragStat == 2)  {	// Drag開始判定移動量、またはDrag中(Drag中にDrag開始判定移動量より減少することがある)
			dragStat = 2;
			scrRct.vertical = false;		//　scrollRsctの停止
			ItemPos.x = Mathf.Clamp (ItemPos.x, ItemRightEndPosX - delWidth,  ItemRightEndPosX);	// 最小:DELETE表示　最大:初期位置
			ItemPos.y = Mathf.Clamp (ItemPos.y, this.transform.position.y, this.transform.position.y);
			this.transform.position = ItemPos;

			// delete,nodifyImageを左から引き出す
//			float newDelX = Mathf.Clamp (ItemRightEndPosX+ItemWidth + moveX, ItemRightEndPosX+ItemWidth*3/4,  ItemRightEndPosX+ItemWidth);	// 最小:DELETE表示　最大:初期位置
//			delTfm.position = new Vector2 (newDelX , delTfm.position.y);
			delTfm.position = new Vector2 (ItemPos.x+ItemWidth, delTfm.position.y);
		} else {
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

		viewRectTfm = viewManager.scriptNoticeView.transform as RectTransform;
		viewPanel = viewRectTfm.TransformVector (new Vector3(viewRectTfm.rect.width, viewRectTfm.rect.height));
		delTfm = this.transform.parent.FindChild ("ImgDelete");
		scrRct = this.transform.parent.parent.parent.GetComponent<ScrollRect>();	// 不要Eventを送る親
		trsOfScrRect = this.transform.parent.parent.parent;
		RectTransform rectTrsOfScrRect = trsOfScrRect.GetComponent<RectTransform>();		// 親からItemの横サイズ(World)を得る
		Vector3 wh  = rectTrsOfScrRect.TransformVector (new Vector3(rectTrsOfScrRect.rect.width, rectTrsOfScrRect.rect.height));
		Vector3 awh  = rectTrsOfScrRect.TransformVector (new Vector3(rectTrsOfScrRect.offsetMin.x, rectTrsOfScrRect.offsetMin.y));
		ItemWidth = wh.x;
		//		ItemRightEndPosX = trsOfScrRect.position.x + ItemWidth / 2;
		ItemRightEndPosX = awh.x + ItemWidth/2;  	// Itemの右端のX座標となる中心点(ピボット)
		itemDelValue = ItemWidth * (-1) * 0.7f;			// Item削除と判断する moveXの移動量 Item横幅の何パーセント
		delWidth = ItemWidth*1/4;								// Delete文字表示幅 Item横幅の25パーセント
	}
	void Update () {
		if (dragStat == 0 && this.transform.position.x != ItemRightEndPosX) {
			if (moveX < itemDelValue && moveX != 0) {       //  itemDelValue幅以上のDragがあったら即削除
                _CallBackBtnElementDelete();
			} else if (moveX > (delWidth * -1)) {	//  
				if (viewPanel.x/2 == viewRectTfm.position.x) {	// Viewが完全に表示されているかを中心位置で確認(中心にあるときだけLeap)
					float newx = Mathf.Lerp (this.transform.position.x, ItemRightEndPosX, Time.deltaTime * 10f);
					this.transform.position = new Vector2 (newx, this.transform.position.y);
					delTfm.position = new Vector2 (this.transform.position.x + ItemWidth, delTfm.position.y);
				} else {
					(this.transform as RectTransform).localPosition = Vector2.zero;
				}
			}
		}
	}
}
