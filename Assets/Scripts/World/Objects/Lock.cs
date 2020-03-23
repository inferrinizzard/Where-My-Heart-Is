using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Lock : InteractableObject
{
	public GameObject oneVersion;
	public GameObject otherVersion;
	public GameObject otherLock;
	public GameObject key;

	public override void Interact()
	{
		if (key.activeSelf == false)
		{
			oneVersion.SetActive(false);
			otherVersion.SetActive(false);
			otherLock.SetActive(false);
			this.gameObject.SetActive(false);

		}
	}
}
