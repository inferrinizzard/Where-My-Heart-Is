using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
public class Gate : MonoBehaviour
{
	[SerializeField] float rotationAngle = 50;
	[SerializeField] float rotationTime = 1;
	[SerializeField] float unlockRadius = 3;
	[SerializeField] Transform leftDoor = default, rightDoor = default;
	[FMODUnity.EventRef] public string GateOpenEvent;
	private bool open = false;
	public List<LockPair> locks = new List<LockPair>();
	public void AddLock(Lock l)
	{
		var worldType = l.GetComponent<ClippableObject>()?.worldType;
		var existingPair = locks.Find(pair => pair.heartLock?.transform.position == l.transform.position || pair.realLock?.transform.position == l.transform.position);
		if (existingPair != null)
		{
			if (worldType == ClippableObject.WorldType.Real) //TODO: make under world
				existingPair.realLock = l;
			else
				existingPair.heartLock = l;
		}
		else
			locks.Add(worldType == ClippableObject.WorldType.Real ? new LockPair(real: l) : new LockPair(heart: l));
		l.name += $" [{worldType}]";
		locks = locks.Where(pair => pair.heartLock != null || pair.realLock != null).ToList();
	}

	public void Unlock(Lock l)
	{
		var removeLock = locks.Find(pair => pair.heartLock == l || pair.realLock == l);
		locks.Remove(removeLock);
		// could potentially play unlock anim here;
		Destroy(removeLock.realLock?.gameObject);
		Destroy(removeLock.heartLock?.gameObject);

		if (locks.Count == 0)
			StartCoroutine(OpenGate(rotationAngle, rotationTime));
	}

	IEnumerator OpenGate(float angle, float time)
	{
		var leftStart = leftDoor.eulerAngles;
		var rightStart = rightDoor.eulerAngles;

		FMODUnity.RuntimeManager.PlayOneShot(GateOpenEvent, transform.position);

		for (var(start, step) = (Time.time, 0f); step <= time; step = Time.time - start)
		{
			yield return null;
			leftDoor.eulerAngles = leftStart + Vector3.up * (angle * step / time);
			rightDoor.eulerAngles = rightStart + Vector3.up * (-angle * step / time);
		}
	}
}

[System.Serializable] public class LockPair
{
	public Lock realLock, heartLock;
	public LockPair(Lock real = null, Lock heart = null) => (realLock, heartLock) = (real, heart);
}
