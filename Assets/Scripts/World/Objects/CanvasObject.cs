using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasObject : CollectableObject
{
	public Texture2D preview;
	[SerializeField] string manualTarget = "";

	public override void Interact()
	{
		//prevent move/rotate here
		StartCoroutine(Collect(
			player.transform.position + player.cam.transform.forward,
			new Vector3(player.rotationX - 25f, player.rotationY, 0)));
	}

	protected override void CollectEndAction()
	{
		// capture screenshot
		// draw to screen, use UI?

		// pass in on start load
		// set shader flag
		// continually set global float in coro
		Effects.mask.AssignTransitionMat(preview);
		if (manualTarget != "")
			GameManager.Instance.ChangeLevel(manualTarget);
		else
			GameManager.Instance.ChangeLevel(GameManager.Instance.levels[GameManager.Instance.sceneIndex + 1]);
	}
}
