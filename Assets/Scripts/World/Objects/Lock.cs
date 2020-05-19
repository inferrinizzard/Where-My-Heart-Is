using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Lock : MonoBehaviour
{
	Gate gate;
	void Start()
	{
		gate = transform.GetComponentInParent<Gate>();
		if (!this.TryComponent<SphereCollider>())
			gameObject.AddComponent<SphereCollider>().isTrigger = true;
		StartCoroutine(WaitAndAttach());
	}

	IEnumerator WaitAndAttach() { yield return new WaitForEndOfFrame(); gate.AddLock(this); }

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Key") && other.gameObject.layer == gameObject.layer)
			gate.Unlock(this, other.gameObject);
	}
}
