using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class ViewManager : MonoBehaviour {
	public readonly int NONE = 0;
	public readonly int IN_TOP = 1;
	public readonly int IN_BOTTOM = 2;
	public readonly int IN_RIGHT = 3;
	public readonly int IN_LEFT = 4;
	public readonly int OUT_TOP = 5;
	public readonly int OUT_BOTTOM = 6;
	public readonly int OUT_RIGHT = 7;
	public readonly int OUT_LEFT = 8;
	public float MOVE_DEST = 750f;
	public readonly float MOVE_SEC = 0.3f;

	private	RectTransform cvTfm;
	private GameObject soucViewObj, destViewObj;
	public GameObject DebugViewHolder;
	public ScriptDebugView scriptDebugView;
    public GameObject TopViewHolder;
    public ScriptTopView scriptTopView;
    public GameObject MainViewHolder;
	public ScriptMainView scriptMainView;
	public GameObject RegisterMemberViewHolder;
	public ScriptRegisterView scriptRegisterView;
	public GameObject SelectViewHolder;
	public ScriptSelectView scriptSelectView;
	public GameObject PareViewHolder;
	public ScriptPairView scriptPairView;
	public GameObject PairSelectViewHolder;
	public ScriptPairSelectView scriptPairSelectView;
	public GameObject EditMemberViewHolder;
	public ScriptEditMemberView scriptEditMemberView;
	public GameObject NoticeViewHolder;
	public ScriptNoticeView scriptNoticeView;
	public GameObject NoticeRegulerHolder;
	public ScriptNoticeWinLoseView scriptNoticeWinLoseView;
	public GameObject NoticeWinLoseHolder;
	public ScriptNoticeRegularView scriptNoticeRegulerView;
	public GameObject NoticeAppInfoViewHolder;
	public ScriptNoticeAppInfoView scriptNoticeAppInfoView;
	public GameObject NoticeTmtViewHolder;
	public ScriptNoticeTmtView scriptNoticeTmtView;
	public GameObject NoticeInfViewHolder;
	public ScriptNoticeInfView scriptNoticeInfView;
	public GameObject SettingViewHolder;
	public ScriptSettingView scriptSettingView;
	public GameObject SettingCourtViewHolder;
	public ScriptSettingCourtView scriptSettingCourtView;
	public GameObject GameViewHolder;
	public ScriptGameView scriptGameView;
	public GameObject ExecStartViewHolder;
	public ScriptExecStartView scriptExecStartView;
    public GameObject ExecSelectMemberViewHolder;
    public ScriptExecSelectMemberView scriptExecSelectMemberView;
    public GameObject ExecRegisterViewHolder;
    public ScriptExecRegisterView scriptExecRegisterView;
    public GameObject ResultViewHolder;
	public ScriptResultView scriptResultView;

	private GameObject ViewHolder;


	public void opnDebugView(int viewPtn) {
//        AdMobManager.bannerView.Hide();
        switch (viewPtn) {
		case 1:
			scriptDebugView.updateView1 ();
			break;
		case 2:
			scriptDebugView.updateView2 ();
			break;
		case 3:
			scriptDebugView.updateView3 ();
			break;
		case 4:
			scriptDebugView.updateView4 ();
			break;
		default :
			break;
		}
		DebugViewHolder.SetActive (true);
		DebugViewHolder.transform.SetAsLastSibling ();
	}
	public void closeDebugView() {
        DebugViewHolder.SetActive (false);
		DebugViewHolder.transform.SetAsFirstSibling ();
	}

	public void chgViewHolderView(GameObject sourceView, int direct) {
        changeView(sourceView, ViewHolder, direct);
	}
	public void chgMainView(GameObject sourceView, int direct) {
        scriptMainView.loadView ();
		changeView (sourceView, MainViewHolder, direct);
	}
	public void chgRegisterMembeView(GameObject sourceView, int direct) {
        ViewHolder = sourceView;
		changeView (sourceView, RegisterMemberViewHolder, direct);
		scriptRegisterView.loadView ();
	}
	public void chgSelsectView(GameObject sourceView, int direct) {
        changeView(sourceView, SelectViewHolder, direct);
	}
	public void chgPareView(GameObject sourceView, int direct) {
        changeView(sourceView, PareViewHolder, direct);
		scriptPairView.loadView ();
	}
	public void chgPareSelectView(GameObject sourceView, int direct) {
        changeView(sourceView, PairSelectViewHolder, direct);
		scriptPairSelectView.loadView ();
	}
	public void chgEditMemberView(GameObject sourceView, int direct) {
        changeView(sourceView, EditMemberViewHolder, direct);
		scriptEditMemberView.loadView ();
	}
	public void chgNoticeView(GameObject sourceView, int direct) {
        changeView(sourceView, NoticeViewHolder, direct);
		scriptNoticeView.loadView ();
	}
	public void chgNoticeWinLoseView(GameObject sourceView, int direct) {
        changeView(sourceView, NoticeWinLoseHolder, direct);
		scriptNoticeWinLoseView.loadView ();
	}
	public void chgNoticeRegulerView(GameObject sourceView, int direct) {
        changeView(sourceView, NoticeRegulerHolder, direct);
		scriptNoticeRegulerView.loadView ();
	}
	public void chgNoticeAppInfoView(GameObject sourceView, int direct) {
        changeView(sourceView, NoticeAppInfoViewHolder, direct);
		scriptNoticeAppInfoView.loadView ();
	}
	public void chgNoticeTmtView(GameObject sourceView, int direct) {
        changeView(sourceView, NoticeTmtViewHolder, direct);
		scriptNoticeTmtView.loadView ();
	}
	public void chgNoticeInfView(GameObject sourceView, int direct) {
        changeView(sourceView, NoticeInfViewHolder, direct);
		scriptNoticeInfView.loadView ();
	}
	public void chgSettingView(GameObject sourceView, int direct) {
        changeView(sourceView, SettingViewHolder, direct);
		scriptSettingView.loadView ();
	}
	public void chgSettingCourtView(GameObject sourceView, int direct) {
        changeView(sourceView, SettingCourtViewHolder, direct);
		scriptSettingCourtView.loadView ();
	}
	public void chgGameView(GameObject sourceView, int direct) {
        changeView(sourceView, GameViewHolder, NONE);
		scriptGameView.preLoadView ();
	}
	public void chgExecStartView(GameObject sourceView, int direct) {
        changeView(sourceView, ExecStartViewHolder, NONE);
		scriptExecStartView.loadView();
	}
	public void chgExecSelectMemberView(GameObject sourceView, int direct) {
        changeView(sourceView, ExecSelectMemberViewHolder, NONE);
		scriptExecSelectMemberView.loadView();
	}

    public void chgExecRegisterView(GameObject sourceView, int direct) {
        changeView(sourceView, ExecRegisterViewHolder, NONE);
        scriptExecRegisterView.loadView();
    }

    public void openResultView() {
        ResultViewHolder.SetActive (true);
		ResultViewHolder.transform.SetAsLastSibling ();
		scriptResultView.loadView ();
	}
	public void closeResultView() {
        ResultViewHolder.SetActive (false);
		GameViewHolder.transform.SetAsLastSibling ();
	}

	private void _changeView(GameObject soucView, GameObject destView, int direct) {
		Transform cvTfm = GameObject.Find ("Canvas").transform;
		int cvChild = cvTfm.childCount;
		for (int cnt=0; cnt<cvChild; cnt++) {
			if (cnt > cvChild - 3)
				cvTfm.GetChild (cnt).gameObject.SetActive (true);
			cvTfm.GetChild (cnt).gameObject.SetActive (false);
		}
	}

    private void changeView(GameObject soucView, GameObject destView, int direct) {
		soucViewObj = soucView;
		destViewObj = destView;
		destView.SetActive (true);
		if (direct == IN_RIGHT) {
			destView.transform.SetAsLastSibling ();
			destView.transform.DOLocalMoveX (0, MOVE_SEC, true);
		} else if (direct == OUT_RIGHT) {
			destView.transform.SetAsLastSibling ();
			soucViewObj.transform.SetAsLastSibling ();
			soucView.transform.DOLocalMoveX (MOVE_DEST, MOVE_SEC, true);
		} else {
			destView.transform.SetAsLastSibling ();
		}
		StartCoroutine("waitChgView", soucView);
//		soucViewObj.SetActive (false);
	}
	IEnumerator waitChgView (GameObject soucView) {
		yield return new WaitForSeconds (MOVE_SEC+0.1f);
//		destViewObj.SetActive (true);
		soucViewObj.SetActive (false);
	}

    public void _setAactiveView() {
        // Top以外の全画面をアクティブにしてそれぞれの start() を実行させる
        DebugViewHolder.SetActive(true);
        MainViewHolder.SetActive(true);
        RegisterMemberViewHolder.SetActive(true);
        RegisterMemberViewHolder.transform.DOLocalMoveX(MOVE_DEST, 0);
        SelectViewHolder.SetActive(true);
        SelectViewHolder.transform.DOLocalMoveX(MOVE_DEST, 0);
        PareViewHolder.SetActive(true);
        PareViewHolder.transform.DOLocalMoveX(MOVE_DEST, 0);
        PairSelectViewHolder.SetActive(true);
        PairSelectViewHolder.transform.DOLocalMoveX(MOVE_DEST, 0);

        EditMemberViewHolder.SetActive(true);
        EditMemberViewHolder.transform.DOLocalMoveX(MOVE_DEST, 0);
        NoticeViewHolder.SetActive(true);
        NoticeViewHolder.transform.DOLocalMoveX(MOVE_DEST, 0);
        NoticeWinLoseHolder.SetActive(true);
        NoticeWinLoseHolder.transform.DOLocalMoveX(MOVE_DEST, 0);
        NoticeRegulerHolder.SetActive(true);
        NoticeRegulerHolder.transform.DOLocalMoveX(MOVE_DEST, 0);
        NoticeAppInfoViewHolder.SetActive(true);
        NoticeAppInfoViewHolder.transform.DOLocalMoveX(MOVE_DEST, 0);
        NoticeTmtViewHolder.SetActive(true);
        NoticeTmtViewHolder.transform.DOLocalMoveX(MOVE_DEST, 0);
        NoticeInfViewHolder.SetActive(true);
        NoticeInfViewHolder.transform.DOLocalMoveX(MOVE_DEST, 0);
        SettingViewHolder.SetActive(true);
        SettingViewHolder.transform.DOLocalMoveX(MOVE_DEST, 0);
        SettingCourtViewHolder.SetActive(true);
        SettingCourtViewHolder.transform.DOLocalMoveX(MOVE_DEST, 0);
        ResultViewHolder.SetActive(true);
        ResultViewHolder.transform.DOLocalMoveX(0, 0);  //Dialog表示おため画面移動はしない
        GameViewHolder.SetActive(true);
        GameViewHolder.transform.DOLocalMoveX(0, 0);    //prefab生成時に移動先座標がずれるため画面移動はしない
        ExecStartViewHolder.SetActive(true);
        ExecStartViewHolder.transform.DOLocalMoveX(0, 0);
        ExecSelectMemberViewHolder.SetActive(true);
        ExecSelectMemberViewHolder.transform.DOLocalMoveX(0, 0);
        ExecRegisterViewHolder.SetActive(true);
        ExecRegisterViewHolder.transform.DOLocalMoveX(0, 0);
    }

    void Start () {
		cvTfm = GameObject.Find ("Canvas").transform as RectTransform;
		Vector3 wh  = cvTfm.TransformVector(new Vector3(cvTfm.rect.width, cvTfm.rect.height));
		MOVE_DEST = cvTfm.rect.width;

        scriptDebugView.debugEnable (SettingManager.DEBUG_MODE);
        TopViewHolder.SetActive(true);
        TopViewHolder.transform.SetAsLastSibling ();	//起動時 トップ画面を最前面とする
	}
}
