using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ResultManager  : SingletonMonoBehaviour<ResultManager> {
	public class _Result {
		public int notice;							// 通知viewにて更新  0:未読　1:既読
		public string sumDate;					// 集計開始日付(集計単位)
		public string sumEndDate;				// 集計終了日付(集計単位)
		public string gameSDate;				// ゲーム開始日付(試合単位)
		public string gameEDate;				// ゲーム終了日付(試合単位)
		public string regDateLTop;				// 左上選手登録日付 Left-Top   Singles-Left
		public string regDateLBtm;				// 左下選手登録日付 Left-Btm
		public string regDateRTop;				// 右上選手登録日付 Right-Top   Singles-Right
		public string regDateRBtm;				// 右下選手登録日付 Right-Btm
		public string memberKLTopFm;		// 左上選手カナ(姓)
		public string memberKLTopFs;		// 左上選手カナ(名)
		public string memberLTopFm;		// 左上選手氏名(姓)
		public string memberLTopFs;			// 左上選手氏名(名)
		public string memberKLBtmFm;		// 左下選手カナ(姓)
		public string memberKLBtmFs;		// 左下選手カナ(名)
		public string memberLBtmFm;		// 左下選手氏名(姓)
		public string memberLBtmFs;			// 左下選手氏名(名)
		public string memberKRTopFm;		// 右上選手カナ(姓)
		public string memberKRTopFs;		// 右上選手カナ(名)
		public string memberRTopFm;		// 右上選手氏名(姓)
		public string memberRTopFs;			// 右上選手氏名(名)
		public string memberKRBtmFm;		// 右下選手カナ(姓)
		public string memberKRBtmFs;		// 右下選手カナ(名)
		public string memberRBtmFm;		// 右下選手氏名(姓)
		public string memberRBtmFs;			// 右下選手氏名(名)
		public string courtName;				// コート名称
		public int gamePointL;					// 左点数
		public int gamePointR;					// 右点数
	}
	private static _Result _result;		// 抽出時の参照ワークとして使用 (他クラスからのget/set用)
/*
	public class _Court {
		public int courtNumber;				// コート番号  コート1:0~コート6:23
		public int courtStat;						// コート使用状態  解除中:0 確定中:1 開始中:2
		public string courtName;				// コート名
		public int courtkind;						// コート種類  Singles:0  Doubles:1
	}
	private static _Court _court;
*/
	public class ResultList: SavableSingleton<ResultList> {
		public List<_Result> resultList;

		public ResultList() {
			resultList = new List<_Result>();
		}
	}
	private static List<_Result> _resultList = new List<_Result>();	// 抽出時の参照ワークとして使用 (他クラスからのget/set用)

	public class _ResultMember: SingletonMonoBehaviour<_ResultMember> {
		public string regDate;
		public string memberKTopFm;		// 選手カナ(姓)
		public string memberKTopFs;		// 選手カナ(名)
		public string memberTopFm;		// 選手氏名(姓)
		public string memberTopFs;		// 選手氏名(名)
		public int gender;						// 性別 (0:男性 1:女性)
	}
	private static List<_ResultMember> _resultMember; // 抽出時の参照ワークとして使用

	// -- setter, getter --
	public static int notice {
		get{return _result.notice ;}
		set{ _result.notice = value; }
	}
	public static string sumDate {
		get{return _result.sumDate ;}
		set{ _result.sumDate = value; }
	}
	public static string sumEndDate {
		get{return _result.sumEndDate ;}
		set{ _result.sumEndDate = value; }
	}
	public static string gameSDate {
		get{return _result.gameSDate ;}
		set{ _result.gameSDate = value; }
	}
	public static string gameEDate {
		get{return _result.gameEDate ;}
		set{ _result.gameEDate = value; }
	}
	public static string regDateLTop {
		get{return _result.regDateLTop ;}
		set{ _result.regDateLTop = value; }
	}
	public static string regDateLBtm {
		get{return _result.regDateLBtm ;}
		set{ _result.regDateLBtm = value; }
	}
	public static string regDateRTop {
		get{return _result.regDateRTop ;}
		set{ _result.regDateRTop = value; }
	}
	public static string regDateRBtm {
		get{return _result.regDateRBtm ;}
		set{ _result.regDateRBtm = value; }
	}
	public static string memberKLTopFm {
		get{return _result.memberKLTopFm ;}
		set{ _result.memberKLTopFm = value; }
	}
	public static string memberKLTopFs {
		get{return _result.memberKLTopFs ;}
		set{ _result.memberKLTopFs = value; }
	}
	public static string memberLTopFm {
		get{return _result.memberLTopFm ;}
		set{ _result.memberLTopFm = value; }
	}
	public static string memberLTopFs {
		get{return _result.memberLTopFs ;}
		set{ _result.memberLTopFs = value; }
	}
	public static string memberKLBtmFm {
		get{return _result.memberKLBtmFm ;}
		set{ _result.memberKLBtmFm = value; }
	}
	public static string memberKLBtmFs {
		get{return _result.memberKLBtmFs ;}
		set{ _result.memberKLBtmFs = value; }
	}
	public static string memberLBtmFm {
		get{return _result.memberLBtmFm ;}
		set{ _result.memberLBtmFm = value; }
	}
	public static string memberLBtmFs {
		get{return _result.memberLBtmFs ;}
		set{ _result.memberLBtmFs = value; }
	}
	public static string memberKRTopFm {
		get{return _result.memberKRTopFm ;}
		set{ _result.memberKRTopFm = value; }
	}
	public static string memberKRTopFs {
		get{return _result.memberKRTopFs ;}
		set{ _result.memberKRTopFs = value; }
	}
	public static string memberRTopFm {
		get{return _result.memberRTopFm ;}
		set{ _result.memberRTopFm = value; }
	}
	public static string memberRTopFs {
		get{return _result.memberRTopFs ;}
		set{ _result.memberRTopFs = value; }
	}
	public static string memberKRBtmFm {
		get{return _result.memberKRBtmFm ;}
		set{ _result.memberKRBtmFm = value; }
	}
	public static string memberKRBtmFs {
		get{return _result.memberKRBtmFs ;}
		set{ _result.memberKRBtmFs = value; }
	}
	public static string memberRBtmFm {
		get{return _result.memberRBtmFm ;}
		set{ _result.memberRBtmFm = value; }
	}
	public static string memberRBtmFs {
		get{return _result.memberRBtmFs ;}
		set{ _result.memberRBtmFs = value; }
	}
	public static string courtName {
		get{return _result.courtName ;}
		set{ _result.courtName = value; }
	}
	public static int gamePointL {
		get{return _result.gamePointL ;}
		set{ _result.gamePointL = value; }
	}
	public static int gamePointR {
		get{return _result.gamePointR ;}
		set{ _result.gamePointR = value; }
	}



	public static void initResult() {
		_result = new _Result ();
	}
	public static void addResultList() {
		ResultList.Instance.resultList.Add (_result);
		initResult ();
	}
	public static bool posNewResult() {			// まだ通知で管理していない集計をワークに移す(1件処理で停止)
		var q = ResultList.Instance.resultList.Where (r => r.notice == 1 && r.sumEndDate != null);
		foreach (_Result r in q) {
			_result = r;
			r.notice = 0;
			Save ();			// 画面移動で重複処理されないようにセーブ
			return true;		// 実行終了後に通知を見ていない場合、複数件見つかる可能性があるので、1件見つけたら戻る
		}
		return false;
	}


	public static int getResultCountOfSumDate(string iSumDate) {		// 指定集計開始時間が同じ試合の件数を返す
		int cnt = 0;
		var q = ResultList.Instance.resultList.Where (r => r.sumDate == iSumDate && r.sumEndDate == null);
		foreach (_Result r in q)
			cnt++;
		return cnt;
	}
	public static void posResultOfSumDate(string iSumDate) {			// 指定集計開始時間で検索した試合をListワークに移す
		_resultList.Clear ();
		var q = ResultList.Instance.resultList.Where (r => r.sumDate == iSumDate && r.sumEndDate == null);
		foreach (_Result r in q)
			_resultList.Add (r);
	}
	public static string[] resMb;
	public static int getResultCountOfOfMember(string iSumDate) {		// 指定集計開始時間が同じ試合の選手数を返し、Regdateワークを作成
		_resultMember =  new List<_ResultMember> ();
		List<string> tmpRegDate = new List<string> ();
		var q = ResultList.Instance.resultList.Where (r => r.sumDate == iSumDate && r.sumEndDate == null);
		foreach (_Result r in q) {
			tmpRegDate.Add (r.memberKLTopFm+" "+r.memberKLTopFs+"|"+r.memberLTopFm+" "+r.memberLTopFs+"|"+r.regDateLTop);
			tmpRegDate.Add (r.memberKLBtmFm+" "+r.memberKLBtmFs+"|"+r.memberLBtmFm+" "+r.memberLBtmFs+"|"+r.regDateLBtm);
			tmpRegDate.Add (r.memberKRTopFm+" "+r.memberKRTopFs+"|"+r.memberRTopFm+" "+r.memberRTopFs+"|"+r.regDateRTop);
			tmpRegDate.Add (r.memberKRBtmFm+" "+r.memberKRBtmFs+"|"+r.memberRBtmFm+" "+r.memberRBtmFs+"|"+r.regDateRBtm);
		}
		tmpRegDate.Sort ();
		string[] mb = tmpRegDate.ToArray ();
		resMb = mb.Distinct().ToArray();
/*		tmpRegDate.Clear ();
		tmpRegDate = resMb.ToList ();
		tmpRegDate = tmpRegDate.OrderBy ();
		resMb = tmpRegDate.ToArray ();*/
		return resMb.Length;
	}
	public static void posResultOfListIdx(int iIdx) {								// ListIndexで指定されたデータをListワークからResultワークに移す
		_result = _resultList[iIdx];
	} 
	public static void deletesResultOfSumDate(string iSumDate) {		// 指定集計開始時間の試合を削除
		ResultList.Instance.resultList.RemoveAll (r => r.sumDate == iSumDate);
		Save ();
	} 
	public static string chkSumDate() {			// 前回集計終了していなかった場合は、集計日付(sumDate)を返す
		string sumDate = null;
		int iLast = ResultList.Instance.resultList.Count;
		if (iLast != 0) {
			_result = ResultList.Instance.resultList [iLast - 1];
			if (_result.sumEndDate == null)
				sumDate = _result.sumDate;
		}
		return sumDate;
	}










	public static void Load() {
		ResultList.Load ();
	}
	public static void Save() {
		ResultList.Save ();
	}
	public static void Reset() {
		ResultList.Reset ();
	}
	void Start () {}
}
