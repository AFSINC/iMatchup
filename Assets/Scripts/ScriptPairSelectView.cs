using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScriptPairSelectView : MonoBehaviour {
	public GameObject myView;
	public GameObject pfbMember;
	private string taPMyDate;
	private string taPYouDate;
	public Transform tPfbParent;
	private ViewManager viewManager;

	// CallBack
	public void _CallBackBtnCansel() {
		viewManager.chgPareView(myView, viewManager.OUT_RIGHT);
	}

	public void _toPareView() {	// メンバー決定時、pareSelectMemberから呼ばれ間接的にViewMagergerに返す
		viewManager.chgPareView(myView, viewManager.OUT_RIGHT);
	}

	public void loadView() {
		// リスト全削除
		int pfCnt = tPfbParent.childCount;
		for (int i=0; i < pfCnt; i++) {
			DestroyImmediate(tPfbParent.Find ("PairSelectMember").gameObject);
		}
	
		// 選択リスト作成
		// 活動中でpareになっていない人+Tapされたpareの人
		PairManager.posPair (PairManager.tempCurrentRow);
		if (PairManager.tempCurrentLR == PairManager.LEFT) {
			taPMyDate = PairManager.pairRegDateL;
			taPYouDate = PairManager.pairRegDateR;
		} else {
			taPMyDate = PairManager.pairRegDateR;
			taPYouDate = PairManager.pairRegDateL;
		}

		MemberManager.initNoPairActivList (taPMyDate);
		int cntParedActiveCnt = MemberManager.getNotPairActiveCount ();

		string noPairMemReg = PairManager.getRegNoPairinList ();

		for (int i=0; i < cntParedActiveCnt ;i++) {
			MemberManager.posPairMember (i);
			if (MemberManager.pairRegDate == taPYouDate) {
				continue;
			}
			if ((noPairMemReg != taPMyDate) && (MemberManager.pairRegDate == noPairMemReg)) {
				continue;
			}
			if (GameManager.chkLock(MemberManager.pairRegDate) == GameManager.LOCK) {	// 試合中Lockの選手
				continue;
			}
			if (GameManager.getPlayerPlace(MemberManager.pairRegDate) == GameManager.PLACE_BREAK) {		// 試合中の休憩中の選手
				continue;
			}

			Transform pt = Instantiate (pfbMember).transform;
			pt.name = pfbMember.name;
			pt.SetParent (tPfbParent, false);

			Image imgListBack = pt.GetComponentInChildren <Image> ();

			// 子コンポーネントのScriptに受け渡し用 日付を埋め込む
			pt.GetComponentInChildren<PairSelectMember>().regDate = MemberManager.pairRegDate;

			string sname = MemberManager.pairNameKaji_family + " " + MemberManager.pairNameKaji_first;
			if (MemberManager.pairRegDate == taPMyDate) 
				sname += "  [設定済]";
							
			pt.GetComponentInChildren <Text> ().text = sname;
			if (MemberManager.pairGender == 0) {
				imgListBack.color = Colors.male;
			} else {
				imgListBack.color = Colors.female;
			}
		}
		this.transform.Find ("PairMemberPanel/ListScroll/").GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
	}

	void Start () {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
		tPfbParent = this.transform.Find ("PairMemberPanel/ListScroll/LayoutVertical");
		this.gameObject.SetActive (false);
	}
}
