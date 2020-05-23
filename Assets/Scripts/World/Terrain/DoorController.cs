using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// Set door animation to play at current scene position
public class DoorController : MonoBehaviour
{
	public Animator anim;
	Player player;
	public bool spawned = false;

	// Use this for initialization
	void Start()
	{
		player = Player.Instance;
		// transform.parent.position = transform.position;
		anim = GetComponentInChildren<Animator>();
		gameObject.SetActive(false);
	}

	void Update() { }

	public void Spawn()
	{
		spawned = true;
		gameObject.SetActive(true);
		Debug.Log(player);
		Vector3 doorPos = (player.transform.forward * 10) + new Vector3(player.transform.position.x, 0.6f, player.transform.position.z);
		transform.position = doorPos;
		transform.rotation = Quaternion.LookRotation(player.transform.forward);
		anim.Play("doorDrop");
	}
}
