using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EditMember : MonoBehaviour {
	ViewManager viewManager;
	ScriptSelectView scriptSelectView;
	public string myRegDate;

	public void _OnEditMember() {
		viewManager.scriptSelectView._CallBackBtnEditMember (myRegDate);
	}

	void Start () {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
	}
}
