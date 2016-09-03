using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ScriptSettingCourtView : MonoBehaviour {
	public GameObject myView;
	private ViewManager viewManager;
	private InputField courtName1;
	private InputField courtName2;
	private InputField courtName3;
	private InputField courtName4;
	private InputField courtName5;
	private InputField courtName6;
	private string no1 = "第１コート";
	private string no2 = "第２コート";
	private string no3 = "第３コート";
	private string no4 = "第４コート";
	private string no5 = "第５コート";
	private string no6 = "第６コート";

	public void _CallBackBtnCancel() {
		viewManager.chgSettingView(myView, viewManager.OUT_RIGHT);
	}

	public void _OnCourtEditEnd1() {
		if (courtName1.text.Length <= 10) {
			if (courtName1.text.Length == 0)
				courtName1.text = no1;
			SettingManager.setCourtName (0, courtName1.text);
			SettingManager.Save ();
		} else {
			courtName1.text = courtName1.text.Remove (9);
			errCourtNameLength ();
		}
	}
	public void _OnCourtEditEnd2() {
		if (courtName2.text.Length <= 10) {
			if (courtName2.text.Length == 0)
				courtName2.text = no2;
			SettingManager.setCourtName (1, courtName2.text);
			SettingManager.Save ();
		} else {
			courtName2.text = courtName1.text.Remove (9);
			errCourtNameLength ();
		}
	}
	public void _OnCourtEditEnd3() {
		if (courtName3.text.Length <= 10) {
			if (courtName3.text.Length == 0)
				courtName3.text = no3;
			SettingManager.setCourtName (2, courtName3.text);
			SettingManager.Save ();
		} else {
			courtName3.text = courtName1.text.Remove (9);
			errCourtNameLength ();
		}
	}
	public void _OnCourtEditEnd4() {
		if (courtName4.text.Length <= 10) {
			if (courtName4.text.Length == 0)
				courtName4.text = no4;
			SettingManager.setCourtName (3, courtName4.text);
			SettingManager.Save ();
		} else {
			courtName4.text = courtName1.text.Remove (9);
			errCourtNameLength ();
		}
	}
	public void _OnCourtEditEnd5() {
		if (courtName5.text.Length <= 10) {
			if (courtName5.text.Length == 0)
				courtName5.text = no5;
			SettingManager.setCourtName (4, courtName5.text);
			SettingManager.Save ();
		} else {
			courtName5.text = courtName1.text.Remove (9);
			errCourtNameLength ();
		}
	}
	public void _OnCourtEditEnd6() {
		if (courtName6.text.Length <= 10) {
			if (courtName6.text == "")
				courtName6.text = no6;
			SettingManager.setCourtName (5, courtName6.text);
			SettingManager.Save ();
		} else {
			courtName6.text = courtName1.text.Remove (9);
			errCourtNameLength ();
		}
	}
	private void errCourtNameLength() {
		string title = "コート名の文字数制限";
		string message = "コート名の文字数制限は10文字です。\n11文字目以降を削除しました。";
		DialogViewController.Show(title, message, null);
	}


	public void loadView () {
		SettingManager.Load ();

		if (SettingManager.getCourtName(0) == null || SettingManager.getCourtName(0) == "") {
			courtName1.text = no1;
			SettingManager.setCourtName(0, courtName1.text);
		} else {
			courtName1.text = SettingManager.getCourtName(0);
		}
		if (SettingManager.getCourtName(1) == null || SettingManager.getCourtName(1) == "") {
			courtName2.text = no2;
			SettingManager.setCourtName(1, courtName1.text);
		} else {
			courtName2.text = SettingManager.getCourtName(1);
		}
		if (SettingManager.getCourtName(2) == null || SettingManager.getCourtName(2) == "") {
			courtName3.text = no3;
			SettingManager.setCourtName(2, courtName1.text);
		} else {
			courtName3.text = SettingManager.getCourtName(2);
		}
		if (SettingManager.getCourtName(3) == null || SettingManager.getCourtName(3) == "") {
			courtName4.text = no4;
			SettingManager.setCourtName(3, courtName1.text);
		} else {
			courtName4.text = SettingManager.getCourtName(3);
		}
		if (SettingManager.getCourtName(4) == null || SettingManager.getCourtName(4) == "") {
			courtName5.text = no5;
			SettingManager.setCourtName(4, courtName1.text);
		} else {
			courtName5.text = SettingManager.getCourtName(4);
		}
		if (SettingManager.getCourtName(5) == null || SettingManager.getCourtName(5) == "") {
			courtName6.text = no6;
			SettingManager.setCourtName(5, courtName1.text);
		} else {
			courtName6.text = SettingManager.getCourtName(5);
		}
		SettingManager.Save ();
	}


	void Start () {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
		courtName1 = this.transform.Find ("SettingPanel/LayoutV/Court1/IptCourt").GetComponent<InputField>();
		courtName2 = this.transform.Find ("SettingPanel/LayoutV/Court2/IptCourt").GetComponent<InputField>();
		courtName3 = this.transform.Find ("SettingPanel/LayoutV/Court3/IptCourt").GetComponent<InputField>();
		courtName4 = this.transform.Find ("SettingPanel/LayoutV/Court4/IptCourt").GetComponent<InputField>();
		courtName5 = this.transform.Find ("SettingPanel/LayoutV/Court5/IptCourt").GetComponent<InputField>();
		courtName6 = this.transform.Find ("SettingPanel/LayoutV/Court6/IptCourt").GetComponent<InputField>();
		this.gameObject.SetActive (false);
	}
}
