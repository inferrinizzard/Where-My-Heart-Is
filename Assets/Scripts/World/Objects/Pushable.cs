using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Pushable : InteractableObject
{
	[SerializeField] float pushDistance = 3;
	private Vector3 spawn;
	Rigidbody rb;
	BoxCollider trigger;
	bool inRange = false, isPushing = false;

	protected override void Start()
	{
		base.Start();
		prompt = "Press E to Start Pushing";
		spawn = transform.position;
		rb = GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
		trigger = GetComponent<BoxCollider>() ?? gameObject.AddComponent<BoxCollider>();

		trigger.isTrigger = true;
		trigger.size = new Vector3(pushDistance, 1, pushDistance);
	}

	void Update()
	{
		if (transform.position.y < Player.Instance.deathPlane.transform.position.y)
			Reset();
	}

	public override void Interact()
	{
		if (inRange)
			if (isPushing)
				StopPushing();
			else
				BeginPushing();
	}

	void BeginPushing()
	{
		isPushing = true;
		rb.isKinematic = false;
		prompt = "Press E to Stop Pushing";
	}
	void StopPushing()
	{
		isPushing = false;
		rb.isKinematic = true;
		prompt = "Press E to Start Pushing";
	}
	void Reset()
	{
		transform.position = spawn;
		rb.velocity = Vector3.zero;
	}

	void OnTriggerEnter(Collider other)
	{
		inRange = true;
	}

	void OnTriggerStay(Collider other)
	{
		if (isPushing)
		{
			rb.velocity = Player.Instance.body.velocity;
		}
	}

	void OnTriggerExit(Collider other)
	{
		inRange = false;
		StopPushing();
	}
}
