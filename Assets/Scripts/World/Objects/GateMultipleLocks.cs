using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GateMultipleLocks : MonoBehaviour
{
	[SerializeField] float rotationAngle = 50;
	[SerializeField] float rotationTime = 1;
	[SerializeField] Transform leftDoor = default;
	[SerializeField] Transform rightDoor = default;

	[FMODUnity.EventRef] public string GateOpenEvent;
	public GameObject keyHole;
	public GameObject keyHoleTwo;
	private bool open = false;

	public void Open()
	{
		if (!open)
		{
			open = true;
			StartCoroutine(OpenGate(rotationAngle, rotationTime));
		}
	}

	IEnumerator OpenGate(float angle, float time)
	{
		float start = Time.time;
		bool inProgress = true;

		var leftStart = leftDoor.eulerAngles;
		var rightStart = rightDoor.eulerAngles;

		FMODUnity.RuntimeManager.PlayOneShot(GateOpenEvent, transform.position);

		while (inProgress)
		{
			yield return null;
			float step = Time.time - start;
			leftDoor.eulerAngles = leftStart + Vector3.up * (angle * step / time);
			rightDoor.eulerAngles = rightStart + Vector3.up * (-angle * step / time);

			if (step > time)
				inProgress = false;
		}
	}
}
