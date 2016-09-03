using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventDoubleTap : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
	private bool isDoubleTapStart;
	private float doubleTapTime = 0f;
	private int cntTap = 0;

	/// DoubleTapと判定する時間間隔
	public float intervalAction = 0.5f;

	/// DoubleTapイベント
	public UnityEvent onDoubleTap = new UnityEvent ();

	void Update () {
		// double tap
		if (isDoubleTapStart) {
			isDoubleTapStart = false;
			cntTap++;
			if (Time.realtimeSinceStartup >= doubleTapTime) {
				doubleTapTime = Time.realtimeSinceStartup + intervalAction;
			} else if (cntTap > 1) {
				cntTap = 0;
				doubleTapTime = 0.0f;
				onDoubleTap.Invoke ();
			}
		}
	}

	public void OnPointerDown (PointerEventData eventData) {
		isDoubleTapStart = true;
	}

	public void OnPointerUp (PointerEventData eventData) {
	}

/*
	// どこをタップしてもイベントが発生するようにするならば
	// IPointerDownHandler, IPointerUpHandler を継承しないようにする
	void Update () {
		// double tap
		if (isDoubleTapStart) {
			doubleTapTime += Time.deltaTime;
			if (doubleTapTime < 0.3f) {
				if (Input.GetMouseButtonDown (0)) {
					Debug.Log ("double tap");
					isDoubleTapStart = false;
					doubleTapTime = 0.0f;
					onDoubleTap.Invoke ();
				}
			} else {
				Debug.Log ("reset");
				// reset
				isDoubleTapStart = false;
				doubleTapTime = 0.0f;
			}
		} else {
			if (Input.GetMouseButtonDown (0)) {
				Debug.Log ("down");
				isDoubleTapStart = true;
			}
		}
	}
*/
}
