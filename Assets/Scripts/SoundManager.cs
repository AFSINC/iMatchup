using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {
//	private GameObject gameObj;
	private static AudioSource audioSource;
	private int audioClipNum = 7;
	private static AudioClip[] sound;
	public static int CLICK00 = 0;
	public static int CLICK01 = 1;
	public static int CLICK02 = 2;
	public static int VOICE00 = 3;
	public static int VOICE01 = 4;
	public static int VOICE02 = 5;
	public static int VOICE03 = 6;
	public AudioClip sndClick00;
	public AudioClip sndClick01;
	public AudioClip sndClick02;
	public AudioClip sndVoice00;
	public AudioClip sndVoice01;
	public AudioClip sndVoice02;
	public AudioClip sndVoice03;

	public static void play(int iSound) {
		if (SettingManager.sound)
			audioSource.PlayOneShot(sound [iSound]);
	}

	void Start () {
//		gameObj = new GameObject ();
//		gameObj.AddComponent<AudioSource> ();
		audioSource = this.transform.GetComponent<AudioSource> ();
		sound = new AudioClip[audioClipNum];
		sound[CLICK01] = sndClick01;
		sound[CLICK02] = sndClick02;
		sound[VOICE00] = sndVoice00;
		sound[VOICE01] = sndVoice01;
		sound[VOICE02] = sndVoice02;
		sound[VOICE03] = sndVoice03;
	}
}
