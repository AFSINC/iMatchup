using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//Buttonオブジェクトに貼りつけて、押下状態としてpressedを見るか、
//onLongPressにイベントを登録すると定期的に呼ばれます。

[System.Serializable]
public class _OnLongPressPara : UnityEvent<PointerEventData>{};
public class _OnLongPressParaExit : UnityEvent<PointerEventData>{};

public class EventLongPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
	/// 押しっぱなし時に呼び出すイベント
	[SerializeField] public UnityEvent onLongPress = new UnityEvent ();
	[SerializeField] public UnityEvent onLongPressExit = new UnityEvent ();
	[SerializeField] public _OnLongPressPara onLongPressPara = new _OnLongPressPara ();
	[SerializeField] public _OnLongPressPara onLongPressParaExit = new _OnLongPressPara ();
	PointerEventData e;

	/// 押しっぱなし判定の間隔（この間隔毎にイベントが呼ばれる）
	public float intervalAction = 1.0f;

	// 押下開始時にもイベントを呼び出すフラグ
	public bool callEventFirstPress;

	// LongPressを停止するフラグ
	public bool enableLongPress = true;

	// 次の押下判定時間
	float nextTime = 0f;

	/// 押下状態
	public bool pressed {
		get;
		private set;
	}

	void Update () {
		if (enableLongPress && pressed && nextTime < Time.realtimeSinceStartup) {
			onLongPress.Invoke ();
			onLongPressPara.Invoke (e);
			// 1回だけにするなら下記のインターバルを大きくする必要がある
			// または、pressed を false にする
			nextTime = Time.realtimeSinceStartup + intervalAction;
		}
	}

	public void OnPointerDown (PointerEventData eventData) {
		e = eventData;
		pressed = true;
		if (callEventFirstPress) {
			onLongPress.Invoke ();
		}
		nextTime = Time.realtimeSinceStartup + intervalAction;
	}

	public void OnPointerUp (PointerEventData eventData) {
		pressed = false;
		onLongPressExit.Invoke ();
		onLongPressParaExit.Invoke (e);
	}
}
