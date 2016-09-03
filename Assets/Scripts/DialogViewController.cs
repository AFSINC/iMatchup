using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogViewOptions {
	public string btnCancelTitle;
	public System.Action btnCancelDelegate;
	public string btnOkTitle;
	public System.Action btnOkDelegate;
	public string btnOkTitleSub;
	public System.Action btnOkDelegateSub;
}

public class DialogViewController : MonoBehaviour {
	[SerializeField] Text LblTitle;
	[SerializeField] Text lblMessage;
	[SerializeField] Button btnCancel;
	[SerializeField] Text lblBtnCancel;
	[SerializeField] Button btnOk;
	[SerializeField] Text lblBtnOk;
	[SerializeField]	Button btnOkSub;
	[SerializeField]	Text lblBtnOkSub;
	[SerializeField] Button btnOkonly;
	[SerializeField] Text lblBtnOkonly;

	private static GameObject prefab = null;
	private System.Action btnCancelAction;
	private System.Action btnOkAction;
	private System.Action btnOkActionSub;

	public static DialogViewController Show (string title, string message, DialogViewOptions options=null) {
		if (prefab == null) {
			prefab = Resources.Load ("DialogView") as GameObject;
			if (prefab==null) Debug.Log ("can't load DialogView!! Check Resouces forlder.");
		}

		GameObject obj = Instantiate (prefab) as GameObject;
		DialogViewController dialogView = obj.GetComponent<DialogViewController> ();
		dialogView.UpDateConttent (title, message, options);

		return dialogView;
	} 

	public void UpDateConttent (string title, string message, DialogViewOptions options=null ) {
		LblTitle.text = title;
		lblMessage.text = message;

        if (options != null) {
            // OK CANCEL がある場合
            btnCancel.gameObject.SetActive(options.btnCancelTitle != null);
            btnOk.gameObject.SetActive(options.btnOkTitle != null);
            btnOkSub.gameObject.SetActive(options.btnOkTitleSub != null);
            btnOkonly.gameObject.SetActive(false);

            btnCancel.gameObject.SetActive(options.btnCancelTitle != null);
            lblBtnCancel.text = options.btnCancelTitle ?? "";
            btnCancelAction = options.btnCancelDelegate;

            btnOk.gameObject.SetActive(options.btnOkTitle != null);
            lblBtnOk.text = options.btnOkTitle ?? "";
            btnOkAction = options.btnOkDelegate;

            if (options.btnOkTitleSub != null) {
                btnOkSub.gameObject.SetActive(options.btnOkTitleSub != null);
                lblBtnOkSub.text = options.btnOkTitleSub ?? "";
                btnOkActionSub = options.btnOkDelegateSub;
            }
        } else {
            // OK しかない場合
            btnCancel.gameObject.SetActive(false);
            btnOk.gameObject.SetActive(false);
            btnOkSub.gameObject.SetActive(false);
            btnOkonly.gameObject.SetActive(true);
            lblBtnOkonly.text = "OK";
        }
	}

	public void _OnPressCancelButton() {
		if (btnCancelAction != null) {
			btnCancelAction.Invoke ();
		}
		Destroy (this.gameObject);
	}

	public void _OnPressOkButton() {
		if (btnOkAction != null) {
			btnOkAction.Invoke ();
		}
		Destroy (this.gameObject);
	}

	public void _OnPressOkSubButton() {
		if (btnOkActionSub != null) {
			btnOkActionSub.Invoke();
		}
		Destroy(this.gameObject);
	}
}
