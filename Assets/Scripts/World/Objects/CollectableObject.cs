using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CollectableObject : InteractableObject
{
	private Vector3 spatialTarget;
	private Vector3 rotationalTarget;

	public override void Interact()
	{
		StartCoroutine(Collect(player.transform.position, transform.eulerAngles));
		// Debug.Log("here");
		// spatialTarget = player.transform.position;
		// rotationalTarget = Quaternion.LookRotation(player.transform.forward, Vector3.up).eulerAngles;
		// pickingUp = true;
		// if (sceneTarget != "")
		// 	GameManager.Instance.ChangeLevel(sceneTarget);
	}

	protected IEnumerator Collect(Vector3 spatialTarget, Vector3 rotationalTarget, float time = .5f)
	{
		foreach (Collider c in GetComponentsInChildren<Collider>())
			Destroy(c);
		float start = Time.time;
		bool inProgress = true;

		Vector3 startPos = transform.position;
		Vector3 startEulers = transform.eulerAngles;
		while (inProgress)
		{
			yield return null;
			float step = Time.time - start;
			transform.position = Vector3.Lerp(startPos, spatialTarget, step / time);
			transform.eulerAngles = Vector3.Lerp(startEulers, rotationalTarget, step / time);
			if (step > time)
				inProgress = false;
		}
		CollectEndAction();
	}

	protected virtual void CollectEndAction()
	{
		gameObject.SetActive(false);
	}
}
