using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ScriptRegisterView : MonoBehaviour {
	public GameObject myView;
	ViewManager viewManager;
	Transform tMemberInfoPanel;
	Image imgMemberInfoPanel;
	InputField iptSerial;
	InputField iptMyoujiKanji;
	InputField iptNamaeKanji;
	InputField iptMyoujiKana;
	InputField iptNamaeKana;
	Dropdown drpGender;
	Dropdown drpRank;
	InputField iptEmail;
	InputField iptMemo;

	// CallBack
	// 登録
	public void _CallBackBtnRegister(GameObject pressedButton) {
		SoundManager.play (SoundManager.CLICK01);
		if (pressedButton.name == "BtnActive") {
			MemberManager.activeStat = 1;
			MemberManager.idxRec = MemberManager.getActiveMemberCount (); // idxRecは、Listと同じで 0 開始
		} else {
			MemberManager.activeStat = 0;
			MemberManager.idxRec = MemberManager.MAX_IDXREC; 		// ソート時に後ろに来るように大きな値を設定
		}

		if (iptMyoujiKana.text.Length == 0 || iptMyoujiKanji.text.Length == 0) { // 必須入力:姓(漢字)と姓(ふりがな)の未入力エラーチェック
			showErrorDialog ();
			return;
		}

		if (true) {		// 連続登録をするしないの仕様でどちらにするか
			ConfirmSave ();
		} else {
			SaveMemberInfo ();
			clearView ();
		}
	}
	// 性別Dropdwn
	public void _CallBackDrpGender() {
		if (drpGender.value == 0) {
			imgMemberInfoPanel.color = Colors.male;
		} else {
			imgMemberInfoPanel.color = Colors.female;
		}
	}
	// 戻る
	public void _CallBackBtnCancel() {
		MemberManager.Save ();
		clearView ();
		viewManager.scriptSelectView.loadView ();
		viewManager.chgSelsectView(myView, viewManager.OUT_RIGHT);
//		backToView ();
	}
	// 呼び出し元へ戻る
/*	private void backToView () {
		clearView ();
		viewManager.scriptMainView.loadView ();
		viewManager.chgViewHolderView(myView, viewManager.OUT_RIGHT);
	}*/


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
		string title = "登録確認";
		string message = "メンバーを登録しますか？";
		DialogViewController.Show(title, message, new DialogViewOptions {
			btnCancelTitle = "キャンセル", btnCancelDelegate = ()=>{
				return;
			},
			btnOkTitle = "登録", btnOkDelegate = ()=>{
				SaveMemberInfo ();
				_CallBackBtnCancel ();
			},
		});
	}

	private void SaveMemberInfo () {
		MemberManager.memberSerial = iptSerial.text;
		MemberManager.nameKaji_family = iptMyoujiKanji.text;
		MemberManager.nameKaji_first = iptNamaeKanji.text;
		MemberManager.nameKana_family = iptMyoujiKana.text;
		MemberManager.nameKana_first = iptNamaeKana.text;
		MemberManager.gender = drpGender.value;
		MemberManager.eMail = iptEmail.text;
		MemberManager.rank = drpRank.value;
		MemberManager.memo = iptMemo.text;

		if (GameManager.gameStatus != 0) {
			// 選択追加もあるのでGameviewでGame数補正をする
		}

		MemberManager.regDate = DateTime.Now.ToString ("s", new System.Globalization.CultureInfo("ja-JP"));

//		Debug.Log("seri:"+iptSerial.text+" NameKJ-Fml:"+iptMyoujiKanji.text + " NameKJ-Fst:" + iptNamaeKanji.text + " NameKN-Fml:" + iptMyoujiKana.text + " NameKN-Fst:" + iptNamaeKana.text + " Gen:" + drpGender.value + " Active:"  + MemberManager.activeStat + " Date:"  + MemberManager.regDate);

		MemberManager.addMember();
		MemberManager.Save ();
	}


	// クリア
	private void clearView () {
		iptSerial.text = "";
		iptMyoujiKanji.text = "";
		iptNamaeKanji.text = "";
		iptMyoujiKana.text = "";
		iptNamaeKana.text = "";
		drpGender.value = 0;
		drpRank.value = 7;
		iptEmail.text = "";
		iptMemo.text = "";
		imgMemberInfoPanel.color = Colors.male;
	}

	public void loadView() {
		MemberManager.newMember ();
		clearView ();
	}

	// Use this for initialization
	void Start () {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
		tMemberInfoPanel = this.transform.Find ("MainPanel/MemberInfoPanel");
		imgMemberInfoPanel = tMemberInfoPanel.parent.GetComponent<Image> ();
		iptSerial = tMemberInfoPanel.Find("SerialPanel/IptSerial").GetComponent<InputField> ();
		drpGender = tMemberInfoPanel.FindChild ("SerialPanel/DrpGender").GetComponent<Dropdown> ();
		iptMyoujiKanji = tMemberInfoPanel.FindChild ("KanjiPanel/IptMyoujiKanji").GetComponent<InputField> ();
		iptNamaeKanji = tMemberInfoPanel.FindChild ("KanjiPanel/IptNamaeKanji").GetComponent<InputField> ();
		iptMyoujiKana = tMemberInfoPanel.FindChild ("KanaPanel/IptMyoujiKana").GetComponent<InputField> ();
		iptNamaeKana = tMemberInfoPanel.FindChild ("KanaPanel/IptNamaeKana").GetComponent<InputField> ();
		drpRank = tMemberInfoPanel.FindChild ("OtherPanel/DrpRank").GetComponent<Dropdown> ();
		iptEmail = tMemberInfoPanel.FindChild ("MemoPanel/IptEmail").GetComponent<InputField> ();
		iptMemo = tMemberInfoPanel.FindChild ("MemoPanel/IptMemo").GetComponent<InputField> ();
		this.gameObject.SetActive (false);
	}
}
