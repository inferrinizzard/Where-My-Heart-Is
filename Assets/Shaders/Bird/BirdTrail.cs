using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BirdTrail : MonoBehaviour
{
	[SerializeField] int count = 3;
	[SerializeField] int length = 60;
	List<Trans> copies = new List<Trans>();

	int step = 0;

	public GameObject test;

	void Start()
	{

	}

	struct Trans
	{
		public Vector3 position;
		public Quaternion rotation;
		public Vector3 eulerAngles => rotation.eulerAngles;
		public Vector3 scale;

		public Trans(Transform t)
		{
			position = t.position;
			rotation = t.rotation;
			scale = t.localScale;
		}
	}

	void FixedUpdate()
	{
		step = ++step % length;
		if (step == 0)
			copies.Add(new Trans(transform));
		if (copies.Count > count)
			copies.RemoveAt(0);
	}

	void Update()
	{
		foreach (Trans t in copies)
			this.DrawCube(t.position, 1, t.rotation, Color.red, depthCheck : true);
	}
}
