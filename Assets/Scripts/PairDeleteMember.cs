using UnityEngine;
using System;
using System.Collections;

public class PairDeleteMember : MonoBehaviour {
	private ViewManager viewManager;
	public Vector2 viewPanel;					// viewが表示されているか隠れているか判断に使用
	public int dragStat = 0;					// Dragステータス 0:NoDrag  1:Drag中  2:delete表示連動中
	public RectTransform viewRectTfm;	// viewが表示されているか隠れているか判断に使用
	public Transform itemTfm;				// Drag対象となるItemTransform(ItemPosのtarnsform)
	public Transform delTfm;					// DeleteTransform
	public float ItemWidth;						// Itemの幅(固定)
	public float itemDelValue;					// Item削除と判断する moveXの移動量
	public float  ItemPosInitX;				// 初期のItemのworld座標
	public float moveX;							// Drag起点からの移動距離(左移動マイナス値、右移動プラス値)
	public float delWidth;						// Delete文字表示幅

	public void _OnDeleteMember () {
		Transform tParent = this.transform.parent;
		viewManager.scriptPairView.deletePair(tParent.gameObject, tParent.GetSiblingIndex ());
	}

	void Start() {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
	}

	void Update () {
		if (dragStat == 0 && itemTfm.position.x != ItemPosInitX) {
			if (moveX < itemDelValue && moveX != 0) {		// Item幅の80%以上の左向きDragがあったら即削除
				_OnDeleteMember ();
			} else if (moveX > delWidth*(-1)  && moveX != 0) {	// 
				if (Mathf.Abs(viewPanel.x / 2 - viewRectTfm.position.x) < 1) {	// Viewが完全に表示されているかを中心位置(誤差1以内)で確認(中心にあるときだけLeap)
					float newx = Mathf.Lerp (itemTfm.position.x, ItemPosInitX, Time.deltaTime * 10f);
					itemTfm.position = new Vector2 (newx, itemTfm.position.y);
					delTfm.position = new Vector2 (itemTfm.position.x+ItemWidth, delTfm.position.y);
				} else {
//					(this.transform as RectTransform).localPosition = Vector2.zero;
				}
			}
		}
	} 
}
