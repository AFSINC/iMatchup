using UnityEngine;
using System.Collections;

public class ScriptExecSelectMemberView : MonoBehaviour {
	public GameObject myView;
	ViewManager viewManager;

	public void _callBackCancel() {
		viewManager.chgExecStartView(myView, viewManager.IN_RIGHT);
	}

    public void _callBackRegister() {
        viewManager.chgExecRegisterView(myView, viewManager.IN_RIGHT);
    }

    public void addMember() {
        viewManager.scriptExecStartView.loadView();
        viewManager.chgExecStartView(myView, viewManager.IN_RIGHT);
    }

    public void loadView() { 
	}

	void Start () {
		viewManager = GameObject.Find("ViewManager").GetComponent<ViewManager>();
	}

	void Update () {	
	}
}
