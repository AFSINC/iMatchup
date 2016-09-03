using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GamePlayer : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
	private ViewManager viewManager;
	public string myRegDate;					// prefab生成時にscriptGameViewから値がセットされる
	public ScrollRect rcvParentTfmWait;	// prefab生成時にscriptGameViewから値がセットされる
	public ScrollRect rcvParentTfmBreak;	// prefab生成時にscriptGameViewから値がセットされる
	private PageScrollRect pageScrollRect;	// PageScrollRectコンポーネント
	private EventLongPress eventLP;
	private bool longPressFlg = false;
	private ScriptGameView scriptGameView;
	private Transform tfOrignParent;
    private bool flgMyPariMk;
	private Transform tfPlayPanel;
	private Vector3 parentPos, posOffset, playerPos, MaxPos;
	private Shadow sd = null;
	private Outline ol = null;
	private bool flgDrag = false;
	private float limtDistance = 500f;
	private Transform[] receiveParentTransform;
	private Vector3[] receiveParentPosition;
	private int minReceiveParentNum;
	private float[] distance;
	private float minDistance;

	void OnLongPressPara(PointerEventData e) {
		if (flgDrag)
			return;

		if (!longPressFlg) {
			if (GameManager.chkLock (myRegDate) == GameManager.LOCK) {
				longPressFlg = false;
				return;
			} else {
				longPressFlg = true;
			}
			playerPos = this.transform.position;

			Transform pp = this.transform;
			pp.gameObject.AddComponent<Shadow> ();
			sd = pp.GetComponent<Shadow> ();
			sd.effectColor = new Vector4 (0f, 0f, 0f, 20f / 255f);
			sd.effectDistance = new Vector2 (20f, -30f);
			pp.gameObject.AddComponent<Outline> ();
			ol = pp.GetComponent<Outline> ();
			ol.effectColor = new Vector4 (0f, 0f, 0f, 30f / 255f);
			ol.effectDistance = new Vector2 (3.0f, 3.0f);
			ol.useGraphicAlpha = true;

			tfOrignParent = this.transform.parent;
			this.transform.SetParent (tfPlayPanel);

			parentPos.x = this.transform.position.x;
			parentPos.y = this.transform.position.y;
			posOffset.x = this.transform.position.x - e.position.x;
			posOffset.y = this.transform.position.y - e.position.y;
			this.transform.SetAsLastSibling ();

			// PairがいたらRingを移動のときに消す
			string myPairRegDate = PairManager.getPairRegDate (myRegDate);
			if (myPairRegDate != null) {
				GameManager.posPlayerOfRegDate (myPairRegDate);			//Pairのワーク移動
                flgMyPariMk = this.transform.FindChild("Ring").gameObject.activeSelf;   // 自分のRing有無を保存
                this.transform.FindChild ("Ring").gameObject.SetActive (false);
				GameManager.trsPlayer.FindChild ("Ring").gameObject.SetActive (false);
			}
		}
	}

	public void OnLongPressExit(PointerEventData e) {
		if (longPressFlg)
			OnEndDrag(e);
	}

	public void OnBeginDrag(PointerEventData e) {
		if (longPressFlg) {
			eventLP.enableLongPress = false;
			rcvParentTfmWait.vertical = false;		//　scrollRsctの停止
			rcvParentTfmBreak.vertical = false;		//　scrollRsctの停止
		} else {
			int iPlaceNum = GameManager.getPlayerPlace (myRegDate);
			if (iPlaceNum == GameManager.PLACE_WAIT)
				transform.DoParentEventSystemHandler<IBeginDragHandler> ((parent) => {rcvParentTfmWait.OnBeginDrag (e);	});
			else if (iPlaceNum == GameManager.PLACE_BREAK)
				transform.DoParentEventSystemHandler<IBeginDragHandler> ((parent) => {rcvParentTfmBreak.OnBeginDrag (e);});
			else
				transform.DoParentEventSystemHandler<IBeginDragHandler>((parent) => { pageScrollRect.OnBeginDrag(e); });
		}
	}

	public void OnDrag(PointerEventData e) {
		flgDrag = true;
		if (longPressFlg) {
			playerPos.x = posOffset.x + e.position.x;
			playerPos.y = posOffset.y + e.position.y;

			// playPanel の中だけ移動できるようにする
			RectTransform pp = this.transform.GetComponent<RectTransform> ();
			Vector3 wh = pp.TransformVector (new Vector3 (pp.rect.width, pp.rect.height));
			float wOffset = wh.x / 2;
			float hOffset = wh.y / 2;

			float px = tfPlayPanel.position.x;
			float py = tfPlayPanel.position.y;
			RectTransform rt = tfPlayPanel.GetComponent<RectTransform> ();
			Vector3 pwh = rt.TransformVector (new Vector3 (rt.rect.width, rt.rect.height));
			playerPos = new Vector3 (Mathf.Clamp (playerPos.x, wOffset, pwh.x - wOffset), Mathf.Clamp (playerPos.y, py - pwh.y / 2 + hOffset, pwh.y + py - pwh.y / 2 - hOffset));

			this.transform.position = playerPos;
		} else {
			int iPlaceNum = GameManager.getPlayerPlace (myRegDate);
			if (iPlaceNum == GameManager.PLACE_WAIT)
				transform.DoParentEventSystemHandler<IBeginDragHandler> ((parent) => {rcvParentTfmWait.OnDrag (e);});
			else if (iPlaceNum == GameManager.PLACE_BREAK)
				transform.DoParentEventSystemHandler<IBeginDragHandler> ((parent) => {rcvParentTfmBreak.OnDrag (e);});
			else
				transform.DoParentEventSystemHandler<IBeginDragHandler>((parent) => { pageScrollRect.OnDrag(e); });
		}
	}
	public void OnEndDrag(PointerEventData e) {
		flgDrag = false;

		if (longPressFlg) {
			Object.Destroy (sd);
			sd = null;
			Object.Destroy (ol);
			ol = null;

			int pageoffset = 0;			// 0:1page  -1:2page  -2:3page
			int MAXPLAYEROFPAGE = 8;
			if (minReceiveParentNum > 23) {
				pageoffset = 0;
			} else {
				pageoffset = GameManager.currentPage * (-1);			// 何ページ目かを計算する  page0:0~7    page1:8~15   page2:16~23
			}
            //			Debug.Log (" position num on page   min[" + minReceiveParentNum +"]  +  ["+ pageoffset * -8+"]  pageoffst:" + pageoffset);

            /// 各種移動前に可能か判定
            GameManager.posPlayerOfRegDate (myRegDate);
            // 最小となる移動先までの距離 (離れすぎていると移動しないように)
			float dis;
			dis = Vector3.Distance (receiveParentPosition [minReceiveParentNum], playerPos);


			// 入れ替え用変更
			// pairの有無 (pairがいる場合は、ダブルスであることが保証されていること)
			bool toWaitBreak = false;
			bool courtLock = false;
			bool flgYouPair = false;
            int YouLock = GameManager.UNLOCK;
            int YouPairLock = GameManager.UNLOCK;
			string youRegDate = null;
            string youPairRegDate = null;
			int youPosNum;
            int iYouPairPositionOffset = 0;
			bool flgMyPair = false;
			int myPairLock = GameManager.UNLOCK;
			string myPairRegDate;
            int myPosNum;
            int iMyPairPositionOffset = 0;


			// 自選手とpair位置にいる選手の存在有無と、pair判定、2人のLockの状況
			myPosNum = GameManager.getPlayerPlace(myRegDate);
			myPairRegDate = PairManager.getPairRegDate(myRegDate);
			if (myPairRegDate != null)
				flgMyPair = true;
			else
				flgMyPair = false;

			if (myPosNum != GameManager.PLACE_WAIT && myPosNum != GameManager.PLACE_BREAK) { // 自分は待機、休憩中ではない コートでのpair位置にいる選手
				iMyPairPositionOffset = myPosNum % SettingManager.form == 0 ? 1 : -1;             // 自分の位置からPairの位置のOffset   上枠にpair 1:下枠にpair
				myPairRegDate = GameManager.getPlaceOfPlayer(myPosNum + iMyPairPositionOffset);
				myPairLock = GameManager.chkLock(myPairRegDate);
			}

			// 移動先の選手とpair位置にいる選手の存在有無と、pair判定、2人のLockの状況
			if (minReceiveParentNum != GameManager.PLACE_WAIT && minReceiveParentNum != GameManager.PLACE_BREAK) {
				toWaitBreak = false;

				// 移動先にいる選手とpair位置にいる選手の存在有無と、pair判定、2人のLockの状況
				youPosNum = minReceiveParentNum + pageoffset * MAXPLAYEROFPAGE;
				iYouPairPositionOffset = youPosNum % SettingManager.form == 0 ? 1 : -1;     // 相手の位置からPairの位置のOffset  -1:上枠にpair 1:下枠にpair
				youRegDate = GameManager.getPlaceOfPlayer(youPosNum);
				if (youRegDate != null) {   // 移動先に選手がいる
					YouLock = GameManager.chkLock(youRegDate);
					youPairRegDate = PairManager.getPairRegDate(youRegDate);
					if (youPairRegDate != null) { // 相手にpairがいる
						flgYouPair = true;
						YouPairLock = YouLock;
					}	else {                                 // 相手にpairがいない
						flgYouPair = false;
						youPairRegDate = GameManager.getPlaceOfPlayer(youPosNum + iYouPairPositionOffset);
						if (youPairRegDate != null) // 相手にpairはいないが選手がいる
							YouPairLock = GameManager.chkLock(youPairRegDate);
					}
				}	else {                                    // 移動先に選手がいない
					flgYouPair = false;
					youPairRegDate = GameManager.getPlaceOfPlayer(youPosNum + iYouPairPositionOffset);
					if (youPairRegDate != null) // 移動先に選手がいないがpair枠に選手がいる
						YouPairLock = GameManager.chkLock(youPairRegDate);
				}

				// Lockされているコートには配置できない
				int iCourtStat = GameManager.chkCourtLockOfPosition(minReceiveParentNum + pageoffset * MAXPLAYEROFPAGE);
				if (iCourtStat == GameManager.LOCK || iCourtStat == -1)
					courtLock = true;

				// 自分がPairのとき、移動先とPair枠の選手のLock状態を判定して、どちらかでもLockされているなら移動できない
				if (flgMyPair) {
					if (YouLock == GameManager.LOCK || YouPairLock == GameManager.LOCK)
						courtLock = true;
				} else {
					if (YouLock == GameManager.LOCK)
						courtLock = true;
				}
				if (flgYouPair) {
					if (myPairLock == GameManager.LOCK)
						courtLock = true;
				}
			} else {
				// 移動先が休憩中か待機中 (無条件で移動できる)
				toWaitBreak = true;
			}

			// 選手の移動
			if (dis < limtDistance && !courtLock) {	// 必ず移動できることが保証されていること
				if (toWaitBreak) {          // 待機中と休憩中へは無条件で移動 minReceiveParentNum=24,25
					this.transform.SetParent(receiveParentTransform[minReceiveParentNum]);
					this.transform.position = receiveParentPosition[minReceiveParentNum];
					GameManager.placeStat = minReceiveParentNum;
					if (flgMyPair) {
						GameManager.posPlayerOfRegDate(myPairRegDate);          //Pairのワーク移動
						GameManager.trsPlayer.SetParent(receiveParentTransform[minReceiveParentNum]);
						GameManager.trsPlayer.position = receiveParentPosition[minReceiveParentNum];
						GameManager.placeStat = minReceiveParentNum;
						//Ring
						this.transform.FindChild("Ring").gameObject.SetActive(false);
						GameManager.trsPlayer.FindChild("Ring").gameObject.SetActive(true);
						GameManager.trsPlayer.SetAsLastSibling();
					}
				} else {
					// 自分達の移動
					this.transform.SetParent(receiveParentTransform[minReceiveParentNum + pageoffset * MAXPLAYEROFPAGE]);
					this.transform.position = receiveParentPosition[minReceiveParentNum];
					GameManager.placeStat = minReceiveParentNum + pageoffset * MAXPLAYEROFPAGE;
					if (flgMyPair || flgYouPair) {
						if (myPairRegDate != null) {
							GameManager.posPlayerOfRegDate(myPairRegDate);          //Pairのワーク移動
							GameManager.trsPlayer.SetParent(receiveParentTransform[minReceiveParentNum + pageoffset * MAXPLAYEROFPAGE + iYouPairPositionOffset]);
							GameManager.trsPlayer.position = receiveParentPosition[minReceiveParentNum + iYouPairPositionOffset];
							GameManager.placeStat = minReceiveParentNum + pageoffset * MAXPLAYEROFPAGE + iYouPairPositionOffset;
						}
						//Ring
						if (flgMyPair && iYouPairPositionOffset == 1) {
							this.transform.FindChild("Ring").gameObject.SetActive(false);
							GameManager.trsPlayer.FindChild("Ring").gameObject.SetActive(true);
						} else if (flgMyPair && iYouPairPositionOffset == -1) {
							this.transform.FindChild("Ring").gameObject.SetActive(true);
							GameManager.trsPlayer.FindChild("Ring").gameObject.SetActive(false);
						}
					}
					// 相手達の移動
					if (youRegDate != null) {
						GameManager.posPlayerOfRegDate(youRegDate);          //相手選手のワーク移動
						GameManager.trsPlayer.SetParent(receiveParentTransform[myPosNum]);
						GameManager.trsPlayer.position = receiveParentPosition[myPosNum - pageoffset * MAXPLAYEROFPAGE];
						GameManager.placeStat = myPosNum;
					}
					if (flgMyPair || flgYouPair) {
						if (youPairRegDate != null) {
							GameManager.posPlayerOfRegDate(youPairRegDate);          //相手Pairのワーク移動
							GameManager.trsPlayer.SetParent(receiveParentTransform[myPosNum + iMyPairPositionOffset]);
							GameManager.trsPlayer.position = receiveParentPosition[myPosNum - pageoffset * MAXPLAYEROFPAGE + iMyPairPositionOffset];
							GameManager.placeStat = myPosNum + iMyPairPositionOffset;
						}
						//Ring
						if (flgYouPair && (myPosNum == GameManager.PLACE_WAIT || myPosNum == GameManager.PLACE_BREAK)) {          //相手選手を待機中、休憩中への移動
							GameManager.posPlayerOfRegDate(youRegDate);          //相手選手のワーク移動
							GameManager.trsPlayer.FindChild("Ring").gameObject.SetActive(false);
							GameManager.posPlayerOfRegDate(youPairRegDate);          //相手Pairのワーク移動
							GameManager.trsPlayer.FindChild("Ring").gameObject.SetActive(true);
							GameManager.trsPlayer.SetAsLastSibling();
						}
						else if (flgYouPair && iMyPairPositionOffset == 1) {
							GameManager.posPlayerOfRegDate(youRegDate);          //相手選手のワーク移動
							GameManager.trsPlayer.FindChild("Ring").gameObject.SetActive(false);
							GameManager.posPlayerOfRegDate(youPairRegDate);          //相手Pairのワーク移動
							GameManager.trsPlayer.FindChild("Ring").gameObject.SetActive(true);
						} else if (flgYouPair && iMyPairPositionOffset == -1) {
							GameManager.posPlayerOfRegDate(youRegDate);          //相手選手のワーク移動
							GameManager.trsPlayer.FindChild("Ring").gameObject.SetActive(true);
							GameManager.posPlayerOfRegDate(youPairRegDate);          //相手Pairのワーク移動
							GameManager.trsPlayer.FindChild("Ring").gameObject.SetActive(false);
						}
					}
				}
			}	else {		// 移動できない
				this.transform.SetParent(tfOrignParent);
				this.transform.position = tfOrignParent.position;
				if (flgMyPair) {
					GameManager.posPlayerOfRegDate(myPairRegDate);          //Pairのワーク移動
					int iOriginPlace = GameManager.getPlayerPlace(myPairRegDate);
					if (iOriginPlace == GameManager.PLACE_WAIT || iOriginPlace == GameManager.PLACE_BREAK) {
						this.transform.FindChild("Ring").gameObject.SetActive(false);
						GameManager.trsPlayer.FindChild("Ring").gameObject.SetActive(true);
						GameManager.trsPlayer.SetAsLastSibling();
					}
					else {
						if (flgMyPariMk)
							this.transform.FindChild("Ring").gameObject.SetActive(true);
						else
							GameManager.trsPlayer.FindChild("Ring").gameObject.SetActive(true);
					}
				}
			}
			rcvParentTfmWait.vertical = true;	//　scrollRsctの再開
			rcvParentTfmBreak.vertical = true;	//　scrollRsctの再開
			longPressFlg = false;
			rcvParentTfmWait.verticalNormalizedPosition = 1;
			rcvParentTfmBreak.verticalNormalizedPosition = 1;
			scriptGameView.loadView ();
		}

		int iPlaceNum = GameManager.getPlayerPlace (myRegDate);
		if (iPlaceNum == GameManager.PLACE_WAIT)
			transform.DoParentEventSystemHandler<IBeginDragHandler> ((parent) => {rcvParentTfmWait.OnEndDrag (e);});
		else if (iPlaceNum == GameManager.PLACE_BREAK)
			transform.DoParentEventSystemHandler<IBeginDragHandler> ((parent) => {rcvParentTfmBreak.OnEndDrag (e);});
		else
			transform.DoParentEventSystemHandler<IBeginDragHandler>((parent) => { pageScrollRect.OnEndDrag(e); });
		eventLP.enableLongPress = true;
	}
	void onDoubleTap() {
		// 待機中、休憩中はLockできない
		int iPlaceNum = GameManager.getPlayerPlace (myRegDate);
		if (iPlaceNum != GameManager.PLACE_WAIT && iPlaceNum != GameManager.PLACE_BREAK) {
			int iCourtNum = GameManager.getCourtOfRegdate (myRegDate);
			// コートがLockされていたら、選手単位ではLock,unLockはできない
			if (iCourtNum < SettingManager.courtNum && GameManager.chkCourtLockOfCortnum (iCourtNum) == GameManager.UNLOCK) {
				if (GameManager.chkLock (myRegDate) == GameManager.LOCK) {
					this.transform.FindChild ("Lock").gameObject.SetActive (false);
					GameManager.setLock (myRegDate, GameManager.UNLOCK);
					// Pairも同じようにLock解除する
					string myPairRegDate = PairManager.getPairRegDate (myRegDate);
					if (myPairRegDate != null) {
						GameManager.getTransformOfRegDate (myPairRegDate).FindChild ("Lock").gameObject.SetActive (false);
						GameManager.setLock (myPairRegDate, GameManager.UNLOCK);
					}

					viewManager.scriptGameView.loadView ();
				} else {
					this.transform.FindChild ("Lock").gameObject.SetActive (true);
					GameManager.setLock (myRegDate, GameManager.LOCK);
					// Pairも同じようにLockする
					string myPairRegDate = PairManager.getPairRegDate (myRegDate);
					if (myPairRegDate != null) {
						GameManager.getTransformOfRegDate (myPairRegDate).FindChild ("Lock").gameObject.SetActive (true);
						GameManager.setLock (myPairRegDate, GameManager.LOCK);
					}
				}
			}
		} else {
			this.transform.FindChild ("Lock").gameObject.SetActive (false);
			GameManager.setLock (myRegDate, GameManager.UNLOCK);
		}
		GameManager.Save ();
	}

	void Update () {
		if (longPressFlg) {
			for (int d = 0; d < distance.Length; d++) {
				if (receiveParentPosition [d] == Vector3.zero) 
					receiveParentPosition [d] = MaxPos;

				distance [d] = Mathf.Abs (Vector3.Distance (receiveParentPosition [d], playerPos));
//							Debug.Log ("     distance:" + distance [d] + "  mypos:" + playerPos+"  rev:"+receiveParentPosition [d]);
			}
			minDistance = Mathf.Min (distance);
			for (int i = 0; i < distance.Length; i++) {
				if (distance [i] == minDistance) {
					minReceiveParentNum = i;
//					minReceiveParentNum = i + GameManager.currentPage * -8;
//					Debug.Log ("minReceiveParentNum:" + minReceiveParentNum);
					break;
				}
			}
			for (int ii = 0; ii < distance.Length; ii++) {
			}
		}
	}

	void Start () {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
		pageScrollRect = GameObject.Find ("SclGame").transform.GetComponent<PageScrollRect>();

		this.gameObject.AddComponent (typeof(EventDoubleTap));
		GetComponent<EventDoubleTap>().onDoubleTap.AddListener(onDoubleTap);
		this.gameObject.AddComponent (typeof(EventLongPress));
		eventLP = GetComponent<EventLongPress> ();
		eventLP.onLongPressPara.AddListener(OnLongPressPara);
		eventLP.onLongPressParaExit.AddListener(OnLongPressExit);
		eventLP.intervalAction = 0.07f;		// LongPress判定時間

		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager> ();
		scriptGameView = viewManager.scriptGameView;
		tfPlayPanel = GameObject.Find ("PlayPanel").transform;
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
