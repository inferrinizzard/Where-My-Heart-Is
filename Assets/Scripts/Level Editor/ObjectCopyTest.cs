using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ObjectCopyTest : MonoBehaviour
{
	public bool testNow = false;
	public GameObject toDuplicate;

	private void Update()
	{
		if (testNow)
		{
			Instantiate(toDuplicate, toDuplicate.transform.position, toDuplicate.transform.rotation, transform);
			testNow = false;
		}
	}
}
