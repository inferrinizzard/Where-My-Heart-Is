using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateKey : Pickupable
{
	public Gate gate;
	public float distanceThreshold;

	public void Update()
	{

	}

	public override void Interact()
	{
		if (GateCheck())
		{
			gate.Open();
			Destroy(gameObject);
		}
		base.Interact();
	}

	public bool GateCheck() => Vector3.Distance(transform.position, gate.keyHole.transform.position) < distanceThreshold;
}
