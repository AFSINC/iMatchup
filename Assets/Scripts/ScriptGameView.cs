 using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScriptGameView : MonoBehaviour {
	private class _Prefab {
		public Transform trsPrefab;
		public string regDate;
	} 
	public GameObject myView;
	public GameObject pfbPlayerOrgin;
	public GameObject[] pfbPlayer;
	private ViewManager viewManager;

	private Text execProgess;
	private Text court1Progess;
	private Text court2Progess;
	private Text court3Progess;
	private Text court4Progess;
	private Text court5Progess;
	private Text court6Progess;

	private bool flgNotInit = false;					// appli起動初回実行禁止フラグ
//	private bool flgInit = true;						// 初回表示フラグ
	private Transform trsStockPanel;			// 選手プレートのストック場所のTransform
//	public Vector3 trsStockPanelPosition;		// 選手プレートのストック場所の座標
	public Transform[] receiveParentTransform;
	public Vector3[] receiveParentPosition;
	public Transform trsWaitParent;				// 計算前に選手プレートを配置する"待機中"スクロール
	private PageScrollRect pageScrollRect;	// PageScrollRectコンポーネント
	private bool flgForceEnd = false;				// 実行終了時のコート試合強制終了フラグ
	private GameObject imgArrowCourt1Right;	// コート1の右矢印
	private GameObject imgArrowCourt2Left;		// コート2の左矢印
	private GameObject imgArrowCourt2Right;	// コート2の右矢印
	private GameObject imgArrowCourt3Left;		// コート3の左矢印
	private AudioSource audioSrc;

	public void _CallBackBtnCancel() {
		GameManager.Save ();
		viewManager.scriptMainView.loadView ();
		viewManager.chgMainView(myView, viewManager.NONE);
	}
	public void _CallBackBtnCourt1Right() {
		pageScrollRect.setPage (-1);
	}
	public void _CallBackBtnCourt2Left() {
		pageScrollRect.setPage (0);
	}
	public void _CallBackBtnCourt2Right() {
		pageScrollRect.setPage (-2);
	}
	public void _CallBackBtnCourt3Left() {
		pageScrollRect.setPage (-1);
	}
		
	public void _CallBtnFix(Transform fncGroup) {			// 確定(Lock)
		SoundManager.play (SoundManager.CLICK02);
		string strCourt = "ImgCourt" + fncGroup.name.Substring (fncGroup.name.Length - 1);	// コート色
		fncGroup.parent.FindChild (strCourt).GetComponent<Image>().color = Colors.lockCourt;

		int courtNum = int.Parse(fncGroup.name.Substring (fncGroup.name.Length - 1)) - 1;
		fncGroup.FindChild ("FncFixFree/BtnFree").GetComponent<Button>().interactable = true;
		fncGroup.FindChild ("FncStartEnd/BtnStart").SetAsLastSibling ();
		fncGroup.FindChild ("FncStartEnd/BtnStart").GetComponent<Button>().interactable = false;
		int iPlayerCount = GameManager.GetPlayerCountInCourt (courtNum);
		if (iPlayerCount == 11 || iPlayerCount == 22) 
			fncGroup.FindChild ("FncStartEnd/BtnStart").GetComponent<Button>().interactable = true;
		GameManager.setLock (courtNum);
		GameManager.Save ();
	}
	public void _CallBtnFree(Transform fncGroup) {			// 解除(Free)
		SoundManager.play (SoundManager.CLICK02);
		string strCourt = "ImgCourt" + fncGroup.name.Substring (fncGroup.name.Length - 1);	// コート色
		fncGroup.parent.FindChild (strCourt).GetComponent<Image>().color = Colors.unLockCourt;

		int courtNum = int.Parse(fncGroup.name.Substring (fncGroup.name.Length - 1)) - 1;
		fncGroup.FindChild ("FncStartEnd/BtnFix").SetAsLastSibling ();
		fncGroup.FindChild ("FncFixFree/BtnFree").GetComponent<Button>().interactable = false;
		GameManager.freeLock (courtNum);
		if (GameManager.gameStatus != 0)
			loadView ();
		GameManager.Save ();
	}
	public void _CallBtnStop(Transform fncGroup) {			// 中止
		SoundManager.play (SoundManager.CLICK02);
		string title = "試合中止の確認";
		string message = "\n試合を中止します。\n";
		DialogViewController.Show (title, message, new DialogViewOptions {
			btnCancelTitle = "キャンセル", btnCancelDelegate = () => {
				return;
			},
			btnOkTitle = "試合中止", btnOkDelegate = () => {
				Stop (fncGroup);
			},
		});
	}
	private void Stop(Transform fncGroup) {
		string strCourt = "ImgCourt" + fncGroup.name.Substring (fncGroup.name.Length - 1);	// コート色
		fncGroup.parent.FindChild (strCourt).GetComponent<Image>().color = Colors.unLockCourt;

		int courtNum = int.Parse(fncGroup.name.Substring (fncGroup.name.Length - 1)) - 1;
		fncGroup.FindChild ("FncStartEnd/BtnFix").SetAsLastSibling ();
		fncGroup.FindChild ("FncFixFree/BtnFree").SetAsLastSibling ();
		fncGroup.FindChild ("FncFixFree/BtnFree").GetComponent<Button>().interactable = false;
		GameManager.freeLock (courtNum);
//		if (flgForceEnd) {		// 組合せ終了から強制停止として呼ばれた
			loadView ();
			GameManager.Save ();
//		}
	}
	public void _CallBtnStart(Transform fncGroup) {		// 試合開始(コート単位)
		SoundManager.play (SoundManager.CLICK02);
		string strCourt = "ImgCourt" + fncGroup.name.Substring (fncGroup.name.Length - 1);	// コート色
		fncGroup.parent.FindChild (strCourt).GetComponent<Image>().color = Colors.playCourt;

		int courtNum = int.Parse(fncGroup.name.Substring (fncGroup.name.Length - 1)) - 1;
		fncGroup.FindChild ("FncFixFree/BtnStop").SetAsLastSibling ();
		fncGroup.FindChild ("FncStartEnd/BtnEnd").SetAsLastSibling ();
		GameManager.setStartCourt (courtNum);
		GameManager.Save ();
	}
	public void _CallBtnEnd(Transform fncGroup) {			// 試合終了(コート単位)
		SoundManager.play (SoundManager.CLICK02);

		int courtNum = int.Parse (fncGroup.name.Substring (fncGroup.name.Length - 1)) - 1;
		viewManager.scriptResultView.fncGroup = fncGroup;
		viewManager.scriptResultView.courtName = fncGroup.FindChild ("Text").GetComponent<Text> ().text;
		viewManager.scriptResultView.gameStartDate = GameManager.getMatchStartDateOfCortnum (courtNum);

		viewManager.scriptResultView.regDateLTop = "";
		viewManager.scriptResultView.regDateLBtm = "";
		viewManager.scriptResultView.regDateRTop = "";
		viewManager.scriptResultView.regDateRBtm = "";
		viewManager.scriptResultView.memberLTopFm = "";
		viewManager.scriptResultView.memberLTopFs = "";
		viewManager.scriptResultView.memberLBtmFm = "";
		viewManager.scriptResultView.memberLBtmFs = "";
		viewManager.scriptResultView.memberRTopFm = "";
		viewManager.scriptResultView.memberRTopFs = "";
		viewManager.scriptResultView.memberRBtmFm = "";
		viewManager.scriptResultView.memberRBtmFs = "";
		viewManager.scriptResultView.memberKLTopFm = "";
		viewManager.scriptResultView.memberKLTopFs = "";
		viewManager.scriptResultView.memberKLBtmFm = "";
		viewManager.scriptResultView.memberKLBtmFs = "";
		viewManager.scriptResultView.memberKRTopFm = "";
		viewManager.scriptResultView.memberKRTopFs = "";
		viewManager.scriptResultView.memberKRBtmFm = "";
		viewManager.scriptResultView.memberKRBtmFs = "";
		string regDateLT = "";
		if ((regDateLT = GameManager.getPlaceOfPlayer (courtNum * 4 + 0)) != null) {
			GameManager.posPlayerOfRegDate (regDateLT);
			viewManager.scriptResultView.regDateLTop = GameManager.regDate;
			viewManager.scriptResultView.memberLTopFm = GameManager.nameKaji_family;
			viewManager.scriptResultView.memberLTopFs = GameManager.nameKaji_first;
			viewManager.scriptResultView.memberKLTopFm = GameManager.nameKana_family;
			viewManager.scriptResultView.memberKLTopFs = GameManager.nameKana_first;
		} 
		string regDateLB = "";
		if ((regDateLB = GameManager.getPlaceOfPlayer (courtNum * 4 + 1)) != null) {
			GameManager.posPlayerOfRegDate (regDateLB);
			viewManager.scriptResultView.regDateLBtm = GameManager.regDate;
			viewManager.scriptResultView.memberLBtmFm = GameManager.nameKaji_family;
			viewManager.scriptResultView.memberLBtmFs = GameManager.nameKaji_first;
			viewManager.scriptResultView.memberKLBtmFm = GameManager.nameKana_family;
			viewManager.scriptResultView.memberKLBtmFs = GameManager.nameKana_first;
		} 
		string regDateRT = "";
		if ((regDateRT = GameManager.getPlaceOfPlayer (courtNum * 4 + 2)) != null) {
			GameManager.posPlayerOfRegDate (regDateRT);
			viewManager.scriptResultView.regDateRTop = GameManager.regDate;
			viewManager.scriptResultView.memberRTopFm = GameManager.nameKaji_family;
			viewManager.scriptResultView.memberRTopFs = GameManager.nameKaji_first;
			viewManager.scriptResultView.memberKRTopFm = GameManager.nameKana_family;
			viewManager.scriptResultView.memberKRTopFs = GameManager.nameKana_first;
		} 
		string regDateRB = "";
		if ((regDateRB = GameManager.getPlaceOfPlayer (courtNum * 4 + 3)) != null) {
			GameManager.posPlayerOfRegDate (regDateRB);
			viewManager.scriptResultView.regDateRBtm = GameManager.regDate;
			viewManager.scriptResultView.memberRBtmFm = GameManager.nameKaji_family;
			viewManager.scriptResultView.memberRBtmFs = GameManager.nameKaji_first;
			viewManager.scriptResultView.memberKRBtmFm = GameManager.nameKana_family;
			viewManager.scriptResultView.memberKRBtmFs = GameManager.nameKana_first;
		}		
			
		if (SettingManager.sum == 0) {		// 集計なし
			string title = "試合終了の確認";
			string message = "\n試合を終了します。\n";
			DialogViewController.Show (title, message, new DialogViewOptions {
				btnCancelTitle = "キャンセル", btnCancelDelegate = () => { 
					return;
				},
				btnOkTitle = "試合終了", btnOkDelegate = () => {
					viewManager.scriptResultView.saveGamePlus();	// 集計なしならPair,Match,Game数のカウントアップのみ行う
					afterResultDialog (fncGroup);
				},
			});
			return;
		} else {		// 集計ありならResultViewで処理する
			viewManager.openResultView ();
			// ここではResultDialogの結果が不明のためLock解除ができないので、openResultViewから呼ばれるafterResultDialog で処理する
		}
	}


	public void afterResultDialog(Transform fncGroup) {		// 集計ありはResultダイアログ結果でOKの場合はscriptResultView から、集計なしは_CallBtnEndから呼び出される
		int courtNum = int.Parse (fncGroup.name.Substring (fncGroup.name.Length - 1)) - 1;
		string strCourt = "ImgCourt" + fncGroup.name.Substring (fncGroup.name.Length - 1);	// コート色
		fncGroup.parent.FindChild (strCourt).GetComponent<Image> ().color = Colors.unLockCourt;

		// 試合結果の保存
		GameManager.gameCount++;
		this.transform.FindChild ("ModePanel/lblGame").GetComponent<Text> ().text = "試合:" + GameManager.gameCount;

		fncGroup.FindChild ("FncStartEnd/BtnFix").SetAsLastSibling ();
		fncGroup.FindChild ("FncFixFree/BtnFree").SetAsLastSibling ();
		fncGroup.FindChild ("FncFixFree/BtnFree").GetComponent<Button> ().interactable = false;
		GameManager.freeLock (courtNum);
		loadView ();
		GameManager.Save ();
	}
	public void _CallBtnManualMode(Transform modeGroup) {
		SoundManager.play (SoundManager.CLICK02);
		modeGroup.FindChild ("btnAutoMode").SetAsLastSibling ();
		GameManager.autoMode = 0;
		GameManager.Save ();
	}
	public void _CallBtnAutoMode(Transform modeGroup) {
		SoundManager.play (SoundManager.CLICK02);
		modeGroup.FindChild ("btnManualMode").SetAsLastSibling ();
		GameManager.autoMode = 1;
		loadView ();
		GameManager.Save ();
	}
	public void _CallBtnGameStart(Transform StartGroup) {	// 実行開始 (組合せ開始)
		SoundManager.play (SoundManager.CLICK02);
		this.transform.FindChild ("ModePanel/ModeGroup/btnAutoMode").GetComponent<Button>().interactable = true;
		this.transform.FindChild ("ModePanel/ModeGroup/btnManualMode").GetComponent<Button>().interactable = true;
		// 前回最終データの集計終了状態を確認してResultViewに開始日付をセットする
		string strDate = "";
		if (GameManager.gameStatus == 0)
			strDate = DateTime.Now.ToString ("s", new System.Globalization.CultureInfo ("ja-JP"));
		else
			strDate = GameManager.gameStartDate;
		viewManager.scriptResultView.chkSumDate (strDate);
		// 開始時間を表示
		string strStart = strDate.Substring (5, 2) + "月" + strDate.Substring (8, 2) + "日 " + strDate.Substring (11, 2) + "時" + strDate.Substring (14, 2) + "分";
		this.transform.FindChild ("MainPanel/PlayPanel/txtStart").GetComponent<Text> ().text = "組合せ開始: " + strStart;

		StartGroup.FindChild ("btnEnd").SetAsLastSibling ();

		GameManager.gameStartDate = strDate;
		// 全選手のGame数表示をゼロにする
		for (int cnt=0; cnt<GameManager.getPlayerCount(); cnt++) {
			GameManager.posPlayerOfListIndex (cnt);
			GameManager.getTransformOfRegDate (GameManager.regDate).FindChild ("txtGame").GetComponent<Text> ().text = "0";
		}

		dispExec(true);
		if (GameManager.autoMode == 0)
			allWait ();
		GameManager.gameStatus = SettingManager.sum + 1;
		loadView ();
		GameManager.Save ();
	}
	public void _CallBtnGameEnd(Transform StartGroup) {	// 実行終了 (組合せ終了 & 集計)
		SoundManager.play (SoundManager.CLICK02);
//		if (GameManager.gameCount != 0) {
		if (true) {
			string title = "試合組合せの終了確認";
			string message = "試合組合せの実行を終了します。\n試合途中の場合はキャンセルされますのでご注意下さい。";
			DialogViewController.Show (title, message, new DialogViewOptions {
				btnCancelTitle = "キャンセル", btnCancelDelegate = () => {
					return;
				},
				btnOkTitle = "実行終了", btnOkDelegate = () => {
					flgForceEnd = true;
					viewManager.scriptResultView.gameEndResult(DateTime.Now.ToString ("s", new System.Globalization.CultureInfo("ja-JP")));
					for (int cnt = 0; cnt < SettingManager.courtNum; cnt++) {
						string imgf = "MainPanel/PlayPanel/SclGame/Viewport/Content/PageLayout" + (cnt / 2 + 1) + "/CourtAreaSpace" + (cnt % 2 + 1) + "/ImgFnc" + (cnt + 1);
						Transform fncG = this.transform.FindChild (imgf);
						Stop (fncG);
					}
					flgForceEnd = false;
					_GameEnd (StartGroup);
				},
			});
		} else {
			flgForceEnd = true;
			for (int cnt = 0; cnt < SettingManager.courtNum; cnt++) {
				string imgf = "MainPanel/PlayPanel/SclGame/Viewport/Content/PageLayout" + (cnt / 2 + 1) + "/CourtAreaSpace" + (cnt % 2 + 1) + "/ImgFnc" + (cnt + 1);
				Transform fncG = this.transform.FindChild (imgf);
				Stop (fncG);
			}
			flgForceEnd = false;
			_GameEnd (StartGroup);
		}
	}
	private void _GameEnd(Transform StartGroup) {	// 実行終了が選択された場合の処理 (組合せ終了 & 集計)
		this.transform.FindChild ("ModePanel/ModeGroup/btnAutoMode").GetComponent<Button>().interactable = false;
		this.transform.FindChild ("ModePanel/ModeGroup/btnManualMode").GetComponent<Button>().interactable = false;
		StartGroup.FindChild ("btnStart").SetAsLastSibling ();
		dispExec(false);
		allStock (true);
		GameManager.clearAllResultPairAndMatch ();
		GameManager.gameStartDate = "";
		this.transform.FindChild ("MainPanel/PlayPanel/txtStart").GetComponent<Text> ().text = "組合せ開始:";
		GameManager.gameStatus = 0;
		GameManager.gameCount = 0;
		this.transform.FindChild ("ModePanel/lblGame").GetComponent<Text> ().text = "試合:" + GameManager.gameCount;
		GameManager.Save ();
	}
	public void dispPairImageForWaitBreak() {
		// 待機中、休憩中エリアでの PairのImageを正しく表示させるため、Pairの選手同士は隣接させる
		for (int pCnt=0; pCnt < GameManager.getPlayerCount (); pCnt++) {
			GameManager.posPlayerOfListIndex (pCnt);
			string sPairRegDate = PairManager.getPairRegDate (GameManager.regDate);
			if (sPairRegDate != null) {													// Pairを表示
				if (GameManager.placeStat == GameManager.PLACE_WAIT || GameManager.placeStat == GameManager.PLACE_BREAK) {
					if (GameManager.placeStat == GameManager.PLACE_WAIT && SettingManager.waitCalc == 1 && GameManager.getTransformOfRegDate (sPairRegDate).GetSiblingIndex() < 4)
						continue;
					
					GameManager.getTransformOfRegDate (GameManager.regDate).FindChild ("Ring").gameObject.SetActive (false);
					GameManager.getTransformOfRegDate (sPairRegDate).FindChild ("Ring").gameObject.SetActive (true);
					GameManager.getTransformOfRegDate (GameManager.regDate).SetAsLastSibling ();
					GameManager.getTransformOfRegDate (sPairRegDate).SetAsLastSibling ();
				} else {
					if (GameManager.placeStat % 2 == 1) {
						GameManager.getTransformOfRegDate (GameManager.regDate).FindChild ("Ring").gameObject.SetActive (true);
						GameManager.getTransformOfRegDate (GameManager.regDate).SetAsLastSibling ();
					}
				}
			}
		}
	}
	public void allStock(bool clearFlg = false) {
		int playerNum = GameManager.getPlayerCount ();
		for (int cnt=0; cnt < playerNum; cnt++) {
			GameManager.posPlayerOfListIndex (cnt);
			if (clearFlg)
				GameManager.placeStat = GameManager.PLACE_STOK;
			GameManager.trsPlayer.SetParent (trsStockPanel, false);
			GameManager.trsPlayer.position = trsStockPanel.position;
			GameManager.trsPlayer.FindChild ("Lock").gameObject.SetActive (false);
			GameManager.trsPlayer.FindChild ("Ring").gameObject.SetActive (false);
		}
	}
	public void allWait() {
		int playerNum = GameManager.getPlayerCount ();
		for (int cnt=0; cnt < playerNum; cnt++) {
			GameManager.posPlayerOfListIndex (cnt);
			GameManager.setPlaceStat (GameManager.regDate, GameManager.PLACE_WAIT);
			GameManager.trsPlayer.SetParent (receiveParentTransform [GameManager.PLACE_WAIT], false);
			GameManager.trsPlayer.FindChild ("Lock").gameObject.SetActive (false);
//			GameManager.trsPlayer.FindChild ("Ring").gameObject.SetActive (false);
		}
	}
	public void dispExec(bool bDisp) {		// 全画面での「実行中」の表示・非表示
		Transform[] tr;
		tr = GameObject.Find ("Canvas").transform.GetComponentsInChildren<Transform>(true);
		var query = tr.Where(p => p.Find("txtExecInfo"));
		foreach (Transform transform in query)
		if (bDisp) {
			transform.gameObject.SetActive(true);
		} else {
			transform.gameObject.SetActive(false);
		}
	}
	public void preLoadView() {	// 画面遷移の場合はここを通る(起動時、選手の増減など各種保存ファイルを復元する)
		string sTotal = MemberManager.getMemberCount ().ToString();
		string sActive = MemberManager.getActiveMemberCount ().ToString();
		this.transform.FindChild("ModePanel/lblActive").GetComponent<Text>().text = "選択: " + sActive + "/" + sTotal + "名";
		string sSum = SettingManager.sum == 0 ? "なし" : "あり";
		this.transform.FindChild ("ModePanel/lblGame").GetComponent<Text> ().text = "試合:" + GameManager.gameCount;
		string sWeight = SettingManager.weight == 2 ? "大" : "小";
		this.transform.FindChild ("ModePanel/lblSum").GetComponent<Text> ().text = "集計:" + sSum + " 重み:" + sWeight;
		string btnTitle = SettingManager.form == 2 ? "ダブルス\n自動配置中" : "シングルス\n自動配置中";
		this.transform.FindChild ("ModePanel/ModeGroup/btnManualMode/txtTitle").GetComponent<Text> ().text = btnTitle;

		// 実行開始button、自動配置buttonの状態復元
		if (GameManager.gameStatus == 0) {	// 終了状態
			this.transform.FindChild ("MainPanel/PlayPanel/txtStart").GetComponent<Text> ().text = "組合せ開始:";
			this.transform.FindChild ("ModePanel/ModeGroup/btnAutoMode").GetComponent<Button>().interactable = false;
			this.transform.FindChild ("ModePanel/ModeGroup/btnManualMode").GetComponent<Button>().interactable = false;
			this.transform.FindChild ("ModePanel/StartGroup/btnStart").SetAsLastSibling ();
		} else {
			// すでに開始されていた場合は、開始日付をセットする
			string strDate = GameManager.gameStartDate;
			viewManager.scriptResultView.chkSumDate (strDate);
			string strStart = strDate.Substring (5, 2) + "月" + strDate.Substring (8, 2) + "日 " + strDate.Substring (11, 2) + "時" + strDate.Substring (14, 2) + "分";
			this.transform.FindChild ("MainPanel/PlayPanel/txtStart").GetComponent<Text> ().text = "組合せ開始:"+ strStart;
			this.transform.FindChild ("ModePanel/ModeGroup/btnAutoMode").GetComponent<Button>().interactable = true;
			this.transform.FindChild ("ModePanel/ModeGroup/btnManualMode").GetComponent<Button>().interactable = true;
			this.transform.FindChild ("ModePanel/StartGroup/btnEnd").SetAsLastSibling ();
		}
		if (GameManager.autoMode == 0) {	// 手動配置
			this.transform.FindChild ("ModePanel/ModeGroup/btnAutoMode").SetAsLastSibling ();
		} else {
			this.transform.FindChild ("ModePanel/ModeGroup/btnManualMode").SetAsLastSibling ();
		}
		assignPlayerFromSavefile ();

		// 解除、確定button、開始、終了buttonの状態復元   ※開始buttonの設定があるため、選手配置復元の後でコート復元を行う
		for (int cnt=0; cnt < SettingManager.courtNum; cnt++) {
			int ctStat = GameManager.chkCourtLockOfCortnum (cnt);
			string imgf = "MainPanel/PlayPanel/SclGame/Viewport/Content/PageLayout" + (cnt/2+1) + "/CourtAreaSpace" + (cnt%2+1) + "/ImgFunc" + (cnt+1);
			Transform fncGroup = this.transform.FindChild ("MainPanel/PlayPanel/SclGame/Viewport/Content/PageLayout" + (cnt/2+1) + "/CourtAreaSpace" + (cnt%2+1) + "/ImgFnc" + (cnt+1));
			Image imgCourtColor = fncGroup.parent.FindChild ("ImgCourt"+(cnt+1)).GetComponent<Image>();
			switch (ctStat) {
			case 0:		// 0:解除中
				break;
			case 1:		// 1:確定中
				imgCourtColor.color = Colors.lockCourt;
				fncGroup.FindChild ("FncFixFree/BtnFree").GetComponent<Button> ().interactable = true;
				fncGroup.FindChild ("FncStartEnd/BtnStart").SetAsLastSibling ();
				fncGroup.FindChild ("FncStartEnd/BtnStart").GetComponent<Button> ().interactable = false;
				int iPlayerCount = GameManager.GetPlayerCountInCourt (cnt);
				if (iPlayerCount == 11 || iPlayerCount == 22) {
					fncGroup.FindChild ("FncStartEnd/BtnStart").GetComponent<Button> ().interactable = true;
				}
				break;
			case 2:		// 2:開始中
				imgCourtColor.color = Colors.playCourt;
				fncGroup.FindChild ("FncFixFree/BtnStop").SetAsLastSibling ();
				fncGroup.FindChild ("FncStartEnd/BtnEnd").SetAsLastSibling ();
				break;
			default:
				break;
			}
		}

		if (GameManager.gameStatus != 0) 
			loadView ();

		if (SettingManager.autoExec == 1) {
			_CallBtnAutoMode (this.transform.FindChild ("ModePanel/ModeGroup"));
			_CallBtnGameStart (this.transform.FindChild ("ModePanel/StartGroup"));
		}
	}
	private void assignPlayerFromSavefile() {

		if (flgNotInit) {		// アプリ起動時以外での初期化 
			// 一旦、全frefabをStackに戻す
//			allWait ();
			allStock (false);
		}
		flgNotInit = true;
		int playerNum = GameManager.getPlayerCount ();
		int activMenNum = MemberManager.getActiveMemberCount ();
		// MemberManager を元に GameManager を変更と追加削除
		GameManager.setDeleteFlgAllPlayer ();
		for (int mCnt = 0; mCnt < activMenNum; mCnt++) {
			MemberManager.posActiveMember (mCnt);
			if (GameManager.posPlayerOfRegDate (MemberManager.regDate)) {		// 両方にあるものは、更新
				GameManager.nameKaji_family = MemberManager.nameKaji_family;	
				GameManager.nameKaji_first = MemberManager.nameKaji_first;	
				GameManager.nameKana_family = MemberManager.nameKana_family;	
				GameManager.nameKana_first = MemberManager.nameKana_first;	
				GameManager.gender = MemberManager.gender;
				GameManager.idxPriority = MemberManager.idxRec + 1;
 				GameManager.flgDelete = 0;
				GameManager.updatePlayer ();
			} else {		// GameManagerにないものは、追加作成
//				GameManager.addPlayer (MemberManager.regDate);
			}
		}
		GameManager.deletePlayerByFlg (trsStockPanel);

		// GameManager を元に pfbPlayer を割り当てる
//		GameManager.createTransformFromPlayerDate ();
		for (int pCnt= 0; pCnt < GameManager.getPlayerCount (); pCnt++) {
			// プログラム移動が困難なためPrefabは使用ぜず、Stockに"Player"GameObjectを用意して使用する
			Transform pt = pfbPlayer [pCnt].transform;
			Image imgBack = pt.GetComponentInChildren <Image> ();
			GameManager.posPlayerOfListIndex (pCnt);
			GameManager.setTransformOfRegDate (GameManager.regDate, pt);

			string sname = GameManager.nameKaji_family + " " + GameManager.nameKaji_first;
			pt.FindChild("MemberName").GetComponent <Text> ().text = sname;
			if (GameManager.gender == 0)
				imgBack.color = Colors.male;
			else
				imgBack.color = Colors.female;

			// prefabを保存位置に再配置する
			if (GameManager.placeStat != GameManager.PLACE_STOK) {
				pt.SetParent (receiveParentTransform [GameManager.placeStat], false);
//				pt.position = receiveParentPosition[GameManager.placeStat];
				pt.position = receiveParentTransform [GameManager.placeStat].position;
				if (GameManager.lockStat == GameManager.LOCK)
					pt.FindChild ("Lock").gameObject.SetActive (true);		// Lockを表示
			}
			// 自動計算の配置時に選手を特定するため、プレハブ作成時に登録日を設定する。
			pt.GetComponent<GamePlayer>().myRegDate = GameManager.regDate;
			pt.GetComponent<GamePlayer>().rcvParentTfmWait = GameObject.Find ("WaitListScroll").transform.GetComponent<ScrollRect>();
			pt.GetComponent<GamePlayer>().rcvParentTfmBreak = GameObject.Find ("BreakListScroll").transform.GetComponent<ScrollRect>();
			pt.Find("Move").GetComponent<DraggablePlayer>().myRegDate = GameManager.regDate;
		}
		// PairのImageの再表示は前後関係でImageをつける側が変わるので、上のprefab作成ループとは別のループで処理する。
		dispPairImageForWaitBreak ();

		GameManager.Save ();
		cortSetting ();

	}

	public void loadView() {
		if (GameManager.autoMode == 1) {
			calcMatch ();
			dispPairImageForWaitBreak ();
		}

		// 試合数を表示
		for (int cnt=0; cnt<GameManager.getPlayerCount(); cnt++) {
			GameManager.posPlayerOfListIndex (cnt);
//			int iGame = int.Parse (GameManager.getTransformOfRegDate (GameManager.regDate).FindChild ("txtGame").GetComponent<Text> ().text);
			string strGame = "";
			if (GameManager.gameAjust != 0)
				strGame = GameManager.game + "(" + GameManager.gameAjust + ")";
			else
				strGame = GameManager.game.ToString();

			GameManager.getTransformOfRegDate (GameManager.regDate).FindChild ("txtGame").GetComponent<Text> ().text = strGame;
		}
		//		GameManager.Save ();
	}

	private void cortSetting() {
		Transform[] trsFnc = new Transform[6];
		for (int cnt = 0; cnt < SettingManager.MaxCoutNum; cnt++) {
			string str = "MainPanel/PlayPanel/SclGame/Viewport/Content/PageLayout" + (cnt / 2 + 1) + "/CourtAreaSpace" + (cnt % 2 + 1) + "/ImgFnc" + (cnt + 1);
			trsFnc [cnt] = this.transform.FindChild (str);
			trsFnc [cnt].FindChild ("Text").GetComponent<Text> ().text = SettingManager.getCourtName (cnt);
			trsFnc [cnt].FindChild ("FncStartEnd/BtnFix").GetComponent<Button> ().interactable = false;
		}
		for (int cnt = 0; cnt < SettingManager.courtNum; cnt++)
			trsFnc [cnt].FindChild ("FncStartEnd/BtnFix").GetComponent<Button> ().interactable = true;

		if (SettingManager.courtNum <= 2) {
			imgArrowCourt1Right.gameObject.SetActive (false);
			this.transform.FindChild ("MainPanel/PlayPanel/SclGame/Viewport/Content").GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);
			if (pageScrollRect.getPage () < 0) {
				pageScrollRect.setPage (0);
			}
		} else if (SettingManager.courtNum <= 4) {
			imgArrowCourt1Right.SetActive (true);
			imgArrowCourt2Left.SetActive (true);
			imgArrowCourt2Right.SetActive (false);
			this.transform.FindChild ("MainPanel/PlayPanel/SclGame/Viewport/Content").GetComponent<RectTransform> ().sizeDelta = new Vector2 (700f, 0f);
			if (pageScrollRect.getPage () < -1) {
				pageScrollRect.setPage (-1);
			}
		} else {
			this.transform.FindChild ("MainPanel/PlayPanel/SclGame/Viewport/Content").GetComponent<RectTransform> ().sizeDelta = new Vector2 (1400f, 0f);
			imgArrowCourt1Right.SetActive (true);
			imgArrowCourt2Left.SetActive (true);
			imgArrowCourt2Right.SetActive (true);
		}
	}
	private void calcMatch() {
		pageScrollRect.saveCurrentPage ();		//  ページ移動したまま配置を行うと選手の配置でズレがあるため一瞬ページを1ページ目にする
		int a = GameManager.getPlayerCount ();
		for (int cnt = 0; cnt < GameManager.getPlayerCount (); cnt++) {	// Lockされた選手と「休憩中」の選手以外を「待機中」に移動する(配置先にいると面倒だから配置場所にいる選手は待機中に集める)
			GameManager.posPlayerOfListIndex(cnt);
			GameManager.setPlayerToPlace (GameManager.regDate, GameManager.trsPlayer, receiveParentTransform [GameManager.PLACE_WAIT]);
		}
		int iPlayerCourtNum = SettingManager.courtNum;
		int iPlayerNumInCourt = 4;		// コート収容人数はDoublesの4を基本としてSinglesも計算する

		if (SettingManager.form == 2) {		// Doublesの場合
			for (int iCourt = 0; iCourt < iPlayerCourtNum; iCourt++) {	// コート面数を基本ループとするため、1ループで4人処理
				if (GameManager.chkCourtLockOfPosition (iCourt * iPlayerNumInCourt) != GameManager.UNLOCK) {
					continue;																		// コートがLock(LOCK、START)されていたら配置しない 
				}
				int courtPNum = GameManager.getPlayerNumOfPlaceNo (iCourt);	// コートにいる人数
				int waitPNum = GameManager.getWaitPlayerCount ();						// 待機中の人数
				if ((courtPNum + waitPNum) < iPlayerNumInCourt)
					break;																			// コートが埋まらないなら終わり

				int iPos = iPlayerNumInCourt * iCourt;
				string sPairRegDate = null;
				// Aポジションを決定 
				string PosARegDate = GameManager.getPlaceOfPlayer (iPos);		// 対象場所にLockされた選手がいたらそのまま
				if (GameManager.chkLock (PosARegDate) == GameManager.UNLOCK || PosARegDate == null)
				if  (GameManager.chkLockOfPosition(iPos+1) == 0)		// positonBがUnlockならば、positonAはPairでも可能
						PosARegDate = PositionA (iPos, true, null); 
				else
						PosARegDate = PositionA (iPos, false, null); 
				sPairRegDate = PairManager.getPairRegDate (PosARegDate);		// Aポジション選手のPair選手を一時保存

				// Bポジションを決定 
				string PosBRegDate = GameManager.getPlaceOfPlayer (++iPos);
				if (GameManager.chkLock (PosBRegDate) == GameManager.UNLOCK || PosBRegDate == null)
				if (sPairRegDate == null) {		// Aポジション選手にPair選手がいないかった場合は、Bポジションの選手を計算
					if (GameManager.chkLockOfPosition (iPos - 1) == 0)		// positonAがUnlockならば、positonBはPairでも可能
						PosBRegDate = PositionB (iPos, PosARegDate, true, null);
					else
						PosBRegDate = PositionB (iPos, PosARegDate, false, null);

					sPairRegDate = PairManager.getPairRegDate (PosBRegDate);
					if (sPairRegDate != null) {	// Bポジション選手にPairがいたら、AポジションにPair選手を配置
						PosARegDate = PositionA (iPos - 1, true, sPairRegDate);
					}
				} else {									// Aポジション選手のPair選手を、Bポジションに配置
					PosBRegDate = PositionB (iPos, null, true, sPairRegDate); 
				}

				// Cポジションを決定 
				string PosCRegDate = GameManager.getPlaceOfPlayer (++iPos);
				if (GameManager.chkLock (PosCRegDate) == GameManager.UNLOCK || PosCRegDate == null)
				if (GameManager.chkLockOfPosition (iPos + 1) == 0)		// positonDがUnlockならば、positonCはPairでも可能
					PosCRegDate = PositionC (iPos, PosARegDate, PosBRegDate, true, null);
				else
					PosCRegDate = PositionC (iPos, PosARegDate, PosBRegDate, false, null);
				
				sPairRegDate = PairManager.getPairRegDate (PosCRegDate);		// Cポジション選手のPair選手を一時保存

				// Dポジションを決定 
				string PosDRegDate = GameManager.getPlaceOfPlayer (++iPos);
				if (GameManager.chkLock (PosDRegDate) == GameManager.UNLOCK || PosDRegDate == null)
				if (sPairRegDate == null) {		// Cポジション選手にPair選手がいないかった場合は、Dポジションの選手を計算
					if (GameManager.chkLockOfPosition (iPos - 1) == 0)		// positonCがUnlockならば、positonDはPairでも可能
						PosDRegDate = PositionD (iPos, PosARegDate, PosBRegDate, PosCRegDate, true, null);
					else
						PosDRegDate = PositionD (iPos, PosARegDate, PosBRegDate, PosCRegDate, false, null);
					
					sPairRegDate = PairManager.getPairRegDate (PosDRegDate);
					if (sPairRegDate != null)		// Dポジション選手にPair選手がいたら、CポジションにPair選手を配置
						PosCRegDate = PositionC (iPos - 1, null, null, true, sPairRegDate);
				} else {									// Cポジション選手のPair選手を、Dポジションに配置
					PosDRegDate = PositionD (iPos, null, null, null, true, sPairRegDate);
				}
			}
		} else {		// Singlesの場合
			for (int iCourt = 0; iCourt < iPlayerCourtNum; iCourt++) {	// コート面数を基本ループとするため、1ループで2人処理
				if (GameManager.chkCourtLockOfPosition (iCourt * iPlayerNumInCourt) == GameManager.LOCK) {
					continue;																		// コートがLockされていたら配置しない
				}
				int courtPNum = GameManager.getPlayerNumOfPlaceNo (iCourt);	// コートにいる人数
				int waitPNum = GameManager.getWaitPlayerCount ();						// 待機中の人数
				if ((courtPNum + waitPNum) < 2)
					break;																			// コートが埋まらないなら終わり

				int iPos = iPlayerNumInCourt * iCourt;
				string sPairRegDate = null;
				// Aポジション選手を決定 
				string PosARegDate = GameManager.getPlaceOfPlayer (iPos);			// AポジションにLockされた選手がいたらそのまま
				string PosBRegDate = GameManager.getPlaceOfPlayer (iPos + 1);		// BポジションにLockされた選手がいたらそのまま
				if ((GameManager.chkLock (PosARegDate) == GameManager.UNLOCK || PosARegDate == null) && (GameManager.chkLock (PosBRegDate) == GameManager.UNLOCK || PosBRegDate == null))
					PosARegDate = PositionASingles (iPos);
	 
				string PosABRegDate = "";
				if (PosARegDate != null)	// Cポジションの計算の際にコート左側に既に選手が配置されていているが、Aポジションに配置されていない場合はBポジション選手を元に計算する
					PosABRegDate = PosARegDate;
				else
					PosABRegDate = PosBRegDate;	

				// Cポジション選手を決定 (Singlesの場合はAの対戦相手であり、表示位置はCポジションとなる)
				iPos += 2;	// 次の選手の配置位置  Doubles換算では、Bポジションは飛ばしてCポジションの位置なので 2増加させる
				string PosCRegDate = GameManager.getPlaceOfPlayer (iPos);			// CポジションにLockされた選手がいたらそのまま
				string PosDRegDate = GameManager.getPlaceOfPlayer (iPos + 1);		// DポジションにLockされた選手がいたらそのまま
				if ((GameManager.chkLock (PosCRegDate) == GameManager.UNLOCK || PosCRegDate == null) && (GameManager.chkLock (PosDRegDate) == GameManager.UNLOCK || PosDRegDate == null))
					PosBRegDate = PositionBSingles (iPos, PosABRegDate); 		//Cポジション選手であるが、B選手が存在しないため、B選手選出ロジック(A選手のみ引数指定)を使う
			}
		}

		if (SettingManager.waitCalc == 1) {		// 待機中の選手に組合せ計算を適用する
			int waitPNum = GameManager.getWaitPlayerCount ();		// 待機中の人数
			for (int iPos = 0; iPos < 1; iPos++) {
				if (waitPNum == iPos++)
					break;																			// 計算対象の待機中選手がいないなら終わり

				string sPairRegDate = null;
				// AポジションとBポジションを決定 (待機中リストの最上位と2番目)
				GameManager.getPositionA (true);
				string PosARegDate = GameManager.regDate;
				GameManager.getTransformOfRegDate (PosARegDate).SetSiblingIndex (0);
				GameManager.getTransformOfRegDate (PosARegDate).FindChild ("Ring").gameObject.SetActive (false);	// Aポジションの選手にはPair有無に関係なくRingが付かない
				string PosBRegDate = PairManager.getPairRegDate (PosARegDate);		// Aポジション選手のPair選手を一時保存
				if (waitPNum == iPos++)
					break;																			// 計算対象の待機中選手がいないなら終わり

				if (PosBRegDate != null) {
					GameManager.getTransformOfRegDate (PosBRegDate).SetSiblingIndex (1);
					GameManager.getTransformOfRegDate (PosBRegDate).FindChild ("Ring").gameObject.SetActive (true);	// AポジションからPair指定の場合、またはAにPairがいた場合はRingが付く
				} else {
					GameManager.getPositionB (PosARegDate, true);
					PosBRegDate = GameManager.regDate;
					sPairRegDate = PairManager.getPairRegDate (PosBRegDate);
					if (sPairRegDate != null) {	// Bポジション選手にPairがいたら、AポジションにPair選手を配置
						PosARegDate = sPairRegDate;
						GameManager.getTransformOfRegDate (PosBRegDate).SetSiblingIndex (0);
						GameManager.getTransformOfRegDate (PosBRegDate).FindChild ("Ring").gameObject.SetActive (false);
						GameManager.getTransformOfRegDate (PosARegDate).SetSiblingIndex (1);
						GameManager.getTransformOfRegDate (PosARegDate).FindChild ("Ring").gameObject.SetActive (true);	// AポジションからPair指定の場合、またはAにPairがいた場合はRingが付く
					} else {
						GameManager.getTransformOfRegDate (PosBRegDate).SetSiblingIndex (1);
						GameManager.getTransformOfRegDate (PosBRegDate).FindChild ("Ring").gameObject.SetActive (false);
					}
				}

				if (waitPNum == iPos++)
					break;																			// 計算対象の待機中選手がいないなら終わり

				// CポジションとDポジションを決定 (待機中リストの3番目と4番目)
				GameManager.getPositionC (PosARegDate, PosBRegDate, true);
				string PosCRegDate = GameManager.regDate;
				GameManager.getTransformOfRegDate (PosCRegDate).SetSiblingIndex (2);
				GameManager.getTransformOfRegDate (PosCRegDate).FindChild ("Ring").gameObject.SetActive (false);	// Cポジションの選手にはPair有無に関係なくRingが付かない
				string PosDRegDate = PairManager.getPairRegDate (PosCRegDate);		// Dポジション選手のPair選手を一時保存
				if (waitPNum == iPos++)
					break;																			// 計算対象の待機中選手がいないなら終わり

				if (PosDRegDate != null) {
					GameManager.getTransformOfRegDate (PosDRegDate).SetSiblingIndex (3);
					GameManager.getTransformOfRegDate (PosDRegDate).FindChild ("Ring").gameObject.SetActive (true);	// CポジションからPair指定の場合、またはAにPairがいた場合はRingが付く
				} else {
					GameManager.getPositionD (PosARegDate, PosBRegDate, PosCRegDate, true);
					PosDRegDate = GameManager.regDate;
					sPairRegDate = PairManager.getPairRegDate (PosDRegDate);
					if (sPairRegDate != null) {	// Bポジション選手にPairがいたら、AポジションにPair選手を配置
						PosCRegDate = sPairRegDate;
						GameManager.getTransformOfRegDate (PosDRegDate).SetSiblingIndex (2);
						GameManager.getTransformOfRegDate (PosDRegDate).FindChild ("Ring").gameObject.SetActive (false);
						GameManager.getTransformOfRegDate (PosCRegDate).SetSiblingIndex (3);
						GameManager.getTransformOfRegDate (PosCRegDate).FindChild ("Ring").gameObject.SetActive (true);	// CポジションからPair指定の場合、またはAにPairがいた場合はRingが付く
					} else {
						GameManager.getTransformOfRegDate (PosDRegDate).SetSiblingIndex (3);
						GameManager.getTransformOfRegDate (PosDRegDate).FindChild ("Ring").gameObject.SetActive (false);
					}
				}
			}
		}
		pageScrollRect.restoreCurrentPage ();		// 元に表示していたページに戻す
		receiveParentTransform [GameManager.PLACE_WAIT].GetComponent<RectTransform> ().anchoredPosition = Vector2.zero;
	}
	private string PositionA(int cortPositionNum, bool pairAllowFlg, string iPairRegDate = null) {
 		string strPairRegDate = iPairRegDate;
		if (strPairRegDate == null) {	// B配置時のPairとしての入れ替えかを判定
			if (!GameManager.getPositionA (pairAllowFlg))
				return "";
			strPairRegDate = GameManager.regDate;
		} else {
			// Aポジションに既に配置されている選手がいたら待機中に戻す(Lockされていないことが保証されていること)
			string aRegDate = GameManager.getPlaceOfPlayer (cortPositionNum);
			if (aRegDate != null) {
				GameManager.posPlayerOfRegDate (GameManager.getPlaceOfPlayer (cortPositionNum));
				GameManager.placeStat = GameManager.PLACE_WAIT;
				GameManager.trsPlayer.SetParent (receiveParentTransform [GameManager.PLACE_WAIT]);
			}
			GameManager.posPlayerOfRegDate (strPairRegDate);
		}

		GameManager.trsPlayer.SetParent (receiveParentTransform [cortPositionNum], false);
		GameManager.trsPlayer.position = receiveParentPosition[cortPositionNum];
		GameManager.trsPlayer.FindChild ("Ring").gameObject.SetActive (false);	// Aポジションの選手にはPair有無に関係なくRingが付かない
		GameManager.placeStat = cortPositionNum;

		return GameManager.regDate;
	}
	private string PositionB(int cortPositionNum, string A, bool pairAllowFlg, string iPairRegDate = null) {
		string strPairRegDate = iPairRegDate;
		if (strPairRegDate == null) {
			if (!GameManager.getPositionB (A, pairAllowFlg))
				return "";
			strPairRegDate = GameManager.regDate;
		} else {
			// Bポジションに既に配置されている選手がいたら待機中に戻す(Lockされていないことが保証されていること)
			string bRegDate = GameManager.getPlaceOfPlayer (cortPositionNum);
			if (bRegDate != null) {
				GameManager.posPlayerOfRegDate (bRegDate);
				GameManager.placeStat = GameManager.PLACE_WAIT;
				GameManager.trsPlayer.SetParent (receiveParentTransform [GameManager.PLACE_WAIT]);
			}
			GameManager.posPlayerOfRegDate (strPairRegDate);
		}

		string exitPair = PairManager.getPairRegDate (strPairRegDate);
		GameManager.trsPlayer.SetParent (receiveParentTransform [cortPositionNum], false);
		GameManager.trsPlayer.position = receiveParentPosition[cortPositionNum];
		if (iPairRegDate != null || exitPair != null)
			GameManager.trsPlayer.FindChild ("Ring").gameObject.SetActive (true);	// AポジションからPair指定の場合、またはAにPairがいた場合はRingが付く
		else
			GameManager.trsPlayer.FindChild ("Ring").gameObject.SetActive (false);	// AポジションからPair指定でない場合はRingが付かない
		GameManager.placeStat = cortPositionNum;

		return GameManager.regDate;
	}
	private string PositionC(int cortPositionNum, string A, string B, bool pairAllowFlg, string iPairRegDate = null) {
		string strPairRegDate = iPairRegDate;
		if (strPairRegDate == null) {
			if (!GameManager.getPositionC (A, B, pairAllowFlg))
				return "";
			
			strPairRegDate = GameManager.regDate;
		} else {
			// Cポジションに既に配置されている選手がいたら待機中に戻す(Lockされていないことが保証されていること)
			string cRegDate = GameManager.getPlaceOfPlayer (cortPositionNum);
			if (cRegDate != null) {
				GameManager.posPlayerOfRegDate (GameManager.getPlaceOfPlayer (cortPositionNum));
				GameManager.placeStat = GameManager.PLACE_WAIT;
				GameManager.trsPlayer.SetParent (receiveParentTransform [GameManager.PLACE_WAIT]);
			}
			GameManager.posPlayerOfRegDate (strPairRegDate);
		}

		GameManager.trsPlayer.SetParent (receiveParentTransform [cortPositionNum], false);
		GameManager.trsPlayer.position = receiveParentPosition[cortPositionNum];
		GameManager.trsPlayer.FindChild ("Ring").gameObject.SetActive (false);	// Aポジションの選手にはPair有無に関係なくRingが付かない
		GameManager.placeStat = cortPositionNum;

		return GameManager.regDate;
	}
	private string PositionD(int cortPositionNum, string A, string B, string C, bool pairAllowFlg, string iPairRegDate = null) {
		string strPairRegDate = iPairRegDate;
		if (strPairRegDate == null) {
			if (!GameManager.getPositionD (A, B, C, pairAllowFlg))
				return "";
			
			strPairRegDate = GameManager.regDate;
		} else {
			// Dポジションに既に配置されている選手がいたら待機中に戻す(Lockされていないことが保証されていること)
			string dRegDate = GameManager.getPlaceOfPlayer (cortPositionNum);
			if (dRegDate != null) {
				GameManager.posPlayerOfRegDate (GameManager.getPlaceOfPlayer (cortPositionNum));
				GameManager.placeStat = GameManager.PLACE_WAIT;
				GameManager.trsPlayer.SetParent (receiveParentTransform [GameManager.PLACE_WAIT]);
			}
			GameManager.posPlayerOfRegDate (strPairRegDate);
		}

		string exitPair = PairManager.getPairRegDate (strPairRegDate);
		GameManager.trsPlayer.SetParent (receiveParentTransform [cortPositionNum], false);
		GameManager.trsPlayer.position = receiveParentPosition[cortPositionNum];
		if (iPairRegDate != null || exitPair != null)
			GameManager.trsPlayer.FindChild ("Ring").gameObject.SetActive (true);	// AポジションからPair指定の場合、またはAにPairがいた場合はRingが付く
		else
			GameManager.trsPlayer.FindChild ("Ring").gameObject.SetActive (false);	// AポジションからPair指定でない場合はRingが付かない
		GameManager.placeStat = cortPositionNum;

		return GameManager.regDate;
	}
	private string PositionASingles(int cortPositionNum) {		// SinglesのAポジション
		GameManager.getPositionA (false);
		GameManager.trsPlayer.SetParent (receiveParentTransform [cortPositionNum], false);
		GameManager.trsPlayer.position = receiveParentPosition[cortPositionNum];
		GameManager.placeStat = cortPositionNum;
		return GameManager.regDate;
	}
	private string PositionBSingles(int cortPositionNum, string A) {
		GameManager.getPositionB (A, false);
		GameManager.trsPlayer.SetParent (receiveParentTransform [cortPositionNum], false);
		GameManager.trsPlayer.position = receiveParentPosition[cortPositionNum];
		GameManager.placeStat = cortPositionNum;
		return GameManager.regDate;
	}

	public void createPrefubPlayer() {
		Transform tPfbParent = this.transform.FindChild ("MainPanel/PlayPanel/Stock");
		int pfCnt = tPfbParent.childCount;
		for (int i=0; i < pfCnt; i++) {
			DestroyImmediate(tPfbParent.Find ("Player").gameObject);
		}
		for (int i = 0; i < MemberManager.MAX_MEMBER; i++) {
			Transform pt = Instantiate (pfbPlayerOrgin).transform;
			pt.name = pfbPlayerOrgin.name;
			pt.SetParent (tPfbParent, false);
		}
	}

	public void sleepView() {           // 新テニスアプリのためupdateを実行しないようにする
		this.gameObject.SetActive(false);
	}

	void Start () {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
		audioSrc = this.transform.GetComponent<AudioSource> ();
		pageScrollRect = GameObject.Find ("SclGame").transform.GetComponent<PageScrollRect>();
		trsStockPanel = GameObject.Find ("Stock").transform;
		trsWaitParent = GameObject.Find ("WaitLayoutVertical").transform;
		createPrefubPlayer ();
		Transform trsStok = this.transform.FindChild ("MainPanel/PlayPanel/Stock");
		GameManager.createTransformFromPlayerDate ();
		pfbPlayer = new GameObject[trsStok.childCount];
		for (int ip=0; ip < pfbPlayer.Count(); ip++) {	// StokのPlayerをActibeMenberに割り当てる準備(Transform配列の作成)
			pfbPlayer [ip] = trsStok.GetChild (ip).gameObject;
		}
		//GameManager.initCourt (SettingManager.courtNum);

		receiveParentPosition  = new Vector3[26];  // 待機休憩2 + コート*4 が必要  (2+最大6面*4=26)
		receiveParentTransform  = new Transform[receiveParentPosition.Length];
		receiveParentTransform[24] = GameObject.Find ("WaitLayoutVertical").transform;
		receiveParentTransform[25] = GameObject.Find ("BreakLayoutVertical").transform;
		receiveParentPosition [24] = GameObject.Find ("WaitListScroll").transform.position;
		receiveParentPosition [25] = GameObject.Find ("BreakListScroll").transform.position;
		setReciveCourtTrans(1, "ImgCourt1", "ImgFnc1");
		setReciveCourtTrans(2, "ImgCourt2", "ImgFnc2");
		setReciveCourtTrans(3, "ImgCourt3", "ImgFnc3");
		setReciveCourtTrans(4, "ImgCourt4", "ImgFnc4");
		setReciveCourtTrans(5, "ImgCourt5", "ImgFnc5");
		setReciveCourtTrans(6, "ImgCourt6", "ImgFnc6");

		execProgess = this.transform.FindChild ("MainPanel/PlayPanel/txtProgress").GetComponent<Text> ();
		court1Progess = this.transform.FindChild ("MainPanel/PlayPanel/SclGame/Viewport/Content/PageLayout1/CourtAreaSpace1/ImgFnc1/txtProgress").GetComponent<Text>();
		court2Progess = this.transform.FindChild ("MainPanel/PlayPanel/SclGame/Viewport/Content/PageLayout1/CourtAreaSpace2/ImgFnc2/txtProgress").GetComponent<Text>();
		court3Progess = this.transform.FindChild ("MainPanel/PlayPanel/SclGame/Viewport/Content/PageLayout2/CourtAreaSpace1/ImgFnc3/txtProgress").GetComponent<Text>();
		court4Progess = this.transform.FindChild ("MainPanel/PlayPanel/SclGame/Viewport/Content/PageLayout2/CourtAreaSpace2/ImgFnc4/txtProgress").GetComponent<Text>();
		court5Progess = this.transform.FindChild ("MainPanel/PlayPanel/SclGame/Viewport/Content/PageLayout3/CourtAreaSpace1/ImgFnc5/txtProgress").GetComponent<Text>();
		court6Progess = this.transform.FindChild ("MainPanel/PlayPanel/SclGame/Viewport/Content/PageLayout3/CourtAreaSpace2/ImgFnc6/txtProgress").GetComponent<Text>();

		imgArrowCourt1Right = this.transform.FindChild ("MainPanel/PlayPanel/SclGame/Viewport/Content/PageLayout1/ImgCourt1Right").gameObject;
		imgArrowCourt2Left = this.transform.FindChild ("MainPanel/PlayPanel/SclGame/Viewport/Content/PageLayout2/ImgCourt2Left").gameObject;
		imgArrowCourt2Right = this.transform.FindChild ("MainPanel/PlayPanel/SclGame/Viewport/Content/PageLayout2/ImgCourt2Right").gameObject;
		imgArrowCourt3Left = this.transform.FindChild ("MainPanel/PlayPanel/SclGame/Viewport/Content/PageLayout3/ImgCourt3Left").gameObject;
	}
	private void setReciveCourtTrans(int numCourt, string strParent, string strParentFnc) {
		int num = numCourt * 4 - 4 ;	//1は待機休憩2のオフセット(0,1の2個分)
		receiveParentTransform[num] = GameObject.Find (strParent+"/CourtL/Top").transform;
		receiveParentPosition [num] = receiveParentTransform [num].position;
		receiveParentTransform[++num] = GameObject.Find (strParent+"/CourtL/Btm").transform;
		receiveParentPosition [num] = receiveParentTransform [num].position;
		receiveParentTransform[++num] = GameObject.Find (strParent+"/CourtR/Top").transform;
		receiveParentPosition [num] = receiveParentTransform [num].position;
		receiveParentTransform[++num] = GameObject.Find (strParent+"/CourtR/Btm").transform;
		receiveParentPosition [num] = receiveParentTransform [num].position;
		GameObject.Find (strParentFnc + "/FncFixFree/BtnFree").transform.GetComponent<Button> ().interactable = false;
	}

	void Update () {
        if (true) return;

		string strDeltaTime = "";					// 組合せ開始日付
		string strCourt1DeltaTime = "";			// コート1試合開始日付
		string strCourt2DeltaTime = "";			// コート2試合開始日付
		string strCourt3DeltaTime = "";			// コート3試合開始日付
		string strCourt4DeltaTime = "";			// コート4試合開始日付
		string strCourt5DeltaTime = "";			// コート5試合開始日付
		string strCourt6DeltaTime = "";			// コート6試合開始日付

		if (GameManager.gameStatus != 0) {
			// 経過時間を表示
			DateTime nowDate = DateTime.Now;
			DateTime startDate = DateTime.Parse (GameManager.gameStartDate);
			TimeSpan ts = nowDate - startDate;
			strDeltaTime = ts.Hours + "時" + ts.Minutes + "分";

			// コート試合
			if (GameManager.chkCourtLockOfCortnum (0) == GameManager.START) {
				ts = nowDate - DateTime.Parse (GameManager.getMatchStartDateOfCortnum (0));
				strCourt1DeltaTime = ts.Hours + ":" + ts.Minutes;
			}
			if (GameManager.chkCourtLockOfCortnum (1) == GameManager.START) {
				ts = nowDate - DateTime.Parse (GameManager.getMatchStartDateOfCortnum (1));
				strCourt2DeltaTime = ts.Hours + ":" + ts.Minutes;
			}
			if (GameManager.chkCourtLockOfCortnum (2) == GameManager.START) {
				ts = nowDate - DateTime.Parse (GameManager.getMatchStartDateOfCortnum (2));
				strCourt3DeltaTime = ts.Hours + ":" + ts.Minutes;
			}
			if (GameManager.chkCourtLockOfCortnum (3) == GameManager.START) {
				ts = nowDate - DateTime.Parse (GameManager.getMatchStartDateOfCortnum (3));
				strCourt4DeltaTime = ts.Hours + ":" + ts.Minutes;
			}
			if (GameManager.chkCourtLockOfCortnum (4) == GameManager.START) {
				ts = nowDate - DateTime.Parse (GameManager.getMatchStartDateOfCortnum (4));
				strCourt5DeltaTime = ts.Hours + ":" + ts.Minutes;
			}
			if (GameManager.chkCourtLockOfCortnum (5) == GameManager.START) {
				ts = nowDate - DateTime.Parse (GameManager.getMatchStartDateOfCortnum (5));
				strCourt6DeltaTime = ts.Hours + ":" + ts.Minutes;
			}
		}
		execProgess.text = strDeltaTime + " 経過";
		court1Progess.text = strCourt1DeltaTime;
		court2Progess.text = strCourt2DeltaTime;
		court3Progess.text = strCourt3DeltaTime;
		court4Progess.text = strCourt4DeltaTime;
		court5Progess.text = strCourt5DeltaTime;
		court6Progess.text = strCourt6DeltaTime;
	}
}

/*
	// イベントの例 (参考)
	// プレイヤーを移動させるため、移動先エリアにマウス移動有無のイベントを取り付ける (呼び元で記述)
	//		addEventToPlayerParent(GameObject.Find ("ImgCourt1/CourtL/Top").transform);

	public Transform tfNewParent;
	public string tfNewParentName;
	private void addEventToPlayerParent(Transform trParent) {
		var trigger = trParent.gameObject.AddComponent<EventTrigger>();
		trigger.triggers = new List<EventTrigger.Entry>();

		// PointerEnter(マウスオーバー)時のイベントを設定
		var entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerEnter; // 他のイベントを設定したい場合はここを変える
		entry.callback.AddListener( (x) => { tfNewParent = trParent; tfNewParentName = trParent.name; });
		trigger.triggers.Add(entry);

		// PointerExit(マウスイグジット)時のイベントを設定
		var entry1 = new EventTrigger.Entry();
		entry1.eventID = EventTriggerType.PointerExit; // 他のイベントを設定したい場合はここを変える
		entry1.callback.AddListener( (x) => {  tfNewParent = null; tfNewParentName = null; });
		trigger.triggers.Add(entry1);
	}
*/
