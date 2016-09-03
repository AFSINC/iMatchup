using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class SettingManager : SingletonMonoBehaviour<SettingManager> {

	public static bool DEBUG_MODE = false;
	public static bool REGULAR_MODE = false;

	public static int MaxCoutNum = 6;
	public class Para : SavableSingleton<Para>{
		public string saveDate;
		public bool sound;			// false:No    true:Yes
		public int regular;			// 0:No        1:Yes
		public int courtNum;		// 1 <----> 10
		public int form;				// 1:Singles  2:Doubles
		public int weight;				// 1:Min       2:Max
		public int sum;					// 0:No        1:Yes
		public int joinInit;				// 0:Zero     1:Min
		public int autoExec;			// 0:No       1:Yes
		public int waitCalc;			// 0:No       1:Yes
		public string[] courtName;

		public Para() {
			courtName = new string[MaxCoutNum];
			sound = false;
			regular = 0;			// 0:No        1:Yes
			courtNum = 1;		// 1 <----> 10
			form = 1;				// 1:Singles  2:Doubles
			weight = 1;			// 1:Min       2:Max
			sum = 0;				// 0:No        1:Yes
			joinInit = 0;			// 0:Zero     1:Min
			autoExec = 0;		// 0:No        1:Yes
			waitCalc = 0;		// 0:No        1:Yes
			for (int cnt=0; cnt<courtName.Length; cnt++)
				courtName[cnt] = "第" + (cnt+1) +  "コート";
		}
	}
	public Para para = new Para();
    public static int FORM_SINGLES = 1;
    public static int FORM_DOUBLES = 2;

    // -- setter, getter --
    public static string saveDate {
		get{return Para.Instance.saveDate;}
	}
	public static bool sound {
		get{return Para.Instance.sound;}
		set{ Para.Instance.sound = value; }
	}
	public static int regular {
		get{return Para.Instance.regular;}
		set{ Para.Instance.regular = value; }
	}
	public static int courtNum {
		get{return Para.Instance.courtNum ;}
		set{ Para.Instance.courtNum = value; }
	}
	public static int form {
		get{return Para.Instance.form ;}
		set{ Para.Instance.form = value; }
	}
	public static int weight {
		get{return Para.Instance.weight ;}
		set{ Para.Instance.weight = value; }
	}
	public static int sum {
		get{return Para.Instance.sum ;}
		set{ Para.Instance.sum = value; }
	}
	public static int joinInit {
		get{return Para.Instance.joinInit ;}
		set{ Para.Instance.joinInit = value; }
	}
	public static int autoExec {
		get{return Para.Instance.autoExec ;}
		set{ Para.Instance.autoExec = value; }
	}
	public static int waitCalc {
		get{return Para.Instance.waitCalc ;}
		set{ Para.Instance.waitCalc = value; }
	}
	public static void setCourtName(int idx, string value) {
		Para.Instance.courtName[idx] = value;
	}
	public static string getCourtName(int idx) {
		return Para.Instance.courtName[idx];
	}

	public static void initSetData() {
		Para.Instance.saveDate = DateTime.Now.ToString ("s", new System.Globalization.CultureInfo("ja-JP"));
		Save ();
	}

	public static void Load() {
		Para.Load ();
	}
	public static void Save()  {
		Para.Save ();
	}
	public static void Reset() {
		Para.Reset ();
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
			Para.Save ();
	}
}
