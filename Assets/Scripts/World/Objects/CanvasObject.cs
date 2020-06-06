using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CanvasObject : CollectableObject
{
	public event Action OnInteract;
	public override string prompt { get => $"Press {InputManager.InteractKey} to Enter Canvas"; }

	public override void Interact()
	{
		OnInteract?.Invoke();
		StartCoroutine(Player.Instance.mask.PreTransition());
		var enclippableParent = transform.GetComponentInParent<EntangledClippable>();
		if (enclippableParent)
			foreach (var o in enclippableParent.GetComponentsInChildren<OutlineObject>())
				o.enabled = false;
		// TODO: await endofframe
	}
}
