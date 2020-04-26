using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;

public class ApplyOutline : MonoBehaviour
{
	Camera cam;
	public static CommandBuffer glowBuffer;
	public static bool rebuild = false;

	int glowTemp = Shader.PropertyToID("_GlowTemp");

	void Start()
	{
		cam = GetComponent<Camera>();
		glowBuffer = new CommandBuffer();
		glowBuffer.name = "Glow Map Buffer";

		cam.AddCommandBuffer(CameraEvent.BeforeLighting, glowBuffer);
	}

	private void Cleanup()
	{
		if (glowBuffer != null && cam)
			cam.RemoveCommandBuffer(CameraEvent.BeforeLighting, glowBuffer);
	}

	public void OnDisable() => Cleanup();

	public void OnEnable() => Cleanup();

	void LateUpdate()
	{
		if (rebuild)
		{
			glowBuffer.Clear();
			glowBuffer.GetTemporaryRT(glowTemp, -1, -1, 24, FilterMode.Bilinear);
			glowBuffer.SetRenderTarget(glowTemp);
			glowBuffer.ClearRenderTarget(true, true, Color.clear);
			rebuild = false;
		}
	}

	void OnPreCull() => glowBuffer.SetGlobalTexture("_GlowMap", glowTemp);
}
