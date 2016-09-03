using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScriptNoticeView : MonoBehaviour {
	public GameObject myView;
	ViewManager viewManager;
	public GameObject pfbMember;
	private Transform tPfbParent; 	 // LayoutVertical(プレハブNoticeElementの親)
	private Dropdown dSort;
	int sortSave;								// sortの一時保存、詳細から戻ったとき同じsortとなるように前回値を保存
	private Transform delElmSave;	// 通知elementを削除時のために一時保管

	public void _CallBackBtnCancel() {
		viewManager.chgMainView(myView, viewManager.OUT_RIGHT);
	}
	public void _CallBackBtnWinLose() {
		SoundManager.play (SoundManager.CLICK01);
		viewManager.chgNoticeWinLoseView(myView, viewManager.IN_RIGHT);
	}
	public void _CallBackBtnAppInfo() {
		SoundManager.play (SoundManager.CLICK01);
		viewManager.chgNoticeAppInfoView(myView, viewManager.IN_RIGHT);
	}
	public void _CallBackBtnRegular() {
		SoundManager.play (SoundManager.CLICK01);
		viewManager.chgNoticeRegulerView(myView, viewManager.IN_RIGHT);
	}
	public void _CallBackBtnTmt() {
		viewManager.chgNoticeTmtView(myView, viewManager.IN_RIGHT);
	}
	public void _CallBackBtnInf() {
		viewManager.chgNoticeInfView(myView, viewManager.IN_RIGHT);
	}
	public void _OnSort() {
		List<Transform> sortList = new List<Transform> ();
		int trsNum = tPfbParent.childCount;
		for (int cnt=0; cnt<trsNum; cnt++) {
			sortList.Add (tPfbParent.GetChild(cnt));
		}

		// 指定Sortの前に、条件外でシステムメッセージ系が下段に来るように初期sortしておく
		sortList = sortList.OrderBy (t => t.FindChild("ImgItem").GetComponent<NoticeElement>().category).ToList<Transform>();
		for (int cnt=0; cnt<trsNum; cnt++) {
			sortList [cnt].SetAsLastSibling ();
		}

		switch (dSort.value) {
		case 0:		// 新着
			sortList = sortList.OrderByDescending (t => t.FindChild("ImgItem").GetComponent<NoticeElement>().sumDate).ToList<Transform>();
			sortList = sortList.OrderByDescending (t => t.FindChild("ImgItem").GetComponent<NoticeElement>().noticeDate).ToList<Transform>();
			break;
		case 1:		// 未読
			sortList = sortList.OrderBy (t => t.FindChild("ImgItem").GetComponent<NoticeElement>().readStat).ToList<Transform>();
			break;
		case 2:		// カテゴリー
			sortList = sortList.OrderBy (t => t.FindChild("ImgItem").GetComponent<NoticeElement>().category).ToList<Transform>();
			break;
		default:
			break;
		}
		sortSave = dSort.value;
		for (int cnt=0; cnt<trsNum; cnt++) {
			sortList [cnt].SetAsLastSibling ();
		}
	}

	// NoticeElementから呼び出される (通知内容を表示させるため、内容表示パネルを開く)
	public void _CallBackBtnDispNoticeinfoElement(Transform tElm, string sCategory, string sNtcDate, string sPara1, string sPara2) {
		delElmSave = tElm;		// prefabのtransform 内容表示で削除が選ばれたときに使用する

		if (sCategory == NoticeManager.CATE_SYSTEM) {
			viewManager.scriptNoticeInfView.category = sCategory;
			viewManager.scriptNoticeInfView.noticeDate = sNtcDate;
			viewManager.scriptNoticeInfView.noticeTitle = sPara1;
			viewManager.scriptNoticeInfView.message = sPara2;
			viewManager.chgNoticeInfView (myView, viewManager.IN_RIGHT);
			// 既読
			if (NoticeManager.posNoticeSYSOfNtcDate (sCategory, sNtcDate)) {
				tElm.FindChild ("txtNew").gameObject.SetActive (false);
                tElm.GetComponent<Image>().color = Colors.White;
                tElm.FindChild("txtTitle").GetComponent<Outline>().enabled = false;
                tElm.GetComponent<NoticeElement> ().readStat = 1;
				NoticeManager.readStat = 1;
			}
		} else if (sCategory == NoticeManager.CATE_RESULT) {
			viewManager.scriptNoticeTmtView.category = sCategory;
			viewManager.scriptNoticeTmtView.sumDate = sPara1;
			viewManager.scriptNoticeTmtView.sumEndDate = sPara2;
			// 既読
			if (NoticeManager.posNoticeRESOfNtcDate (sCategory, sNtcDate, sPara1)) {
				tElm.FindChild ("txtNew").gameObject.SetActive (false);
                tElm.GetComponent<Image>().color = Colors.White;
                tElm.FindChild("txtTitle").GetComponent<Outline>().enabled = false;
                tElm.GetComponent<NoticeElement> ().readStat = 1;
				NoticeManager.readStat = 1;
			}
			viewManager.chgNoticeTmtView (myView, viewManager.IN_RIGHT);
		} else if (sCategory == NoticeManager.CATE_NETWK) {
			// 既読
			if (NoticeManager.posNoticeNETOfNtcDate (sCategory, sNtcDate, sPara1)) {
				tElm.FindChild ("txtNew").gameObject.SetActive (false);
                tElm.GetComponent<Image>().color = Colors.White;
                tElm.FindChild("txtTitle").GetComponent<Outline>().enabled = false;
                tElm.GetComponent<NoticeElement> ().readStat = 1;
				NoticeManager.readStat = 1;
			}
			viewManager.chgNoticeInfView (myView, viewManager.IN_RIGHT);
		}
		NoticeManager.Save ();
	}
	// 内容表示パネルから削除が選択されたときに呼び出される
	public void deleteNoticeItem(Transform iTrs, string iCategory, string iPara1, string iPara2) {
//		DestroyImmediate (delElmSave.gameObject);
		DestroyImmediate (iTrs.gameObject);

		if (iCategory == NoticeManager.CATE_SYSTEM) {
			NoticeManager.delSysNoticeOfNDate (iPara1);
		} else if (iCategory == NoticeManager.CATE_RESULT) {
			ResultManager.deletesResultOfSumDate(iPara1);
			NoticeManager.delResNoticeOfResult (iPara1);
		} else if (iCategory == NoticeManager.CATE_NETWK) {
		} 
	}


	public void loadView () {
		if (SettingManager.REGULAR_MODE)
            this.transform.FindChild("FuncPanel").gameObject.SetActive(true);
        else
            this.transform.FindChild("FuncPanel").gameObject.SetActive(false);

		NoticeManager.getResNoticeCount ();
		// リスト全削除
		int pfCnt = tPfbParent.childCount;
		for (int i = 0; i < pfCnt; i++) {
			DestroyImmediate (tPfbParent.Find ("NoticeElement").gameObject);
		}

		// ビルトインSYSMessageデータ作成
		createBuiltInMessage ();
		// Resultデータ作成 (新規 結果集計)
		createNoticeFromResult ();

		// リストPrefab作成
		// リスト作成 (ビルトインSYSMessage)
		for (int pCnt = 0; pCnt < NoticeManager.getSysNoticeCount (); pCnt++) {
			NoticeManager.posNoticeOfListIdx (pCnt);
			if (NoticeManager.noDispFlg)
				continue;
			
			Transform pt = Instantiate (pfbMember).transform;
			pt.name = pfbMember.name;
			pt.SetParent (tPfbParent, false);

			if (NoticeManager.readStat == 0) {
				pt.FindChild("ImgItem/txtNew").gameObject.SetActive (true);
                pt.FindChild("ImgItem/txtTitle").GetComponent<Outline>().enabled = true;
                pt.FindChild("ImgItem").GetComponent<Image>().color = Colors.Lightyellow;
            }
            else {
				pt.FindChild("ImgItem/txtNew").gameObject.SetActive (false);
                pt.FindChild("ImgItem/txtTitle").GetComponent<Outline>().enabled = false;
                pt.FindChild("ImgItem").GetComponent<Image>().color = Colors.White;
            }

            if (NoticeManager.category == NoticeManager.CATE_SYSTEM) {
				pt.FindChild ("ImgItem/txtTitle").GetComponent<Text> ().text = NoticeManager.noticeTitle;
				NoticeElement noticeElement = pt.FindChild ("ImgItem").GetComponent<NoticeElement> ();
				noticeElement.readStat = NoticeManager.readStat;
				noticeElement.category = NoticeManager.category;
				noticeElement.noticeDate = NoticeManager.noticeDate;
				noticeElement.title = NoticeManager.noticeTitle;
				noticeElement.message = NoticeManager.message;
			}
		}
		// リスト作成 (結果集計)
		for (int pCnt = 0; pCnt < NoticeManager.getResNoticeCount (); pCnt++) {
			NoticeManager.posNoticeOfListIdx (pCnt);
			Transform pt = Instantiate (pfbMember).transform;
			pt.name = pfbMember.name;
			pt.SetParent (tPfbParent, false);

			if (NoticeManager.readStat == 0) {
				pt.FindChild ("ImgItem/txtNew").gameObject.SetActive (true);
                pt.FindChild("ImgItem/txtTitle").GetComponent<Outline>().enabled = true;
                pt.GetComponent<Image>().color = Colors.Lightyellow;
            }
            else {
				pt.FindChild ("ImgItem/txtNew").gameObject.SetActive (false);
                pt.FindChild("ImgItem/txtTitle").GetComponent<Outline>().enabled = false;
                pt.GetComponent<Image>().color = Colors.White;
            }

            if (NoticeManager.category == NoticeManager.CATE_RESULT) {
				string txtTitle = NoticeManager.sumDate.Substring (0, 4) + "年" + NoticeManager.sumDate.Substring (5, 2) + "月" + NoticeManager.sumDate.Substring (8, 2) +"日";
				pt.FindChild ("ImgItem/txtTitle").GetComponent<Text> ().text = "試合集計結果  " + txtTitle;
				NoticeElement noticeElement = pt.FindChild ("ImgItem").GetComponent<NoticeElement> ();
				noticeElement.readStat = NoticeManager.readStat;
				noticeElement.category = NoticeManager.category;
				noticeElement.noticeDate = NoticeManager.noticeDate;
				noticeElement.sumDate = NoticeManager.sumDate;
				noticeElement.sumEndDate = NoticeManager.sumEndDate;
			}
		}
		_OnSort ();
	}


	// ビルトインメッセージがなければNoticeDataを作成
	private void createBuiltInMessage() {
		List<string> ntcDate = new List<string> () {"2016-10-10T00:00:01", "2016-10-10T00:00:02"};	 //このitem数に対応するメッセージを用意する
		for (int cnt=0; cnt<ntcDate.Count; cnt++) {
			if (!NoticeManager.chkSysNoticeOfNDate (ntcDate [cnt]))
				initBuiltInMessage (ntcDate [cnt], cnt);
		}
	}
	private void initBuiltInMessage(string iNoticeDate, int iCnt) {	// 組み込みメッセージの表示データを作成
		NoticeManager.initNotice ();
		NoticeManager.category = NoticeManager.CATE_SYSTEM;
		NoticeManager.noDispFlg = false;
		switch (iCnt) {
		case 0:
    			NoticeManager.noticeDate = iNoticeDate;
	       		NoticeManager.noticeTitle = "初期バージョン機能のご紹介";
                NoticeManager.message = "この度は、iMatchupをダウンロードして頂き、ありがとうございます。\n\n";
                NoticeManager.message += "初期バージョンは10月に完成予定です。\n";
                NoticeManager.addNotice ();
			break;
		case 1:
			    NoticeManager.noticeDate = iNoticeDate;
			    NoticeManager.noticeTitle = "有償版機能のご紹介";
		    	NoticeManager.message = "\n有償版のみで利用できる機能をお知らせ致します。\n\n【有償版のみでの限定機能】\n１：試合集計中の選手の追加\n２：試合集計中の選手のパス・退出\n３：ペア固定の設定\n４：お知らせに「試合集計結果」の表示\n５：直木賞受賞作家\u3000荒木氏の書き下ろし短編ミステリー小説「振り向けばウィンブル丼」\n６：松田氏熱血講和「新宿２丁目、あの日、僕は...」\n７：Sdhindraと行く真夏のインド日帰りの旅\n８：Kalyaniスマイリー教室 冷蔵庫持参者限定\n９：吉野野球教室 ケツバット100回無料\n10：澤田ヨガ教室入会記念 3000割増クーポン\n\n有償版へのアップグレードをお待ちしております。";
                NoticeManager.message = "【 有償版限定機能のご紹介 】\n";
                NoticeManager.message += "有償版では、無償版にはない便利な機能が使用できるようになります。\n";
                NoticeManager.message += "\n1. ペア設定\n";
                NoticeManager.message += "メンバー管理でペア設定が可能となります。設定されたペアは組み合せ実行時に、常にペア固定で表示されます。\n";
                NoticeManager.message += "\n2. 組み合せ実行時のメンバー追加\n";
                NoticeManager.message += "組み合せ実行中に、実行終了することなく、選択されていないメンバーの追加と、未登録メンバーの新規登録が可能となります。\n";
                NoticeManager.message += "\n3. 組み合せ実行時のパス・退出\n";
                NoticeManager.message += "組み合せ実行中に、実行終了することなく、メンバーのパスと、退出が可能となります。\n";
                NoticeManager.message += "\n4.試合の集計結果を表示\n";
                NoticeManager.message += "設定から試合の結果集計の有無が設定可能となります。組み合せ実行毎の集計結果はお知らせに表示されます。\n";
                NoticeManager.message += "\n5.総対戦成績の表示\n";
                NoticeManager.message += "集計結果を個人別に累積した総対戦成績が、お知らせに表示されるようになります。\n";
                NoticeManager.addNotice ();
			break;
		default:
			break;
		}
	}
	// 新しい集計結果があればNoticeDataを作成
	private int createNoticeFromResult() {
		int cnt = 0;

		while (ResultManager.posNewResult()) {
			NoticeManager.initNotice ();
			NoticeManager.category = NoticeManager.CATE_RESULT;
			NoticeManager.sumDate = ResultManager.sumDate;
			NoticeManager.sumEndDate = ResultManager.sumEndDate;
			NoticeManager.addNotice ();
			cnt++;
		}
		NoticeManager.Save ();
		return cnt;
	} 


	void Start () {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
		tPfbParent = this.transform.Find("NoticePanel/ListScroll/LayoutVertical");
		dSort = this.transform.Find ("NoticePanel/DrpSort").GetComponent<Dropdown> ();
		dSort.value = 0;
//		this.gameObject.SetActive (false);
	}
}
