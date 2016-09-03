using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScriptDebugView : MonoBehaviour {
	ViewManager viewManager;
	Text debugTxt;

	public void updateView1() {	// for mainView
		debugTxt.text = "";
		for (int i = 0; i < MemberManager.getMemberCount (); i++) {
			MemberManager.posMember (i);
			debugTxt.text = debugTxt.text + "[" + i + "] idx:" + MemberManager.idxRec + " Act:" + MemberManager.activeStat + " N:" + MemberManager.nameKana_family + MemberManager.nameKana_first + " :" + MemberManager.nameKaji_family + MemberManager.nameKaji_first + " Gen:" + MemberManager.gender + " Reg:" + MemberManager.regDate + " Par:" + MemberManager.pairDate + "\n";
		}
	}
	public void updateView2() {	// for pairView
		debugTxt.text = "";
		for (int i = 0; i < PairManager.getPairCount (); i++) {
			PairManager.posPair (i);
			debugTxt.text = debugTxt.text + "[" + i + "] [L]reg:" + PairManager.pairRegDateL + " N:" + PairManager.pairNameKnji_familyL + PairManager.pairNameKnji_firstL + " Gen:" + PairManager.pairGenderL + " -- [R]reg:" + PairManager.pairRegDateR + " N:" + PairManager.pairNameKnji_familyR + PairManager.pairNameKnji_firstR + " Gen:" + PairManager.pairGenderR + "\n";
		}
	}
	public void updateView3() {	//Regiter
		List<List<string>> ml = new List<List<string>>();
		ml.Add(new List<string> () {"A004","松岡","修造","まつおか","しゅうぞう","0","2","matuoka@arifacture.jp","食いしん坊！万才","20160101T00:00:01"});
		ml.Add(new List<string> () {"B002","森田","あゆみ","もりた","あゆみ","1","1","morita@arifacture.jp","WTAツアーダブルスで準優勝2回","20160101T00:00:02"});
		ml.Add(new List<string> () {"C005","ダニエル","太郎","だにえる","たろう","0","1","dany@arifacture.jp","ニューヨーク出身　エイブル所属","20160101T00:00:03"});
		ml.Add(new List<string> () {"C004","奈良","くるみ","なら","くるみ","1","7","nara@arifacture.jp","WTAランキング シングル32位 ダブルス145位\n安藤証券所属","20160101T00:00:04"});
		ml.Add(new List<string> () {"B004","杉山","愛","すぎやま","あい","1","2","sugiyama@arifacture.jp","WTAランキング シングルス8位 ダブルス1位","20160101T00:00:05"});
		ml.Add(new List<string> () {"A005","神和住","純","かみわずみ","じゅん","0","3","kamiwa@arifacture.jp","法政大学スポーツ健康科学部教授","20160101T00:00:06"});
		ml.Add(new List<string> () {"A002","伊藤","竜馬","いとう","たつま","0","3","ito@arifacture.jp","バックハンド・ストロークは両手打ち","20160101T00:00:07"});
		ml.Add(new List<string> () {"B001","土井","美咲","どい","みさき","1","4","doi@arifacture.jp","ミキハウス所属","20160101T00:00:08"});
		ml.Add(new List<string> () {"D002","西岡","良仁","にしおか","よしひと","0","4","nishioka@arifacture.jp","ヨネックス所属","20160101T00:00:09"});
		ml.Add(new List<string> () {"B003","熊谷","一弥","くまがい","いちや","0","5","kumagai@arifacture.jp","旧漢字では「熊谷一彌」","20160101T00:00:10"});
		ml.Add(new List<string> () {"A003","神尾","米","かみお","よね","1","5","soeda@arifacture.jp","著書「ベストフォームでレベルアップ！テニス」","20160101T00:00:11"});
		ml.Add(new List<string> () {"C003","佐藤","直子","さとう","なおこ","1","6","sato@arifacture.jp","前日本プロテニス協会理事長","20160101T00:00:12"});
		ml.Add(new List<string> () {"A001","錦織","圭","にしこり","けい","0","0","nishikori@arifacture.jp","「チケットの元はとれたかな」","20160101T00:00:013"});
		ml.Add(new List<string> () {"B005","石黒","修","いしぐろ","おさむ","0","6","ishiguro@arifacture.jp","日本プロテニス界のパイオニア\n俳優石黒賢の父親","20160101T00:00:14"});
		ml.Add(new List<string> () {"D001","クルム伊達","公子","くるむだて","きみこ","1","0","date@arifacture.jp","WTAランキング自己最高位：シングルス4位、ダブルス28位","20160101T00:00:15"});

		if (false) {			
			ml.Add(new List<string> () {"V001","夏目","漱石","なつめ","そうせき","0","7","natume@arifacture.jp","こころ","20160101T00:01:01"});
			ml.Add(new List<string> () {"V002","芥川","龍之介","あくたがわ","りゅうのすけ","0","7","akutagawa@arifacture.jp","羅生門","20160101T00:01:02"});
			ml.Add(new List<string> () {"V003","正岡","子規","まさおか","しき","0","7","masa@arifacture.jp","柿食へば 鐘が鳴るなり 法隆寺","20160101T00:01:03"});
			ml.Add(new List<string> () {"V004","森","鴎外","もり","おうがい","0","7","mishima@arifacture.jp","高瀬舟","20160101T00:01:04"});
			ml.Add(new List<string> () {"V005","三島","由紀夫","みしま","ゆきお","0","7","natume@arifacture.jp","金閣寺","20160101T00:01:05"});
			ml.Add(new List<string> () {"V006","泉","鏡花","いずみ","きょうか","0","0","izumi@arifacture.jp","草迷宮","20160101T00:01:06"});
			ml.Add(new List<string> () {"V007","川端","康成","かわばた","やすなり","0","7","natume@arifacture.jp","雪国","20160101T00:01:07"});
			ml.Add(new List<string> () {"V008","梶井","基次郎","かじい","きじろう","0","7","natume@arifacture.jp","檸檬","20160101T00:01:08"});
			ml.Add(new List<string> () {"V009","坂口","安吾","さかぐち","あんご","0","7","natume@arifacture.jp","堕落論","20160101T00:01:09"});
			ml.Add(new List<string> () {"V010","江戸川","乱歩","えどがわ","らんぽ","0","7","natume@arifacture.jp","怪人二十面相","20160101T00:01:10"});
			ml.Add(new List<string> () {"V011","国木田","独歩","くにきだ","どっぽ","0","7","natume@arifacture.jp","武蔵野","20160101T00:01:11"});
			ml.Add(new List<string> () {"V012","太宰","治","だざい","おさむ","0","7","natume@arifacture.jp","人間失格","20160101T00:01:12"});
			ml.Add(new List<string> () {"V013","宮沢","賢治","みやざわ","けんじ","0","7","natume@arifacture.jp","銀河鉄道の夜","20160101T00:01:13"});
			ml.Add(new List<string> () {"V014","二葉亭","四迷","ふたばてい","しめい","0","7","natume@arifacture.jp","浮雲","20160101T00:01:14"});
			ml.Add(new List<string> () {"V015","福沢","諭吉","ふくざわ","ゆきち","0","7","natume@arifacture.jp","学問のすすめ","20160101T00:01:15"});

			ml.Add(new List<string> () {"W001","樋口","一葉","ひぐち","いちよう","1","7","natume@arifacture.jp","たけくらべ","20160101T00:02:01"});
			ml.Add(new List<string> () {"W002","与謝野","晶子","よさの","あきこ","1","7","natume@arifacture.jp","歌集「みだれ髪」","20160101T00:02:02"});
			ml.Add(new List<string> () {"W003","北原","亜以子","きたはら","あいこ","1","7","natume@arifacture.jp","恋忘れ草","20160101T00:02:03"});
			ml.Add(new List<string> () {"W004","藤原","伊織","ふじわら","いおり","1","7","natume@arifacture.jp","テロリストのパラソル","20160101T00:02:04"});
			ml.Add(new List<string> () {"W005","中山","千夏","なかやま","ちなつ","1","7","natume@arifacture.jp","マルチタレント","20160101T00:02:05"});
			ml.Add(new List<string> () {"W006","小池","真理子","こいけ","まりこ","1","7","natume@arifacture.jp","妻の友達","20160101T00:02:06"});
			ml.Add(new List<string> () {"W007","中村","うさぎ","なかむら","うさぎ","1","7","natume@arifacture.jp","ゴクドーくん漫遊記","20160101T00:02:07"});
			ml.Add(new List<string> () {"W008","内田","春菊","うちだ","しゅんぎく","1","7","natume@arifacture.jp","ファザーファッカー","20160101T00:02:08"});
			ml.Add(new List<string> () {"W009","井上","由美子","いのうえ","ゆみこ","1","7","natume@arifacture.jp","北条時宗","20160101T00:02:09"});
			ml.Add(new List<string> () {"W010","俵","万智","たわら","まち","1","7","natume@arifacture.jp","サラダ記念","20160101T00:02:10"});
			ml.Add(new List<string> () {"W011","よしもと","ばなな","よしもと","ばなな","1","7","natume@arifacture.jp","キッチン","20160101T00:02:11"});
			ml.Add(new List<string> () {"W012","岩井子","志麻","いわい","しまこ","1","7","natume@arifacture.jp","チャイ・コイ","20160101T00:02:12"});
			ml.Add(new List<string> () {"W013","酒井","順子","さかい","じゅんこ","1","7","natume@arifacture.jp","負け犬の遠吠え","20160101T00:02:13"});
			ml.Add(new List<string> () {"W014","室井","佑月","むろい","ゆづき","1","7","natume@arifacture.jp","熱帯植物園","20160101T00:02:14"});
			ml.Add(new List<string> () {"W015","綿矢","りさ","わたや","りさ","1","7","natume@arifacture.jp","インストール","20160101T00:02:15"});

			ml.Add(new List<string> () {"Z001","松本","潤","まつもと","じゅん","0","7","natume@arifacture.jp","嵐","20160101T00:03:01"});
			ml.Add(new List<string> () {"Z002","二宮","和也","にのみや","かずなり","0","7","natume@arifacture.jp","嵐","20160101T00:03:02"});
			ml.Add(new List<string> () {"Z003","相葉","雅紀","あいば","まさき","0","7","natume@arifacture.jp","嵐","20160101T00:03:03"});
			ml.Add(new List<string> () {"Z004","櫻井","翔","さくらい","しょう","0","7","natume@arifacture.jp","嵐","20160101T00:03:04"});
			ml.Add(new List<string> () {"Z005","大野","智","おおの","さとし","0","7","natume@arifacture.jp","嵐","20160101T00:03:05"});
			ml.Add(new List<string> () {"Z011","横山","由依","よこやま","ゆい","1","7","natume@arifacture.jp","AKB48 チームA","20160101T00:04:01"});
			ml.Add(new List<string> () {"Z012","小嶋","菜月","こじま","なつき","1","7","natume@arifacture.jp","AKB48 チームA","20160101T00:04:02"});
			ml.Add(new List<string> () {"Z013","島崎","遥香","しまざき","はるか","1","7","natume@arifacture.jp","AKB48 チームA","20160101T00:04:03"});
			ml.Add(new List<string> () {"Z014","大和田","南那","おおわだ","なな","1","7","natume@arifacture.jp","AKB48 チームA","20160101T00:04:04"});
			ml.Add(new List<string> () {"Z015","佐々木","優佳里","ささき","ゆかり","1","7","natume@arifacture.jp","AKB48 チームA","20160101T00:04:05"});
		}
		 
		foreach (List<string> m in ml) {
			MemberManager.memberSerial = m[0];
			MemberManager.nameKaji_family = m[1];
			MemberManager.nameKaji_first = m[2];
			MemberManager.nameKana_family = m[3];
			MemberManager.nameKana_first = m[4];
			MemberManager.gender = int.Parse(m[5]);
			MemberManager.rank = int.Parse(m[6]);
			MemberManager.eMail = m[7];
			MemberManager.memo = m[8];
			MemberManager.regDate = m[9];
			MemberManager.activeStat = 1;
			MemberManager.addMember();
		}
		debugTxt.text = "";
		for (int i = 0; i < MemberManager.getMemberCount (); i++) {
			MemberManager.posMember (i);
			debugTxt.text = debugTxt.text + "[" + i + "] idx:" + MemberManager.idxRec + " Act:" + MemberManager.activeStat + " N:" + MemberManager.nameKana_family + MemberManager.nameKana_first + " :" + MemberManager.nameKaji_family + MemberManager.nameKaji_first + " Gen:" + MemberManager.gender + " Reg:" + MemberManager.regDate + " Par:" + MemberManager.pairDate + "\n";
		}
	}
	public void updateView4() {	// for GameView
		debugTxt.text = "";
		for (int i = 0; i < GameManager.getPlayerCount (); i++) {
			GameManager.posPlayerOfListIndex (i);
			debugTxt.text = debugTxt.text + "[" + i + "] p:" + GameManager.idxPriority + " Ct:"  + GameManager.placeStat + " L:" + GameManager.lockStat + " N:" + GameManager.nameKaji_family + GameManager.nameKaji_first  + " reg:" + GameManager.regDate + "\n";
		}
	}

	public void _btnOpen() {
		viewManager.opnDebugView (1);
	}
	public void _btnOpen2() {
		viewManager.opnDebugView (2);
	}
	public void _btnOpen3() {
		viewManager.opnDebugView (3);
	}
	public void _btnOpen4() {
		viewManager.opnDebugView (4);
	}
	public void _btnClose() {
		viewManager.closeDebugView ();
	}

	public void debugEnable(bool stat) {
		Transform[] tr;
		tr = GameObject.Find ("Canvas").transform.GetComponentsInChildren<Transform>(true);
		var query = tr.Where(p => p.Find("txtDEBUG"));

		foreach (Transform transform in query) {
			transform.gameObject.SetActive(stat);
		}
	}

	// Use this for initialization
	void Start () {
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager>();
		debugTxt = this.transform.Find ("Panel/ScrollView/Text").GetComponent<Text> ();
		this.gameObject.SetActive (false);
	}
}
