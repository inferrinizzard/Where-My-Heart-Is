using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Pushable : InteractableObject
{
	[SerializeField] float pushDistance = 3;
	string _prompt = "Press E to Start Pushing";
	public override string prompt { get => _prompt; set => _prompt = value; }
	private Vector3 spawn;
	Rigidbody rb;
	BoxCollider trigger;
	bool inRange = false, isPushing = false;

	protected override void Start()
	{
		base.Start();
		spawn = transform.position;
		rb = this.TryComponent<Rigidbody>() ? GetComponent<Rigidbody>() : gameObject.AddComponent<Rigidbody>();
		rb.constraints = ~RigidbodyConstraints.FreezePositionY;

		trigger = this.TryComponent<BoxCollider>() ? GetComponent<BoxCollider>() : gameObject.AddComponent<BoxCollider>();
		trigger.isTrigger = true;
		trigger.size = new Vector3(pushDistance, 2, pushDistance);
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
		rb.constraints = RigidbodyConstraints.FreezeRotation;
		prompt = "Press E to Stop Pushing";
		Effects.Instance.SetGlow(this, Color.white);
	}
	void StopPushing()
	{
		isPushing = false;
		rb.constraints = ~RigidbodyConstraints.FreezePositionY;
		prompt = "Press E to Start Pushing";
		Effects.Instance.SetGlow(this);
	}
	void Reset()
	{
		transform.position = spawn;
		rb.velocity = Vector3.zero;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			inRange = true;
			if (!this.TryComponent<OutlineObject>())
				Effects.Instance.SetGlow(this);
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			inRange = true;
			if (isPushing)
			{
				rb.velocity = new Vector3(Player.Instance.body.velocity.x, rb.velocity.y, Player.Instance.body.velocity.z);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			inRange = false;
			StopPushing();
			Effects.Instance.SetGlow(null);
		}
	}
}
