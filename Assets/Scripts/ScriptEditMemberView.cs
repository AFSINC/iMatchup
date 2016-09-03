using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ScriptEditMemberView : MonoBehaviour {
	public GameObject myView;
	ViewManager viewManager;
	Transform tMemberInfoPanel;
	InputField iptSerial;
	InputField iptMyoujiKanji;
	InputField iptNamaeKanji;
	InputField iptMyoujiKana;
	InputField iptNamaeKana;
	Dropdown drpGender;
	Image imgPanelColor;
	InputField iptEmail;
	InputField iptMemo;
	Dropdown drpRank;
	InputField iptWin;
	InputField iptLose;
	InputField iptDraw;

	// CallBack
	public void _CallBackBtnRegister(GameObject pressedButton = null) {
		if (iptMyoujiKana.text.Length == 0 || iptMyoujiKanji.text.Length == 0) { // 必須入力:苗字カナ(ふりがな)のエラーチェック
			showErrorDialog ();
			return;
		} else {
			saveView ();
			UpdateMemberInfo ();
			viewManager.scriptSelectView.loadView ();
			viewManager.chgSelsectView (myView, viewManager.OUT_RIGHT);
		}
	}

	private void showErrorDialog(){
		string title = "必須項目の入力エラー";
		string message = "姓(漢字)と姓(ふりがな)は必ず入力して下さい。";
		DialogViewController.Show(title, message, null);
	}	

	private void showRegKindDialog(){
		string title = "選択状態の指定";
		string message = "選択状態を指定して下さい。\n未選択を指定した場合、初期状態では「メンバー一覧」に表示されません。\n選択状態は「メンバー選択」で変更できます。";
		DialogViewController.Show(title, message, new DialogViewOptions {
			btnCancelTitle = "未選択", btnCancelDelegate = ()=>{
				MemberManager.activeStat = 0;
				ConfirmSave();
			},
			btnOkTitle = "選択中", btnOkDelegate = ()=>{
				MemberManager.activeStat = 1;
				ConfirmSave();
			},
		});
	}
	private void ConfirmSave() {
/*
 		string title = "更新確認";
		string message = "メンバー情報を修正内容で更新しますか？";
		DialogViewController.Show(title, message, new DialogViewOptions {
			btnCancelTitle = "キャンセル", btnCancelDelegate = ()=>{
				return;
			},
			btnOkTitle = "更新", btnOkDelegate = ()=>{
				UpdateMemberInfo ();
				backToView ();
			},
		});
*/
	}

	private void UpdateMemberInfo () {
		MemberManager.updateMemberInfo ();
	}

	public void _CallBackBtnCancel() {
		_CallBackBtnRegister ();
//		viewManager.chgSelsectView (myView, viewManager.OUT_RIGHT);
	}
	// 性別Dropdwn
	public void _OnChgGenderColor() {
		if (drpGender.value == 0)
			imgPanelColor.color = Colors.male;
		else
			imgPanelColor.color = Colors.female;
	}

	public void loadView () {
		if (SettingManager.REGULAR_MODE) {
			tMemberInfoPanel.FindChild("GamePanelLabel").gameObject.SetActive(true);
			tMemberInfoPanel.FindChild("GamePanel").gameObject.SetActive(true);
		} else {
			tMemberInfoPanel.FindChild("GamePanelLabel").gameObject.SetActive(false);
			tMemberInfoPanel.FindChild("GamePanel").gameObject.SetActive(false);
		}


		iptSerial.text = MemberManager.memberSerial;
		iptMyoujiKanji.text = MemberManager.nameKaji_family;
		iptNamaeKanji.text = MemberManager.nameKaji_first;
		iptMyoujiKana.text = MemberManager.nameKana_family;
		iptNamaeKana.text = MemberManager.nameKana_first;
		drpGender.value = MemberManager.gender;
		iptEmail.text = MemberManager.eMail;
		iptMemo.text = MemberManager.memo;
		drpRank.value = MemberManager.rank;
		iptWin.text = MemberManager.win.ToString();
		iptLose.text = MemberManager.lose.ToString();
		iptDraw.text = MemberManager.draw.ToString();
		_OnChgGenderColor ();
	}

	public void saveView () {
		MemberManager.memberSerial = iptSerial.text;
		MemberManager.nameKaji_family = iptMyoujiKanji.text;
		MemberManager.nameKaji_first = iptNamaeKanji.text;
		MemberManager.nameKana_family = iptMyoujiKana.text;
		MemberManager.nameKana_first = iptNamaeKana.text;
		MemberManager.gender = drpGender.value;
		MemberManager.eMail = iptEmail.text;
		MemberManager.memo = iptMemo.text;
		MemberManager.rank = drpRank.value;

		if (iptWin.text.Length != 0)
			MemberManager.win = int.Parse (iptWin.text);
		else
			MemberManager.win = 0;

		if (iptLose.text.Length != 0)
			MemberManager.lose =int.Parse ( iptLose.text);
		else
			MemberManager.lose = 0;

		if (iptDraw.text.Length != 0)
			MemberManager.draw = int.Parse (iptDraw.text);
		else
			MemberManager.draw = 0;

		MemberManager.game =MemberManager.win+MemberManager.lose+MemberManager.draw;
	}

	// 呼び出し元へ戻る
	private void backToView () {
//		clearView ();
		viewManager.scriptMainView.loadView ();
		viewManager.chgViewHolderView(myView, viewManager.OUT_RIGHT);
	}

	// Use this for initialization
	void Start () {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
		tMemberInfoPanel = this.transform.Find ("MainPanel/MemberInfoPanel");
		imgPanelColor = tMemberInfoPanel.parent.GetComponent<Image> ();
		iptSerial = tMemberInfoPanel.Find("SerialPanel/IptSerial").GetComponent<InputField> ();
		drpGender = tMemberInfoPanel.FindChild ("SerialPanel/DrpGender").GetComponent<Dropdown> ();
		iptMyoujiKanji = tMemberInfoPanel.FindChild ("KanjiPanel/IptMyoujiKanji").GetComponent<InputField> ();
		iptNamaeKanji = tMemberInfoPanel.FindChild ("KanjiPanel/IptNamaeKanji").GetComponent<InputField> ();
		iptMyoujiKana = tMemberInfoPanel.FindChild ("KanaPanel/IptMyoujiKana").GetComponent<InputField> ();
		iptNamaeKana = tMemberInfoPanel.FindChild ("KanaPanel/IptNamaeKana").GetComponent<InputField> ();
		iptEmail = tMemberInfoPanel.FindChild ("MemoPanel/IptEmail").GetComponent<InputField> ();
		iptMemo = tMemberInfoPanel.FindChild ("MemoPanel/IptMemo").GetComponent<InputField> ();
		drpRank = tMemberInfoPanel.FindChild ("OtherPanel/DrpRank").GetComponent<Dropdown> ();
		iptWin = tMemberInfoPanel.FindChild ("GamePanel/IptWin").GetComponent<InputField> ();
		iptLose = tMemberInfoPanel.FindChild ("GamePanel/IptLose").GetComponent<InputField> ();
		iptDraw = tMemberInfoPanel.FindChild ("GamePanel/IptDraw").GetComponent<InputField> ();
		this.gameObject.SetActive (false);
	}

	void OnEnable() {
		//		Debug.Log ("OnEnable");
	}
}
