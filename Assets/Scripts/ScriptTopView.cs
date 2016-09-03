using UnityEngine;
using System.Collections;

public class ScriptTopView : MonoBehaviour {
    public GameObject myView;
    ViewManager viewManager;

    // DEBUG用
    public void _OnDebug() {
        SoundManager.play(SoundManager.VOICE00);
        SettingManager.DEBUG_MODE = true;
        viewManager.scriptDebugView.debugEnable(SettingManager.DEBUG_MODE);
    }
    public void _OnRegular() {
        SettingManager.REGULAR_MODE = true;
        this.transform.FindChild("btnRegularMode").gameObject.SetActive(false);
        this.transform.FindChild("imgLite").gameObject.SetActive(false);
    }

    public void _CallBackTap () {
        this.transform.gameObject.SetActive(false);
        viewManager._setAactiveView();
        viewManager.MainViewHolder.transform.SetAsLastSibling();	//Main画面を最前面とする
    }




    void Start () {
 //       AdMobManager.bannerView.Hide();
        viewManager = GameObject.Find("ViewManager").GetComponent<ViewManager>();
    }
}
