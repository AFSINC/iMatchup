using UnityEngine;
using System.Collections;

public class InitialSystem : MonoBehaviour {

    // データ初期化
    public void _OnInit() {
        string title = "初期化の確認";
        string message = "すべてのデータを削除してダウンロード時の状態にしますか？";
        DialogViewController.Show(title, message, new DialogViewOptions {
            btnCancelTitle = "キャンセル",
            btnCancelDelegate = () => {
                return;
            },
            btnOkTitle = "初期化",
            btnOkDelegate = () => {
                SettingManager.Reset();
                MemberManager.Reset();
                PairManager.Reset();
                GameManager.Reset();
                ResultManager.Reset();
                NoticeManager.Reset();
            },
            btnOkTitleSub = null,
            btnOkDelegateSub = null,
        });
    }

    void OnLongPress() { }

    void Start () {
		this.gameObject.AddComponent (typeof(EventLongPress));
		GetComponent<EventLongPress>().onLongPress.AddListener(OnLongPress);
	}
}
