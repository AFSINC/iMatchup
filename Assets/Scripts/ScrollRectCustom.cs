using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

// Content: Content
// Viewport: Viewport
public class ScrollRectCustom : ScrollRect {
	public bool fStop = false;

	public override void OnBeginDrag (PointerEventData e) {
		base.OnBeginDrag (e);
//		Debug.Log ("ScrollRectCustom OnBeginDrag");
	}

	public override void OnEndDrag (PointerEventData e) {
		base.OnEndDrag (e);
//		Debug.Log ("ScrollRectCustom OnEndDrag");
	}

	public override void OnDrag (PointerEventData e) {
		base.OnDrag (e);
//		Debug.Log ("ScrollRectCustom OnDrag");
	}

	public void _aaa () {
		StopMovement ();
//		Debug.Log ("ScrollRectCustom StopMovement");
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
/*		if (fStop)
			vertical = false;
*/
	}
}
