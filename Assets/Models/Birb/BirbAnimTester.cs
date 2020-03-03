using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirbAnimTester : MonoBehaviour
{
	public KeyCode idleKey = KeyCode.Alpha1;
	public KeyCode flyKey = KeyCode.Alpha2;
	Animator anim;

    public float GlidePercent = 0.4f;
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

    public void _UpdateGlide()
    {
        if (Random.Range(0f, 1f) < GlidePercent)
            anim.SetBool("IsGliding", true);
        else
            anim.SetBool("IsGliding", false);
    }
}
