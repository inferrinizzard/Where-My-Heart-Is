using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary> Controls the headbob animation of the player camera. </summary>
public class Headbobber : MonoBehaviour
{
	/// <summary> Internal timer for animation. </summary>
	private float timer = 0f;
	/// <summary> The speed of the bobbing animation. </summary>
	public float bobbingSpeed = 0.18f;
	/// <summary> The amount the camera will bob. </summary>
	public float bobbingAmount = 0.05f;
	/// <summary> The midpoint of the camera. </summary>
	public float midpoint = 0f;

	Rigidbody rb;

	void Start() => rb = GetComponentInParent<Rigidbody>();

	void Update()
	{
		float waveslice = 0f;

		// Check if the player is moving.
		float horizontal = rb.velocity.x;
		float vertical = rb.velocity.z;

		// If the player is not moving, don't do anything
		if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
		{
			timer = 0f;
		}
		// If the player is doing something, start the timer.
		else if (!PauseMenu.GameIsPaused)
		{
			waveslice = Mathf.Sin(timer);
			timer += bobbingSpeed;
			if (timer > (Mathf.PI * 2))
				timer -= (Mathf.PI * 2);
		}

		// If the timer is going, bob the camera.
		if (waveslice != 0 && Player.Instance.IsGrounded())
		{
			float translateChange = waveslice * bobbingAmount;
			float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
			totalAxes = Mathf.Clamp(totalAxes, 0f, 1f);
			translateChange *= totalAxes;
			transform.localPosition = new Vector3(transform.localPosition.x, midpoint + translateChange, transform.localPosition.z);
		}
		else
			transform.localPosition = new Vector3(transform.localPosition.x, midpoint, transform.localPosition.z);
	}
}
