using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScriptSettingView : MonoBehaviour {
	public GameObject myView;
	private PageScrollRect pageScrollRect;
	private ViewManager viewManager;
	private Dropdown courtNum;
	private Toggle formDoubles;
	private Toggle formSingles;
    private Text formDoublesText;
    private Text formSinglesText;
    public GameObject SinglesKind;
	private Toggle sgsReguler;
	private Toggle sgsFree;
	public GameObject DoublesKind;
	private Toggle dlsReguler;
	private Toggle dlsComplex;
	private Toggle dlsFree;
	private GameObject bPairFix;
	private Toggle weightMin;
	private Toggle weightMax;
    private Text weightMinText;
    private Text weightMaxText;
    private Toggle sumYes;
    private Toggle sumNo;
    private Text sumYesText;
    private Text sumNoText;
    private Toggle joinInitMin;
    private Toggle joinInitZero;
    private Text joinInitMinText;
    private Text joinInitZeroText;
    private Toggle execAuto;
    private Toggle execManual;
	private Toggle waitAuto;
	private Toggle waitManual;
	private Toggle soundOn;
	private Toggle soundOff;


	public void _CallBackBtnCancel() {
		viewManager.chgMainView(myView, viewManager.OUT_RIGHT);
	}
	// コート名設定
	public void _CallBackSettingCouart() {
		SoundManager.play (SoundManager.CLICK01);
		viewManager.chgSettingCourtView(myView, viewManager.IN_RIGHT);
	}

	public void _OnCourtNum() {
		bool bLock = false;
		for (int cnt = (courtNum.value + 1); cnt < SettingManager.MaxCoutNum; cnt++) {		// コート1~6 範囲外となるコートがロックされていないか
			if (GameManager.chkCourtLockOfCortnum (cnt) == 1) {
				bLock = true;
				break;
			}
		}
		for (int cnt = (courtNum.value+1) * 4; cnt < (4 * 6 - 1); cnt++) {		// 範囲外となるコートにロックされている選手がいないか
			if (GameManager.chkLockOfPosition (cnt) == 1) {
				bLock = true;
				break;
			}
		}
		if (bLock) {	// 上段のブロック条件がクリアなら設定可能
			string title = "コート数変更の制限";
			string message = "指定外となるコートにロックされている選手がいます。\n選手のロックを解除した後、コート数を変更して下さい。";
			DialogViewController.Show(title, message, null);
			courtNum.value = SettingManager.courtNum - 1;
		} else {
			SettingManager.courtNum = courtNum.value + 1;
			SettingManager.Save ();

			// PageScrollRectへPage数を通知 1~3
			pageScrollRect.pageNum = (SettingManager.courtNum + 1) / 2;
				
		}
	}
	public void _OnFormDoubles() {
		bPairFix.SetActive (true);
		SettingManager.form = formDoubles.isOn ? 2: 1;
        formDoublesText.color = formDoubles.isOn ? Colors.White : Colors.titleback;
        formSinglesText.color = formDoubles.isOn ? Colors.titleback : Colors.White;
        //SinglesKind.SetActive (false);
        //DoublesKind.SetActive (true);
        dlsFree.isOn = true;
		SettingManager.Save ();
	}
	public void _OnFormSingles() {
		if (formSingles.isOn == true && PairManager.getPairCount() != 0) {
			formDoubles.isOn = true;		// Dialog表示中に落とされたらSinglesになっていまうので、一旦Doublesにする
            formDoublesText.color = Colors.White;

            string title = "ペア解除の確認";
			string message = "ペアが設定されています。\nシングルスではペアは設定できません。\n全てのペアを解除してシングルスに変更しますか？";
			DialogViewController.Show (title, message, new DialogViewOptions {
				btnCancelTitle = "キャンセル", btnCancelDelegate = () => {
					singlesSettingCancel();
				},
				btnOkTitle = "OK", btnOkDelegate = () => {
					singlesSettingOK ();
				},
			});
		} else if (formSingles.isOn == true) {
			singlesSettingOK ();
		}
	}
	private void singlesSettingOK() {
		PairManager.tempCurrentLR = PairManager.INIT;
		PairManager.cleanPairAll ();	// Dialogの2度呼び出しにならないように、Singles.isOnを再設定するまえにPairをクリアする

		bPairFix.SetActive (false);
		//DoublesKind.SetActive (false);
		//SinglesKind.SetActive (true);
		formSingles.isOn = true;            // Dialog確認前にDoublesにした可能性があるので、Singlesを設定する
        formSinglesText.color = Colors.White;
        sgsFree.isOn = true;

		MemberManager.Save ();
		PairManager.Save ();
//		PairManager.Load ();

		SettingManager.form = formSingles.isOn ? 1: 2;
		SettingManager.Save ();
	}
	private void singlesSettingCancel() {
		formSingles.gameObject.SetActive (false);
		formSingles.isOn = false;
        formSinglesText.color = Colors.titleback;
        formDoubles.isOn = true;
        formDoublesText.color = Colors.White;
        formSingles.gameObject.SetActive (true);
	}
	public void _OnWeightMin() {
		SettingManager.weight = weightMin.isOn ? 1: 2;
        weightMinText.color = weightMin.isOn ? Colors.White : Colors.titleback;
        SettingManager.Save ();
	}
	public void _OnWeightMax() {
		SettingManager.weight = weightMax.isOn ? 2: 1;
        weightMaxText.color = weightMax.isOn ? Colors.White : Colors.titleback;
        SettingManager.Save ();
	}
	public void _OnSumYes() {
		SettingManager.sum = sumYes.isOn ? 1: 0;
        sumYesText.color = sumYes.isOn ? Colors.White : Colors.titleback;
        SettingManager.Save ();
	}
	public void _OnSumNo() {
		SettingManager.sum = sumNo.isOn ? 0: 1;
        sumNoText.color = sumNo.isOn ? Colors.White : Colors.titleback;
        SettingManager.Save ();
	}
	public void _OnJoinInitMin() {
		SettingManager.joinInit = joinInitMin.isOn ? 1: 0;
        joinInitMinText.color = joinInitMin.isOn ? Colors.White : Colors.titleback;
        SettingManager.Save ();
	}
	public void _OnJoinInitZero() {
		SettingManager.joinInit = joinInitZero.isOn ? 0: 1;
        joinInitZeroText.color = joinInitZero.isOn ? Colors.White : Colors.titleback;
        SettingManager.Save ();
	}
	public void _OnAutoExecYes() {
		SettingManager.autoExec = execAuto.isOn ? 1: 0;
		SettingManager.Save ();
	}
	public void _OnAutoExecNo() {
		SettingManager.autoExec = execManual.isOn ? 0: 1;
		SettingManager.Save ();
	}
	public void _OnAutoWaitYes() {
		SettingManager.waitCalc = waitAuto.isOn ? 1: 0;
		SettingManager.Save ();
	}
	public void _OnAutoWaitNo() {
		SettingManager.waitCalc = waitManual.isOn ? 0: 1;
		SettingManager.Save ();
	}
	public void _OnSoundOn() {
		SettingManager.sound = soundOn.isOn;
		SettingManager.Save ();
	}
	public void _OnSoundOff() {
		SettingManager.sound = !soundOff.isOn;
		SettingManager.Save ();
	}

	public void loadView () {
		SettingManager.Load ();

		if (SettingManager.REGULAR_MODE)
			myView.transform.Find("SettingPanel/LayoutV/ElmSum").gameObject.SetActive(true);
		else
			myView.transform.Find("SettingPanel/LayoutV/ElmSum").gameObject.SetActive(false);

        formSinglesText.color = Colors.titleback;
        formDoublesText.color = Colors.titleback;
        weightMinText.color = Colors.titleback;
        weightMaxText.color = Colors.titleback;
        sumYesText.color = Colors.titleback;
        sumNoText.color = Colors.titleback;
        joinInitZeroText.color = Colors.titleback;
        joinInitMinText.color = Colors.titleback;

        courtNum.value = SettingManager.courtNum - 1;
		if (SettingManager.form == 1) {
			formSingles.isOn = false;
			formSingles.isOn = true;
            formSinglesText.color = Colors.White;
        } else {	
			formDoubles.isOn = false;
			formDoubles.isOn = true;
            formDoublesText.color = Colors.White;
        }
        if (SettingManager.weight == 1) {
			weightMin.isOn = true;
            weightMinText.color = Colors.White;
        } else {	
			weightMax.isOn = true;
            weightMaxText.color = Colors.White;
        }
        if (SettingManager.sum == 0) {
			sumNo.isOn = true;
            sumNoText.color = Colors.White;
        } else {	
			sumYes.isOn = true;
            sumYesText.color = Colors.White;
        }
        if (SettingManager.joinInit == 0) {
			joinInitZero.isOn = true;
            joinInitZeroText.color = Colors.White;
        } else {	
			joinInitMin.isOn = true;
            joinInitMinText.color = Colors.White;
        }
        if (SettingManager.autoExec == 0) {
			execManual.isOn = true;
		} else {	
			execAuto.isOn = true;
		}
		if (SettingManager.waitCalc == 0) {
			waitManual.isOn = true;
		} else {	
			waitAuto.isOn = true;
		}
		if (SettingManager.sound) {
			soundOn.isOn = true;
		} else {	
			soundOff.isOn = true;
		}
	}


	void Start () {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
		pageScrollRect = GameObject.Find ("Game/MainPanel/PlayPanel/SclGame").GetComponent<PageScrollRect>();
		courtNum = myView.transform.Find ("SettingPanel/LayoutV/ElmCourt/DrpCourt").GetComponent<Dropdown>();
		formDoubles = myView.transform.Find ("SettingPanel/LayoutV/ElmForm/tglGroup/TglDoubles").GetComponent<Toggle>();
		formSingles = myView.transform.Find ("SettingPanel/LayoutV/ElmForm/tglGroup/TglSingles").GetComponent<Toggle>();
		weightMin = myView.transform.Find ("SettingPanel/LayoutV/ElmWeight/tglGroup/TglMin").GetComponent<Toggle>();
		weightMax = myView.transform.Find ("SettingPanel/LayoutV/ElmWeight/tglGroup/TglMax").GetComponent<Toggle>();
		sumYes = myView.transform.Find ("SettingPanel/LayoutV/ElmSum/tglGroup/TglYes").GetComponent<Toggle>();
		sumNo = myView.transform.Find ("SettingPanel/LayoutV/ElmSum/tglGroup/TglNo").GetComponent<Toggle>();
		joinInitMin = myView.transform.Find ("SettingPanel/LayoutV/ElmJoinInit/tglGroup/TglMin").GetComponent<Toggle>();
		joinInitZero = myView.transform.Find ("SettingPanel/LayoutV/ElmJoinInit/tglGroup/TglZero").GetComponent<Toggle>();
		execAuto = myView.transform.Find ("SettingPanel/LayoutV/ElmExec/tglGroup/TglAuto").GetComponent<Toggle>();
		execManual = myView.transform.Find ("SettingPanel/LayoutV/ElmExec/tglGroup/TglManual").GetComponent<Toggle>();
		waitAuto = myView.transform.Find ("SettingPanel/LayoutV/ElmWait/tglGroup/TglWaitAuto").GetComponent<Toggle>();
		waitManual = myView.transform.Find ("SettingPanel/LayoutV/ElmWait/tglGroup/TglWaitManual").GetComponent<Toggle>();
		soundOn = myView.transform.Find ("SettingPanel/LayoutV/ElmSound/tglGroup/TglSoundOn").GetComponent<Toggle>();
		soundOff = myView.transform.Find ("SettingPanel/LayoutV/ElmSound/tglGroup/TglSoundOff").GetComponent<Toggle>();
        // text
        formDoublesText = myView.transform.Find("SettingPanel/LayoutV/ElmForm/tglGroup/TglDoubles/Label").GetComponent<Text>();
        formSinglesText = myView.transform.Find("SettingPanel/LayoutV/ElmForm/tglGroup/TglSingles/Label").GetComponent<Text>();
        weightMinText = myView.transform.Find("SettingPanel/LayoutV/ElmWeight/tglGroup/TglMin/Label").GetComponent<Text>();
        weightMaxText = myView.transform.Find("SettingPanel/LayoutV/ElmWeight/tglGroup/TglMax/Label").GetComponent<Text>();
        sumYesText = myView.transform.Find("SettingPanel/LayoutV/ElmSum/tglGroup/TglYes/Label").GetComponent<Text>();
        sumNoText = myView.transform.Find("SettingPanel/LayoutV/ElmSum/tglGroup/TglNo/Label").GetComponent<Text>();
        joinInitMinText = myView.transform.Find("SettingPanel/LayoutV/ElmJoinInit/tglGroup/TglMin/Label").GetComponent<Text>();
        joinInitZeroText = myView.transform.Find("SettingPanel/LayoutV/ElmJoinInit/tglGroup/TglZero/Label").GetComponent<Text>();


        SinglesKind = myView.transform.Find ("SettingPanel/LayoutV/ElmSingles").gameObject;
		sgsReguler = myView.transform.Find ("SettingPanel/LayoutV/ElmSingles/tglGroup/TglReguler").GetComponent<Toggle>();
		sgsFree = myView.transform.Find ("SettingPanel/LayoutV/ElmSingles/tglGroup/TglFree").GetComponent<Toggle>();
		DoublesKind = myView.transform.Find ("SettingPanel/LayoutV/ElmDoubles").gameObject;
		dlsReguler = myView.transform.Find ("SettingPanel/LayoutV/ElmDoubles/tglGroup/TglReguler").GetComponent<Toggle>();
		dlsComplex = myView.transform.Find ("SettingPanel/LayoutV/ElmDoubles/tglGroup/TglComplex").GetComponent<Toggle>();
		dlsFree = myView.transform.Find ("SettingPanel/LayoutV/ElmDoubles/tglGroup/TglFree").GetComponent<Toggle>();
		bPairFix = viewManager.SelectViewHolder.transform.Find("FuncPanel/btnPair").gameObject;
		this.gameObject.SetActive (false);
	}
}
