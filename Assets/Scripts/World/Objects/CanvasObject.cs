using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasObject : CollectableObject
{
	public Texture2D preview;
	public override void Interact()
	{
		//prevent move/rotate here
		StartCoroutine(Collect(
			player.transform.position + player.cam.transform.forward,
			new Vector3(player.rotationX - 25f, player.rotationY, 0)));
	}

	protected override void CollectEndAction()
	{
		StartCoroutine(Effects.mask.PreTransition(preview, GameManager.Instance.levels[GameManager.Instance.sceneIndex + 1]));
	}
}
