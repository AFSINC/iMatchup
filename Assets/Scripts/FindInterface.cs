using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public static class FindInterface {
	public static void DoParentEventSystemHandler<T>(this Transform self, Action<T> action) where T:IEventSystemHandler
	{
		Transform parent = self.transform.parent;
		while(parent != null) {
			foreach(var component in parent.GetComponents<Component>()) {
				if(component is T)
					action((T)(IEventSystemHandler)component);
			}
			parent = parent.parent;
		}
	}
}