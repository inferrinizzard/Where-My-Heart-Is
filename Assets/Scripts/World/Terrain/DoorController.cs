using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// Set door animation to play at current scene position
public class DoorController : MonoBehaviour
{
	public Animator anim;

	// Use this for initialization
	void Start()
	{
		transform.parent.position = transform.position;
		anim = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update()
	{
		anim.Play("doorDrop");
	}
}
