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
		StartCoroutine(Player.Instance.mask.PreTransition(GameManager.Instance.levels[GameManager.Instance.sceneIndex + 1]));
		foreach (var o in transform.GetComponentInParent<EntangledClippable>().GetComponentsInChildren<OutlineObject>())
			o.enabled = false;
		// TODO: await endofframe
	}
}
