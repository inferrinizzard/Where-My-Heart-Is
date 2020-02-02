using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntangledObjectManager : MonoBehaviour
{
	public GameObject sharedObjectPrefab;

	public void AddObject()
	{
		GameObject createdObject = Instantiate(sharedObjectPrefab);
		createdObject.transform.parent = transform;
		createdObject.AddComponent<EntangledClipable>();
		createdObject.layer = 9;
	}
}
