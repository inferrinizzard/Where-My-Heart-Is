using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;

public class ApplyOutline : MonoBehaviour
{
	Camera cam;
	public static CommandBuffer glowBuffer;
	public static bool drawGlow = false;

	int glowTempID = Shader.PropertyToID("_GlowTemp"), glowMapID = Shader.PropertyToID("_GlowMap");

	void Start()
	{
		cam = GetComponent<Camera>();
		glowBuffer = new CommandBuffer();
		glowBuffer.name = "Glow Map Buffer";

		cam.AddCommandBuffer(CameraEvent.BeforeLighting, glowBuffer);
		ResetScreenBuffer(glowBuffer, glowTempID);
	}

	public void OnEnable() => Cleanup();
	public void OnDisable() => Cleanup();
	private void Cleanup()
	{
		if (glowBuffer != null && cam)
			cam.RemoveCommandBuffer(CameraEvent.BeforeLighting, glowBuffer);
	}

	public static void ResetScreenBuffer(CommandBuffer buffer, int tempID)
	{
		buffer.Clear();
		buffer.GetTemporaryRT(tempID, -1, -1, 24, FilterMode.Bilinear);
		buffer.SetRenderTarget(tempID);
		buffer.ClearRenderTarget(true, true, Color.clear);
	}

	void LateUpdate()
	{
		if (drawGlow)
			ResetScreenBuffer(glowBuffer, glowTempID);
	}

	void OnPreCull()
	{
		if (drawGlow)
		{
			glowBuffer.SetGlobalTexture(glowMapID, glowTempID);
			drawGlow = false;
		}
	}
}
