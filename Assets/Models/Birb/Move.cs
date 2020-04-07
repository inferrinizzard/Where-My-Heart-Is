using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

public class Move : MonoBehaviour
{
	void Start()
	{
		// var bam = GetComponent<BirbAnimTester>();
		// var b2 = gameObject.AddComponent<BirbAnimTester>();
		// EditorUtility.CopySerialized(bam, b2);
		GetComponent<Animator>().SetBool("IsFlying", true);
	}

	void Update()
	{
		transform.position = new Vector3(5 * Mathf.Cos(Time.time), Mathf.Sin(Time.time / 10) / 2 + 1, 5 * Mathf.Sin(Time.time));
		transform.LookAt(Vector3.Cross(new Vector3(5 * Mathf.Cos(Time.time), 0, 5 * Mathf.Sin(Time.time)), Vector3.up));
	}
}
