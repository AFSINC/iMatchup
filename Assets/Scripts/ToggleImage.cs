using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleImage : MonoBehaviour {
	private ViewManager viewManager;
	public Graphic offGraphic;
	private Toggle tToggle;
	public string myRegDate;		// scriptSelectViewからprefab作成にセット


	void OnValueChanged(bool value) {

		if (GameManager.chkLock(myRegDate) == GameManager.LOCK) {		// Lockされた選手が選択から外されてはいけない
			if (value)
				return;
			string title = "試合ロック中の制限";
			string message = "試合ロック中のメンバーは選択から外せません。";
			DialogViewController.Show(title, message, null);
			tToggle.isOn = true;
			return;
		}

		if (offGraphic != null) {
			offGraphic.enabled = !value;
		}

		ScriptSelectView sc = GameObject.Find ("SelectView").GetComponent<ScriptSelectView>();
		sc._changeMemberStat (this.transform.parent.parent, this.transform.parent.parent.GetSiblingIndex());
	}

	void Start() {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
		tToggle = GetComponent<Toggle>();
		tToggle.onValueChanged.AddListener((value) => {
			OnValueChanged(value);
		});
		//初期状態を反映
		offGraphic.enabled = !tToggle.isOn;
	}
}
