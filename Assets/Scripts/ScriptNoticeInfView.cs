using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScriptNoticeInfView : MonoBehaviour {
	public GameObject myView;
	ViewManager viewManager;
	Text comTtl;
	Text cmpMsg;
	Transform trsTitle;
	public string category;				// 呼び出し時にNoticeviewからセット
	public string noticeDate;			// 呼び出し時にNoticeviewからセット
	public string noticeTitle;			// 呼び出し時にNoticeviewからセット
	public string message;				// 呼び出し時にNoticeviewからセット

	public void _CallBackBtnCancel() {
		viewManager.chgNoticeView(myView, viewManager.OUT_RIGHT);
	}

	public void _CallBackBtnDelete() {
		string title = "通知の削除確認";
		string message = "この通知を削除します。";
		DialogViewController.Show (title, message, new DialogViewOptions {
			btnCancelTitle = "キャンセル", btnCancelDelegate = () => {
				return;
			},
			btnOkTitle = "削除", btnOkDelegate = () => {
				viewManager.scriptNoticeView.deleteNoticeItem (this.transform, category, noticeDate, noticeTitle);
				viewManager.chgNoticeView(myView, viewManager.OUT_RIGHT);
			},
		});
	}

	public void loadView () {
		comTtl.text = noticeTitle + "   " +  noticeDate.Substring (0, 4) + "年" + noticeDate.Substring (5, 2) + "月" + noticeDate.Substring (8, 2) +"日";
		cmpMsg.text = message;
	}


	// Use this for initialization
	void Start () {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
		comTtl = this.transform.FindChild  ("NoticePanel/txtTitle").GetComponent<Text>();
		cmpMsg = this.transform.FindChild  ("NoticePanel/ListPanel/txtMessage").GetComponent<Text>();
		this.gameObject.SetActive (false);
	}
}
