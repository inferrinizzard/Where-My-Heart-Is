// #define DEBUG
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[ExecuteInEditMode]
public class Effects : MonoBehaviour
{
	Fade fadeController;
	Wave waveController;
	bool glowOn = false;
	bool edgeOn = true;
	bool bloomOn = true;
	bool dissolveOn = false;

	[SerializeField, Range(0, 30)] float lightPower = 5;

	void Awake()
	{
		Shader.SetGlobalFloat("_LightAttenBias", 30 - lightPower);
		fadeController = GetComponent<Fade>();
		waveController = GetComponent<Wave>();

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

#if UNITY_EDITOR
		Shader.SetGlobalFloat("_LightAttenBias", 30 - lightPower);
#endif
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

	public void StartFade(bool fadingIn, float dur) => fadeController.StartFade(fadingIn, dur);

	public void SetWave(float distance) => waveController.waveDistance = distance;

	void ToggleEffect(bool on, string keyword)
	{
		if (on)
			Shader.EnableKeyword(keyword);
		else
			Shader.DisableKeyword(keyword);
	}
}
