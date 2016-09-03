using UnityEngine;
using System.Collections;

public class ScriptNoticeRegularView : MonoBehaviour {
	public GameObject myView;
	ViewManager viewManager;

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
