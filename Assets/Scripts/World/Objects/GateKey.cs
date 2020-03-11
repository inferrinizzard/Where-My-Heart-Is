using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GateKey : Pickupable
{
	[SerializeField] Gate gate = default;
	[SerializeField] float distanceThreshold = .5f;

	void Awake() => dissolves = true;

	public override void Interact()
	{
		if (GateCheck())
		{
			gate.Open();
			Destroy(gameObject);
		}
		base.Interact();
	}

	private void Update()
	{
		if (GateCheck())
			player.prompt.Enable().SetText("Press E to Unlock");
	}

	bool GateCheck() => Vector3.Distance(transform.position, gate.keyHole.transform.position) < distanceThreshold;
}
