using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
	/// <summary> reference to Mask controller </summary>
	ApplyMask mask;

	public void Awake()
	{
		mask = GetComponent<ApplyMask>();
	}

	/// <summary> toggles mask on and off </summary>
	/// <param name="on"> Is mask on? </summary>
	public void ToggleMask(bool on) => mask.enabled = on;

	// need:
	// GlowOutline
	// turn bloom on and off
	// EdgeOutline
}
