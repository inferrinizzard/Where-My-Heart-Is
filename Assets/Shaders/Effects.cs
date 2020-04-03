#define DEBUG
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Effects : MonoBehaviour
{
	Fade fadeController;
	bool outlineOn = true;
	bool bloomOn = false;
	bool dissolveOn = false;
	[SerializeField] Material defaultGlowMat = default;

	void Start()
	{
		fadeController = GetComponent<Fade>();

		ToggleMask(false);
		ToggleEdgeOutline(outlineOn);
		ToggleDissolve(dissolveOn);
	}

	void Update()
	{
#if DEBUG // debug toggles
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			ToggleEdgeOutline(outlineOn = !outlineOn);
			print($"edge: {outlineOn}");
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			ToggleBloom(bloomOn = !bloomOn);
			print($"bloom: {bloomOn}");
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			ToggleDissolve(dissolveOn = !dissolveOn);
			print($"dissolve: {dissolveOn}");
		}
#endif
	}

	/// <summary> toggles mask on and off </summary>
	/// <param name="on"> Is mask on? </summary>
	// public void ToggleMask(bool on) => mask.enabled = on;
	public void ToggleMask(bool on) => ToggleEffect(on, "MASK");

	/// <summary> toggles edge outline on and off </summary>
	/// <param name="on"> Is edge outline on? </summary>
	public void ToggleEdgeOutline(bool on) => ToggleEffect(on, "OUTLINE");

	public void ToggleBloom(bool on) => ToggleEffect(on, "BLOOM");

	public void ToggleDissolve(bool on) => ToggleEffect(on, "DISSOLVE");

	public void StartFade(bool fadingIn, float dur) => fadeController.StartFade(fadingIn, dur);

	void ToggleEffect(bool on, string keyword)
	{
		if (on)
			Shader.EnableKeyword(keyword);
		else
			Shader.DisableKeyword(keyword);
	}

	public void RenderGlowMap(Renderer[] renderers, Material mat = null)
	{
		mat = mat ?? defaultGlowMat;
		// mat.SetColor("_Colour", col);
		foreach (Renderer r in renderers)
			ApplyOutline.glowBuffer.DrawRenderer(r, mat);
	}
}
