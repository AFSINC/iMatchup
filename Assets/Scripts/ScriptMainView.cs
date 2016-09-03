using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

[System.Serializable]
public class ScriptMainView : MonoBehaviour {
	public GameObject myView;
	ViewManager viewManager;
	Text lblActive;
	public GameObject pfbMember;
	Transform tPfbParent;  // LayoutVertical(プレハブMemberの親)
	AudioSource audioSrc;

    // DEBUG用
    public void _OnDebug() {
        SoundManager.play(SoundManager.VOICE00);
        SettingManager.DEBUG_MODE = true;
        viewManager.scriptDebugView.debugEnable(SettingManager.DEBUG_MODE);
    }

    // スタート
    public void _CallBackBtnStart() {
		SoundManager.play (SoundManager.CLICK01);
		MemberManager.Save ();
		viewManager.chgExecStartView(myView, viewManager.IN_RIGHT);
	}

	// シャッフル
	public void _CallBackBtnShuffle() {
		SoundManager.play (SoundManager.CLICK01);
		int iMemCnt = MemberManager.getActiveMemberCount ();
		for (int i=0; i<iMemCnt; i++) {
			MemberManager.movMember (i, Random.Range (0, iMemCnt));
		}
		loadView ();
	}

	// メンバー選択View
	public void _CallBackBtnSelect() {
		SoundManager.play (SoundManager.CLICK01);
		MemberManager.Save ();
		viewManager.scriptSelectView.loadView ();
		viewManager.chgSelsectView(myView, viewManager.IN_RIGHT);
	}

	// お知らせView
	public void _CallBackBtnNotice() {
		SoundManager.play (SoundManager.CLICK01);
		MemberManager.Save ();
		viewManager.chgNoticeView(myView, viewManager.IN_RIGHT);
	}

	// 設定View
	public void _CallBackBtnSetting() {
		SoundManager.play (SoundManager.CLICK01);
		viewManager.chgSettingView(myView, viewManager.IN_RIGHT);
	}

	public void loadView() {
		MemberManager.Load ();
		// 活動中ラベル
		string sTotal = MemberManager.getMemberCount ().ToString();
		string sActive = MemberManager.getActiveMemberCount ().ToString();
		lblActive.text = "選択中 : " + sActive + "/" + sTotal + "名";

		if (GameManager.gameStatus != 0)
			this.transform.FindChild ("ActiveMemberListPanel/txtDoubleTap").gameObject.SetActive (true);
		else
			this.transform.FindChild ("ActiveMemberListPanel/txtDoubleTap").gameObject.SetActive (false);

		// リスト全削除
		int pfCnt = tPfbParent.childCount;
		for (int i=0; i < pfCnt; i++) {
			DestroyImmediate(tPfbParent.Find ("ListMember").gameObject);
		}

		// リスト作成
		tPfbParent.GetComponent<RectTransform>().anchoredPosition = new Vector2 (0f, 395.5f);
		MemberManager.sortAvtiveMember ();
		for (int i = 0; i < MemberManager.getActiveMemberCount (); i++) {
			Transform pt = Instantiate (pfbMember).transform;
			pt.name = pfbMember.name;
			pt.SetParent (tPfbParent, false);

			MemberManager.posMember (i);
			string sname = MemberManager.nameKaji_family + " " + MemberManager.nameKaji_first;
			pt.GetComponentInChildren <Text> ().text = sname;

			if (MemberManager.gender == 0) {
				pt.GetComponentInChildren <Image> ().color = Colors.male;
			} else {	
				pt.GetComponentInChildren <Image> ().color = Colors.female;
			}

			// prefab に regDate を設定
			pt.FindChild("baseWhite"). GetComponent<ListItemMainView> ().myRegDate = MemberManager.regDate;
		}
    }

    void Start () {
		audioSrc = this.transform.GetComponent<AudioSource> ();
		MemberManager.initMember (); //MemberManagerの初期化(ここで1回のみ)
		PairManager.initPair (); //PairManagerの初期化(ここで1回のみ)
		this.transform.Find ("TitlePanel/btnDEBUG").gameObject.SetActive (SettingManager.DEBUG_MODE);
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
		Vector2 rectMainPanel = this.transform.TransformVector (new Vector3((this.transform as RectTransform).rect.width, (this.transform as RectTransform).rect.height));
		viewManager.MOVE_DEST = (this.transform as RectTransform).rect.width;
		lblActive = this.transform.Find ("ActiveMemberListPanel/lblActive").GetComponent<Text> ();
		tPfbParent = this.transform.Find ("ActiveMemberListPanel/ListScroll/LayoutVertical");
		if (SettingManager.form == 0)
			SettingManager.initSetData();
		
//		GameObject.Find ("Game/MainPanel/PlayPanel/SclGame").GetComponent<PageScrollRect>().pageNum = (SettingManager.courtNum + 1) / 2;
		SettingManager.Save ();
		MemberManager.Save ();
		GameManager.Save ();
		PairManager.Save ();

		//if (GameManager.gameStatus != 0)
		//	viewManager.scriptGameView.dispExec (true);
		loadView ();
	}
}
