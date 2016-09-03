using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PairManager : SingletonMonoBehaviour<PairManager> {
	public class _Pair {
		public string pairNameKnji_familyL;
		public string pairNameKnji_firstL;
		public string pairRegDateL;
		public int pairGenderL;
		public string pairNameKnji_familyR;
		public string pairNameKnji_firstR;
		public string pairRegDateR;
		public int pairGenderR;
	}
	private static _Pair _pair;

	// View受け渡しのためのテンポラリー
	public static int tempCurrentRow;
	public static int tempCurrentLR;
	public static int INIT  = 0;
	public static int LEFT  = 1;
	public static int RIGHT  = 2;
	public static string tempSelectMyDate;	

	class PairList: SavableSingleton<PairList>{
		public List<_Pair> pairList;

		public PairList() {
			pairList = new List<_Pair>();
		}
	}

	// -- setter, getter --
	public static string pairNameKnji_familyL {
		get{return _pair.pairNameKnji_familyL ;}
		set{ _pair.pairNameKnji_familyL = value; }
	}
	public static string pairNameKnji_firstL {
		get{return _pair.pairNameKnji_firstL ;}
		set{ _pair.pairNameKnji_firstL = value; }
	}
	public static string pairRegDateL {
		get{return _pair.pairRegDateL ;}
		set{ _pair.pairRegDateL = value; }
	}
	public static int pairGenderL {
		get{return _pair.pairGenderL ;}
		set{ _pair.pairGenderL = value; }
	}
	public static string pairNameKnji_familyR {
		get{return _pair.pairNameKnji_familyR ;}
		set{ _pair.pairNameKnji_familyR = value; }
	}
	public static string pairNameKnji_firstR {
		get{return _pair.pairNameKnji_firstR ;}
		set{ _pair.pairNameKnji_firstR = value; }
	}
	public static string pairRegDateR {
		get{return _pair.pairRegDateR ;}
		set{ _pair.pairRegDateR = value; }
	}
	public static int pairGenderR {
		get{return _pair.pairGenderR ;}
		set{ _pair.pairGenderR = value; }
	}

	// -- function --
	public static void initPair() {
		_pair = new _Pair ();
	}

	public static void newPair() {
		_pair = new _Pair ();
	}

	public static void addPair() {
		PairList.Instance.pairList.Add(_pair);
		newPair ();
	}

	public static int getPairCount() {
		return PairList.Instance.pairList.Count;
	}

	public static string getPairRegDate(string iRegDate) {		// 指定選手にPairがいたらRegDateを返す
		foreach (_Pair p in PairList.Instance.pairList) {
			if (p.pairRegDateL == iRegDate)
				return p.pairRegDateR;
			if (p.pairRegDateR == iRegDate)
				return p.pairRegDateL;
		}
		return null;
	}

	public static string getPairPartnerNameOfMyRegDate(string iRegDate) {
		foreach (_Pair p in PairList.Instance.pairList) {
			if (p.pairRegDateL == iRegDate) {
				return p.pairNameKnji_familyR +" " + p.pairNameKnji_firstR;
			}
			if (p.pairRegDateR == iRegDate) {
				return p.pairNameKnji_familyL + " " + p.pairNameKnji_firstL;
			}
		}
		return null;
	}

	public static int getPairListIndexOf(string iRegDate) {
		for (int i=0; i<PairList.Instance.pairList.Count; i++) {
			if (PairList.Instance.pairList[i].pairRegDateL == iRegDate)
				return i;
			if (PairList.Instance.pairList[i].pairRegDateR == iRegDate)
				return i;
		}
		return -1;
	}

	public static string getRegNoPairinList() {
		string result = null;
		foreach (_Pair p in  PairList.Instance.pairList) {
			if (p.pairRegDateL != null && p.pairRegDateR == null)
				result = p.pairRegDateL;
			if (p.pairRegDateR != null && p.pairRegDateL == null)
				result = p.pairRegDateR;
		}
		return result;
	}



	public static bool posPair(int i) {
		if (i < PairList.Instance.pairList.Count) {
			_pair = PairList.Instance.pairList[i];
			return true;
		} else {
			Debug.Log ("Error：Over MemberCount");
			return false;
		}
	}

	public static void inserNewtPair() {
		PairList.Instance.pairList.Add (new _Pair());
	}

	public static void cleanEmptyPair() {
		PairList.Instance.pairList.RemoveAll (a => a.pairRegDateL == null || a.pairRegDateR == null);
	}

	public static void clearPairMemberOfRegDate(int pairIdx) {
		MemberManager.clearPairMemberOfRegDate (PairList.Instance.pairList[pairIdx].pairRegDateL);
		MemberManager.clearPairMemberOfRegDate (PairList.Instance.pairList[pairIdx].pairRegDateR);
		PairList.Instance.pairList.RemoveAt(pairIdx);
		initPair ();
	}

	public static void clearPairMemberOfRegDate(string sRegDate) {
		int idx = PairManager.getPairListIndexOf (sRegDate);
		if (idx == -1)
			return;
		
		MemberManager.clearPairMemberOfRegDate (PairList.Instance.pairList[idx].pairRegDateL);
		MemberManager.clearPairMemberOfRegDate (PairList.Instance.pairList[idx].pairRegDateR);
		PairList.Instance.pairList.RemoveAt(idx);
		initPair ();
	}

	public static void cleanPairAll() {
		PairList.Instance.pairList.Clear();
		initPair ();
		MemberManager.cleanPairAll ();
	}

	public static void updatePair(int iListIdx) {
		PairList.Instance.pairList.RemoveAt(iListIdx);
		PairList.Instance.pairList.Insert (iListIdx, _pair);
	}

	public static void updateName(string iRegDate, string sName_Family, string sName_First, int iGender) {
		for (int i =0; i < PairList.Instance.pairList.Count; i++) {
			if (PairList.Instance.pairList[i].pairRegDateL == iRegDate) {
				PairList.Instance.pairList [i].pairNameKnji_familyL = sName_Family;
				PairList.Instance.pairList [i].pairNameKnji_firstL = sName_First;
				PairList.Instance.pairList [i].pairGenderL = iGender;
			}
			if (PairList.Instance.pairList[i].pairRegDateR == iRegDate) {
				PairList.Instance.pairList [i].pairNameKnji_familyR = sName_Family;
				PairList.Instance.pairList [i].pairNameKnji_firstR = sName_First;
				PairList.Instance.pairList [i].pairGenderR = iGender;
			}
		}
		Save ();
	}



	public static void Load() {
		PairList.Load ();
	}
	public static void Save() {
		PairList.Save ();
	}
	public static void Reset() {
		PairList.Reset ();
	}


	public void Awake()	{
		if(this != Instance) {
			Destroy(this);
			return;
		}
		DontDestroyOnLoad(this.gameObject);
	}

	void OnApplicationPause (bool pauseStatus) {
		if (pauseStatus)
			PairList.Save ();
	}
}
