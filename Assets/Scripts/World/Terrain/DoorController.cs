using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// Set door animation to play at current scene position
public class DoorController : InteractableObject
{
	Animator anim;
	public bool spawned = false;
	[SerializeField] GameObject house = default, blocker = default, holder = default;
	bool snowstormLevel = false;

	protected override void Start()
	{
		base.Start();
		anim = GetComponentInParent<Animator>();

		if (Player.Instance.GetComponentInChildren<Snowstorm>())
			snowstormLevel = true;

		// holder = transform.parent.gameObject;

		if (house)
			foreach (Renderer r in house.GetComponentsInChildren<Renderer>())
				foreach (Material m in r.materials)
					m.renderQueue = 3002;

		if (snowstormLevel)
			holder.SetActive(false);
	}

	public override void Interact()
	{
		base.Interact();
		if (!snowstormLevel)
			anim.Play("Locked");
		else
			anim.Play("Opening- Slow");
		// Debug.Log("door");
	}

	public void Spawn()
	{
		spawned = true;
		holder.SetActive(true);
		Vector3 doorPos = (player.transform.forward * 10) + new Vector3(player.transform.position.x, 0.6f, player.transform.position.z);
		holder.transform.position = doorPos;
		holder.transform.rotation = Quaternion.LookRotation(player.transform.forward);
		// anim.Play("doorDrop");
	}

	public void Open() => anim.SetTrigger("Open");

	void OnTriggerEnter()
	{
		blocker.GetComponent<Collider>().enabled = false;
		foreach (var c in GetComponentsInChildren<Collider>())
			c.enabled = false;
	}

	void OnTriggerExit()
	{
		blocker.GetComponent<Collider>().enabled = true;
		foreach (var c in GetComponentsInChildren<Collider>())
			c.enabled = true;
	}
}
