using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// Set door animation to play at current scene position
public class DoorController : MonoBehaviour
{
	Animator anim;
	Player player;
	public bool spawned = false;
	GameObject holder;
	[SerializeField] GameObject house = default, blocker = default;

	void Start()
	{
		holder = transform.parent.gameObject;
		player = Player.Instance;
		anim = GetComponent<Animator>();

		foreach (Renderer r in house.GetComponentsInChildren<Renderer>())
			foreach (Material m in r.materials)
				m.renderQueue = 3002;

		holder.SetActive(false);
	}

	public void Spawn()
	{
		spawned = true;
		holder.SetActive(true);
		Vector3 doorPos = (player.transform.forward * 10) + new Vector3(player.transform.position.x, 0.6f, player.transform.position.z);
		holder.transform.position = doorPos;
		holder.transform.rotation = Quaternion.LookRotation(player.transform.forward);
		anim.Play("doorDrop");
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
