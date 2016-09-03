using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DraggablePlayer : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
	public string myRegDate;		// prefab生成時にscriptGameViewから値がセットされる
	private ViewManager viewManager;
	private ScriptGameView scriptGameView;
	private Transform tfOrignParent;
	private Transform tfPlayPanel;
	private Vector3 parentPos, posOffset, playerPos, MaxPos;
	private Shadow sd = null;
	private Outline ol = null;
	private bool flgDrag = false;
	private float limtDistance = 100f;
	private Transform[] receiveParentTransform;
	private Vector3[] receiveParentPosition;
	private int minReceiveParentNum;
	private float[] distance;
	private float minDistance;

	public void OnBeginDrag(PointerEventData e) {
		if (GameManager.chkLock (myRegDate) == GameManager.LOCK) {
			flgDrag = false;
		} else {
			flgDrag = true;
		}

		Transform pp = this.transform.parent;
		pp.gameObject.AddComponent<Shadow> ();
		sd = pp.GetComponent<Shadow> ();
		sd.effectColor = new Vector4 (0f, 0f, 0f, 20f/255f);
		sd.effectDistance = new Vector2 (20f, -50f);
		pp.gameObject.AddComponent<Outline> ();
		ol = pp.GetComponent<Outline> ();
		ol.effectColor = new Vector4 (0f, 0f, 0f, 60f/255f);
		ol.effectDistance = new Vector2 (1.0f, -1.0f);
		ol.useGraphicAlpha = true;

		tfOrignParent = this.transform.parent.parent;
		this.transform.parent.SetParent (tfPlayPanel);

		parentPos.x = this.transform.parent.position.x;
		parentPos.y = this.transform.parent.position.y;
		posOffset.x = this.transform.parent.position.x - e.position.x;
		posOffset.y = this.transform.parent.position.y - e.position.y;
		this.transform.parent.SetAsLastSibling ();

 		//		Debug.Log (this.name + " " + parentPos.x);
	}

	public void OnDrag(PointerEventData e) {
		if (flgDrag) {
			playerPos.x = posOffset.x + e.position.x;
			playerPos.y = posOffset.y + e.position.y;

			// playPanel の中だけ移動できるようにする
			RectTransform pp = this.transform.parent.GetComponent<RectTransform> ();
			Vector3 wh  = pp.TransformVector (new Vector3(pp.rect.width, pp.rect.height));
			float wOffset = wh.x / 2;
			float hOffset = wh.y / 2;

			float px = tfPlayPanel.position.x;
			float py = tfPlayPanel.position.y;
			RectTransform rt = tfPlayPanel.GetComponent<RectTransform> ();
			Vector3 pwh  = rt.TransformVector (new Vector3(rt.rect.width, rt.rect.height));
			playerPos = new Vector3(Mathf.Clamp (playerPos.x, wOffset, pwh.x-wOffset),Mathf.Clamp (playerPos.y, py-pwh.y/2+hOffset, pwh.y+py-pwh.y/2-hOffset));

			this.transform.parent.position = playerPos;
		}
}

	public void OnEndDrag(PointerEventData e) {
		flgDrag = false;
		Object.Destroy(sd);
		sd = null;
		Object.Destroy(ol);
		ol = null;

		int pageoffset = 0;
		if (minReceiveParentNum > 23) {
			pageoffset = 0;
		} else {
			pageoffset = GameManager.currentPage;			// 何ページ目かを計算する  page0:0~7    page1:8~15   page2:16~23
		}
//		Debug.Log (" position num on page   min[" + minReceiveParentNum +"]  +  ["+ pageoffset * -8+"]");

		GameManager.posPlayerOfRegDate (myRegDate);
		float dis;
		dis= Vector3.Distance (receiveParentPosition[minReceiveParentNum], playerPos);

		bool existChild = true;
		if (receiveParentTransform [minReceiveParentNum + pageoffset * -8].childCount == 0 || minReceiveParentNum == 24 || minReceiveParentNum == 25)
			existChild = false;			// 指定枠に選手はいないので配置できる

		bool enptyPairPsition = false;
		string myPairRegDate = PairManager.getPairRegDate (myRegDate);
		int iPairPositionOffset = 0;
		if (myPairRegDate != null) {
			if (minReceiveParentNum == GameManager.PLACE_WAIT || minReceiveParentNum == GameManager.PLACE_BREAK) {
				enptyPairPsition = true;			// 自分はPair設定されていているが、待機中または休憩中への移動
			} else {
				iPairPositionOffset = minReceiveParentNum % SettingManager.form == 0 ? 1 : -1;				// 自分の位置からPairの位置のOffsetを計算
				if (GameManager.getPlaceOfPlayer (minReceiveParentNum + iPairPositionOffset) == null)
					enptyPairPsition = true;		// 自分はPair設定されており、Pair枠に選手はいない
			}
		} else
			enptyPairPsition = true;				// 自分はPair設定されていていない

		bool courtLock = false;
		if  (minReceiveParentNum != GameManager.PLACE_WAIT && minReceiveParentNum != GameManager.PLACE_BREAK) {	// 待機中、休憩中以外
			int iCourtStat = GameManager.chkCourtLockOfPosition (minReceiveParentNum + pageoffset * -8);
			if (iCourtStat == GameManager.LOCK || iCourtStat == -1)
				courtLock = true;		// Lockされているコートには配置できない
		}

		if (dis < limtDistance && !existChild && enptyPairPsition && !courtLock) {
			this.transform.parent.SetParent (receiveParentTransform[minReceiveParentNum + pageoffset * -8]);
			this.transform.parent.position = receiveParentPosition[minReceiveParentNum];
			GameManager.placeStat  = minReceiveParentNum + pageoffset * -8;
			if (myPairRegDate != null) {
				GameManager.posPlayerOfRegDate (myPairRegDate);			//Pairのワーク移動
				if (minReceiveParentNum == GameManager.PLACE_WAIT || minReceiveParentNum == GameManager.PLACE_BREAK)
					iPairPositionOffset = 0;
				
				GameManager.trsPlayer.SetParent (receiveParentTransform[minReceiveParentNum + pageoffset * -8 + iPairPositionOffset]);
				GameManager.trsPlayer.position = receiveParentPosition[minReceiveParentNum + iPairPositionOffset];
				GameManager.placeStat  = minReceiveParentNum + pageoffset * -8 + iPairPositionOffset;
				//Ring
				if (iPairPositionOffset == 0) {		// 0 のときは待機中か休憩中への移動
					this.transform.parent.FindChild ("Ring").gameObject.SetActive (false);
					GameManager.trsPlayer.FindChild ("Ring").gameObject.SetActive (true);
					GameManager.trsPlayer.SetAsLastSibling ();
				} else if (iPairPositionOffset == 1) {	
					this.transform.parent.FindChild ("Ring").gameObject.SetActive (false);
					GameManager.trsPlayer.FindChild ("Ring").gameObject.SetActive (true);
				} else {
					this.transform.parent.FindChild ("Ring").gameObject.SetActive (true);
					GameManager.trsPlayer.FindChild ("Ring").gameObject.SetActive (false);
				}
			}
		} else {
			this.transform.parent.SetParent (tfOrignParent);
			this.transform.parent.position = tfOrignParent.position;
			if (myPairRegDate != null) {
				GameManager.posPlayerOfRegDate (myPairRegDate);			//Pairのワーク移動
				int iOriginPlace = GameManager.getPlayerPlace (myPairRegDate);
				if (iOriginPlace == GameManager.PLACE_WAIT || iOriginPlace == GameManager.PLACE_BREAK) {
					this.transform.parent.FindChild ("Ring").gameObject.SetActive (false);
					GameManager.trsPlayer.FindChild ("Ring").gameObject.SetActive (true);
					GameManager.trsPlayer.SetAsLastSibling ();
				}
			}
		}

		scriptGameView.loadView ();
		//		Debug.Log ("["+minReceiveParentNum+"]  Minname  - > " + receiveParentTransform[minReceiveParentNum].name +"   "+dis);
	}

	private void setPlaceParent(Transform tf) {
	}

	void Update () {
		if (flgDrag) {
			for (int d = 0; d < distance.Length; d++) {
				if (receiveParentPosition [d] == Vector3.zero) 
					receiveParentPosition [d] = MaxPos;
				
				distance [d] = Mathf.Abs (Vector3.Distance (receiveParentPosition [d], playerPos));
//				Debug.Log ("     distance:" + distance [d] + "  mypos:" + playerPos+"  rev:"+receiveParentPosition [d]);
			}
			minDistance = Mathf.Min (distance);
			for (int i = 0; i < distance.Length; i++) {
				if (distance [i] == minDistance) {
					minReceiveParentNum = i;
//					Debug.Log ("minReceiveParentNum:" + minReceiveParentNum);
					break;
				}
			}
		}
	}

	void Start() {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager> ();
		scriptGameView = viewManager.scriptGameView;
		tfPlayPanel = GameObject.Find ("PlayPanel").transform;

//		receiveParentPosition = viewManager.scriptGameView.receiveParentPosition;
//		receiveParentTransform = viewManager.scriptGameView.receiveParentTransform;
// 	   	distance = new float[viewManager.scriptGameView.receiveParentPosition.Length];
		MaxPos = new Vector3 (99999f, 99999f);

	

		receiveParentPosition  = new Vector3[26];  // 待機休憩2 + コート*4 が必要  (2+最大6面*4=26)
		receiveParentTransform  = new Transform[receiveParentPosition.Length];
		receiveParentTransform[24] = GameObject.Find ("WaitLayoutVertical").transform;
		receiveParentTransform[25] = GameObject.Find ("BreakLayoutVertical").transform;
		receiveParentPosition [24] = GameObject.Find ("WaitListScroll").transform.position;
		receiveParentPosition [25] = GameObject.Find ("BreakListScroll").transform.position;
		distance = new float[receiveParentPosition.Length];
		setReciveCourtTrans(1, "ImgCourt1");
		setReciveCourtTrans(2, "ImgCourt2");
		setReciveCourtTrans(3, "ImgCourt3");
		setReciveCourtTrans(4, "ImgCourt4");
		setReciveCourtTrans(5, "ImgCourt5");
		setReciveCourtTrans(6, "ImgCourt6");
	}
	private void setReciveCourtTrans(int numCourt, string strParent) {
		int num = numCourt * 4 - 4 ;	//1は待機休憩2のオフセット(0,1の2個分)
		receiveParentTransform[num] = GameObject.Find (strParent+"/CourtL/Top").transform;
		receiveParentPosition [num] = receiveParentTransform [num].position;
		receiveParentTransform[++num] = GameObject.Find (strParent+"/CourtL/Btm").transform;
		receiveParentPosition [num] = receiveParentTransform [num].position;
		receiveParentTransform[++num] = GameObject.Find (strParent+"/CourtR/Top").transform;
		receiveParentPosition [num] = receiveParentTransform [num].position;
		receiveParentTransform[++num] = GameObject.Find (strParent+"/CourtR/Btm").transform;
		receiveParentPosition [num] = receiveParentTransform [num].position;
	}
} 
