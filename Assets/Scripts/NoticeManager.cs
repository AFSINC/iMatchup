using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class NoticeManager : SingletonMonoBehaviour<NoticeManager> {

	public class _Notice {
		public int readStat;				// 0:未読      1:既読
		public string noticeDate;		// 通知作成日付(NoticeDataの作成日)
		public string noticeTitle;		// 通知タイトル
		public string category;			// カテゴリー  SYS, RES, NET
		public string message;			// カテゴリーSYS: 通知内容
		public bool  noDispFlg;			// カテゴリーSYS: 表示設定 true:表示しない false:表示する
		public string sumDate;			// カテゴリーRES: (結果集計)に使用 集計開始日付
		public string sumEndDate;	// カテゴリーRES: (結果集計)に使用 結果終了日付
		public string noticeID;			// カテゴリー毎に使用

		public _Notice() {
			readStat = 0;
			noticeDate = DateTime.Now.ToString ("s", new System.Globalization.CultureInfo("ja-JP"));
		}
	}
	private static _Notice _notice = new _Notice();		// 更新・参照用のワーク

	public class Notice : SavableSingleton<Notice>{
		public string saveDate ;
		public List<_Notice> noticeList;

		public Notice() {
			noticeList = new List<_Notice>();
		}
	}
	private static List<_Notice> _noticeList = new List<_Notice>();		// 更新・参照用のワーク

	public static string CATE_SYSTEM = "SYS";
	public static string CATE_RESULT = "RES";
	public static string CATE_NETWK = "NET";

	// -- setter, getter --
	public static int readStat {
		get{return _notice.readStat;}
		set{ _notice.readStat = value; }
	}
	public static string noticeDate {
		get{return _notice.noticeDate;}
		set{ _notice.noticeDate = value; }
	}
	public static string noticeTitle {
		get{return _notice.noticeTitle;}
		set{ _notice.noticeTitle = value; }
	}
	public static string category {
		get{return _notice.category;}
		set{ _notice.category = value; }
	}
	public static string message {
		get{return _notice.message;}
		set{ _notice.message = value; }
	}
	public static bool noDispFlg {
		get{return _notice.noDispFlg;}
		set{ _notice.noDispFlg = value; }
	}
	public static string sumDate {
		get{return _notice.sumDate;}
		set{ _notice.sumDate = value; }
	}
	public static string sumEndDate {
		get{return _notice.sumEndDate;}
		set{ _notice.sumEndDate = value; }
	}
	public static string noticeID {
		get{return _notice.noticeID;}
		set{ _notice.noticeID = value; }
	}

	public static void initNotice() {
		_notice = new _Notice();
	}
	public static int getAllNoticeCount() {
		return Notice.Instance.noticeList.Count;
	}
	public static int getSysNoticeCount() {
		int cnt = 0;
		_noticeList = Notice.Instance.noticeList.Where (r => r.category == NoticeManager.CATE_SYSTEM).ToList();
		foreach (_Notice n in _noticeList)
			cnt++;
		return cnt;
	}
	public static int getResNoticeCount() {
		int cnt = 0;
		_noticeList = Notice.Instance.noticeList.Where (r => r.category == NoticeManager.CATE_RESULT).ToList();
		foreach (_Notice n in _noticeList)
			cnt++;
		return cnt;
	}
	public static void posNoticeOfListIdx(int iIdx) {
		_notice =  _noticeList[iIdx];
	}
	public static bool posNoticeSYSOfNtcDate(string iCate,  string iNoticeDate) {
		var q = Notice.Instance.noticeList.Where (r => r.category == iCate && r.noticeDate == iNoticeDate);
		foreach (_Notice n in  q) {
			_notice = n;
			return true;
		}
		return false;
	}
	public static bool posNoticeRESOfNtcDate(string iCate,  string iNoticeDate, string iSumDate) {
		var q = Notice.Instance.noticeList.Where (r => r.category == iCate && r.noticeDate == iNoticeDate && r.sumDate == iSumDate);
		foreach (_Notice n in  q) {
			_notice = n;
			return true;
		}
		return false;
	}
	public static bool posNoticeNETOfNtcDate(string iCate,  string iNoticeDate, string iDeliDate) {
		var q = Notice.Instance.noticeList.Where (r => r.category == iCate && r.noticeDate == iNoticeDate);
		foreach (_Notice n in  q) {
			_notice = n;
			return true;
		}
		return false;
	}
	public static void addNotice() {
		Notice.Instance.noticeList.Add (_notice);
		_notice = new _Notice();
	}
	public static bool chkSysNoticeOfNDate(string noticeDate) {
		var q = Notice.Instance.noticeList.Where (r => r.category == NoticeManager.CATE_SYSTEM && r.noticeDate == noticeDate);
		foreach (_Notice n in q) {
			_notice = n;
			return true;
		}
		return false;
	}
	public static void delSysNoticeOfNDate(string noticeDate) {
		var q = Notice.Instance.noticeList.Where (r => r.category == NoticeManager.CATE_SYSTEM && r.noticeDate == noticeDate);
		foreach (_Notice n in q) {
			n.noDispFlg = true;
		}
		Save ();
	}
	public static void delResNoticeOfResult(string iSumDate) {
		Notice.Instance.noticeList.RemoveAll (r => r.category == NoticeManager.CATE_RESULT && r.sumDate == iSumDate);
		Save ();
	}




	public static void Load() {
		Notice.Load ();
	}
	public static void Save()  {
		Notice.Instance.saveDate = DateTime.Now.ToString ("s", new System.Globalization.CultureInfo("ja-JP"));
		Notice.Save ();
	}
	public static void Reset() {
		Notice.Reset ();
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
			Notice.Save ();
	}
}
