using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ScriptResultView : MonoBehaviour {
	ViewManager viewManager;
	Text txtCourtName;
	Text txtMatchDate;
	Text txtSTime;
	Text txtETime;
	Text txtMatchCount;
	Text txtMemberL;
	Text txtMemberR;
	Dropdown drpMemberL;
	Dropdown drpMemberR;
	public GameObject myView;
	public bool OkCancel;				// GameView へ返すダイアログの結果(ダイアログでは値設定のみ)
	public Transform fncGroup;		// GameViewがセットする終了ボタンの親Transform
	public string sumDate;				// 組合せ実行開始日付 (集計単位となる時間) 前回日付を確認してセットする
	public string courtName;			// GameViewがセットするコート名
	public string regDateLTop;		// GameViewがセットする左上の選手RegDate
	public string regDateLBtm;		// GameViewがセットする左下の選手RegDate
	public string regDateRTop;		// GameViewがセットする右上の選手RegDate
	public string regDateRBtm;		// GameViewがセットする右下の選手RegDate
	public string memberLTopFm;		// GameViewがセットする左上の選手氏名(姓)
	public string memberLTopFs;		// GameViewがセットする左下の選手氏名(名)
	public string memberLBtmFm;		// GameViewがセットする右上の選手氏名(姓)
	public string memberLBtmFs;		// GameViewがセットする右下の選手氏名(名)
	public string memberRTopFm;		// GameViewがセットする左上の選手氏名(姓)
	public string memberRTopFs;		// GameViewがセットする左下の選手氏名(名)
	public string memberRBtmFm;	// GameViewがセットする右上の選手氏名(姓)
	public string memberRBtmFs;		// GameViewがセットする右下の選手氏名(名)
	public string memberKLTopFm;	// GameViewがセットする左上の選手カナ(姓)
	public string memberKLTopFs;	// GameViewがセットする左下の選手カナ(名)
	public string memberKLBtmFm;	// GameViewがセットする右上の選手カナ(姓)
	public string memberKLBtmFs;	// GameViewがセットする右下の選手カナ(名)
	public string memberKRTopFm;	// GameViewがセットする左上の選手カナ(姓)
	public string memberKRTopFs;	// GameViewがセットする左下の選手カナ(名)
	public string memberKRBtmFm;	// GameViewがセットする右上の選手カナ(姓)
	public string memberKRBtmFs;	// GameViewがセットする右下の選手カナ(名)
	public string gameStartDate;		// GameViewがセットするコート試合開始日付
	private string gameDate;			// 現在日付(コート試合終了日付)

	public void _btnOk() {
		//if (drpMemberL.value == drpMemberR.value) {
		//	string title = "引き分け結果の確認";
		//	string message = "試合結果を引き分けとして決定しますか？\n";
		//	DialogViewController.Show (title, message, new DialogViewOptions {
		//		btnCancelTitle = "キャンセル", btnCancelDelegate = () => {
		//			return;
		//		},
		//		btnOkTitle = "OK", btnOkDelegate = () => {
		//			_afterOk ();
		//		},
		//	});
		//} else
			_afterOk ();
	}
	public void _afterOk() {
		ResultManager.initResult ();
		ResultManager.sumDate = sumDate;
		ResultManager.gameSDate = gameStartDate;
		ResultManager.gameEDate = gameDate;
		ResultManager.courtName = courtName;
		ResultManager.regDateLTop = regDateLTop;
		ResultManager.regDateLBtm = regDateLBtm;
		ResultManager.regDateRTop = regDateRTop;
		ResultManager.regDateRBtm = regDateRBtm;
		ResultManager.memberKLTopFm = memberKLTopFm;
		ResultManager.memberKLTopFs = memberKLTopFs;
		ResultManager.memberKLBtmFm = memberKLBtmFm;
		ResultManager.memberKLBtmFs = memberKLBtmFs;
		ResultManager.memberKRTopFm = memberKRTopFm;
		ResultManager.memberKRTopFs = memberKRTopFs;
		ResultManager.memberKRBtmFm = memberKRBtmFm;
		ResultManager.memberKRBtmFs = memberKRBtmFs;
		ResultManager.memberLTopFm = memberLTopFm;
		ResultManager.memberLTopFs = memberLTopFs;
		ResultManager.memberLBtmFm = memberLBtmFm;
		ResultManager.memberLBtmFs = memberLBtmFs;
		ResultManager.memberRTopFm = memberRTopFm;
		ResultManager.memberRTopFs = memberRTopFs;
		ResultManager.memberRBtmFm = memberRBtmFm;
		ResultManager.memberRBtmFs = memberRBtmFs;
		ResultManager.gamePointL = drpMemberL.value;
		ResultManager.gamePointR = drpMemberR.value;
		ResultManager.addResultList ();
		ResultManager.Save ();

		clearValue ();
		OkCancel = true;

		// Pair,Match,Game数の記録
		saveGamePlus ();

		// 勝敗数の記録
		if (drpMemberL.value > drpMemberR.value) {			// 左側勝利
			GameManager.countUpPlayerWinLoseOfPlaceNo (regDateLTop, GameManager.GAMEWIN);
			GameManager.countUpPlayerWinLoseOfPlaceNo (regDateLBtm, GameManager.GAMEWIN);
			GameManager.countUpPlayerWinLoseOfPlaceNo (regDateRTop, GameManager.GAMELOSE);
			GameManager.countUpPlayerWinLoseOfPlaceNo (regDateRBtm, GameManager.GAMELOSE);
		} else if(drpMemberL.value < drpMemberR.value) {	// 右側勝利
			GameManager.countUpPlayerWinLoseOfPlaceNo (regDateLTop, GameManager.GAMELOSE);
			GameManager.countUpPlayerWinLoseOfPlaceNo (regDateLBtm, GameManager.GAMELOSE);
			GameManager.countUpPlayerWinLoseOfPlaceNo (regDateRTop, GameManager.GAMEWIN);
			GameManager.countUpPlayerWinLoseOfPlaceNo (regDateRBtm, GameManager.GAMEWIN);
		} else {		// 引き分け
			GameManager.countUpPlayerWinLoseOfPlaceNo (regDateLTop, GameManager.GAMEDRAW);
			GameManager.countUpPlayerWinLoseOfPlaceNo (regDateLBtm, GameManager.GAMEDRAW);
			GameManager.countUpPlayerWinLoseOfPlaceNo (regDateRTop, GameManager.GAMEDRAW);
			GameManager.countUpPlayerWinLoseOfPlaceNo (regDateRBtm, GameManager.GAMEDRAW);
		}

//		viewManager.scriptGameView.afterResultDialog (fncGroup);
		viewManager.closeResultView ();
	}
	public void saveGamePlus() {	// Pair,Match,Game数の記録
		// Pair,Match数の記録
		GameManager.setResltPairPlus (regDateLTop, regDateLBtm);
		GameManager.setResltPairPlus (regDateRTop, regDateRBtm);
		GameManager.setResltMatchPlus (regDateLTop, regDateRTop);
		GameManager.setResltMatchPlus (regDateLTop, regDateRBtm);
		GameManager.setResltMatchPlus (regDateLBtm, regDateRTop);
		GameManager.setResltMatchPlus (regDateLBtm, regDateRBtm);

		//Game数の記録
		GameManager.countUpPlayerGameOfPlaceNo (regDateLTop);
		GameManager.countUpPlayerGameOfPlaceNo (regDateLBtm);
		GameManager.countUpPlayerGameOfPlaceNo (regDateRTop);
		GameManager.countUpPlayerGameOfPlaceNo (regDateRBtm);

		MemberManager.Save ();	// Gamemanagerを介してGame数の更新が行われているため保存する
		GameManager.Save ();
	}
	public void _btnCancel() {
		clearValue ();
		OkCancel = false;
		viewManager.closeResultView ();
	}
	public string chkSumDate(string iSumDate) {	// 前回集計終了でなかったら前回のsumDateを使用する (途中でAppliを落とされた場合)
		string preSumDate =  ResultManager.chkSumDate ();
		if (preSumDate != null)
			sumDate = preSumDate;
		else
			sumDate = iSumDate;
		return sumDate;
	}

	private void clearValue() {
	}

	public void gameEndResult(string endDate) {
		if (ResultManager.getResultCountOfSumDate (sumDate) == 0)	// 試合データがなかったら終了データは作成しない
			return;
		
		ResultManager.initResult ();
		ResultManager.notice = 1;
		ResultManager.sumDate = sumDate;
		ResultManager.sumEndDate = endDate;
		ResultManager.addResultList ();
		ResultManager.Save ();
	}

	public void loadView() {
		txtCourtName.text =  courtName;
		txtMatchDate.text = sumDate.Substring (0, 4) + "年" + sumDate.Substring (5, 2) + "月" + sumDate.Substring (8, 2) + "日   " + sumDate.Substring (11, 2) + "時" + sumDate.Substring (14, 2) + "分";
		txtSTime.text = gameStartDate.Substring (11, 2) + "時" + gameStartDate.Substring (14, 2) + "分";
		gameDate = DateTime.Now.ToString ("s", new System.Globalization.CultureInfo("ja-JP"));
		txtETime.text = gameDate.Substring (11, 2) + "時" + gameDate.Substring (14, 2) + "分";
		txtMatchCount.text = "試合数："+ (GameManager.gameCount + 1);

		string nameLT = "";
		string nameLB = "";
		string nameRT = "";
		string nameRB = "";
		if (MemberManager.posMemberOfRegDate (regDateLTop))
			nameLT = MemberManager.nameKaji_family + " " + MemberManager.nameKaji_first;
		if (MemberManager.posMemberOfRegDate (regDateLBtm))
			nameLB = MemberManager.nameKaji_family + " " + MemberManager.nameKaji_first;
		if (MemberManager.posMemberOfRegDate (regDateRTop))
			nameRT = MemberManager.nameKaji_family + " " + MemberManager.nameKaji_first;
		if (MemberManager.posMemberOfRegDate (regDateRBtm))
			nameRB = MemberManager.nameKaji_family + " " + MemberManager.nameKaji_first;

		if (nameLT != "" && nameLB != "")
			txtMemberL.text = nameLT + "\n" + nameLB;
		else if (nameLT != "")
			txtMemberL.text = nameLT;
		else
			txtMemberL.text = nameLB;

		if (nameRT != "" && nameRB != "")
			txtMemberR.text = nameRT + "\n" + nameRB;
		else if (nameRT != "")
			txtMemberR.text = nameRT;
		else
			txtMemberR.text = nameRB;

		drpMemberL.value = 0;
		drpMemberR.value = 0;
 	}

	void Start () {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
		txtCourtName = this.transform.FindChild ("ResultPanel/DialogPanel/InputGroup/CourtName").GetComponent<Text> ();
		txtMatchDate = this.transform.FindChild ("ResultPanel/DialogPanel/InputGroup/txtMatchDate").GetComponent<Text> ();
		txtSTime = this.transform.FindChild ("ResultPanel/DialogPanel/InputGroup/txtSTime").GetComponent<Text> ();
		txtETime = this.transform.FindChild ("ResultPanel/DialogPanel/InputGroup/txtETime").GetComponent<Text> ();
		txtMatchCount = this.transform.FindChild ("ResultPanel/DialogPanel/InputGroup/MatchCount").GetComponent<Text> ();
		txtMemberL = this.transform.FindChild ("ResultPanel/DialogPanel/InputGroup/Left/MemberL").GetComponent<Text> ();
		txtMemberR = this.transform.FindChild ("ResultPanel/DialogPanel/InputGroup/Right/MemberR").GetComponent<Text> ();
		drpMemberL = this.transform.FindChild ("ResultPanel/DialogPanel/InputGroup/Left/drpPointL").GetComponent<Dropdown> ();
		drpMemberR = this.transform.FindChild ("ResultPanel/DialogPanel/InputGroup/Right/drpPointR").GetComponent<Dropdown> ();
		this.gameObject.SetActive (false);
	}
}
