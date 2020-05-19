using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Opens gate if all locks are unlocked, then destroy key
/// </summary>
public class GateMultipleKeys : Pickupable
{
	[SerializeField] GateMultipleLocks gate = default;
	[SerializeField] GateMultipleLocks gateTwo = default;
	[SerializeField] float distanceThreshold = .5f;
	[SerializeField] int keyProngs = 1;

	private static bool leftUnlocked;
	private static bool rightUnlocked;

	void Awake() => dissolves = true;

	public override void Interact()
	{
		if (GateCheck())
		{
			gate.keyHole.GetComponent<Renderer>().material.SetColor("_Color", new Color(0.6f, 1, 0.6f));
			leftUnlocked = true;
			gameObject.SetActive(false);

			PutDown();
		}
		else if (GateCheckRight())
		{
			gate.keyHoleTwo.GetComponent<Renderer>().material.SetColor("_Color", new Color(0.6f, 1, 0.6f));
			rightUnlocked = true;
			gameObject.SetActive(false);

			PutDown();
		}
		else if (GateTwoCheck())
		{
			gateTwo.Open();
			gameObject.SetActive(false);

			PutDown();
		}
		if (leftUnlocked && rightUnlocked)
		{
			gate.Open();
			Destroy(gameObject);
		}
		base.Interact();

	}

	private void Update()
	{
		if (GateCheck() || GateTwoCheck())
			player.prompt.SetText("Press E to Unlock with 1-Pronged Key");
		else if (GateCheckRight())
			player.prompt.SetText("Press E to Unlock with 3-Pronged Key");
	}

	// Main Gate checks
	bool GateCheck() => Vector3.Distance(transform.position, gate.keyHole.transform.position) < distanceThreshold && gate.keyHole.layer == 9 && keyProngs == 1;
	bool GateCheckRight() => Vector3.Distance(transform.position, gate.keyHoleTwo.transform.position) < distanceThreshold && gate.keyHoleTwo.layer == 9 && keyProngs == 3;
	
	// Island 2 Gate Check
	bool GateTwoCheck() => Vector3.Distance(transform.position, gateTwo.keyHole.transform.position) < distanceThreshold && gateTwo.keyHole.layer == 9 && keyProngs == 1;
}
