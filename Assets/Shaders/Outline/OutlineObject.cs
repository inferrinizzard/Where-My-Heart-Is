using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class OutlineObject : MonoBehaviour
{
	// <summary> If the outline cares about occlusion (probably only canvas is false) </summary>
	[SerializeField] bool depthCheck = true;
	// <summary> Glow outline colour </summary>
	[SerializeField] Color outlineColour = Color.blue;
	Material outlineMat;
	Renderer[] renderers;

	void Start() => renderers = GetComponentsInChildren<Renderer>();

	void OnEnable()
	{
		outlineMat = new Material(Shader.Find("Outline/GlowObject"));
		outlineMat.color = outlineColour;
		outlineMat.SetInt("_Occlude", depthCheck ? 1 : 0);
		outlineMat.name = $"[{name}] Outline";
	}

	void OnWillRenderObject()
	{
		if (Camera.current == Effects.Instance.mainCam || (Effects.Instance.maskOn && Camera.current == Effects.Instance.heartCam))
			Effects.Instance.RenderGlowMap(renderers, outlineMat);
	}
}
