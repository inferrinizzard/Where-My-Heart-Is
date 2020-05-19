using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
public class Gate : MonoBehaviour
{
	[SerializeField] float rotationAngle = 50;
	[SerializeField] float rotationTime = 1;
	[SerializeField] float unlockRadius = .5f;
	[SerializeField] Transform leftDoor = default, rightDoor = default;
	[FMODUnity.EventRef] public string GateOpenEvent;
	[SerializeField] GameObject crackedLock;
	public List<LockPair> locks = new List<LockPair>();
	public void AddLock(Lock l)
	{
		var worldType = l.GetComponent<ClippableObject>()?.worldType;
		var existingPair = locks.Find(pair =>
			(pair.heartLock && (pair.heartLock.transform.position - l.transform.position).sqrMagnitude < .001) ||
			(pair.realLock && (pair.realLock.transform.position - l.transform.position).sqrMagnitude < .001)
		);
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
		l.GetComponent<SphereCollider>().radius = unlockRadius;
		locks = locks.Where(pair => pair.heartLock != null || pair.realLock != null).ToList();
	}

	public void Unlock(Lock l, GameObject key)
	{
		var removeLock = locks.Find(pair => pair.heartLock == l || pair.realLock == l);
		locks.Remove(removeLock);
		// Debug.Log(removeLock);
		locks = locks.Where(pair => pair.heartLock != null || pair.realLock != null).ToList(); // TODO: prevent new locks on cut
		// could potentially play unlock anim here;
		StartCoroutine(KeyAnim(l.transform, key.transform, removeLock));
	}

	IEnumerator KeyAnim(Transform _lock, Transform key, LockPair destroy, float time = 1.5f)
	{
		key.parent = key.GetComponent<ClippableObject>().worldType == ClippableObject.WorldType.Heart ? World.Instance.heartWorldContainer : World.Instance.realWorldContainer;
		key.position = _lock.position + _lock.up;
		key.eulerAngles = new Vector3(-90, _lock.transform.eulerAngles.y, 0);
		for (var(start, step) = (Time.time, 0f); step <= time; step = Time.time - start)
		{
			yield return null;
			if (step < time / 3)
				key.position = _lock.position + _lock.up * (1 - EaseMethods.CubicEaseOut(step * 3 / time, 0, 1, 1));
			else if (step < time * 2 / 3)
				key.Rotate(_lock.forward, 5 / (time / -3));
			else
			{
				if (key)
				{
					key.GetComponent<Pickupable>().Disintegrate();
					// key.GetComponent<InteractableObject>().Interact();
					// Destroy(key.gameObject);
					if (crackedLock)
						_lock.GetComponent<MeshFilter>().mesh = crackedLock.GetComponent<MeshFilter>().sharedMesh;
				}
				_lock.transform.Translate(-Vector3.forward / 5 * EaseMethods.QuartEaseOut((3 * step - time * 2) / time, 0, 1, 1));
			}
		}

		Destroy(destroy.realLock?.gameObject);
		Destroy(destroy.heartLock?.gameObject);

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
