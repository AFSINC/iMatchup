using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class ExecPlayer : MonoBehaviour {
	EventLongPress eventLP;
    //    public bool flgEnable = true;
    ScriptExecStartView scriptExecStartView;
    public bool flgDispImgPair;
    public string regDate;                  // 登録日付
    public string nameKaji_family;
    public string nameKaji_first;
    public int gender;

    public void OnPassAndLeave() {
        int waitPlayerMum = GameManager.getWaitPlayerCount();

        if (waitPlayerMum > SettingManager.form * 2) {
            string title = "パス/退出の確認";
            string message = "組み合せから1回除外するならばパスを、以後組み合せに表示させないならば退出を選択してください。";
            DialogViewController.Show(title, message, new DialogViewOptions {
                btnCancelTitle = "キャンセル",
                btnCancelDelegate = () => {
                    return;
                },
                btnOkTitle = "退出",
                btnOkDelegate = () => {
                    scriptExecStartView._callLeavePlayer(regDate);   // ScriptExecStartViewの退出処理を呼び出す
                },
                btnOkTitleSub = "パス",
                btnOkDelegateSub = () => {
                    scriptExecStartView._callPassPlayer(regDate);   // ScriptExecStartViewのパス処理を呼び出す
                },
            });
        } else {
            string title = "退出の確認";
            string message = "以後組み合せに表示させないならば退出を選択してください。";
            DialogViewController.Show(title, message, new DialogViewOptions {
                btnCancelTitle = "キャンセル",
                btnCancelDelegate = () => {
                    return;
                },
                btnOkTitle = "退出",
                btnOkDelegate = () => {
                    scriptExecStartView._callLeavePlayer(regDate);   // ScriptExecStartViewの退出処理を呼び出す
                },
            });
        }
    }

    public void loadView(bool flgena) {
    }

    public void OnLongPressPara(PointerEventData e) {
	}
	public void OnLongPressExit(PointerEventData e) {
	}
	 
	void Start () {
		this.gameObject.AddComponent(typeof(EventLongPress));
		eventLP = GetComponent<EventLongPress>();
		eventLP.onLongPressPara.AddListener(OnLongPressPara);
		eventLP.onLongPressParaExit.AddListener(OnLongPressExit);
		eventLP.intervalAction = 0.4f;      // LongPress判定時間

        scriptExecStartView = GameObject.Find("ExecStart").transform.GetComponent<ScriptExecStartView>();
    }

	void Update () {	
	}
}
