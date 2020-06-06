using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Pushable : InteractableObject
{
	[SerializeField] float pushDistance = 4;
	public override string prompt { get => $"Press {InputManager.InteractKey} to {(isPushing ? "Stop" : "Start")} Moving Box"; }

	[Header("Audio")]
	[FMODUnity.EventRef]
	public string pushEvent;
	[FMODUnity.EventRef]
	public string impactEvent;
	public float maxPushSpeed;
	public float minPushSpeed;
	public float groundOffsetX;
	public float groundOffsetY;
	public float groundOffsetZ;

	private Vector3 spawn;
	private Rigidbody rb;
	private BoxCollider trigger;
	private bool inRange = false, isGrounded = true;
	[HideInInspector] public bool isPushing = false;

	private FMOD.Studio.EventInstance pushInstance;

	protected override void Start()
	{
		base.Start();
		spawn = transform.position;
		rb = this.TryComponent<Rigidbody>() ? GetComponent<Rigidbody>() : gameObject.AddComponent<Rigidbody>();
		rb.constraints = ~RigidbodyConstraints.FreezePositionY;

		trigger = this.TryComponent<BoxCollider>() ? GetComponent<BoxCollider>() : gameObject.AddComponent<BoxCollider>();
		trigger.isTrigger = true;
		trigger.size = new Vector3(pushDistance, 2, pushDistance);

		pushInstance = FMODUnity.RuntimeManager.CreateInstance(pushEvent);
		FMODUnity.RuntimeManager.AttachInstanceToGameObject(pushInstance, transform, GetComponent<Rigidbody>());
		pushInstance.start();
		pushInstance.setParameterByName("Push Speed", 0);
	}

	void Update()
	{
		if (transform.position.y < Player.Instance.deathPlane.transform.position.y)
			Reset();

		if (isPushing)
		{
			UpdateAudio();
		}

		UpdateGrounded();
	}

	public override void Interact()
	{
		if (inRange)
			if (isPushing)
				StopPushing();
			else
				BeginPushing();
	}

	private void BeginPushing()
	{
		isPushing = true;
		rb.constraints = RigidbodyConstraints.FreezeRotation;
		Effects.Instance.SetGlow(this, Color.white);
	}

	private void StopPushing()
	{
		isPushing = false;
		rb.constraints = ~RigidbodyConstraints.FreezePositionY;
		Effects.Instance.SetGlow(this);

		pushInstance.setParameterByName("Push Speed", 0);
	}

	private void UpdateAudio()
	{
		if (isGrounded)
		{
			float speed = (GetComponent<Rigidbody>().velocity.magnitude - minPushSpeed) / (maxPushSpeed - minPushSpeed);
			speed = Mathf.Clamp(speed, 0, 1);
			pushInstance.setParameterByName("Push Speed", speed);
		}
		else
		{
			pushInstance.setParameterByName("Push Speed", 0);
		}
	}

	private bool IsGrounded()
	{
		// opposite diagonals
		Vector3 frontOrigin = transform.localToWorldMatrix.MultiplyPoint(new Vector3(groundOffsetX, groundOffsetY, groundOffsetZ));
		Vector3 backOrigin = transform.localToWorldMatrix.MultiplyPoint(new Vector3(-groundOffsetX, groundOffsetY, -groundOffsetZ));

		RaycastHit hit;

		if (Physics.Raycast(frontOrigin, Vector3.down, out hit, 1f))
		{
			if (hit.collider.gameObject.CompareTag("Untagged"))
			{
				return true;
			}
		}

		Debug.DrawLine(frontOrigin, frontOrigin + Vector3.down, Color.red, 4);

		if (Physics.Raycast(backOrigin, Vector3.down, out hit, 1f))
		{
			if (hit.collider.gameObject.CompareTag("Untagged"))
			{
				return true;
			}
		}

		return false;
	}

	private void UpdateGrounded()
	{
		if (isGrounded != IsGrounded())
		{
			isGrounded = !isGrounded;
			//if we landed
			if (isGrounded)
			{
				if (GetComponent<Rigidbody>().velocity.y < -0.2)
				{
					Debug.Log(GetComponent<Rigidbody>().velocity.y);
					FMODUnity.RuntimeManager.PlayOneShotAttached(impactEvent, gameObject);
				}
			}
		}

	}

	private void Reset()
	{
		transform.position = spawn;
		rb.velocity = Vector3.zero;
	}

	/*private void OnCollisionEnter(Collision collision)
	{
	    
	}*/

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
