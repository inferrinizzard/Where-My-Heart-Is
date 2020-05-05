using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CanvasObject : CollectableObject
{
	public event Action OnInteract;

	void Awake() => prompt = "Press E to Enter Canvas";

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
