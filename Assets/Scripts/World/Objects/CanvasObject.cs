using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CanvasObject : Pickupable
{
	public event Action OnInteract;
	[SerializeField] bool onEasel = true;
	[SerializeField] Transform placeTarget;
	public override string prompt { get => "Press E to Enter Canvas"; }
	// public override string prompt { get => "Press E to Place Canvas"; }

	public override void Interact()
	{
		if (onEasel)
		{
			OnInteract?.Invoke();
			StartCoroutine(Player.Instance.mask.PreTransition());
			var enclippableParent = transform.GetComponentInParent<EntangledClippable>();
			if (enclippableParent)
				foreach (var o in enclippableParent.GetComponentsInChildren<OutlineObject>())
					o.enabled = false;
			// TODO: await endofframe
		}
		else
		{
			if (active && (transform.position - placeTarget.position).sqrMagnitude < player.playerReach * player.playerReach / 4)
			{
				PutDown();
				transform.parent = placeTarget;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				transform.localScale = Vector3.one;
				onEasel = true;
			}
			else
				base.Interact();
			// PickUp();
		}
	}
}
