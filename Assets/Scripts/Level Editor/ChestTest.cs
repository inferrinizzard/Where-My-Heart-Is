using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ChestTest : MonoBehaviour
{
	public bool spin = false;
	// Update is called once per frame
	void Update()
	{
		if (spin)
		{
			transform.Rotate(Vector3.up, 15 * Time.deltaTime);
		}
	}
}
