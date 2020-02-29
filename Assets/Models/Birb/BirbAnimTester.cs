using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirbAnimTester : MonoBehaviour
{
	public KeyCode idleKey = KeyCode.Alpha1;
	public KeyCode flyKey = KeyCode.Alpha2;
	Animator anim;
	// Start is called before the first frame update
	void Start()
	{
		anim = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(flyKey))
			anim.SetBool("IsFlying", true);
		if (Input.GetKeyDown(idleKey))
			anim.SetBool("IsIdle", true);
	}

	public void setFlying()
	{
		anim.SetBool("IsFlying", true);
	}
}
