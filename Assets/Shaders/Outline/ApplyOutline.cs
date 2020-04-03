using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;

public class ApplyOutline : MonoBehaviour
{
	Camera cam;
	public static CommandBuffer glowBuffer;

	int glowTemp = Shader.PropertyToID("_GlowTemp");

	void Start()
	{
		cam = GetComponent<Camera>();
		glowBuffer = new CommandBuffer();
		glowBuffer.GetTemporaryRT(glowTemp, -1, -1, 24, FilterMode.Bilinear);
		glowBuffer.SetRenderTarget(glowTemp);
		glowBuffer.ClearRenderTarget(true, true, Color.clear);
		glowBuffer.SetGlobalTexture("_GlowMap", glowTemp);
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

	void LateUpdate() => glowBuffer.ClearRenderTarget(true, true, Color.clear);

	void OnPreCull()
	{
		glowBuffer.SetGlobalTexture("_GlowMap", glowTemp);
	}

}
