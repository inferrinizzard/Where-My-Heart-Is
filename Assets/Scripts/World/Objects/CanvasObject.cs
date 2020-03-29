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
		StartCoroutine(Collect(
			player.transform.position + player.cam.transform.forward,
			new Vector3(player.rotation.x - 25f, player.rotation.y, 0)));
	}

	protected override void CollectEndAction()
	{
		StartCoroutine(Player.Instance.mask.PreTransition(preview, GameManager.Instance.levels[GameManager.Instance.sceneIndex + 1]));
		// StartCoroutine(Effects.mask.PreTransition(preview, manualTarget == "" ? "Intro" : manualTarget));
	}
}
