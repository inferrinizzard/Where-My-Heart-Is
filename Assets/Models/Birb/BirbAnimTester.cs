using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirbAnimTester : MonoBehaviour
{
	public KeyCode idleKey = KeyCode.Alpha1;
	public KeyCode flyKey = KeyCode.Alpha2;
	Animator anim;

    public float GlidePercent = 0.4f;
    public float PreenPercent = 0.4f;
	// Start is called before the first frame update
	void Start()
	{
		anim = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(flyKey))
        {
            anim.SetBool("IsFlying", true);
            anim.SetBool("IsIdle", false);
        }
		if (Input.GetKeyDown(idleKey))
        {
            anim.SetBool("IsIdle", true);
            anim.SetBool("IsFlying", false);
        }
	}

    public void _UpdateFlyState()
    {
        if (Random.Range(0f, 1f) < GlidePercent)
            anim.SetInteger("FlyingState", 1);
        else
            anim.SetInteger("FlyingState", 0);
    }

    public void _UpdateIdleState()
    {
        if (Random.Range(0f, 1f) < PreenPercent)
            anim.SetInteger("IdleState", 1);
        else
            anim.SetInteger("IdleState", 0);
    }
}
