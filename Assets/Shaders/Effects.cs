// #define DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
	bool glowOn = false;
	bool edgeOn = true;
	bool bloomOn = true;
	bool dissolveOn = false;

	void Start()
	{
		ToggleMask(false);
		ToggleGlowOutline(glowOn);
		ToggleEdgeOutline(edgeOn);
		ToggleDissolve(dissolveOn);
	}

#if DEBUG
	void Update()
	{ // debug toggles
		if (Input.GetKeyDown(KeyCode.Alpha1))
			ToggleGlowOutline(glowOn = !glowOn);
		if (Input.GetKeyDown(KeyCode.Alpha2))
			ToggleEdgeOutline(edgeOn = !edgeOn);
		if (Input.GetKeyDown(KeyCode.Alpha3))
			ToggleBloom(bloomOn = !bloomOn);
		if (Input.GetKeyDown(KeyCode.Alpha4))
			ToggleDissolve(dissolveOn = !dissolveOn);
	}
#endif

	/// <summary> toggles mask on and off </summary>
	/// <param name="on"> Is mask on? </summary>
	// public void ToggleMask(bool on) => mask.enabled = on;
	public void ToggleMask(bool on) => ToggleEffect(on, "MASK");

	/// <summary> toggles glow outline on and off </summary>
	/// <param name="on"> Is glow outline on? </summary>
	public void ToggleGlowOutline(bool on) => ToggleEffect(on, "OUTLINE_GLOW");

	/// <summary> toggles edge outline on and off </summary>
	/// <param name="on"> Is edge outline on? </summary>
	public void ToggleEdgeOutline(bool on) => ToggleEffect(on, "OUTLINE_EDGE");

	public void ToggleBloom(bool on) => ToggleEffect(on, "BLOOM");

	public void ToggleDissolve(bool on) => ToggleEffect(on, "DISSOLVE");

	void ToggleEffect(bool on, string keyword)
	{
		if (on)
			Shader.EnableKeyword(keyword);
		else
			Shader.DisableKeyword(keyword);
	}
}
