using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Lock : MonoBehaviour
{
	Gate gate;
	[HideInInspector] public int hash = 0;
	void Start()
	{
		/*
		hash = 0;
		for (var(i, j, v) = (0, 0, 0f); i < 3; hash += (int) v)
			for ((j, v) = (0, transform.position[i++]); v % 1 != 0 && ++j < 5; v *= 10);
	    */
		// this.Print(hash, transform.position.ToString("F8"));

		gate = transform.GetComponentInParent<Gate>();
		if (!this.TryComponent<SphereCollider>())
			gameObject.AddComponent<SphereCollider>().isTrigger = true;
		StartCoroutine(WaitAndAttach());
	}

	IEnumerator WaitAndAttach()
	{
		yield return new WaitForEndOfFrame();
		gate.AddLock(this);
		GetComponent<ClippableObject>().OnClip += () => gate.AddLock(this);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Key") && other.gameObject.layer == gameObject.layer)
			gate.Unlock(this, other.gameObject);
	}
}
