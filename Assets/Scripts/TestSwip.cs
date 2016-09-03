using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;

// 全体を通して、World空間座標(Eventが空間座標) で計算する。
public class TestSwip : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
	ScrollRect scrRct;					//  EventMessage を通過させる hierarchyでの親となる ScrollRectComponent
  	Transform trsOfScrRect;						// Itemの横サイズを決定している親(ScrollRect)
	private Vector2 ItemPos;					// Itemのworld座標
	private Vector2 offsetPos;					// Item起点のDragポイントまでの距離(左移動マイナス値、右移動プラス値)
	private float DragSatrtX;						// Dragの開始位置
	private float ItemWidth;						// Itemの幅
	private float ItemRightEndPosX;			// Itemの右端のX座標
	private int dragStat = 0;						// Dragステータス 0:NoDrag  1:Drag中  2:delete表示連動中
	private float moveX;							// Drag起点からの移動距離(左移動マイナス値、右移動プラス値)
	private float dragJudgeValue = -10f;	// Drag開始と判断する moveXの移動量
	private float itemDelValue;					// Item削除と判断する moveXの移動量
	private float delWidth = 60f;				// Delete文字表示幅

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
		Debug.Log ("moveX:" + moveX);
		if (moveX < dragJudgeValue || dragStat == 2)  {	// Drag開始判定移動量、またはDrag中(Drag中にDrag開始判定移動量より減少することがある)
			dragStat = 2;
			scrRct.vertical = false;		//　scrollRsctの停止
			ItemPos.x = Mathf.Clamp (ItemPos.x, ItemRightEndPosX - delWidth,  ItemRightEndPosX);	// 最小:DELETE表示　最大:初期位置
			ItemPos.y = Mathf.Clamp (ItemPos.y, this.transform.parent.position.y, this.transform.parent.position.y);
			this.transform.position = ItemPos;
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
		scrRct = this.transform.parent.parent.parent.parent.parent.GetComponent<ScrollRect>();	// 不要Eventを送る親
		trsOfScrRect = this.transform.parent.parent.parent;
		RectTransform rectTrsOfScrRect = trsOfScrRect.GetComponent<RectTransform>();		// 親からItemの横サイズ(World)を得る
		Vector3 wh  = rectTrsOfScrRect.TransformVector (new Vector3(rectTrsOfScrRect.rect.width, rectTrsOfScrRect.rect.height));
		ItemWidth = wh.x;
		ItemRightEndPosX = trsOfScrRect.position.x + ItemWidth / 2;
		itemDelValue = ItemWidth * (-1);
	}
	void Update () {
		if (dragStat == 0 && this.transform.position.x != ItemRightEndPosX) {
			if (moveX < itemDelValue * 0.8) {		// Item幅の90%以上のDragがあったら即削除
				DestroyImmediate (this.transform.parent.parent.gameObject);	// Item	をListから消す
			} else if (moveX > (delWidth * -1)) {	// 
				float newx = Mathf.Lerp (this.transform.position.x, ItemRightEndPosX, Time.deltaTime * 10f);
				this.transform.position = new Vector2 (newx, this.transform.position.y);
			}
		}
	}
}
