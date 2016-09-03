using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using NendUnityPlugin.AD;
using NendUnityPlugin.AD.Native;

public class ScriptExecStartView : MonoBehaviour {
	public GameObject myView;
	ViewManager viewManager;
	EventLongPress eventLP;
    NendAdNativeView nativeAdView;  // Nend広告 ネイティブバナー
    NendAdNative nativeAd;                  // Nend広告 ネイティブバナー
    public GameObject pfbMember;
    Transform tPfbParent;  // LayoutVertical(プレハブMemberの親)
    Transform tsfCourtList;
    Transform[] tsfCourt;
    Text txtTtaiki;
    Button btnAsignCourt;
    Button btnAddMember;
    bool flgAllowCalc = true; // 次の組合せコートでの計算許可(表示許可)フラグ

    // 組み合せ実行終了終了ボタン
    public void _callBackExecEnd() {
        // Lockされている試合コートを探す
        int empyCourt = -1;
        for (int cnt = 0; cnt < SettingManager.courtNum; cnt++) {
            if (GameManager.chkCourtLockOfCortnum(cnt) != 0) {
                empyCourt = cnt;
                break;
            }
        }
        if (empyCourt != -1) {  // Lock(1:試合コート配置済 2:試合中)された試合コートがある
            string title = "実行終了できません";
            string message = "試合中または試合開始前のコートがあります。\n試合組合せ処理実行を終了するためには、全てのコートで試合終了または試合取消を行ってください。";
            DialogViewController.Show(title, message, null);
        } else {
            viewManager.chgMainView(myView, viewManager.IN_RIGHT);
            if (!SettingManager.REGULAR_MODE) {
                NendAdInterstitial.Instance.Show();     // Nend広告 インタースティシャル表示
            }
        }
    }
    // 試合開始ボタン
    public void _callBackStart(Transform trfCourtListMember) {
        trfCourtListMember.FindChild("btnStop").gameObject.SetActive(false);
        trfCourtListMember.FindChild("btnEnd").gameObject.SetActive(true);
        trfCourtListMember.FindChild("btnStart").GetComponent<Button>().interactable = false;
        trfCourtListMember.FindChild("btnEnd").GetComponent<Button>().interactable = true;

        string strCourt = trfCourtListMember.name.Substring(trfCourtListMember.name.Length - 1);
        int iCourt = int.Parse(strCourt) - 1;
        GameManager.setStartCourt(iCourt); // コートをLock(START)に変更

        flgAllowCalc = true;   // 次の組合せコートでの計算許可
        calcPlayerInWork();
    }
    // 試合中止ボタン
    public void _callBackStop(Transform trfCourtListMember) {
        string title = "試合取消の確認";
        string message = "\n試合を取り消します。\n";
        DialogViewController.Show(title, message, new DialogViewOptions {
            btnCancelTitle = "キャンセル",
            btnCancelDelegate = () => {
                return;
            },
            btnOkTitle = "試合取消",
            btnOkDelegate = () => {
                flgAllowCalc = true;   // 次の組合せコートでの計算許可
                _crearCourt(trfCourtListMember);
            },
        });
    }
    public void _crearCourt(Transform trfCourtListMember) { // 試合コートをクリア
        trfCourtListMember.FindChild("btnStop").gameObject.SetActive(true);
        trfCourtListMember.FindChild("btnEnd").gameObject.SetActive(false);
        trfCourtListMember.FindChild("btnStart").GetComponent<Button>().interactable = false;
        trfCourtListMember.FindChild("btnStop").GetComponent<Button>().interactable = false;
//        btnAsignCourt.interactable = true;
//        btnAddMember.interactable = true;

        // MemberSetの選手の場所を次の組合せコート(WAIT)に設定
        for (int mcnt = 0; mcnt < 4; mcnt++) {
            string imem = "CourtBase/MemberSet/imgMember" + (mcnt + 1);
            string mRegDate = trfCourtListMember.FindChild(imem).GetComponent<ExecPlayer>().regDate;
            if (mRegDate != "")
                GameManager.setPlaceStat(mRegDate, GameManager.PLACE_WAIT);
        }
        string strCourt = trfCourtListMember.name.Substring(trfCourtListMember.name.Length - 1);
        int iCourt = int.Parse(strCourt) - 1;
        GameManager.freeLock(iCourt); // コートをLock解除

        // 試合コートから削除して、次の組合せを再計算
        Transform trfCourtBase = trfCourtListMember.FindChild("CourtBase");
        trfCourtBase.GetComponent<Image>().sprite = Resources.Load<Sprite>("court_LG");

        DestroyImmediate(trfCourtBase.Find("MemberSet").gameObject);
        calcPlayerInWork();
    }
    // 試合終了ボタン
    public void _callBackEnd(Transform trfCourtListMember) {
		if (SettingManager.sum == 0) {      // 集計なし
			string title = "試合終了の確認";
			string message = "\n試合を終了します。\n";
			DialogViewController.Show(title, message, new DialogViewOptions {
				btnCancelTitle = "キャンセル",
				btnCancelDelegate = () => {
					return;
				},
				btnOkTitle = "試合終了",
				btnOkDelegate = () => {
                    _End(trfCourtListMember);
				},
			});
			return;
		}
		else {      // 集計ありならResultViewで処理する
			viewManager.openResultView();
			// ここではResultDialogの結果が不明のためLock解除ができないので、openResultViewから呼ばれるafterResultDialog で処理する
		}
	}
    private void _End(Transform trfCourtListMember) {
        // game, match, pair のカウントアップ
        // MemberSetの選手の場所を次の組合せコート(WAIT)に設定
       string mRegDate1 = trfCourtListMember.FindChild("CourtBase/MemberSet/imgMember1").GetComponent<ExecPlayer>().regDate;
       string mRegDate2 = trfCourtListMember.FindChild("CourtBase/MemberSet/imgMember2").GetComponent<ExecPlayer>().regDate;
       string mRegDate3 = trfCourtListMember.FindChild("CourtBase/MemberSet/imgMember3").GetComponent<ExecPlayer>().regDate;
       string mRegDate4 = trfCourtListMember.FindChild("CourtBase/MemberSet/imgMember4").GetComponent<ExecPlayer>().regDate;

        if (SettingManager.form == SettingManager.FORM_DOUBLES) {
            GameManager.countUpPlayerGameOfPlaceNo(mRegDate1);
            GameManager.countUpPlayerGameOfPlaceNo(mRegDate2);
            GameManager.countUpPlayerGameOfPlaceNo(mRegDate3);
            GameManager.countUpPlayerGameOfPlaceNo(mRegDate4);
            GameManager.setResltPairPlus(mRegDate1, mRegDate2);
            GameManager.setResltPairPlus(mRegDate3, mRegDate4);
            GameManager.setResltMatchPlus(mRegDate1, mRegDate3);
            GameManager.setResltMatchPlus(mRegDate1, mRegDate4);
            GameManager.setResltMatchPlus(mRegDate2, mRegDate3);
            GameManager.setResltMatchPlus(mRegDate2, mRegDate4);
        } else {
            GameManager.countUpPlayerGameOfPlaceNo(mRegDate1);
            GameManager.countUpPlayerGameOfPlaceNo(mRegDate3);
            GameManager.setResltMatchPlus(mRegDate1, mRegDate3);
        }
        _crearCourt(trfCourtListMember);
        txtTtaiki.text = "待機選手: " + GameManager.getWaitPlayerCount() + "名";
    }

    // メンバー追加ボタン
    public void _callBackAddMember() {
        if (true) return;

        viewManager.chgExecSelectMemberView(myView, viewManager.IN_RIGHT);
	}

    // コート割当ボタン
    public void _callBackAsignCourt() {
        //        if (true) return;

        // workの全選手をDisableに設定
        dispDisableWorkAllPlayer();
        flgAllowCalc = false;   // 次の組合せコートでの計算不可

        // Lockされていない空きコートを探す
        int empyCourt = 0;
        for (int cnt=0; cnt < SettingManager.courtNum; cnt++) {
            if (GameManager.chkCourtLockOfCortnum(cnt) == 0) {
                empyCourt = cnt;
                break;
            }
        }

        // MemberSetの選手の場所を試合コートに設定
        for (int mcnt = 0; mcnt < 4; mcnt++) {
            string imem = "ActiveMemberListPanel/Work/CourtBase/MemberSet/imgMember" + (mcnt + 1);
            string mRegDate = this.transform.FindChild(imem).GetComponent<ExecPlayer>().regDate;
            if (mRegDate != "")
                GameManager.setPlaceStat(mRegDate, empyCourt);
        }
        GameManager.setLock(empyCourt); // コートをLock
        //       tPfbParent.GetComponent<Image>().sprite = Resources.Load<Sprite>("court_LG");
        // Listを割当コートにMemberSetを移動
        Transform court = tsfCourt[empyCourt].FindChild("CourtBase");
        court.GetComponent<Image>().sprite = Resources.Load<Sprite>("court_G");
        Transform mset = this.transform.FindChild("ActiveMemberListPanel/Work/CourtBase/MemberSet");
        mset.SetParent(court);
        mset.position = court.position;

        // Listを割当コートまでスクロール   Listの縦幅は1コート:360
        int dispAjust = 0;
        if (empyCourt == SettingManager.courtNum - 1 && SettingManager.courtNum > 1)
            dispAjust = 1;
        float posList = 360 * (empyCourt - dispAjust);
        tsfCourtList.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, posList);

        // 試合コートの状態設定
        tsfCourt[empyCourt].FindChild("btnStart").GetComponent<Button>().interactable = true;
        tsfCourt[empyCourt].FindChild("btnStop").GetComponent<Button>().interactable = true;
        // 次の組合せエリアの状態設定
        btnAsignCourt.interactable = false;
        btnAddMember.interactable = false;
        txtTtaiki.text = "待機選手: " + GameManager.getWaitPlayerCount() + "名";
    }
    private void dispDisableWorkAllPlayer() {       // workの全選手をDisableに設定
        ExecPlayer scrEp;
        Image imgMem;

        this.transform.FindChild("ActiveMemberListPanel/Work/CourtBase/MemberSet/imgMember1").GetComponent<Button>().interactable = false;
        scrEp = this.transform.FindChild("ActiveMemberListPanel/Work/CourtBase/MemberSet/imgMember1").GetComponent<ExecPlayer>();
        imgMem = this.transform.FindChild("ActiveMemberListPanel/Work/CourtBase/MemberSet/imgMember1").GetComponent<Image>();
        if (scrEp.gender == 0)
            imgMem.color = Colors.maleDisable;
        else
            imgMem.color = Colors.femaleDisable;

        this.transform.FindChild("ActiveMemberListPanel/Work/CourtBase/MemberSet/imgMember2").GetComponent<Button>().interactable = false;
        scrEp = this.transform.FindChild("ActiveMemberListPanel/Work/CourtBase/MemberSet/imgMember2").GetComponent<ExecPlayer>();
        imgMem = this.transform.FindChild("ActiveMemberListPanel/Work/CourtBase/MemberSet/imgMember2").GetComponent<Image>();
        if (scrEp.gender == 0)
            imgMem.color = Colors.maleDisable;
        else
            imgMem.color = Colors.femaleDisable;

        this.transform.FindChild("ActiveMemberListPanel/Work/CourtBase/MemberSet/imgMember3").GetComponent<Button>().interactable = false;
        scrEp = this.transform.FindChild("ActiveMemberListPanel/Work/CourtBase/MemberSet/imgMember3").GetComponent<ExecPlayer>();
        imgMem = this.transform.FindChild("ActiveMemberListPanel/Work/CourtBase/MemberSet/imgMember3").GetComponent<Image>();
        if (scrEp.gender == 0)
            imgMem.color = Colors.maleDisable;
        else
            imgMem.color = Colors.femaleDisable;

        this.transform.FindChild("ActiveMemberListPanel/Work/CourtBase/MemberSet/imgMember4").GetComponent<Button>().interactable = false;
        scrEp = this.transform.FindChild("ActiveMemberListPanel/Work/CourtBase/MemberSet/imgMember4").GetComponent<ExecPlayer>();
        imgMem = this.transform.FindChild("ActiveMemberListPanel/Work/CourtBase/MemberSet/imgMember4").GetComponent<Image>();
        if (scrEp.gender == 0)
            imgMem.color = Colors.maleDisable;
        else
            imgMem.color = Colors.femaleDisable;
    }
    // パス ExecPlayer から呼ばれる
    public void _callPassPlayer(string iRegDate) {
        GameManager.setPlaceBreak(iRegDate);
        calcPlayerInWork();
    }
    // 退出 ExecPlayer から呼ばれる
    public void _callLeavePlayer(string iRegDate) {
        GameManager.removePlayerByRegdate(iRegDate);    // Gamemanager から選手と選手のpair, match数を削除
        MemberManager.posMemberOfRegDate(iRegDate);    // Membermanager のワークに選手を指定
        MemberManager.idxRec = MemberManager.MAX_IDXREC;
        MemberManager.activeStat = 0;
        MemberManager.Save();
        calcPlayerInWork();
    }

    public void loadView() {
#if (UNITY_IOS || UNITY_ANDROID)
        if (SettingManager.REGULAR_MODE) {
            this.transform.FindChild("ActiveMemberListPanel/Work/btnAdd").gameObject.SetActive(true);
            myView.transform.FindChild("BannerArea").gameObject.SetActive(false);
            nativeAd.DisableAutoReload();                   // Nend広告 一時停止
        } else {
            this.transform.FindChild("ActiveMemberListPanel/Work/btnAdd").gameObject.SetActive(false);
            myView.transform.FindChild("BannerArea").gameObject.SetActive(true);
            if (nativeAdView.Loaded) {    // Nend広告　ネイティブバナー表示
                nativeAdView.RenderAd();
                nativeAd.EnableAutoReload(60000.0);      // Nend広告 1分間隔でリロード
            }
        }
#endif
        viewManager.scriptGameView.sleepView();     // 旧GameViewのupdate()を停止する

        initCortSetting();  // 試合コートの初期状態設定

        // Game選手を作成
        GameManager.removeAllPlayer();
        int activMenNum = MemberManager.getActiveMemberCount();
        for (int mCnt = 0; mCnt < activMenNum; mCnt++) {    // 選択メンバーから試合選手を作成
            MemberManager.posActiveMember(mCnt);
            GameManager.addPlayer(MemberManager.regDate, false);
        }

        // 組合せを作成
        calcPlayerInWork();
    }

    private void initCortSetting() {    // ユーザ設定に基づいて試合コートの名称と表示状態を設定する
        tsfCourtList.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);  // 第1コートをViewのTopにする
        GameManager.initCourt(SettingManager.courtNum);
        for (int cnt = 0; cnt < SettingManager.MaxCoutNum; cnt++) {
            // 試合コートの名称
            string str = "ActiveMemberListPanel/ListScroll/LayoutVertical/CourtListMember" + (cnt + 1) + "/Text";
            this.transform.FindChild(str).GetComponent<Text>().text = SettingManager.getCourtName(cnt);

            // 試合コートの表示・非表示
            string strCourtLst = "ActiveMemberListPanel/ListScroll/LayoutVertical/CourtListMember" + (cnt + 1);
            if (cnt < SettingManager.courtNum) {
                this.transform.FindChild(strCourtLst).gameObject.SetActive(true);
            } else {
                this.transform.FindChild(strCourtLst).gameObject.SetActive(false);
            }
        }
    }

    private void calcPlayerInWork() {   // 次の組合せエリアでの組合せ計算
        if (!flgAllowCalc)
            return;

        tPfbParent.GetComponent<Image>().sprite = Resources.Load<Sprite>("court_B");

        if (tPfbParent.childCount != 0)
            DestroyImmediate(tPfbParent.Find("MemberSet").gameObject);

        btnAddMember.interactable = true;
        btnAsignCourt.interactable = false;
        bool bFullMember = false;
        if (GameManager.getWaitPlayerCount() != 0) {
            Transform pt = Instantiate(pfbMember).transform;
            pt.name = pfbMember.name;
            pt.SetParent(tPfbParent, false);

            // 対戦選手を計算して表示
            bFullMember = calcMatch();
        }

        // Lockされていない空きコートがあるか確認
        int empyCourt = -1;
        for (int cnt = 0; cnt < SettingManager.courtNum; cnt++) {
            if (GameManager.chkCourtLockOfCortnum(cnt) == 0) {
                empyCourt = cnt;
                break;
            }
        }
        if (empyCourt != -1 && bFullMember)  // 空きコートがあり、人数不足なし ならコート割当が可能
            btnAsignCourt.interactable = true;
        else
            btnAsignCourt.interactable = false;

        GameManager.freePlaceBreak();   // パス選手(WAIT状態の全選手)を待機中にする
        txtTtaiki.text = "待機選手: " + GameManager.getWaitPlayerCount() + "名";
    }
    private bool calcMatch() {  // 対戦を計算し、次の組合せエリアに表示 コート許容人数に満たない場合は、false を返す
        Transform trsMem1 = this.transform.FindChild("ActiveMemberListPanel/Work/CourtBase/MemberSet/imgMember1");
        Transform trsMem2 = this.transform.FindChild("ActiveMemberListPanel/Work/CourtBase/MemberSet/imgMember2");
        Transform trsMem3 = this.transform.FindChild("ActiveMemberListPanel/Work/CourtBase/MemberSet/imgMember3");
        Transform trsMem4 = this.transform.FindChild("ActiveMemberListPanel/Work/CourtBase/MemberSet/imgMember4");

        // 人数不足に備えて計算前に選手を非表示
        trsMem1.gameObject.SetActive(false);
        trsMem2.gameObject.SetActive(false);
        trsMem3.gameObject.SetActive(false);
        trsMem4.gameObject.SetActive(false);
        trsMem1.FindChild("imgPair").GetComponent<Image>().gameObject.SetActive(false);
        trsMem2.FindChild("imgPair").GetComponent<Image>().gameObject.SetActive(false);
        trsMem3.FindChild("imgPair").GetComponent<Image>().gameObject.SetActive(false);
        trsMem4.FindChild("imgPair").GetComponent<Image>().gameObject.SetActive(false);

        int waitPNum = GameManager.getWaitPlayerCount();      // 待機中の人数
        int iPos = 0;
        if (SettingManager.form == 2) {     // Doublesの場合
            if (waitPNum == iPos++)
                return false;   // 計算対象の待機中選手がいないなら終わり
            else
                trsMem1.gameObject.SetActive(true);

            string sPairRegDate = null;
            // AポジションとBポジションを決定
            GameManager.getPositionA(true);
            string PosARegDate = GameManager.regDate;
            dispPlayer(PosARegDate, trsMem1, false, false);
            string PosBRegDate = PairManager.getPairRegDate(PosARegDate);       // Aポジション選手のPair選手を一時保存
            if (waitPNum == iPos++)
                return false;   // 計算対象の待機中選手がいないなら終わり
            else
                trsMem2.gameObject.SetActive(true);

            if (PosBRegDate != null) {
                dispPlayer(PosARegDate, trsMem1, true, false);
                dispPlayer(PosBRegDate, trsMem2, true, true);
            } else {
                GameManager.getPositionB(PosARegDate, true);
                PosBRegDate = GameManager.regDate;
                sPairRegDate = PairManager.getPairRegDate(PosBRegDate);
                if (sPairRegDate != null) { // Bポジション選手にPairがいたら、AポジションにPair選手を配置
                    PosARegDate = sPairRegDate;
                    dispPlayer(PosARegDate, trsMem1, true, false);
                    dispPlayer(PosBRegDate, trsMem2, true, true);
                } else {
                    dispPlayer(PosBRegDate, trsMem2, false, false);
                }
            }

            if (waitPNum == iPos++)
                return false;   // 計算対象の待機中選手がいないなら終わり
            else
                trsMem3.gameObject.SetActive(true);

            // CポジションとDポジションを決定 (待機中リストの3番目と4番目)
            GameManager.getPositionC(PosARegDate, PosBRegDate, true);
            string PosCRegDate = GameManager.regDate;
            dispPlayer(PosCRegDate, trsMem3, false, false);
            string PosDRegDate = PairManager.getPairRegDate(PosCRegDate);       // Dポジション選手のPair選手を一時保存
            if (waitPNum == iPos++)
                return false;   // 計算対象の待機中選手がいないなら終わり
            else
                trsMem4.gameObject.SetActive(true);

            if (PosDRegDate != null) {
                dispPlayer(PosCRegDate, trsMem3, true, false);
                dispPlayer(PosDRegDate, trsMem4, true, true);
            } else {
                GameManager.getPositionD(PosARegDate, PosBRegDate, PosCRegDate, true);
                PosDRegDate = GameManager.regDate;
                sPairRegDate = PairManager.getPairRegDate(PosDRegDate);
                if (sPairRegDate != null) { // Bポジション選手にPairがいたら、AポジションにPair選手を配置
                    PosCRegDate = sPairRegDate;
                    dispPlayer(PosCRegDate, trsMem3, true, false);
                    dispPlayer(PosDRegDate, trsMem4, true, true);
                } else {
                    dispPlayer(PosDRegDate, trsMem4, false, false);
                }
            }
        } else {
            // シングルスの場合
        }
        return true;
    }

    private void dispPlayer(string iRegDate, Transform iTrs, bool flgPair, bool flgPairImage) {
        GameManager.posPlayerOfRegDate(iRegDate);
        // 名前
        iTrs.FindChild("Text").GetComponent<Text>().text = GameManager.nameKaji_family + " " + GameManager.nameKaji_first;
        // 色
        if (GameManager.gender == 0) {
            if (SettingManager.REGULAR_MODE && !flgPair) {
                iTrs.GetComponent<Button>().interactable = true;
                iTrs.GetComponent<Image>().color = Colors.male;
            } else {
                iTrs.GetComponent<Button>().interactable = false;
                iTrs.GetComponent<Image>().color = Colors.maleDisable;
            }
        } else {
            if (SettingManager.REGULAR_MODE && !flgPair) {
                iTrs.GetComponent<Button>().interactable = true;
                iTrs.GetComponent<Image>().color = Colors.female;
            } else {
                iTrs.GetComponent<Button>().interactable = false;
                iTrs.GetComponent<Image>().color = Colors.femaleDisable;
            }
        }
        // ペアイメージ
        iTrs.FindChild("imgPair").GetComponent<Image>().gameObject.SetActive(flgPairImage);

        // MemberSetのScriptに選手情報を設定
        ExecPlayer scrEp = iTrs.GetComponent<ExecPlayer>();
        scrEp.regDate = GameManager.regDate;
        scrEp.nameKaji_family = GameManager.nameKaji_family;
        scrEp.nameKaji_first = GameManager.nameKaji_first;
        scrEp.gender = GameManager.gender;
        scrEp.flgDispImgPair = flgPairImage;
}

    // アプリがBackgrouandになったとき
/*
    void OnApplicationPause(bool pauseStatus) {
        if (pauseStatus)
            nativeAd.DisableAutoReload();                   // Nend広告 一時停止
        else
            nativeAd.EnableAutoReload(60000.0);      // Nend広告 1分間隔でリロード
    }
*/
    void Start () {
        // Nend広告 インタースティシャル
#if UNITY_IOS
        NendAdInterstitial.Instance.Load("f016f0e95fa72120c9de627972520c83cfd00754", "649006");
#elif UNITY_ANDROID
        NendAdInterstitial.Instance.Load("android apiKey", "android spotId");
#endif
        // Nend広告 ネイティブバナー (apiKey, spotId は inspector で設定 [NendAdNative(Script)])
        nativeAdView = this.transform.FindChild("BannerArea/NendAdNativeView").GetComponent<NendAdNativeView>();
        nativeAd = this.transform.FindChild("BannerArea").GetComponent<NendAdNative>();
        nativeAd.LoadAd();

        viewManager = GameObject.Find("ViewManager").GetComponent<ViewManager>();
        tPfbParent = this.transform.Find("ActiveMemberListPanel/Work/CourtBase");
        tsfCourtList = this.transform.FindChild("ActiveMemberListPanel/ListScroll/LayoutVertical");
        tsfCourt = new Transform[6];
        tsfCourt[0] = this.transform.FindChild("ActiveMemberListPanel/ListScroll/LayoutVertical/CourtListMember1");
        tsfCourt[1] = this.transform.FindChild("ActiveMemberListPanel/ListScroll/LayoutVertical/CourtListMember2");
        tsfCourt[2] = this.transform.FindChild("ActiveMemberListPanel/ListScroll/LayoutVertical/CourtListMember3");
        tsfCourt[3] = this.transform.FindChild("ActiveMemberListPanel/ListScroll/LayoutVertical/CourtListMember4");
        tsfCourt[4] = this.transform.FindChild("ActiveMemberListPanel/ListScroll/LayoutVertical/CourtListMember5");
        tsfCourt[5] = this.transform.FindChild("ActiveMemberListPanel/ListScroll/LayoutVertical/CourtListMember6");
        txtTtaiki = this.transform.Find("ActiveMemberListPanel/Work/txtTaiki").GetComponent<Text>();
        btnAsignCourt = this.transform.Find("ActiveMemberListPanel/Work/btnAsignCourt").GetComponent<Button>();
        btnAddMember = this.transform.Find("ActiveMemberListPanel/Work/btnAdd").GetComponent<Button>();
    }
    void Update () {}
}
