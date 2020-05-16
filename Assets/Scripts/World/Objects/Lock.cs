using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Lock : MonoBehaviour
{
	Gate gate;
	void Start()
	{
		gate = transform.GetComponentInParent<Gate>();
		StartCoroutine(WaitAndAttach());
	}

	IEnumerator WaitAndAttach() { yield return new WaitForEndOfFrame(); gate.AddLock(this); }

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Key") && other.gameObject.layer == gameObject.layer)
		{
			// k.Destroy();
			Destroy(other.gameObject);
			gate.Unlock(this);
		}
	}
}
