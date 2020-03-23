using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class WorldObject : MonoBehaviour
{
	// public bool interactable = false;
	public bool clipable = false;
	public bool pickupable = false; // set interactable
	public bool collectable = false; // set interactable

	// public void ToggleInteractable()
	// {
	// 	interactable = !interactable;
	// 	SummonComponent<InteractableObject>(interactable);
	// }
	public void ToggleClipable()
	{
		clipable = !clipable;
		SummonComponent<ClipableObject>(clipable);
	}
	public void TogglePickupable()
	{
		pickupable = !pickupable;
		// interactable = pickupable;
		SummonComponent<Pickupable>(pickupable);
	}
	public void ToggleCollectable()
	{
		collectable = !collectable;
		// interactable = collectable;
		SummonComponent<CollectableObject>(collectable);
	}

	Component SummonComponent<T>(bool flag) where T : Component
	{
		if (flag)
		{
			int numComponents = GetComponents<Component>().Length;
			var newComponent = gameObject.AddComponent<T>();
			// for (int i = 0; i < numComponents - 2; i++)
			// 	UnityEditorInternal.ComponentUtility.MoveComponentUp(newComponent);
			return newComponent;
		}
		else
			DestroyImmediate(GetComponent<T>());
		return null;
	}
}
