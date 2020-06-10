using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

// Set door animation to play at current scene position
public class DoorController : InteractableObject
{
	[HideInInspector] public Animator anim;
	public bool spawned = false;
	[SerializeField] GameObject house = default, holder = default;
	[SerializeField] Transform closeTrigger = default;
	public GameObject blocker;
	bool snowstormLevel = false;

	public BoxCollider doorCollider;

	[FMODUnity.EventRef]
	public string doorLocked;

	[FMODUnity.EventRef]
	public string openDoor;

	[FMODUnity.EventRef]
	public string closeDoor;
	protected override void Start()
	{
		base.Start();
		anim = GetComponentInParent<Animator>();

		if (Player.Instance.GetComponentInChildren<Snowstorm>())
			snowstormLevel = true;

		if (house)
			foreach (Renderer r in house.GetComponentsInChildren<Renderer>())
				foreach (Material m in r.materials)
					m.renderQueue = 3002;

		if (snowstormLevel)
			holder.SetActive(false);
		else
			GetComponent<Collider>().isTrigger = false;
	}

	public override void Interact()
	{
		base.Interact();
		if (!snowstormLevel)
		{
			anim.Play("Locked");
			FMODUnity.RuntimeManager.PlayOneShot(doorLocked, transform.position);
			Debug.Log("locked door you idiot");
		}
		else
		{
			anim.Play("Opening- Slow");
			FMODUnity.RuntimeManager.PlayOneShot(openDoor, transform.position);
			blocker.SetActive(false);
			foreach (Renderer r in house.GetComponentsInChildren<Renderer>())
				foreach (Material m in r.materials)
					m.renderQueue = 2000;
			// delete the trees in an area
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);
			foreach (Collider col in hitColliders)
			{
				if (col.gameObject.tag == "Tree")
				{
					Destroy(col.gameObject);
				}
			}
		}
	}

	public void Spawn()
	{
		spawned = true;
		holder.SetActive(true);
		Vector3 doorPos = (player.transform.forward * 10) + new Vector3(player.transform.position.x, 0.4f, player.transform.position.z);
		holder.transform.position = doorPos;
		holder.transform.rotation = Quaternion.LookRotation(player.transform.forward);
	}

	void OnTriggerEnter()
	{
		blocker.GetComponent<Collider>().enabled = false;
	}

	void OnTriggerExit()
	{
		blocker.GetComponent<Collider>().enabled = true;

		if (Player.Instance.playerCollider.bounds.Intersects(closeTrigger.GetComponent<Collider>().bounds))
		{
			anim.Play("Slam");
			Destroy(this);
			doorCollider.enabled = true;
			FMODUnity.RuntimeManager.PlayOneShot(closeDoor, transform.position);
		}
	}
}
