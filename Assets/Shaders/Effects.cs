#define DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
	/// <summary> reference to Mask controller </summary>
	// public static ApplyMask mask;

	bool glowOn = false;
	bool edgeOn = true;
	bool bloomOn = true;
	bool dissolveOn = false;

	// void Awake()
	// {
	// 	mask = GetComponent<ApplyMask>();
	// }

	void Start()
	{
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

	public static IEnumerator DissolveOnDrop(GateKey obj, float time = .25f)
	{
		obj.transform.parent = obj.oldParent;
		Material mat = obj.GetComponent<MeshRenderer>().material;
		mat.EnableKeyword("DISSOLVE_MANUAL");
		int ManualDissolveID = Shader.PropertyToID("_ManualDissolve");

		float start = Time.time;
		bool inProgress = true;

		while (inProgress)
		{
			yield return null;
			float step = Time.time - start;
			mat.SetFloat(ManualDissolveID, step / time);
			if (step > time)
				inProgress = false;
		}
		mat.DisableKeyword("DISSOLVE_MANUAL");
		mat.SetFloat(ManualDissolveID, 1);

		obj.Interact();
		obj.active = false;
	}
}
