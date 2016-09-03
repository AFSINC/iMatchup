using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScriptNoticeWinLoseView : MonoBehaviour {
	public GameObject myView;
	ViewManager viewManager;
	public GameObject pfbMember;
	private Transform tPfbParent; 	 // LayoutVertical(プレハブWinLoseElementの親)
	private Dropdown dSort;
	int sortSave;								// sortの一時保存、詳細から戻ったとき同じsortとなるように前回値を保存
	private int preSelectViewSort = 99;

	public void _CallBackBtnCancel() {
		preSelectViewSort = 99;
		viewManager.chgNoticeView(myView, viewManager.OUT_RIGHT);
	}
	public void _OnSort() {
		if (preSelectViewSort == dSort.value)
			return;
		
		switch (dSort.value) {
		case 0:
			MemberManager.sortMemberName ();
			break;
		case 1:
			MemberManager.sortGame ();
			break;
		case 2:
			MemberManager.sortWin ();
			break;
		case 3:
			MemberManager.sortWinPer ();
			break;
		case 4:
			MemberManager.sortGender ();
			break;
		default:
			MemberManager.sortMemberName ();
			break;
		}
		preSelectViewSort = dSort.value;
		loadView ();
	}

    public void _OnReset() {
        string title = "総対戦成績リセットの確認";
        string message = "全メンバーの総対戦成績をゼロにリセットしますか？";
        DialogViewController.Show(title, message, new DialogViewOptions {
            btnCancelTitle = "キャンセル",
            btnCancelDelegate = () => {
                return;
            },
            btnOkTitle = "OK",
            btnOkDelegate = () => {
                resetOK();
            },
        });
    }
    private void resetOK() {
        // MemberManagerの対戦数reset処理
        loadView();
    }
    public void loadView() {
		// リスト全削除
		int pfCnt = tPfbParent.childCount;
		for (int i = 0; i < pfCnt; i++) {
			DestroyImmediate (tPfbParent.Find ("WinLoseElement").gameObject);
		}

		// リストPrefab作成
		for (int pCnt = 0; pCnt < MemberManager.getMemberCount (); pCnt++) {
			MemberManager.posMember (pCnt);
			Transform pt = Instantiate (pfbMember).transform;
			pt.name = pfbMember.name;
			pt.SetParent (tPfbParent, false);

			pt.FindChild ("ImgItem/txtMember").GetComponent<Text> ().text = MemberManager.nameKaji_family + " " + MemberManager.nameKaji_first;
			if (MemberManager.gender == 0)
				pt.FindChild ("ImgItem").GetComponent<Image> ().color = Colors.male;
			else
				pt.FindChild ("ImgItem").GetComponent<Image> ().color = Colors.female;

			float iWin = MemberManager.win;
			float iLose = MemberManager.lose;
			float iDraw = MemberManager.draw;
			string strWinLose = iWin + "勝 " + iLose + "敗";
			strWinLose += "\n試合数:" + (iWin + iLose + iDraw);
			string strWinPer = "";
			if (iDraw != 0)
				strWinPer = iDraw + "引分";
			float fGame = iWin + iLose + iDraw;
			if (fGame != 0)
				strWinPer += "\n勝率:" + Mathf.Round(iWin / fGame * 100) + "%";

			pt.FindChild ("ImgItem/txtWinLose").GetComponent<Text> ().text = strWinLose;
			pt.FindChild ("ImgItem/txtWinPer").GetComponent<Text> ().text = strWinPer;
		}
		_OnSort ();
	}

	void Start () {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
		tPfbParent = this.transform.Find("WinLosePanel/ListScroll/LayoutVertical");
		dSort = this.transform.Find ("WinLosePanel/DrpSort").GetComponent<Dropdown> ();
		dSort.value = 0;
		this.gameObject.SetActive (false);
	}
}
