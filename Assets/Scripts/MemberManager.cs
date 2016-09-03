using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class MemberManager : SingletonMonoBehaviour<MemberManager> {

	public class _Member {
		public int idxRec;
		public string serial;
		public int activeStat;
		public string nameKaji_family;
		public string nameKaji_first;
		public string nameKana_family;
		public string nameKana_first;
		public string eMail;
		public int rank;
		public string memo;
		public int gender;
		public string regDate;
		public string pairDate;
		public string bfrDate;
		public int game;
		public float winPer;
		public int win;
		public int lose;
		public int draw;
	}
	public const int MAX_IDXREC = 999999; 	// Activeではない者に割り当てる idxRec値
	public const int MAX_MEMBER = 100; 		// Member登録できる最大数
	private static _Member _member;
	private static _Member _noPairActivMember;
	private static List<_Member> noPairActivList;

	class MemberList: SavableSingleton<MemberList>{
		public string saveDate;						// 保存日付
		public List<_Member> memberList;

		public MemberList() {
			memberList = new List<_Member>();
			noPairActivList = new List<_Member>();
		}
	}


	// -- setter, getter --
	public static string saveDate {
		get{return MemberList.Instance.saveDate;}
	}
	public static int idxRec {
		get{return _member.idxRec ;}
		set{ _member.idxRec = value; }
	}
	public static string memberSerial {
		get{return _member.serial ;}
		set{ _member.serial = value; }
	}
	public static int activeStat {
		get{return _member.activeStat ;}
		set{ _member.activeStat = value; }
	}
	public static string nameKaji_family {
		get{return _member.nameKaji_family ;}
		set{ _member.nameKaji_family = value; }
	}
	public static string nameKaji_first {
		get{return _member.nameKaji_first ;}
		set{ _member.nameKaji_first = value; }
	}
	public static string nameKana_family {
		get{return _member.nameKana_family ;}
		set{ _member.nameKana_family = value; }
	}
	public static string nameKana_first {
		get{return _member.nameKana_first ;}
		set{ _member.nameKana_first = value; }
	}
	public static int gender {
		get{return _member.gender ;}
		set{ _member.gender = value; }
	}
	public static string regDate {
		get{return _member.regDate ;}
		set{ _member.regDate = value; }
	}
	public static string pairDate {
		get{return _member.pairDate ;}
		set{ _member.pairDate = value; }
	}
	public static string eMail {
		get{return _member.eMail ;}
		set{ _member.eMail = value; }
	}
	public static int rank {
		get{return _member.rank ;}
		set{ _member.rank = value; }
	}
	public static string memo {
		get{return _member.memo ;}
		set{ _member.memo = value; }
	}
	public static int game {
		get{return _member.game ;}
		set{ _member.game = value; }
	}
	public static int win {
		get{return _member.win ;}
		set{ _member.win = value; }
	}
	public static int lose {
		get{return _member.lose ;}
		set{ _member.lose = value; }
	}
	public static int draw {
		get{return _member.draw ;}
		set{ _member.draw = value; }
	}


	// -- function --
	public static void initMember() {
		_member = new _Member ();
	}

	public static int getMemberCount() {
		return MemberList.Instance.memberList.Count;
	}

	public static int getActiveMemberCount() {
		var query = MemberList.Instance.memberList.Where(p => p.activeStat == 1); 
		return query.Count ();
	}

	public static int getActiveMemberLastNum() {
/*
		var query = MemberList.Instance.memberList.Where(p => p.activeStat == 1).OrderByDescending(p => p.idxRec); 
		int iMax;
		foreach (_Member m in query) {
			iMax = m.idxRec;
			break;
		}
		return iMax;
*/
		var query =  MemberList.Instance.memberList.Where (p => p.activeStat == 1);
		if (query.Count () != 0)
			return query.Max (p => p.idxRec);
		else
			return 0;
	}

	public static int getIdxRecOfListIdx(int iListIdx) {
		return MemberList.Instance.memberList[iListIdx].idxRec;
	}

	public static string getRegDateOfListIdx(int iListIdx) {
		return MemberList.Instance.memberList[iListIdx].regDate;
	}

	public static void newMember() {
		_member = new _Member ();
	}

	public static void posMemberFirst() {
		if (MemberList.Instance.memberList.Count == 0) {
			return;
		} else {
			_member = MemberList.Instance.memberList[0];
		}
	}

	public static bool posMember(int i) {
		if (i < MemberList.Instance.memberList.Count) {
			_member = MemberList.Instance.memberList[i];
			return true;
		} else {
			Debug.Log ("Error：Over MemberCount");
			return false;
		}
	}

	public static void posMember(string iRegDate) {  // posMemberOfRegDate と同じ
		_member = new _Member ();
		var query = MemberList.Instance.memberList.Where(p => p.regDate == iRegDate); 
		foreach (_Member m in query) {
/*			_member.idxRec = m.idxRec;
			_member.serial = m.serial;
			_member.activeStat = m.activeStat;
			_member.nameKaji_family = m.nameKaji_family;
			_member.nameKaji_first = m.nameKaji_first;
			_member.nameKana_family = m.nameKana_family;
			_member.nameKana_first = m.nameKana_first;
			_member.eMail = m.eMail;
			_member.rank = m.rank;
			_member.memo = m.memo;
			_member.gender = m.gender;
			_member.regDate = m.regDate;
			_member.pairDate = m.pairDate;
			_member.bfrDate = m.bfrDate;
			_member.game = m.game;
			_member.win = m.win;
			_member.lose = m.lose;*/
			_member = m;
		}
	}

	public static bool posActiveMember(int i) {
		if (i < getActiveMemberCount()) {
			_member = MemberList.Instance.memberList[i];
			return true;
		} else {
			Debug.Log ("Error：Over MemberCount");
			return false;
		}
	}

	public static bool existActiveMemberOfRegDate(string iRegDate) {		// 指定登録日の選択メンバがいるか確認　true:存在　false:いない
		return MemberList.Instance.memberList.Exists (p => p.regDate == iRegDate && p.activeStat == 1);
	}

	public static bool posMemberOfRegDate (string iRegDate) {
		_member = MemberList.Instance.memberList.Find(a => a.regDate == iRegDate);
		if (_member != null)
			return true;
		else
			return false;
	}

	public static void addMember() {
		MemberList.Instance.memberList.Add(_member);
		newMember ();
	}

	public static void updateMemberInfo() {
		for (int i = 0; i < MemberList.Instance.memberList.Count; i++) {
			if (MemberList.Instance.memberList [i].regDate == _member.regDate) {
				MemberList.Instance.memberList.RemoveAt (i);
				MemberList.Instance.memberList.Insert (i, _member);
				PairManager.updateName (_member.regDate, _member.nameKaji_family, _member.nameKaji_first, _member.gender);
			}
		}
		Save ();
	}

	public static void movMember(int sListIdx, int dListIdx) {
		_member = MemberList.Instance.memberList[sListIdx];
		MemberList.Instance.memberList.RemoveAt(sListIdx);
		MemberList.Instance.memberList.Insert (dListIdx, _member);
		renumIdxRec ();
		Save ();
	}

	public static void exchengeIdxRec(int sIdxRec, int dIdxRec) {
		int i = MemberList.Instance.memberList.FindIndex (m => m.idxRec == sIdxRec);
		MemberList.Instance.memberList[i].idxRec = dIdxRec;
		int j = MemberList.Instance.memberList.FindIndex (m => m.idxRec == dIdxRec);
		MemberList.Instance.memberList[j].idxRec = sIdxRec;
		Save ();
	}

	private static void renumIdxRec() {
		int i = 0;
		foreach (_Member m in MemberList.Instance.memberList) {
			if (m.idxRec == MAX_IDXREC)
				return;
			m.idxRec = i++;
		}
	}

	public static void removeMember(int idx) {
		MemberList.Instance.memberList.RemoveAt (idx);
	}

	public static void updatePairMember(string iRegDate, string iPairDate) {
		foreach (_Member m in MemberList.Instance.memberList) {
			if (m.regDate == iRegDate)
				m.pairDate = iPairDate;
		}
	}

	public static void clearPairMemberOfRegDate (string iRegDate) {
		foreach (_Member m in MemberList.Instance.memberList) {
			if (m.regDate == iRegDate)
				m.pairDate = null;
		}
	}

	public static void cleanPairAll() {
		foreach (_Member m in MemberList.Instance.memberList) {
			if (m.pairDate != null && m.pairDate != "")
				m.pairDate = null;
		}
	}

//	public static void sortMember(int key) {
//	}
	public static void sortIdxRec() {
		MemberList.Instance.memberList = MemberList.Instance.memberList.OrderBy (a => a.idxRec).ToList<_Member>();
	}
	public static void sortAvtiveMember() {
//		MemberList.Instance.memberList = MemberList.Instance.memberList.OrderBy (a => a.idxRec).ToList<_Member>();
		MemberList.Instance.memberList = MemberList.Instance.memberList.OrderByDescending (a => a.activeStat).ToList<_Member>();
	}
	public static void sortMemberName() {
		MemberList.Instance.memberList = MemberList.Instance.memberList.OrderBy (a => a.nameKana_family).ThenBy(a => a.nameKana_first).ToList<_Member>();
	}
	public static void sortRegDate() {
		MemberList.Instance.memberList = MemberList.Instance.memberList.OrderBy (a => a.regDate).ToList<_Member>();
	}
	public static void sortSrial() {
		MemberList.Instance.memberList = MemberList.Instance.memberList.OrderBy (a => a.serial).ToList<_Member>();
	}
	public static void sortGender() {
		MemberList.Instance.memberList = MemberList.Instance.memberList.OrderBy (a => a.gender).ToList<_Member>();
	}
	public static void sortGame() {
		MemberList.Instance.memberList = MemberList.Instance.memberList.OrderByDescending (a => a.game).ToList<_Member>();
	}
	public static void sortWin() {
		MemberList.Instance.memberList = MemberList.Instance.memberList.OrderByDescending (a => a.win).ToList<_Member>();
	}
	public static void sortWinPer() {
		foreach (_Member m in MemberList.Instance.memberList) {
			float fWin = m.win;
			float fLose = m.lose;
			float fDraw = m.draw;
			m.winPer =Mathf.Round (fWin / (fWin + fLose + fDraw) * 100);
		}
		MemberList.Instance.memberList = MemberList.Instance.memberList.OrderByDescending (a => a.winPer).ToList<_Member>();
	}



	// function for Pare
	// -- setter, getter --
	public static string pairRegDate {
		get{return _noPairActivMember.regDate ;}
		set{ _noPairActivMember.regDate = value; }
	}
	public static string pairNameKana_family {
		get{return _noPairActivMember.nameKana_family ;}
		set{ _noPairActivMember.nameKana_family = value; }
	}
	public static string pairNameKana_first {
		get{return _noPairActivMember.nameKana_first ;}
		set{ _noPairActivMember.nameKana_first = value; }
	}
	public static string pairNameKaji_family {
		get{return _noPairActivMember.nameKaji_family ;}
		set{ _noPairActivMember.nameKaji_family = value; }
	}
	public static string pairNameKaji_first {
		get{return _noPairActivMember.nameKaji_first ;}
		set{ _noPairActivMember.nameKaji_first = value; }
	}
	public static int pairGender {
		get{return _noPairActivMember.gender ;}
		set{ _noPairActivMember.gender = value; }
	}

	public static void initNoPairActivList(string myRegDate) {
		noPairActivList.Clear();
		var query = MemberList.Instance.memberList.Where (p => p.activeStat == 1 && (p.pairDate == "" || p.pairDate == null) || p.regDate == myRegDate); 
		foreach (_Member m in query) {
			_noPairActivMember = new _Member();
			_noPairActivMember.regDate = m.regDate;
			_noPairActivMember.nameKana_family = m.nameKana_family;
			_noPairActivMember.nameKana_first = m.nameKana_first;
			_noPairActivMember.nameKaji_family = m.nameKaji_family;
			_noPairActivMember.nameKaji_first = m.nameKaji_first;
			_noPairActivMember.gender = m.gender;
			noPairActivList.Add (_noPairActivMember);
		}
		noPairActivList = noPairActivList.OrderBy (a => a.nameKana_family).ThenBy (a => a.nameKana_first).ToList<_Member> ();

//		foreach (_Member m in noPairActivList)
//			Debug.Log ("name " + m.nameKana_family+" "+m.nameKana_first);
	}
	public static int getNotPairActiveCount() {
		return noPairActivList.Count ();
	}
	public static bool posPairMember(int i) {
		if (i < noPairActivList.Count) {
			_noPairActivMember = noPairActivList[i];
			return true;
		} else {
			Debug.Log ("Error：Over noPairActivListCount");
			return false;
		}
	}
	public static void countUpPlayerGameOfPlaceNo(string  iRegDate) {	// 指定選手の勝敗数とGame数を1増やす(永続的記録)
		bool result = false;
		var qry = MemberList.Instance.memberList.Where(p => p.regDate == iRegDate);
		foreach (_Member m in qry) {
			++m.game;
		}
	}
	public static void countUpPlayerWinLoseOfPlaceNo(string  iRegDate, int iWinLose) {	// 指定選手の勝敗数を1増やす(集計時のみの記録)
		bool result = false;
		var qry = MemberList.Instance.memberList.Where(p => p.regDate == iRegDate);
		foreach (_Member m in qry) {
			if (iWinLose == GameManager.GAMEWIN)
				++m.win;
			else if (iWinLose == GameManager.GAMELOSE)
				++m.lose;
			else
				++m.draw;
		}
	}


	public static void Load() {
		MemberList.Load ();
	}
	public static void Save() {
		sortIdxRec ();
		renumIdxRec ();
		MemberList.Instance.saveDate = DateTime.Now.ToString ("s", new System.Globalization.CultureInfo("ja-JP"));
		MemberList.Save ();
	}
	public static void Reset() {
		MemberList.Reset ();
	}


	public void Awake()	{
		if(this != Instance) {
			Destroy(this);
			return;
		}
		DontDestroyOnLoad(this.gameObject);
	}


	void OnApplicationPause (bool pauseStatus)
	{
		if (pauseStatus)
			MemberList.Save ();
	}
}
