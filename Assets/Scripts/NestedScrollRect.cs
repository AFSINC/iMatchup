using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class NestedScrollRect : ScrollRect {

	private bool routeToParent = false;

	public override void OnInitializePotentialDrag (PointerEventData eventData)
	{
		transform.DoParentEventSystemHandler<IInitializePotentialDragHandler>((parent) => { parent.OnInitializePotentialDrag(eventData); });
		base.OnInitializePotentialDrag (eventData);
	}

	public override void OnDrag (UnityEngine.EventSystems.PointerEventData eventData)
	{
		if(routeToParent)
			transform.DoParentEventSystemHandler<IDragHandler>((parent) => { parent.OnDrag(eventData); });
		else
			base.OnDrag (eventData);
	}

	public override void OnBeginDrag (UnityEngine.EventSystems.PointerEventData eventData)
	{
		if(!horizontal && Math.Abs (eventData.delta.x) > Math.Abs (eventData.delta.y))
			routeToParent = true;
		else if(!vertical && Math.Abs (eventData.delta.x) < Math.Abs (eventData.delta.y))
			routeToParent = true;
		else
			routeToParent = false;

		if(routeToParent)
			transform.DoParentEventSystemHandler<IBeginDragHandler>((parent) => { parent.OnBeginDrag(eventData); });
		else
			base.OnBeginDrag (eventData);
	}

	public override void OnEndDrag (UnityEngine.EventSystems.PointerEventData eventData)
	{
		if(routeToParent)
			transform.DoParentEventSystemHandler<IEndDragHandler>((parent) => { parent.OnEndDrag(eventData); });
		else
			base.OnEndDrag (eventData);
		routeToParent = false;
	}
}