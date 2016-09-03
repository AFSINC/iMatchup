using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DeletePair : MonoBehaviour {
	ViewManager viewManager;
	ScriptSelectView scriptSelectView;

	// Event CallBack
	void OnLongPress() {
		/*
		string title = "ペア解除の確認";
		string txt = this.transform.parent.Find ("MemberName").GetComponent<Text>().text;
		string message = "ペア構成を解除しますか？\n";
		DialogViewController.Show(title, message, new DialogViewOptions {
			btnCancelTitle = "キャンセル", btnCancelDelegate = ()=>{
				return;
			},
			btnOkTitle = "削除", btnOkDelegate = ()=>{
				viewManager.scriptSelectView.deleteMember(this.transform.parent.gameObject, this.transform.parent.GetSiblingIndex ());
			},
		});
		*/
	}

	void Start() {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
		this.gameObject.AddComponent (typeof(EventLongPress));
		GetComponent<EventLongPress>().onLongPress.AddListener(OnLongPress);
	}
}
