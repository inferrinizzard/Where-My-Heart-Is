using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CanvasObject : CollectableObject
{
	public Texture2D preview;
	[SerializeField] public string manualTarget = "";
	public event Action OnInteract;

	void Awake() => prompt = "Press E to Enter Canvas";

	public override void Interact()
	{
		OnInteract?.Invoke();
		//prevent move/rotate here
		float targetAngle = (player.rotation.y % 360f) - transform.rotation.y;

		if(targetAngle > 180)
		{
			StartCoroutine(Collect(
				player.transform.position + player.cam.transform.forward,
				new Vector3(player.rotation.x, targetAngle - 180f, 0)));
		}
		else
		{
			StartCoroutine(Collect(
				player.transform.position + player.cam.transform.forward,
				new Vector3(player.rotation.x, targetAngle + 180f, 0)));
		}
	}

	protected override void CollectEndAction()
	{
		StartCoroutine(Player.Instance.mask.PreTransition(preview));
		// StartCoroutine(Effects.mask.PreTransition(preview, manualTarget == "" ? "Intro" : manualTarget));
	}
}
