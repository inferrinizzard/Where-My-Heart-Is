using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class OutlineObject : MonoBehaviour
{
	[SerializeField] Shader outlineShader = default;
	// <summary> If the outline cares about occlusion (probably only canvas is false) </summary>
	[SerializeField] bool depthCheck = true;
	// <summary> Glow outline colour </summary>
	[SerializeField] Color outlineColour = Color.blue;
	Material outlineMat;
	public Material Colour { get => outlineMat; }

	Renderer[] renderers;

	void Start()
	{
		renderers = GetComponentsInChildren<Renderer>();
	}

	void OnEnable()
	{
		outlineMat = new Material(outlineShader);
		outlineMat.SetColor("_Colour", outlineColour);
		outlineMat.SetInt("_Occlude", depthCheck ? 1 : 0);
		outlineMat.name = $"[{name}] Outline";
	}

	// void OnWillRenderObject() => GameManager.Instance.VFX.RenderGlowMap(renderers, outlineMat);
	void OnWillRenderObject()
	{
		this.Print("owro", this);
		GameManager.Instance.VFX.RenderGlowMap(renderers, outlineMat);
	}

}
