using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
	/// <summary> reference to Mask controller </summary>
	ApplyMask mask;

	bool glowOn = false;
	bool edgeOn = false;
	bool bloomOn = true;

	void Awake()
	{
		mask = GetComponent<ApplyMask>();
	}

	void Start()
	{
		ToggleGlowOutline(glowOn);
		ToggleEdgeOutline(edgeOn);
	}

	void Update()
	{ // debug toggles
		if (Input.GetKeyDown(KeyCode.Alpha1))
			ToggleGlowOutline(glowOn = !glowOn);
		if (Input.GetKeyDown(KeyCode.Alpha2))
			ToggleEdgeOutline(edgeOn = !edgeOn);
		if (Input.GetKeyDown(KeyCode.Alpha3))
			ToggleBloom(bloomOn = !bloomOn);
	}

	/// <summary> toggles mask on and off </summary>
	/// <param name="on"> Is mask on? </summary>
	// public void ToggleMask(bool on) => mask.enabled = on;
	public void ToggleMask(bool on)
	{
		if (on)
			Shader.EnableKeyword("MASK");
		else
			Shader.DisableKeyword("MASK");
	}

	/// <summary> toggles glow outline on and off </summary>
	/// <param name="on"> Is glow outline on? </summary>
	public void ToggleGlowOutline(bool on)
	{
		if (on)
			Shader.EnableKeyword("OUTLINE_GLOW");
		else
			Shader.DisableKeyword("OUTLINE_GLOW");
	}

	/// <summary> toggles edge outline on and off </summary>
	/// <param name="on"> Is edge outline on? </summary>
	public void ToggleEdgeOutline(bool on)
	{
		if (on)
			Shader.EnableKeyword("OUTLINE_EDGE");
		else
			Shader.DisableKeyword("OUTLINE_EDGE");
	}

	public void ToggleBloom(bool on)
	{
		if (on)
			Shader.EnableKeyword("BLOOM");
		else
			Shader.DisableKeyword("BLOOM");
	}

}
