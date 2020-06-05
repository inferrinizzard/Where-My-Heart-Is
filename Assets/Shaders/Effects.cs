using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Effects : MonoBehaviour
{
	public static Effects Instance;

	Fade fadeController;
	bool dissolveOn = false;
	[HideInInspector] public bool maskOn = false;

	[HideInInspector] public Camera mainCam, heartCam;

	void Awake()
	{
		Instance = this;
		mainCam = GetComponent<Camera>();
		heartCam = this.GetComponentOnlyInChildren<Camera>();

		fadeController = GetComponent<Fade>();

		ToggleWave(false);
		ToggleMask(maskOn);
		// ToggleDissolve(dissolveOn);
		// ToggleBoil(true);
		// ToggleBird(true);
		ToggleFog(false);

		glowMat = new Material(Shader.Find("Outline/GlowObject"));
		glowMat.color = Color.black;
	}

	public void SubcribeToCutEvents(Window window)
	{
		window.OnClippableCut += SetWave;
		window.OnBeginCut += () => { ToggleWave(true); Player.Instance.mask.screenMat.SetVector(ShaderID._WaveOrigin, mainCam.transform.position); };
		window.OnCompleteCut += () => { ToggleWave(false); SetWave(0); };
	}

	#region toggles

	/// <summary> toggles mask on and off </summary>
	/// <param name="on"> Is mask on? </summary>
	// public void ToggleMask(bool on) => mask.enabled = on;
	public void ToggleMask(bool on) => ToggleEffect(maskOn = on, "MASK");

	/// <summary> toggles edge outline on and off </summary>
	/// <param name="on"> Is edge outline on? </summary>
	// public void ToggleEdgeOutline(bool on) => ToggleEffect(on, "OUTLINE");
	public void ToggleDissolve(bool on) => ToggleEffect(on, "DISSOLVE");
	public void ToggleBoil(bool on) => ToggleEffect(on, "BOIL");
	public void ToggleWave(bool on) => ToggleEffect(on, "WAVE");
	public void ToggleBird(bool on) => ToggleEffect(on, "BIRD");
	public void ToggleFog(bool on) => ToggleEffect(on, "FOG");

	public void StartFade(bool fadingIn, float dur) => fadeController.StartFade(fadingIn, dur);

	// public void SetWave(float distance) => waveController.waveDistance = distance;
	public void SetWave(float distance) => Player.Instance.mask.screenMat.SetFloat(ShaderID._WaveDistance, distance);
	public void SetWave(ClippableObject clippable) => Player.Instance.mask.screenMat.SetFloat(ShaderID._WaveDistance, (clippable.transform.position - transform.position).magnitude);

	void ToggleEffect(bool on, string keyword)
	{
		if (on)
			Shader.EnableKeyword(keyword);
		else
			Shader.DisableKeyword(keyword);
	}
	#endregion

	[SerializeField] OutlineColours glowColours = default;
	Material glowMat;
	[HideInInspector] public InteractableObject currentGlowObj;
	Color targetColour = Color.black;
	Coroutine glowRoutine;

	public void RenderGlowMap(Renderer[] renderers, Material mat)
	{
		ApplyOutline.drawGlow = true;
		foreach (Renderer r in renderers)
			ApplyOutline.glowBuffer.DrawRenderer(r, mat);
	}

	public void SetGlow(InteractableObject obj, Color custom = default)
	{
		targetColour = custom != Color.clear ? custom : glowColours[obj];
		if (glowRoutine != null)
			StopCoroutine(glowRoutine);
		if (obj)
		{
			ApplyOutline.drawGlow = true;
			glowRoutine = StartCoroutine(RenderGlowLerp(obj.renderers));
			currentGlowObj = obj;
		}
		else
			glowRoutine = StartCoroutine(RenderGlowLerp(currentGlowObj?.renderers, off : true));
	}

	IEnumerator RenderGlowLerp(Renderer[] renderers, float time = 2, bool off = false)
	{
		for (float step = time; step > 0; step -= Time.deltaTime)
		{
			yield return null;

			glowMat.color = Color.Lerp(glowMat.color, targetColour, Time.deltaTime * time);

			foreach (Renderer r in renderers)
				if (r.isVisible)
					ApplyOutline.glowBuffer.DrawRenderer(r, glowMat);

			if (glowMat.color.Equals(targetColour) || renderers.Length == 0)
				yield break;
		}
		if (off)
			currentGlowObj = null;
	}
}
