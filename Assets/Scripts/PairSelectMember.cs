using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PairSelectMember : MonoBehaviour {
	public string regDate;
	private ViewManager viewManager;

	public void _OnTap() {
		PairManager.tempSelectMyDate = regDate;
		viewManager.scriptPairSelectView._toPareView();
	}

	void onDoubleTap() {
	}


	void Start() {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
		this.gameObject.AddComponent (typeof(EventDoubleTap));
		GetComponent<EventDoubleTap>().onDoubleTap.AddListener(onDoubleTap);
	}
}
