using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
	/// <summary> reference to Mask controller </summary>
	ApplyMask mask;

	void Awake()
	{
		mask = GetComponent<ApplyMask>();
	}

	void Start()
	{
		ToggleGlowOutline(true);
	}

	/// <summary> toggles mask on and off </summary>
	/// <param name="on"> Is mask on? </summary>
	public void ToggleMask(bool on) => mask.enabled = on;

	/// <summary> toggles glow outline on and off </summary>
	/// <param name="on"> Is glow outline on? </summary>
	public void ToggleGlowOutline(bool on)
	{
		if (on)
			Shader.EnableKeyword("OUTLINE_GLOW");
		else
			Shader.DisableKeyword("OUTLINE_GLOW");
	}

	// need:
	// turn bloom on and off
	// EdgeOutline
}
