using UnityEngine;
using System.Collections;

public class ScriptNoticeAppInfoView : MonoBehaviour {
	public GameObject myView;
	ViewManager viewManager;

	public void _DebugOn() {
		SoundManager.play (SoundManager.VOICE00);
		SettingManager.DEBUG_MODE = true;
		viewManager.scriptDebugView.debugEnable (SettingManager.DEBUG_MODE);
	}
	public void _DebugOff() {
		SoundManager.play (SoundManager.VOICE01);
		SettingManager.DEBUG_MODE = false;
		viewManager.scriptDebugView.debugEnable (SettingManager.DEBUG_MODE);
	}
	public void _Regular() {
		SoundManager.play (SoundManager.VOICE02);
		SettingManager.REGULAR_MODE = true;
	}
	public void _Try() {
		SoundManager.play (SoundManager.VOICE03);
		SettingManager.REGULAR_MODE = false;
	}


	public void _CallBackBtnCancel() {
		viewManager.scriptNoticeView.loadView ();
		viewManager.chgNoticeView(myView, viewManager.OUT_RIGHT);
	}

	public void loadView () {
	}


	// Use this for initialization
	void Start () {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
		this.gameObject.SetActive (false);
	}
}
