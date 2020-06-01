using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;

public class ApplyOutline : MonoBehaviour
{
	Camera cam;
	public static CommandBuffer glowBuffer;
	public static bool drawGlow = false;

	void Start()
	{
		cam = GetComponent<Camera>();
		glowBuffer = new CommandBuffer();
		glowBuffer.name = "Glow Map Buffer";

		cam.AddCommandBuffer(CameraEvent.BeforeLighting, glowBuffer);
		ResetGlowBuffer();
	}

	public void OnEnable() => Cleanup();
	public void OnDisable() => Cleanup();
	private void Cleanup()
	{
		if (glowBuffer != null && cam)
			cam.RemoveCommandBuffer(CameraEvent.BeforeLighting, glowBuffer);
	}

	void ResetGlowBuffer()
	{
		glowBuffer.Clear();
		glowBuffer.GetTemporaryRT(ShaderID._GlowTemp, -1, -1, 24, FilterMode.Bilinear);
		glowBuffer.SetRenderTarget(ShaderID._GlowTemp);
		glowBuffer.ClearRenderTarget(true, true, Color.clear);
	}

	void LateUpdate()
	{
		if (drawGlow)
			ResetGlowBuffer();
	}

	void OnPreCull()
	{
		if (drawGlow)
		{
			glowBuffer.SetGlobalTexture(ShaderID._GlowMap, ShaderID._GlowTemp);
			drawGlow = false;
		}
	}
}
