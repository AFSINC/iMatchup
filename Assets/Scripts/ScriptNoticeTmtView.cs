using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScriptNoticeTmtView : MonoBehaviour {
	public GameObject myView;
	ViewManager viewManager;
	public GameObject pfbGame;
	public GameObject pfbMember;
	private Transform tPfbGameParent;		// LayoutVertical(プレハブResultGameの親)
	private Transform tPfbMemberParent;	// LayoutVertical(プレハブResultMembertの親)
	public string category;				// 呼び出し時にNoticeviewからセット
	public string sumDate;				// 呼び出し時にNoticeviewからセット
	public string sumEndDate;		// 呼び出し時にNoticeviewからセット

	public void _CallBackBtnCancel() {
		viewManager.chgNoticeView(myView, viewManager.OUT_RIGHT);
	}
	public void _CallBackBtnDelete() {
		string title = "通知の削除確認";
		string message = "この通知を削除します。";
		DialogViewController.Show (title, message, new DialogViewOptions {
			btnCancelTitle = "キャンセル", btnCancelDelegate = () => {
				return;
			},
			btnOkTitle = "削除", btnOkDelegate = () => {
				viewManager.scriptNoticeView.deleteNoticeItem (this.transform, category, sumDate, sumEndDate);
				viewManager.chgNoticeView(myView, viewManager.OUT_RIGHT);
			},
		});
	}
	// 試合別結果リスト
	public void _CallBackBtnGameSum() {
		this.transform.FindChild ("ResultPanel/ListGame").gameObject.SetActive (true);
		this.transform.FindChild ("ResultPanel/ListMember").gameObject.SetActive (false);

		string txtTitleS = sumDate.Substring (0, 4) + "年" + sumDate.Substring (5, 2) + "月" + sumDate.Substring (8, 2) +"日   " + sumDate.Substring (11, 2) + "時" + sumDate.Substring (14, 2) + "分";
		string txtTitleE = sumEndDate.Substring (11, 2) + "時" + sumEndDate.Substring (14, 2) + "分";
		this.transform.FindChild ("ResultPanel/txtDate").GetComponent<Text> ().text = txtTitleS + " ~ " + txtTitleE;

		// リスト全削除
		int pfCnt = tPfbGameParent.childCount;
		for (int i = 0; i < pfCnt; i++) {
			DestroyImmediate (tPfbGameParent.Find ("ResultGame").gameObject);
		}

		int iResNum = ResultManager.getResultCountOfSumDate (sumDate);
		for (int pCnt = 0; pCnt < iResNum; pCnt++) {
			ResultManager.posResultOfSumDate (sumDate);
			ResultManager.posResultOfListIdx (pCnt);

			Transform pt = Instantiate (pfbGame).transform;
			pt.name = pfbGame.name;
			pt.SetParent (tPfbGameParent, false);

			// 試合時間
			string strSTime = ResultManager.gameSDate.Substring (11, 2) + "時" + ResultManager.gameSDate.Substring (14, 2) + "分";
			string strETime = ResultManager.gameEDate.Substring (11, 2) + "時" + ResultManager.gameEDate.Substring (14, 2) + "分";
			pt.FindChild ("cmnInf/txtTime").GetComponent<Text> ().text = strSTime + " ~ " + strETime;

			// コート名
			pt.FindChild ("cmnInf/txtCourt").GetComponent<Text> ().text = ResultManager.courtName;

			// 選手名
			string strName = "";
			string strNameLT = ResultManager.memberLTopFm + " " + ResultManager.memberLTopFs;
			string strNameLB = ResultManager.memberLBtmFm + " " + ResultManager.memberLBtmFs;
			string strNameRT = ResultManager.memberRTopFm + " " + ResultManager.memberRTopFs;
			string strNameRB = ResultManager.memberRBtmFm + " " + ResultManager.memberRBtmFs;
			if (strNameLT != " " && strNameLB != " ")
				strName = strNameLT + "\n" + strNameLB;
			else 	if (strNameLT != " ")
				strName = strNameLT;
			else
				strName = strNameLB;
			pt.FindChild ("pointInf/LeftMember/Member").GetComponent<Text> ().text = strName;

			if (strNameRT != " " && strNameRB != " ")
				strName = strNameRT + "\n" + strNameRB;
			else 	if (strNameRT != " ")
				strName = strNameRT;
			else
				strName = strNameRB;
			pt.FindChild ("pointInf/RightMember/Member").GetComponent<Text> ().text = strName;

			// 点数
			pt.FindChild ("pointInf/Separator/txtPoint").GetComponent<Text> ().text = ResultManager.gamePointL + " - " + ResultManager.gamePointR;

			// 王冠イメージ
			if (ResultManager.gamePointL > ResultManager.gamePointR) {
				pt.FindChild ("pointInf/LeftMember/ImgWinLose").gameObject.SetActive (true);
				pt.FindChild ("pointInf/RightMember/ImgWinLose").gameObject.SetActive (false);
			} else if (ResultManager.gamePointL < ResultManager.gamePointR) {
				pt.FindChild ("pointInf/LeftMember/ImgWinLose").gameObject.SetActive (false);
				pt.FindChild ("pointInf/RightMember/ImgWinLose").gameObject.SetActive (true);
			} else {
				pt.FindChild ("pointInf/LeftMember/ImgWinLose").gameObject.SetActive (false);
				pt.FindChild ("pointInf/RightMember/ImgWinLose").gameObject.SetActive (false);
			}
		}
		this.transform.Find ("ResultPanel/ListGame").GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
	}
	// 個人別勝敗リスト
	public void _CallBackBtnMemberSum() {
		this.transform.FindChild ("ResultPanel/ListMember").gameObject.SetActive (true);
		this.transform.FindChild ("ResultPanel/ListGame").gameObject.SetActive (false);

		// リスト全削除
		int pfCnt = tPfbMemberParent.childCount;
		for (int i = 0; i < pfCnt; i++) {
			DestroyImmediate (tPfbMemberParent.Find ("ResultMember").gameObject);
		}
		int iMemberNum = ResultManager. getResultCountOfOfMember (sumDate);
		for (int mCnt = 0; mCnt < iMemberNum; mCnt++) {
			string[] resMember = ResultManager.resMb [mCnt].Split ('|');
			string strMemberName = resMember[1];
			string pRegDate = resMember[2];
			if (pRegDate == "")
				continue;				// シングルスの場合の空き枠のデータ

			Transform pt = Instantiate (pfbMember).transform;
			pt.name = pfbMember.name;
			pt.SetParent (tPfbMemberParent, false);

			float iWin = 0;
			float iLose = 0;
			float iDraw = 0;
			int iResNum = ResultManager.getResultCountOfSumDate (sumDate);
			for (int gCnt = 0; gCnt < iResNum; gCnt++) {
				ResultManager.posResultOfSumDate (sumDate);
				ResultManager.posResultOfListIdx (gCnt);
				if (ResultManager.regDateLTop == pRegDate) {
					if (ResultManager.gamePointL > ResultManager.gamePointR)
						iWin++;
					else if (ResultManager.gamePointL < ResultManager.gamePointR)
						iLose++;
					else
						iDraw++;
				} if (ResultManager.regDateLBtm == pRegDate) {
					if (ResultManager.gamePointL > ResultManager.gamePointR)
						iWin++;
					else if (ResultManager.gamePointL < ResultManager.gamePointR)
						iLose++;
					else
						iDraw++;
				} else if (ResultManager.regDateRTop == pRegDate) {
					if (ResultManager.gamePointL < ResultManager.gamePointR)
						iWin++;
					else if (ResultManager.gamePointL > ResultManager.gamePointR)
						iLose++;
					else
						iDraw++;
				} else if (ResultManager.regDateRBtm == pRegDate) {
					if (ResultManager.gamePointL < ResultManager.gamePointR)
						iWin++;
					else if (ResultManager.gamePointL > ResultManager.gamePointR)
						iLose++;
					else
						iDraw++;
				}
			}
			string strWinLose = iWin + "勝 " + iLose + "敗";
			strWinLose += "\n試合数:" + (iWin + iLose + iDraw);
			string strWinPer = "";
			if (iDraw != 0)
				strWinPer = iDraw + "引分";
			strWinPer += "\n勝率:" + Mathf.Round(iWin / (iWin + iLose + iDraw)*100) + "%";

			pt.FindChild ("txtMember").GetComponent<Text> ().text = strMemberName;
			pt.FindChild ("txtWinLose").GetComponent<Text> ().text = strWinLose;
			pt.FindChild ("txtWinPer").GetComponent<Text> ().text = strWinPer;
		}
	}
		
	public void loadView () {
		_CallBackBtnGameSum ();
	}


	// Use this for initialization
	void Start () {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
		tPfbGameParent = this.transform.FindChild ("ResultPanel/ListGame/LayoutVertical");
		tPfbMemberParent = this.transform.FindChild ("ResultPanel/ListMember/LayoutVertical");
		this.gameObject.SetActive (false);
	}
}
