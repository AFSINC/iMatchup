using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
	Vector3 parentPos, posOffset, p;
    GameObject placeHolder = null;
	Shadow sd = null;
	Outline ol = null;

	Transform placeholderParent = null;

	int sousceItemIndex, destItemIndex;

	public void OnBeginDrag(PointerEventData e) {
		Transform pp = this.transform.parent.Find ("baseWhite");
		pp.gameObject.AddComponent<Shadow> ();
		sd = pp.GetComponent<Shadow> ();
		sd.effectColor = new Vector4 (0f, 0f, 0f, 20f/255f);
		sd.effectDistance = new Vector2 (20f, -50f);
		pp.gameObject.AddComponent<Outline> ();
		ol = pp.GetComponent<Outline> ();
		ol.effectColor = new Vector4 (0f, 0f, 0f, 60f/255f);
		ol.effectDistance = new Vector2 (1.0f, -1.0f);
		ol.useGraphicAlpha = true;

		sousceItemIndex = this.transform.parent.GetSiblingIndex ();

		parentPos.x = this.transform.parent.position.x;
		parentPos.y = this.transform.parent.position.y;
		posOffset.x = this.transform.parent.position.x - e.position.x;
		posOffset.y = this.transform.parent.position.y - e.position.y;
		this.transform.parent.SetAsLastSibling ();
//		Debug.Log (this.name + " " + parentPos.x);

		placeHolder = new GameObject("placeHolder");
		placeHolder.transform.SetParent (this.transform.parent.parent);
		LayoutElement le = placeHolder.AddComponent<LayoutElement> ();
		le.preferredHeight = this.transform.parent.GetComponent<LayoutElement> ().preferredHeight;
		placeHolder.transform.SetSiblingIndex (this.transform.parent.GetSiblingIndex ());
		placeholderParent = this.transform.parent.parent;

		this.transform.parent.SetParent (this.transform.parent.parent.parent);

	}

	public void OnDrag(PointerEventData e) {
		p.x = posOffset.x + e.position.x;
		p.y = posOffset.y + e.position.y;
		p.x = Mathf.Clamp (p.x, parentPos.x, parentPos.x);
		this.transform.parent.position = p;

		int newSiblingIndex = placeholderParent.childCount;
		for (int i=0; i < placeholderParent.childCount; i++) {
			if (this.transform.parent.position.y > placeholderParent.GetChild(i).position.y + 30) {
				newSiblingIndex = i;
				if (placeholderParent.transform.GetSiblingIndex() < newSiblingIndex) 
					newSiblingIndex--;

				break;
			}
		}
		placeHolder.transform.SetSiblingIndex (newSiblingIndex);
	}

	public void OnEndDrag(PointerEventData e) {
		Object.Destroy(sd);
		sd = null;
		Object.Destroy(ol);
		ol = null;

		destItemIndex = placeHolder.transform.GetSiblingIndex ();
		MemberManager.movMember (sousceItemIndex, destItemIndex);

		this.transform.parent.SetParent (placeholderParent);
		this.transform.parent.SetSiblingIndex (placeHolder.transform.GetSiblingIndex());
		Destroy (placeHolder);

	}
}
