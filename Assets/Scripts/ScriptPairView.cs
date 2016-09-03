using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using System.Threading;

public class ScriptPairView : MonoBehaviour {
	public GameObject myView;
	private ViewManager viewManager;
	public int preSelectViewSort;		// 選択画面で選ばれていたソート (Pairから戻るときに元に戻してもらう)
	public GameObject pfbMember;
	private Transform tPfbParent;  	// LayoutVertical(プレハブMemberの親)

	// CallBack
	public void _CallBackBtnAllClear() {
		SoundManager.play (SoundManager.CLICK01);
		string title = "全ペア解除の確認";
		string message = "全てのペア構成を解除しますか？\n";
		DialogViewController.Show(title, message, new DialogViewOptions {
			btnCancelTitle = "キャンセル", btnCancelDelegate = ()=>{
				return;
			},
			btnOkTitle = "削除", btnOkDelegate = ()=>{
				_deletePairAll();
			},
		});
	}

	private void _deletePairAll() {
		PairManager.tempCurrentLR = PairManager.INIT;
		PairManager.cleanPairAll ();
//		MemberManager.cleanPairAll ();
		MemberManager.Save ();
		PairManager.Save ();
//		PairManager.Load ();
		loadView ();
	}

	public void _CallBackBtnCancel() {
		PairManager.tempCurrentLR = PairManager.INIT;
		PairManager.cleanEmptyPair ();
		PairManager.Save();
		MemberManager.Save ();
		viewManager.scriptSelectView.loadView ();
		viewManager.chgSelsectView(myView, viewManager.OUT_RIGHT);
	}

	public void _toPareSelectView(string regDate) {	// pareMemberからdoubletap(メンバー選択)で呼ばれ、間接的にペア設定に移動
		if (GameManager.chkLock(regDate) == GameManager.LOCK) {
			string title = "試合ロック中の制限";
			string message = "試合ロック中メンバーはメンバー単位での選択変更はできません。\nペア設定の削除のみ行えます。";
			DialogViewController.Show(title, message, null);
		} else
			viewManager.chgPareSelectView(myView, viewManager.IN_RIGHT);
	}

	public void deletePair(GameObject o, int idx) {	// pareMemberからdoubletap(長押し)で呼ばれる
		PairManager.tempCurrentLR = PairManager.INIT;
		PairManager.clearPairMemberOfRegDate (idx);
		PairManager.cleanEmptyPair ();
		MemberManager.Save ();
		PairManager.Save ();
		loadView ();
	}


	public void loadView(){
		Transform pt;
		if (PairManager.tempCurrentLR == PairManager.INIT) {	// ファイルからリスト作成
			// リスト全削除
			int pfCnt = tPfbParent.childCount;
			for (int i=0; i < pfCnt; i++) {
				DestroyImmediate(tPfbParent.Find ("Pair").gameObject);
			}

			// 選択リスト作成
			PairManager.Load ();	// load pareFile
			int cntPareCnt = PairManager.getPairCount ();
			for (int i = 0; i < cntPareCnt; i++) {
				pt = Instantiate (pfbMember).transform;	// prefabは１つに左右メンバーが対となっている
				pt.name = pfbMember.name;
				pt.SetParent (tPfbParent, false);

				PairManager.posPair (i);

				// 左側メンバー
				pt.transform.Find ("ImgPairBase/LeftMember/MemberName").GetComponent<Text>().text = PairManager.pairNameKnji_familyL+" "+PairManager.pairNameKnji_firstL;
				Image imgListBack;
				imgListBack = pt.transform.Find ("ImgPairBase/LeftMember/ImgMember").GetComponent<Image>();
				if (PairManager.pairGenderL == 0) {
					imgListBack.color = Colors.male;
				} else {
					imgListBack.color = Colors.female;
				}
				imgListBack.GetComponent<PairMember> ().myRegDate = PairManager.pairRegDateL;	// ScriptにRegDateを代入設定
				if (GameManager.chkLock (PairManager.pairRegDateL) == GameManager.LOCK)
					pt.transform.Find ("ImgPairBase/LeftMember/ImgLock").gameObject.SetActive (true);
				else
					pt.transform.Find ("ImgPairBase/LeftMember/ImgLock").gameObject.SetActive (false);
				
				// 右側メンバー
				pt.transform.Find ("ImgPairBase/RightMember/MemberName").GetComponent<Text> ().text = PairManager.pairNameKnji_familyR + " " + PairManager.pairNameKnji_firstR;
				imgListBack = pt.transform.Find ("ImgPairBase/RightMember/ImgMember").GetComponent<Image>();
				if (PairManager.pairGenderR == 0) {
					imgListBack.color = Colors.male;
				} else {
					imgListBack.color = Colors.female;
				}
				imgListBack.GetComponent<PairMember> ().myRegDate = PairManager.pairRegDateR;	// ScriptにRegDateを代入設定
				if (GameManager.chkLock (PairManager.pairRegDateR) == GameManager.LOCK)
					pt.transform.Find ("ImgPairBase/RightMember/ImgLock").gameObject.SetActive (true);
				else
					pt.transform.Find ("ImgPairBase/RightMember/ImgLock").gameObject.SetActive (false);
			}
			pt = Instantiate (pfbMember).transform;
			pt.name = pfbMember.name;
			pt.SetParent (tPfbParent, false);
			PairManager.newPair ();
			PairManager.addPair ();
		} else {	// PareSelectView からデータを渡された場合
			if (PairManager.tempSelectMyDate == "NULL") return;

			MemberManager.posMemberOfRegDate (PairManager.tempSelectMyDate);
			PairManager.posPair (PairManager.tempCurrentRow);

			// PairManagerの空白状態を調べる(空白bit)  0:空白なし  10:左空白  1:右空白  11:両方空白
			int enptyAreaBit = 0;
			if (PairManager.pairRegDateL == null)
				enptyAreaBit += 10;
			if (PairManager.pairRegDateR == null)
				enptyAreaBit++;

			// PairManagerの空白が埋まるのか調べる(空白bit)  99:newLine(空白なし)  10:左空白  1:右空白
//			bool flgNewLine = false;
			if (PairManager.tempCurrentLR == PairManager.LEFT) {
				switch (enptyAreaBit) {
				case 10:
					enptyAreaBit = 99;
					break;
				case 11:
					enptyAreaBit = 1;
					break;
				default :
					break;
				}
			}
			if (PairManager.tempCurrentLR == PairManager.RIGHT) {
				switch (enptyAreaBit) {
				case 1:
					enptyAreaBit = 99;
					break;
				case 11:
					enptyAreaBit = 10;
					break;
				default :
					break;
				}
			}

			// 空白なし上書きの場合は前処理として更新前のPairManagerを使ってMemberManagerのpare情報を削除
			// MemberManagerのpare情報は 'pairが完成したときのみ' 付加されている
			if (enptyAreaBit == 0) {
				MemberManager.clearPairMemberOfRegDate (PairManager.getPairRegDate (PairManager.pairRegDateL));
				MemberManager.clearPairMemberOfRegDate (PairManager.getPairRegDate (PairManager.pairRegDateR));
			} 
				
			if (PairManager.tempCurrentLR == PairManager.LEFT) {
				PairManager.pairNameKnji_familyL = MemberManager.nameKaji_family;
				PairManager.pairNameKnji_firstL = MemberManager.nameKaji_first;
				PairManager.pairGenderL = MemberManager.gender;
				PairManager.pairRegDateL = MemberManager.regDate; 
				tPfbParent.GetChild (PairManager.tempCurrentRow).Find ("ImgPairBase/LeftMember/MemberName").GetComponent<Text> ().text = MemberManager.nameKaji_family + " " + MemberManager.nameKaji_first;
				tPfbParent.GetChild (PairManager.tempCurrentRow).Find ("ImgPairBase/LeftMember/ImgMember").GetComponent<Image> ().color = MemberManager.gender == 0 ? Colors.male : Colors.female;
			} else {
				PairManager.pairNameKnji_familyR = MemberManager.nameKaji_family;
				PairManager.pairNameKnji_firstR = MemberManager.nameKaji_first;
				PairManager.pairGenderR = MemberManager.gender;
				PairManager.pairRegDateR = MemberManager.regDate; 
				tPfbParent.GetChild (PairManager.tempCurrentRow).Find ("ImgPairBase/RightMember/MemberName").GetComponent<Text>().text = MemberManager.nameKaji_family + " " + MemberManager.nameKaji_first;
				tPfbParent.GetChild (PairManager.tempCurrentRow).Find ("ImgPairBase/RightMember/ImgMember").GetComponent<Image>().color = MemberManager.gender == 0 ? Colors.male : Colors.female;
			}

			if (enptyAreaBit == 99) {
				// 今回埋まったので行追加の場合、MemberManagerにpair情報を付加
				pt = Instantiate (pfbMember).transform;
				pt.name = pfbMember.name;
				pt.SetParent (tPfbParent, false);
				MemberManager.updatePairMember (PairManager.pairRegDateL, PairManager.pairRegDateR);
				MemberManager.updatePairMember (PairManager.pairRegDateR, PairManager.pairRegDateL);
				PairManager.inserNewtPair ();
			}
			if (enptyAreaBit == 0) {
				// 空白なし上書きの場合、MemberManagerにpair情報を付加
				MemberManager.updatePairMember (PairManager.pairRegDateL, PairManager.pairRegDateR);
				MemberManager.updatePairMember (PairManager.pairRegDateR, PairManager.pairRegDateL);
				PairManager.updatePair (PairManager.tempCurrentRow);
			} else {
				// 両方または片方が空白の場合
				PairManager.updatePair (PairManager.tempCurrentRow);
			}
		}
		PairManager.tempSelectMyDate = "NULL";
		PairManager.tempCurrentRow = 0;
		PairManager.tempCurrentLR = 0;
	}


	// Use this for initialization
	void Start () {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
		tPfbParent = this.transform.Find ("PairMemberPanel/ListScroll/LayoutVertical");
		PairManager.initPair ();

		this.gameObject.SetActive (false);
	}
}
