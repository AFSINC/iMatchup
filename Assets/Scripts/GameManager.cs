using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : SingletonMonoBehaviour<GameManager> {
	public class _TrsPlayer {
		public Transform trsPlayer;			// loadの都度、regDateをもとに割り当てられた Stock/Player のTransformで更新されるワーク的な変数
		public string regDate;					// 登録日付
	}
	public class TrsPlayer : SavableSingleton<TrsPlayer> {
		public List<_TrsPlayer> trsPlayerList;		// ※ transform が含まれているためJsonファイルとして保存できないため _Player から分離
		public TrsPlayer() {
			trsPlayerList = new List<_TrsPlayer>();
		}
	}
	private static _TrsPlayer _trsPlayer;	//ワーク

	public class _Player {
		public string regDate;					// 登録日付
		public string nameKaji_family;	
		public string nameKaji_first;
		public string nameKana_family;
		public string nameKana_first;
		public int gender;
		public int idxPriority;					// 優先順位
//		public int gameStat;					// 準備:0  試合:1  待機:2  休憩:3
		public int placeStat;						// どこに配置されているか  コート1:0~コート6:23  待機:24  休憩:25
		public int lockStat;						// コートでのロック状態   unlock:0   lock:1
//		public string pairRegDate;			// ペアになっている人の登録日付
		public int game;
		public int gameAjust;					// Game数の補正+-　手動修正または途中参加
		public int win;
		public int lose;
		public int draw;
		public int flgDelete;						// 変更確認するために使用 0:変更なし 1:選手から除外
	}
	private static _Player _player;
	public static int MAX_COURTNUM = 6;
	public static int MAX_PLAYERNUM = 21;
	public static int PLACE_STOK = 9999;
	public static int PLACE_WAIT = 24;
	public static int PLACE_BREAK = 25;
	public static int UNLOCK = 0;
	public static int LOCK = 1;
	public static int START = 2;
	public static int GAMEWIN = 2;
	public static int GAMELOSE = 1;
	public static int GAMEDRAW = 0;

	public class _Match {
		public string myRegDate;				// 自分の登録日付
		public string youRegDate;				// 相手の登録日付
		public int pair;								// 相手とのペア回数
		public int pairAjust;						// pair数の補正+-　手動修正または途中参加
		public int match;							// 相手との対戦回数
		public int matchAjust;					// match数の補正+-　手動修正または途中参加
	} 
	private static _Match _match;			// work
	public static List<_Match> _matchList;		// work

	class GameList: SavableSingleton<GameList>{
		public string saveDate;					// 保存日付
		public string gameStartDate;			// 組合せ試合開始日付
		public int gameStatus;					// 0:stop  1:集計なしstart  2:集計ありstart
		public int autoMode;						// 0:手動配置  1:自動配置
		public int gameCount;					// 試合数
		public List<_Player> playerList;
		public List<_Match> matchList;

		public GameList() {
			gameStatus = 0;
			autoMode = 1;
			playerList = new List<_Player>();		// 選手の並び
			matchList = new List<_Match>();		// ペア、対戦の並び
		}
	}
	class CourtList : SavableSingleton<CourtList>{
		public string saveDate;					// 保存日付
		public List<int> coutList;				// コート単位(0~5)の  0:解除中  1:確定中   2:開始中 の状態
		public List<string> matchStartDateList;	// コート試合開始日付
		public CourtList() {
			coutList = new List<int>();
			matchStartDateList = new List<string>();
			for (int cnt=0; cnt<MAX_COURTNUM; cnt++) {
				coutList.Add(new int());
				matchStartDateList.Add("");
			}
		}
	}

	public	static int currentPage;			// いま見ているページ PageScrollRect が更新

	// -- setter, getter --
	public static Transform trsPlayer {
		get{return _trsPlayer.trsPlayer;}
		set{ _trsPlayer.trsPlayer = value; }
	}
	public static string saveDate {
		get{return GameList.Instance.saveDate;}
	}
	public static string gameStartDate {
		get{return GameList.Instance.gameStartDate;}
		set{ GameList.Instance.gameStartDate = value; }
	}
	public static int gameStatus {
		get{return GameList.Instance.gameStatus;}
		set{ GameList.Instance.gameStatus = value; }
	}
	public static int autoMode {
		get{return GameList.Instance.autoMode;}
		set{ GameList.Instance.autoMode = value; }
	}
	public static int gameCount {
		get{return GameList.Instance.gameCount;}
		set{ GameList.Instance.gameCount = value; }
	}
	public static string regDate {
		get{return _player.regDate;}
		set{ _player.regDate = value; }
	}
	public static string nameKaji_family {
		get{return _player.nameKaji_family;}
		set{ _player.nameKaji_family = value; }
	}
	public static string nameKaji_first {
		get{return _player.nameKaji_first;}
		set{ _player.nameKaji_first = value; }
	}
	public static string nameKana_family {
		get{return _player.nameKana_family;}
		set{ _player.nameKana_family = value; }
	}
	public static string nameKana_first {
		get{return _player.nameKana_first;}
		set{ _player.nameKana_first = value; }
	}
	public static int gender {
		get{return _player.gender;}
		set{ _player.gender = value; }
	}
	public static int idxPriority {
		get{return _player.idxPriority;}
		set{ _player.idxPriority = value; }
	}
	public static int game {
		get{return _player.game;}
		set{ _player.game = value; }
	}
	public static int gameAjust {
		get{return _player.gameAjust;}
		set{ _player.gameAjust = value; }
	}
	public static int win {
		get{return _player.win;}
		set{ _player.win = value; }
	}
	public static int lose {
		get{return _player.lose;}
		set{ _player.lose = value; }
	}
	public static int flgDelete {
		get{return _player.flgDelete;}
		set{ _player.flgDelete = value; }
	}
	public static int placeStat {
		get{return _player.placeStat;}
		set{ _player.placeStat = value; }
	}
	public static int lockStat {
		get{return _player.lockStat;}
		set{ _player.lockStat = value; }
	}
/*	public static string pairRegDate {
		get{return _player.pairRegDate;}
		set{ _player.pairRegDate = value; }
	}*/


	// P:pri  N:PlayerNum  Q:weight  Nq:pair  Nr:match 
	//  Score = P + 2NG + (QN+1)(Aq+Bq+Cq)+(QN-1)(Ar+Br+Cr)
	public static bool getPositionA(bool pairAllowFlg){			// Aポジションになる選手をワークに移す
		int n = getPlayerCount ();
		int saveScr = 0;
		string regDate = "";
		var q1 = GameList.Instance.playerList.Where (p => p.lockStat == 0 && p.placeStat == 24);
		foreach (_Player p in q1) {										// 全選手で選択可能な選手を対象とする
			if (!pairAllowFlg) {												// PositonBがLockされている場合、Pairの選手は対象から除外する
				if (PairManager.getPairRegDate(p.regDate) != null) 
					continue;
			}
				
			// score部分計算　P + 2NG
			int scr = p.idxPriority + 2 * n * (p.game + p.gameAjust);
			if (scr < saveScr || saveScr == 0) {
				saveScr = scr;
				regDate = p.regDate;
			}
		}
		if (posPlayerOfRegDate (regDate))
			return true;
		else
			return false;
	}
	public static bool getPositionB(string iRegDatePosA, bool pairAllowFlg) {		// Bポジションになる選手をワークに移す
		int n = getPlayerCount ();

//		int priority = 0;
//		int game = 0;
		int saveScr = 0;
		string regDate = "";
		var q1 = GameList.Instance.playerList.Where (p => p.lockStat == 0 && p.placeStat == 24 && (p.regDate != iRegDatePosA));
		foreach (_Player p in q1) {										// Aポジションの選手以外で選択可能な選手を対象とする
			if (!pairAllowFlg) {								// PositonAがLockされている場合、Pairの選手は対象から除外する
				if (PairManager.getPairRegDate(p.regDate) != null) 
					continue;
			}
			// score部分計算　P + 2NG
			int scr1 = p.idxPriority + 2 * n * (p.game + p.gameAjust);
			int scr2 = 0;
			var q2 = GameList.Instance.matchList.Where (m => m.youRegDate == iRegDatePosA && m.myRegDate == p.regDate);
			foreach (_Match m in q2) {
				// score部分計算　(QN+1)(Aq)+(QN-1)(Ar)
// 				scr2 += (SettingManager.weight * n + 1) * m.pair + (SettingManager.weight * n - 1) * m.match;
				scr2 += (SettingManager.weight * n + 1) * (m.pair + m.pairAjust) + (SettingManager.weight * n - 1) * (m.match + m.matchAjust);
			}
			int scr = scr1 + scr2;
			if (scr < saveScr || saveScr == 0) {
				saveScr = scr;
				regDate = p.regDate;
			}
		}
		if (posPlayerOfRegDate(regDate))
			return true;
		else
			return false;
	}
	public static bool getPositionC(string iRegDatePosA, string iRegDatePosB, bool pairAllowFlg) {		// Cポジションになる選手をワークに移す
		int n = getPlayerCount ();

//		int priority = 0;
//		int game = 0;
		int saveScr = 0;
		string regDate = "";
		var q1 = GameList.Instance.playerList.Where (p => p.lockStat == 0 && p.placeStat == 24 && (p.regDate != iRegDatePosA && p.regDate != iRegDatePosB));
		foreach (_Player p in q1) {										// ABポジションの選手以外で選択可能な選手を対象とする
			if (!pairAllowFlg) {												// PositonDがLockされている場合、Pairの選手は対象から除外する
				if (PairManager.getPairRegDate(p.regDate) != null) 
					continue;
			}

			// score部分計算　P + 2NG
			int scr1 = p.idxPriority + 2 * n * (p.game + p.gameAjust);
			int scr2 = 0;
			int scr3 = 0;
			var q2 = GameList.Instance.matchList.Where (ma => ma.youRegDate == iRegDatePosA && ma.myRegDate == p.regDate);
			foreach (_Match ma in q2) {		// A と p(A,B以外) のpair,match履歴
				// score部分計算　(QN+1)(Aq)+(QN-1)(Ar)
				scr2 += (SettingManager.weight * n + 1) * (ma.pair + ma.pairAjust) + (SettingManager.weight * n - 1) * (ma.match + ma.matchAjust);
			}
			var q3 = GameList.Instance.matchList.Where (mb => mb.youRegDate == iRegDatePosB && mb.myRegDate == p.regDate);
			foreach (_Match mb in q3) {		// B と p(A,B以外) のpair,match履歴
				// score部分計算　(QN+1)(Bq)+(QN-1)(Br)
				scr3 += (SettingManager.weight * n + 1) * (mb.pair + mb.pairAjust) + (SettingManager.weight * n - 1) * (mb.match + mb.matchAjust);
			}
			int scr = scr1 + scr2 + scr3;
			if (scr < saveScr || saveScr == 0) {
				saveScr = scr;
				regDate = p.regDate; 
			}
		}
		if (posPlayerOfRegDate(regDate))
			return true;
		else
			return false;
	}
	public static bool getPositionD(string iRegDatePosA, string iRegDatePosB, string iRegDatePosC, bool pairAllowFlg) {		// Dポジションになる選手をワークに移す
		int n = getPlayerCount ();

//		int priority = 0;
//		int game = 0;
		int saveScr = 0;
		string regDate = "";
		var q1 = GameList.Instance.playerList.Where (p => p.lockStat == 0 && p.placeStat == 24 && (p.regDate != iRegDatePosA && p.regDate != iRegDatePosB && p.regDate != iRegDatePosC));
		foreach (_Player p in q1) {				// ABCポジションの選手以外で選択可能な選手を対象とする
			if (!pairAllowFlg) {								// PositonCがLockされている場合、Pairの選手は対象から除外する
				if (PairManager.getPairRegDate(p.regDate) != null) 
					continue;
			}
			// score部分計算　P + 2NG
			int scr1 = p.idxPriority + 2 * n * (p.game + p.gameAjust);
			int scr2 = 0;
			int scr3 = 0;
			int scr4 = 0;
			var q2 = GameList.Instance.matchList.Where (ma => ma.youRegDate == iRegDatePosA && ma.myRegDate == p.regDate);
			foreach (_Match ma in q2) {		// A と p(A,B,C以外) のpair,match履歴
				// score部分計算　(QN+1)(Aq)+(QN-1)(Ar)
				scr2 += (SettingManager.weight * n + 1) * (ma.pair + ma.pairAjust) + (SettingManager.weight * n - 1) * (ma.match + ma.matchAjust);
			}
			var q3 = GameList.Instance.matchList.Where (mb => mb.youRegDate == iRegDatePosB && mb.myRegDate == p.regDate);
			foreach (_Match mb in q3) {		// B と p(A,B,C以外) のpair,match履歴
				// score部分計算　(QN+1)(Bq)+(QN-1)(Br)
				scr3 += (SettingManager.weight * n + 1) * (mb.pair + mb.pairAjust) + (SettingManager.weight * n - 1) * (mb.match + mb.matchAjust);
			}
			var q4 = GameList.Instance.matchList.Where (mc => mc.youRegDate == iRegDatePosC && mc.myRegDate  == p.regDate);
			foreach (_Match mc in q4) {		// C と p(A,B,C以外) のpair,match履歴
				// score部分計算　(QN+1)(Cq)+(QN-1)(Cr)
				scr4 += (SettingManager.weight * n + 1) * (mc.pair + mc.pairAjust) + (SettingManager.weight * n - 1) * (mc.match + mc.matchAjust);
			}
			int scr = scr1 + scr2 + scr3 + scr4;
			if (scr < saveScr || saveScr == 0) {
				saveScr = scr;
				regDate = p.regDate; 
			}
		}
		if (posPlayerOfRegDate(regDate))
			return true;
		else
			return false;
	}


	// -- function --
	public static void initPlayer() {								// ワークの初期化生成
		_player = new _Player ();
		_match = new _Match ();
	}
	public static void initCourt(int courtNum) {			// コートの数だけコートロックを管理する
		CourtList.Instance.coutList.Clear ();
		for (int cnt = 0; cnt < courtNum; cnt++) {
			CourtList.Instance.coutList.Add(new int());
		}
	}
	public static void setPlayerToPlace(string iRegDate, Transform tsPlayer, Transform tsPlace) {
		var qry = GameList.Instance.playerList.Where(p => p.regDate == iRegDate && p.lockStat == 0  && p.placeStat != PLACE_BREAK);
		foreach (_Player p in qry) {
			tsPlayer.SetParent (tsPlace, false);  
			p.placeStat = PLACE_WAIT;
		}
	}
	public static void setPlaceStat(string iRegDate, int iPlaceStat) {  // 指定選手の場所を、指定場所に設定する (iMachup)
        if (iRegDate == "")
            return;

        var q = GameList.Instance.playerList.Where(p => p.regDate == iRegDate);
		foreach (_Player p in q) {
			p.placeStat = iPlaceStat;
		}
	}
	public static int getPlayerCount() {							// ゲームに参加している人数を返す
		return GameList.Instance.playerList.Count;
	}
	public static int getWaitPlayerCount() {					// 待機中(WAIT)の人数を返す
		int cnt = 0;
		var q = GameList.Instance.playerList.Where(p => p.placeStat == PLACE_WAIT);
		foreach (_Player p in q) {
			cnt++;
		}
		return cnt;
	}
	public static int getBreakPlayerCount() {				// 休憩中(BREAK)の人数を返す
		int cnt = 0;
		var q = GameList.Instance.playerList.Where(p => p.placeStat == PLACE_BREAK);
		foreach (_Player p in q) {
			cnt++;
		}
		return cnt;
	}
	public static bool posPlayerOfListIndex(int listIdx) {			// 指定ListIndexの選手のplayList参照をワークに移す
		if (listIdx < GameList.Instance.playerList.Count) {
			_player = GameList.Instance.playerList[listIdx];
			_trsPlayer = TrsPlayer.Instance.trsPlayerList[listIdx];
			return true;
		} else {
			Debug.Log ("Error：Over PlayerCount");
			return false;
		}
	}
	public static bool posPlayerOfRegDate(string iRegDate) {    // 指定RegDateの選手のplayList参照をワークに移す
		bool result = true;
		var qry = GameList.Instance.playerList.Where(p => p.regDate == iRegDate);
		if (qry.Count() != 0) {
			foreach (_Player p in qry)
				_player = p;
		} else {
			result = false;
		}
		var qt = TrsPlayer.Instance.trsPlayerList.Where(p => p.regDate == iRegDate);
		if (qt.Count() != 0) {
			foreach (_TrsPlayer t in qt)
				_trsPlayer = t;
		}
		return result;
	}
	// List更新系
    /*
	public static void addPlayer(string iRegDate, Transform trsPrefab = null) {			// 新規の選手情報のList追加と同時に対戦リストも追加
		MemberManager.posMemberOfRegDate (iRegDate);
		GameManager.initPlayer ();
		GameManager.regDate = MemberManager.regDate;
		GameManager.nameKaji_family = MemberManager.nameKaji_family;
		GameManager.nameKaji_first = MemberManager.nameKaji_first;
		GameManager.nameKana_family = MemberManager.nameKana_family;
		GameManager.nameKana_first = MemberManager.nameKana_first;
		GameManager.gender = MemberManager.gender;
		GameManager.idxPriority = MemberManager.idxRec + 1;
		GameManager.placeStat = GameManager.PLACE_WAIT;
		GameManager.flgDelete = 0;

		DateTime dtMem = DateTime.Parse (MemberManager.saveDate);
		DateTime dtGam = DateTime.Parse (GameManager.saveDate);
		if (dtMem > dtGam) {
			int gameAjt = 0;		// 試合途中参加のGame数補正
			if (GameList.Instance.playerList.Count != 0 && SettingManager.joinInit == 1)	{					// 既存選手のgame数+補正の最小値に合わせる
				gameAjt = GameList.Instance.playerList.Select (p => p.game+p.gameAjust).Min ();		// 連続追加でgame数が減少しつづけるため最小-1はしない
			}
			GameManager.gameAjust = gameAjt;
		}
		GameList.Instance.playerList.Add (_player);

		List<_Match> ml = new List<_Match>();
		_Match m0 = new _Match ();
		m0.myRegDate = _player.regDate;		//最初の自分対自分は2人目でデータ作成時にRegDateを取り出すためだけに必要 
		m0.youRegDate = _player.regDate;
		ml.Add (m0);

		_TrsPlayer ts = new _TrsPlayer ();
		ts.regDate = MemberManager.regDate;
		ts.trsPlayer = trsPrefab;
		TrsPlayer.Instance.trsPlayerList.Add (ts);

		var q = GameList.Instance.matchList.Select(m => m.myRegDate).Distinct();
		foreach (string qRegDate in q)  {
			_Match m1 = new _Match ();
			m1.myRegDate = _player.regDate;
			m1.youRegDate = qRegDate;
			ml.Add (m1);
			_Match m2 = new _Match ();
			m2.myRegDate = qRegDate;
			m2.youRegDate = _player.regDate;
			ml.Add (m2);
		}

		foreach (_Match m in ml) {
			GameList.Instance.matchList.Add (m);
		}
	}
    */
    public static void addPlayer(string iRegDate, bool flgAdd) {         // 新規の選手情報のList追加と同時に対戦リストも追加 (iMatchup専用)
        MemberManager.posMemberOfRegDate(iRegDate);
        GameManager.initPlayer();
        GameManager.regDate = MemberManager.regDate;
        GameManager.nameKaji_family = MemberManager.nameKaji_family;
        GameManager.nameKaji_first = MemberManager.nameKaji_first;
        GameManager.nameKana_family = MemberManager.nameKana_family;
        GameManager.nameKana_first = MemberManager.nameKana_first;
        GameManager.gender = MemberManager.gender;
        GameManager.idxPriority = MemberManager.idxRec + 1;
        GameManager.placeStat = GameManager.PLACE_WAIT;
        GameManager.flgDelete = 0;

        if (flgAdd) {
            int gameAjt = 0;        // 試合途中参加のGame数補正
            if (GameList.Instance.playerList.Count != 0 && SettingManager.joinInit == 1) {                  // 既存選手のgame数+補正の最小値に合わせる
                gameAjt = GameList.Instance.playerList.Select(p => p.game + p.gameAjust).Min() - 1;     // 連続追加でgame数から最小-1する
            }
            GameManager.gameAjust = gameAjt;
        }
        GameList.Instance.playerList.Add(_player);

        List<_Match> ml = new List<_Match>();
        _Match m0 = new _Match();
        m0.myRegDate = _player.regDate;     //最初の自分対自分は2人目でデータ作成時にRegDateを取り出すためだけに必要 
        m0.youRegDate = _player.regDate;
        ml.Add(m0);

        var q = GameList.Instance.matchList.Select(m => m.myRegDate).Distinct();
        foreach (string qRegDate in q) {
            _Match m1 = new _Match();
            m1.myRegDate = _player.regDate;
            m1.youRegDate = qRegDate;
            ml.Add(m1);
            _Match m2 = new _Match();
            m2.myRegDate = qRegDate;
            m2.youRegDate = _player.regDate;
            ml.Add(m2);
        }

        foreach (_Match m in ml) {
            GameList.Instance.matchList.Add(m);
        }
    }
    //	private static void deleteUnmatchPlayerOfMatchList() {		// ActiveでないPlayerをPlayerListとMatchListから削除　結果として ActiveMemberList >= PlayerList,MatchList となる
    //		for (int listIdx=0; listIdx<GameManager.getPlayerCount(); listIdx++) {
    //			string regDate = GameList.Instance.playerList [listIdx].regDate;
    //			if (!MemberManager.existActiveMemberOfRegDate (regDate)) {
    //				GameList.Instance.playerList.RemoveAll( p => p.regDate == regDate);
    //				GameList.Instance.matchList.RemoveAll( p => p.myRegDate == regDate);
    //			}
    //		}
    //	}
    public static bool reassginPlayerToStockPlate(string iRegDate) {			// ActivMember(メンバー選択)に存在しないplayerをリストから削除し、存在する場合は情報更新する
		if (!MemberManager.existActiveMemberOfRegDate (iRegDate)) {
			GameList.Instance.playerList.RemoveAll( p => p.regDate == regDate);
			GameList.Instance.matchList.RemoveAll( p => p.myRegDate == regDate);
			return false;
		} else {
			MemberManager.posMemberOfRegDate (iRegDate);
			GameManager.posPlayerOfRegDate (iRegDate);			// 既存選手の枠を作業ワークに設定
			GameManager.nameKaji_family = MemberManager.nameKaji_family;
			GameManager.nameKaji_first = MemberManager.nameKaji_first;
			GameManager.gender = MemberManager.gender;
			GameManager.idxPriority = MemberManager.idxRec + 1;
			return true;
		}
	}
	public static int  GetPlayerCountInCourt(int courtNum) {				// 指定コート番号 0 ~ 5 に配置されている選手の数を組合せを表す形式で返す 10の位:左人数 1の位:右人数
		int iCourtPlayerFrom = courtNum * 4;  			// コート上のAポジションのシーケンシャル番号
		int iPlayerCount = 0;
		var q1 = GameList.Instance.playerList.Where (p => p.placeStat >=iCourtPlayerFrom && p.placeStat <=  iCourtPlayerFrom + 1);
		foreach (_Player p in q1)
			iPlayerCount += 10;
		var q2 = GameList.Instance.playerList.Where (p => p.placeStat >=iCourtPlayerFrom  +  2 && p.placeStat <=  iCourtPlayerFrom + 3);
		foreach (_Player p in q2)
			iPlayerCount++;
		return iPlayerCount;
	}
	public static void  createTransformFromPlayerDate() {		// transform がFile読み込みで作成されないため、playerList との整合性を取るため再作成する
		TrsPlayer.Instance.trsPlayerList.Clear ();
		foreach (_Player p in GameList.Instance.playerList) {
			_TrsPlayer trs = new _TrsPlayer();
			trs.regDate = p.regDate;
			TrsPlayer.Instance.trsPlayerList.Add(trs);
		}
	}
	public static Transform  getTransformOfRegDate(string  iRegDate) {		// 指定選手のTransformを返す
		var q = TrsPlayer.Instance.trsPlayerList.Where (p => p.regDate == iRegDate);
		foreach (_TrsPlayer trs in q)
			return trs.trsPlayer;
		return null;
	}
	public static void  setTransformOfRegDate(string  iRegDate, Transform iTrs) {		// 指定選手のTransformを更新する
		var q = TrsPlayer.Instance.trsPlayerList.Where (p => p.regDate == iRegDate);
		foreach (_TrsPlayer trs in q)
			trs.trsPlayer = iTrs;
	}

	public static int  getPlayerPlace(string  iRegDate) {									// 選手の配置されている場所を返す
		var q = GameList.Instance.playerList.Where (p => p.regDate == iRegDate);
		foreach (_Player p in q)
			return p.placeStat;
		return -1;
	}
	public static string  getPlaceOfPlayer(int  iPlace) {									// コートの指定場所に配置されている選手のRegDateを返す  
		var q = GameList.Instance.playerList.Where (p => p.placeStat == iPlace);
		foreach (_Player p in q)
			return p.regDate;
		return null;
	}
	public static int getCourtOfRegdate(string iRegDate) {					// 指定選手(RegDate)のいる 0-5:コート番号  -1:コートにいない を返す
		int result = -1;
		var q = GameList.Instance.playerList.Where (p => p.regDate == iRegDate);
		foreach (_Player p in q) {
			if (p.placeStat == GameManager.PLACE_WAIT || p.placeStat == GameManager.PLACE_BREAK)
				result =  -1;
			else 
				result = p.placeStat / 4;
		}
		return result; 
	}
	public static int  getPlayerNumOfPlaceNo(int  iPlace) {					// 指定コート(0~5)に配置されている選手の数を返す
		int iPlayerNum = 0;
		int iPlaceStartNo = iPlace * 4;
		int iPlaceEndNo = iPlaceStartNo + 3;
		var q = GameList.Instance.playerList.Where (p => p.placeStat >= iPlaceStartNo && p.placeStat <= iPlaceEndNo);
		foreach (_Player p in q) {
			iPlayerNum++;
		}
		return iPlayerNum;
	}
	public static int  chkLock(string  iRegDate) {									// 選手のLock状態を返す
		var q = GameList.Instance.playerList.Where (p => p.regDate == iRegDate);
		foreach (_Player p in q)
			return p.lockStat;
		return -1;
	}
	public static void  setLock(string iRegDate, int  iLockStat) {			// 選手のLock状態を変更する
		var q = GameList.Instance.playerList.Where (p => p.regDate == iRegDate);
		foreach (_Player p in q)
			p.lockStat = iLockStat;
	}
/*
    public static void  setLock(int courtNum) {												// コート番号 0 ~ 5
		CourtList.Instance.coutList[courtNum] =  GameManager.LOCK;		// コートListに確定中(Lock)を代入
		int iCourtPlayerFrom = courtNum * 4;  												// コート上のAポジションのシーケンシャル番号
		var q = GameList.Instance.playerList.Where (p => p.placeStat >=iCourtPlayerFrom && p.placeStat <=  iCourtPlayerFrom + 3);
		foreach (_Player p in q) {
			p.lockStat = GameManager.LOCK;
			var qt = TrsPlayer.Instance.trsPlayerList.Where (t => t.regDate == p.regDate);
			foreach (_TrsPlayer ts in qt)
				ts.trsPlayer.FindChild ("Lock").gameObject.SetActive (true);
		}
	}
*/
    public static void setLock(int courtNum) {                                              // コート番号 0 ~ 5      (iMatchup)
        CourtList.Instance.coutList[courtNum] = GameManager.LOCK;       // コートListに確定中(Lock)を代入
    }
    /*
        public static void  freeLock(int courtNum) {											// コート番号 0 ~ 5
            CourtList.Instance.matchStartDateList[courtNum] = "";					// コート試合時間クリア
            CourtList.Instance.coutList[courtNum] =  GameManager.UNLOCK;	// コートListに(解除中)Lockを代入
            int iCourtPlayerFrom = courtNum * 4;  												// コート上のAポジションのシーケンシャル番号
            var q = GameList.Instance.playerList.Where (p => p.placeStat >=iCourtPlayerFrom && p.placeStat <=  iCourtPlayerFrom + 3);
            foreach (_Player p in q) {
                p.lockStat = GameManager.UNLOCK;
                var qt = TrsPlayer.Instance.trsPlayerList.Where (t => t.regDate == p.regDate);
                foreach (_TrsPlayer ts in qt)
                    ts.trsPlayer.FindChild ("Lock").gameObject.SetActive (false);
            }
        }
    */
    public static void freeLock(int courtNum) {                                         // コート番号 0 ~ 5      (iMatchup)
        CourtList.Instance.matchStartDateList[courtNum] = "";                   // コート試合時間クリア
        CourtList.Instance.coutList[courtNum] = GameManager.UNLOCK; // コートListに(解除中)Lockを代入
    }
    public static void  setStartCourt(int courtNum) {									// コート番号 0 ~ 5
		CourtList.Instance.coutList[courtNum] =  GameManager.START;		// コートListに(開始中)STARTを代入
		CourtList.Instance.matchStartDateList[courtNum] = DateTime.Now.ToString ("s", new System.Globalization.CultureInfo ("ja-JP"));	// コート試合時間
	}
	public static int chkCourtLockOfCortnum(int iCourtNum) {	// コート番号は 0~5 で受けとりLock状態(free:0 lock:1or2)を返す   0:解除中  1:確定中   2:開始中
		if (CourtList.Instance.coutList.Count > iCourtNum)
			return CourtList.Instance.coutList [iCourtNum];
		else
			return -1;
	}
	public static string getMatchStartDateOfCortnum(int iCourtNum) {	// コート番号は 0~5 で受けとりコート開始時間を返す
		if (CourtList.Instance.coutList.Count > iCourtNum)
			return CourtList.Instance.matchStartDateList [iCourtNum];
		else
			return null;
	}
	public static int chkCourtLockOfPosition(int iPositionNum) {	// ポジションは 0~23 で受けとりコートLock状態(free:0 lock:1or2)を返す   0:解除中  1:確定中   2:開始中
		return chkCourtLockOfCortnum (iPositionNum / 4);
	}
	public static int chkLockOfPosition(int iPositionNum) {	// ポジションは 0~23 で受けとりポジションのLock状態(free:0 lock:1)を返す
		int result = 0;
		var q = GameList.Instance.playerList.Where (p => p.placeStat == iPositionNum);
		foreach (_Player p in q) 
			result = p.lockStat;
		return result;
	}


	// ファイル再読み込み時の反映削除系
	public static void  setDeleteFlgAllPlayer() {			// 削除フラグがある選手をPlayer,Matach から削除
		foreach (_Player p in GameList.Instance.playerList)
			p.flgDelete = 1;
	}
	public static void  deletePlayerByFlg(Transform trsStock) {			// 削除フラグがある選手をPlayer,Matach から削除
		var q = GameList.Instance.playerList.Where (p => p.flgDelete == 1);
		foreach (_Player p in q) {
			// 選手を消す前にSTOCKに戻す
			posPlayerOfRegDate (p.regDate);
			GameManager.trsPlayer.SetParent (trsStock, false);
			GameList.Instance.matchList.RemoveAll (m => m.myRegDate == p.regDate || m.youRegDate == p.regDate);
			TrsPlayer.Instance.trsPlayerList.RemoveAll (m => m.regDate == p.regDate);
		}
		GameList.Instance.playerList.RemoveAll (p => p.flgDelete == 1);
	}

    public static void removeAllPlayer() {          // 全選手を削除 (iMatachup)
        GameList.Instance.matchList.Clear();
        GameList.Instance.playerList.Clear();
    }
    public static void removePlayerByRegdate(string iRegdate) {          // Regdate指定の選手を削除 (iMatachup)
        GameList.Instance.matchList.RemoveAll(m => m.myRegDate == iRegdate || m.youRegDate == iRegdate);
        GameList.Instance.playerList.RemoveAll(p => p.regDate == iRegdate);
    }


    public static void  updatePlayer() {			// 削除フラグがある選手をPlayer,Matach から削除
		var q = GameList.Instance.playerList.Where (p => p.regDate == _player.regDate);
		foreach (_Player p in q) {
			p.nameKaji_family =  _player.nameKaji_family;	
			p.nameKaji_first =  _player.nameKaji_first;
			p.nameKana_family =  _player.nameKana_family;
			p.nameKana_first =  _player.nameKana_first;
			p.gender =  _player.gender;
			p.idxPriority =  _player.idxPriority;
			p.flgDelete =  _player.flgDelete;
		}
	}


	// Result系 対戦履歴更新
	public static void countUpPlayerGameOfPlaceNo(string iRegDate) {        // 指定選手のGame数を1増やす(集計有無を問わず)
		var qry = GameList.Instance.playerList.Where(p => p.regDate == iRegDate);
		foreach (_Player p in qry) {
			MemberManager.countUpPlayerGameOfPlaceNo(iRegDate); // 同様にMemberManagerのGame数を1増やす(永続的記録)
			++p.game;
		}
	}
	public static void countUpPlayerWinLoseOfPlaceNo(string iRegDate, int iWinLose) {   // 指定選手の勝敗数を1増やす(集計時のみの記録)
		var qry = GameList.Instance.playerList.Where(p => p.regDate == iRegDate);
		foreach (_Player p in qry) {
			MemberManager.countUpPlayerWinLoseOfPlaceNo(iRegDate, iWinLose);    // 同様にMemberManagerの勝敗数を1増やす(永続的記録)
			if (iWinLose == GameManager.GAMEWIN)
				++p.win;
			else if (iWinLose == GameManager.GAMELOSE)
				++p.lose;
			else
				++p.draw;
		}
	}
	public static void setResltPairPlus(string myRegDate, string youRegDate) {        // 指定Pair両者のPair数をカウントアップする
		var q1 = GameList.Instance.matchList.Where (p => p.myRegDate == myRegDate && p.youRegDate == youRegDate);
		foreach (_Match m in q1)
			m.pair++;	
		var q2 = GameList.Instance.matchList.Where (p => p.myRegDate == youRegDate && p.youRegDate == myRegDate);
		foreach (_Match m in q2)
			m.pair++;
	}
	public static void setResltMatchPlus(string myRegDate, string youRegDate) {        // 指定両者のMatch数をカウントアップする
		var q1 = GameList.Instance.matchList.Where (p => p.myRegDate == myRegDate && p.youRegDate == youRegDate);
		foreach (_Match m in q1)
			m.match++;	
		var q2 = GameList.Instance.matchList.Where (p => p.myRegDate == youRegDate && p.youRegDate == myRegDate);
		foreach (_Match m in q2)
			m.match++;
	}
	public static void clearAllResultPairAndMatch() {		// 組合せ終了時に、Piar,Match履歴情報をクリアする
		foreach (_Player p in GameList.Instance.playerList) {
			p.game = 0;
			p.gameAjust = 0;
		}
		foreach (_Match m in GameList.Instance.matchList) {
			m.pair = 0;
			m.pairAjust = 0;
			m.match = 0;
			m.matchAjust = 0;
		}
	}
	public static void setGameAjust(string iRegDate, int iAjust) {         // Game補正値を元にPari,Matchの補正値を割り振る
		int preGameAjust = GameManager.gameAjust;
		GameManager.gameAjust = iAjust;
		setGameAjustMinus(iRegDate, preGameAjust);
		setGameAjustPlus(iRegDate, iAjust);
	}
	private static void setGameAjustPlus(string iRegDate, int iAjust) {         // プラスのGame補正値
		int playerNum = getPlayerCount();
		for (int gameCnt=0; gameCnt < iAjust * SettingManager.form; gameCnt++) {
			posPlayerOfListIndex(gameCnt);
			string youRegDate = GameManager.regDate;
			if (youRegDate == iRegDate) {
//				gameCnt--;
				continue;
			}

//			for (int fmCnt=0; fmCnt < SettingManager.form; fmCnt++) {   // 試合換算のループ回数は、ダブルスなら2回、シングルスなら1回 (1試合での相手の数)
				var qm1 = GameList.Instance.matchList.Where(p => p.myRegDate == iRegDate && p.youRegDate == youRegDate);
				foreach (_Match m in qm1)
					m.matchAjust++;
				var qm2 = GameList.Instance.matchList.Where(p => p.myRegDate == youRegDate && p.youRegDate == iRegDate);
				foreach (_Match m in qm2)
					m.matchAjust++;

				if (gameCnt%2 == 1) {     // ダブルスなら偶数回にPair数をカウントアップ
					var qp1 = GameList.Instance.matchList.Where(p => p.myRegDate == iRegDate && p.youRegDate == youRegDate);
					foreach (_Match m in qp1)
						m.pairAjust++;
					var qp2 = GameList.Instance.matchList.Where(p => p.myRegDate == youRegDate && p.youRegDate == iRegDate);
					foreach (_Match m in qp2)
						m.pairAjust++;
				}
//			}
		}
	}
	private static void setGameAjustMinus(string iRegDate, int iAjust) {         // プラスのGame補正値
		int playerNum = getPlayerCount();
		for (int gameCnt = 0; gameCnt < iAjust*(-1); gameCnt++) {
			posPlayerOfListIndex(gameCnt);
			string youRegDate = GameManager.regDate;
			if (youRegDate == iRegDate) {
//				gameCnt--;
				continue;
			}

//			for (int fmCnt = 0; fmCnt < SettingManager.form; fmCnt++) {   // 試合換算のループ回数は、ダブルスなら2回、シングルスなら1回 (1試合での相手の数)
				var qm1 = GameList.Instance.matchList.Where(p => p.myRegDate == iRegDate && p.youRegDate == youRegDate);
				foreach (_Match m in qm1)
					m.matchAjust--;
				var qm2 = GameList.Instance.matchList.Where(p => p.myRegDate == youRegDate && p.youRegDate == iRegDate);
				foreach (_Match m in qm2)
					m.matchAjust--;

				if (gameCnt%2 == 1) {     // ダブルスなら偶数回にPair数をカウントダウン
					var qp1 = GameList.Instance.matchList.Where(p => p.myRegDate == iRegDate && p.youRegDate == youRegDate);
					foreach (_Match m in qp1)
						m.pairAjust--;
					var qp2 = GameList.Instance.matchList.Where(p => p.myRegDate == youRegDate && p.youRegDate == iRegDate);
					foreach (_Match m in qp2)
						m.pairAjust--;
				}
//			}
		}
	}

    //*********  iMatchup専用 **********
    public static void setPlaceBreak(string iRegDate) {  // 指定選手を休憩中にする (iMatchupのパス)
        freePlaceBreak();   // 休憩中の選手は一人しかいないようするため、一旦休憩中の選手を待機中に戻す
        var q = GameList.Instance.playerList.Where(p => p.regDate == iRegDate);
        foreach (_Player p in q) {
            p.placeStat = PLACE_BREAK;
        }
    }
    public static void freePlaceBreak() {  // 休憩中の選手を待機中にする (iMatchupのパス解除)
        var q = GameList.Instance.playerList.Where(p => p.placeStat == PLACE_BREAK);
        foreach (_Player p in q) {
            p.placeStat = PLACE_WAIT;
        }
    }


    public static void Load() {
		GameList.Load ();
		CourtList.Load ();
	}
	public static void Save() {
		string dt = DateTime.Now.ToString ("s", new System.Globalization.CultureInfo("ja-JP"));
		GameList.Instance.saveDate = dt;
		GameList.Save ();
		CourtList.Instance.saveDate = dt;
		if (CourtList.Instance.coutList.Count() == 0)
			CourtList.Instance.coutList.Add(new int());
		CourtList.Save ();
	}
	public static void Reset() {
		GameList.Reset ();
		CourtList.Reset ();
	}
	void Start () {	// Singletonではここは通らない
		initPlayer ();
		new TrsPlayer ();		//  Transformを含み保存できない
		new GameList ();
		new CourtList ();
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
			GameList.Save ();
	}
}
