using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScriptSelectView : MonoBehaviour {
	public GameObject myView;
	private ViewManager viewManager;
	public GameObject pfbMember;
	private Transform tPfbParent;  // LayoutVertical(プレハブMemberの親)
	private Dropdown dSort;
	private int preSelectViewSort = 0;

	// CallBack
	// 新規登録View
	public void _CallBackBtnRegister () {
		SoundManager.play (SoundManager.CLICK01);
		if (MemberManager.getMemberCount() >= MemberManager.MAX_MEMBER) {
			string title = "登録人数の制限";
			string message = "登録できる最大人数は、" + MemberManager.MAX_MEMBER + "名です。";
			DialogViewController.Show(title, message, null);
		} else {
			MemberManager.newMember ();
			viewManager.chgRegisterMembeView(myView, viewManager.IN_RIGHT);
		}
	}
	// Pair設定View
	public void _CallBackBtnPair() {
		SoundManager.play (SoundManager.CLICK01);
		MemberManager.Save();
		viewManager.chgPareView (myView, viewManager.IN_RIGHT);
	}
	// 戻る
	public void _CallBackBtnCancel() {
		MemberManager.Save();
//		viewManager.scriptMainView.loadView ();
		viewManager.chgMainView (myView, viewManager.OUT_RIGHT);
	}
	// 編集View
	public void _CallBackBtnEditMember(string iRegDate) {
		MemberManager.posMember (iRegDate);
//		viewManager.scriptEditMemberView.loadView ();
		viewManager.chgEditMemberView (myView, viewManager.IN_RIGHT);
	}

	public void loadView() {
		if (SettingManager.form == 1 || !SettingManager.REGULAR_MODE ){
			this.transform.Find ("FuncPanel/btnPair").gameObject.SetActive (false);
		} else {
			this.transform.Find ("FuncPanel/btnPair").gameObject.SetActive (true);
		}

		_OnSort ();
	}
	public void _OnSort() {
		switch (dSort.value) {
		case 0:
			MemberManager.sortMemberName ();
			break;
		case 1:
			MemberManager.sortAvtiveMember ();
			break;
		case 2:
			MemberManager.sortRegDate ();
			break;
		case 3:
			MemberManager.sortSrial ();
			break;
		case 4:
			MemberManager.sortGender ();
			break;
		default:
			MemberManager.sortMemberName ();
			break;
		}
		preSelectViewSort = dSort.value;
		//		viewManager.scriptPairView.preSelectViewSort = dSort.value;
		updeteView ();
	}
	private void updeteView() {
//		MemberManager.Load ();
		// 活動中ラベル
		// リスト全削除
//		sortViewItems();
		int pfCnt = tPfbParent.childCount;
		for (int i=0; i < pfCnt; i++) {
			DestroyImmediate(tPfbParent.Find ("ListSelectMember").gameObject);
		}

		// 選択リスト作成
		for (int i = 0; i < MemberManager.getMemberCount (); i++) {
			Transform pt = Instantiate (pfbMember).transform;
			pt.name = pfbMember.name;
			pt.SetParent (tPfbParent, false);

			Image imgListBack = pt.FindChild("BaseWhite").GetComponent <Image> ();
			Toggle toggleStat = pt.GetComponentInChildren <Toggle> ();

//			sortViewItems ();
			MemberManager.posMember (i);
			string sname = MemberManager.nameKaji_family + " " + MemberManager.nameKaji_first;
			pt.FindChild("BaseWhite/MemberName").GetComponent <Text> ().text = sname;
			string pname = PairManager.getPairPartnerNameOfMyRegDate (MemberManager.regDate);
			if (pname != null) 
				pt.FindChild("BaseWhite/PairName").GetComponent <Text> ().text = pname;
			else 
				pt.FindChild("BaseWhite/PairName").GetComponent <Text> ().text = "";

			if (MemberManager.gender == 0) {
				if (MemberManager.activeStat == 1) { 
					imgListBack.color = Colors.male;
					toggleStat.isOn = true;
				} else {
					imgListBack.color = Colors.maleDisable;
					toggleStat.isOn = false;
				}
			} else {
				if (MemberManager.activeStat == 1) {
					imgListBack.color = Colors.female;
					toggleStat.isOn = true;
				} else {
					imgListBack.color = Colors.femaleDisable;
					toggleStat.isOn = false;
				}
			}

			if (GameManager.chkLock(MemberManager.regDate) == GameManager.LOCK)
				pt.FindChild ("BaseWhite/ImgLock").gameObject.SetActive (true);
			else
				pt.FindChild ("BaseWhite/ImgLock").gameObject.SetActive (false);


			// EditMemberでメンバー特定するため、プレハブ作成時に登録日を設定する。
			pt.Find("BaseWhite").GetComponent<DeleteMember>().myRegDate = MemberManager.regDate;
			pt.Find("BaseWhite/SelectTglSwitch").GetComponent<ToggleImage>().myRegDate = MemberManager.regDate;
		}
		//// Debug
//		Debug.Log("------------------------------------------- ScriptSelectView");
//		for (int i = 0; i < MemberManager.getMemberCount (); i++) {
//			MemberManager.posMember (i);
//			Debug.Log ("[" + i +"] idx:" + MemberManager.idxRec + " Active:" + MemberManager.activeStat + " seri:" + MemberManager.memberSerial + " NameKJ-Fml:" + MemberManager.nameKaji_family + " NameKJ-Fst:" + MemberManager.nameKaji_first + " NameKN-Fml:" + MemberManager.nameKana_family + " NameKN-Fst:" + MemberManager.nameKana_first + " Gen:" + MemberManager.gender + " Date:" + MemberManager.regDate);
//		}
//		Debug.Log("-------------------------------------------");
		//// Debug
	}

//	public Transform TargetListMember;		// toggle(script) から設定
	public void _changeMemberStat(Transform tListMember, int idxPrefub) {	// toggle(script) から関節呼び出し
		bool actFlag = false;
		MemberManager.posMember(tListMember.transform.GetSiblingIndex());
		Color cl = tListMember.transform.Find("BaseWhite").GetComponent<Image>().color;
		if (cl == Colors.male || cl == Colors.maleDisable) {
			if (MemberManager.activeStat == 0) {
				cl = Colors.male;
				actFlag = true;
			} else {
				cl = Colors.maleDisable;
				actFlag = false;
			}
		} else {
			if (cl == Colors.female || cl == Colors.femaleDisable) {
				if (MemberManager.activeStat == 0) {
					cl = Colors.female;
					actFlag = true;
				} else {
					cl = Colors.femaleDisable;
					actFlag = false;
				}
			} else {
				Debug.Log ("ERROR: SelectView _changeMemberStat   DATAがおかしい");
				return;
			}
		}

		if (actFlag == true) {
			tListMember.transform.Find ("BaseWhite").GetComponent<Image> ().color = cl;
			MemberManager.idxRec = MemberManager.getActiveMemberLastNum() + 1;
			MemberManager.activeStat = 1; // Activeにする場合は、status更新前の最大Activeidx取得後にstatusを更新すること
		} else {
			PairManager.clearPairMemberOfRegDate (MemberManager.getRegDateOfListIdx(idxPrefub));	//　メンバー非選択時はペア解除
			PairManager.Save ();

			tListMember.transform.Find ("BaseWhite").GetComponent<Image> ().color = cl;
			MemberManager.idxRec = MemberManager.MAX_IDXREC;
			MemberManager.activeStat = 0;

			loadView ();
		}
	}

	public void deleteMember(GameObject o, int idx) {
		string regDate = MemberManager.getRegDateOfListIdx (idx);
		if (GameManager.chkLock(regDate) == GameManager.LOCK) {		// Lockされた選手が選択から外されてはいけない
			string title = "試合ロック中の制限";
			string message = "試合ロック中のメンバーは削除できせません。";
			DialogViewController.Show(title, message, null);
			return;
		}

		PairManager.clearPairMemberOfRegDate (regDate);	//　メンバー削除時はペア解除
		PairManager.Save ();

		DestroyImmediate(o);
		MemberManager.removeMember (idx);
		updeteView ();
	}
		
	private void sortViewItems() {
		dSort.value = preSelectViewSort;
//		MemberManager.sortMemberName ();
	}



	// Use this for initialization
	void Start () {
		this.transform.Find ("TitlePanel/btnDEBUG").gameObject.SetActive (SettingManager.DEBUG_MODE);
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
		if (SettingManager.form == 1 || SettingManager.form == 0){
			this.transform.Find ("FuncPanel/btnPair").gameObject.SetActive (false);
		}
			
		tPfbParent = this.transform.Find ("SelectMemberPanel/ListScroll/LayoutVertical");
		dSort = this.transform.Find ("SelectMemberPanel/DrpSort").GetComponent<Dropdown> ();
		dSort.value = 0;

//		this.gameObject.SetActive (false);
	}
}
