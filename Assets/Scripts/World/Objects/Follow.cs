using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Follow : MonoBehaviour
{
	[SerializeField] Transform target;
	ClippableObject clippableObject;
	void Start()
	{
		clippableObject = GetComponent<ClippableObject>();
	}

	void Update()
	{
		if (clippableObject && !clippableObject.isClipped)
			transform.position = target.position;
	}
}
