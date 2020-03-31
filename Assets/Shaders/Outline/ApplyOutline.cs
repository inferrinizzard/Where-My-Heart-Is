using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;

public class ApplyOutline : MonoBehaviour
{
	public Camera cam;
	public static CommandBuffer glowBuffer;
	public Transform root;

	// OutlineObject[] glowObjects = new OutlineObject[0];

	// [Range(0, 10)] public float intensity = 1;
	// [Range(0, 10)] public float threshold = 1;
	// [Range(0, 1)] public float softThreshold = 0.5f;

	int glowTemp = Shader.PropertyToID("_GlowTemp");

	void Start()
	{
		//cam = Camera.main;
		// glowObjects = root.GetComponentsInChildren<OutlineObject>();

		glowBuffer = new CommandBuffer();
		glowBuffer.GetTemporaryRT(glowTemp, -1, -1, 24, FilterMode.Bilinear);
		glowBuffer.SetRenderTarget(glowTemp);
		glowBuffer.ClearRenderTarget(true, true, Color.clear);
		glowBuffer.SetGlobalTexture("_GlowMap", glowTemp);
		glowBuffer.name = "Glow Map Buffer";

		// float knee = threshold * softThreshold;
		// Vector4 filter;
		// filter.x = threshold;
		// filter.y = filter.x - knee;
		// filter.z = 2f * knee;
		// filter.w = 0.25f / (knee + 0.00001f);
		// glowBuffer.SetGlobalVector("_Filter", filter);
		// glowBuffer.SetGlobalFloat("_Intensity", Mathf.GammaToLinearSpace(intensity));

		cam.AddCommandBuffer(CameraEvent.BeforeLighting, glowBuffer);
	}

	private void Cleanup()
	{
		if (glowBuffer != null && cam)
			cam.RemoveCommandBuffer(CameraEvent.BeforeLighting, glowBuffer);
	}

	public void OnDisable() => Cleanup();

	public void OnEnable() => Cleanup();

	public void OnWillRenderObject()
	{
		// Cleanup();
		// if (!(gameObject.activeInHierarchy && enabled))
		// 	return;

		// create new command buffer
		// var tempBuffer = new CommandBuffer();
		// tempBuffer.name = "Glow Map buffer";
		// glowBuffer = tempBuffer;

		// create render texture for glow map
		// glowBuffer.GetTemporaryRT(glowTemp, -1, -1, 24, FilterMode.Bilinear);
		// glowBuffer.SetRenderTarget(glowTemp);
		// glowBuffer.ClearRenderTarget(true, true, Color.clear); // clear before drawing to it each frame!!

		// draw all glow objects to it
		// foreach (OutlineObject o in glowObjects)
		// {
		// 	// Renderer r = o.GetComponent<Renderer>();
		// 	foreach (Renderer r in o.GetComponentsInChildren<Renderer>())
		// 		glowBuffer.DrawRenderer(r, o.Colour);
		// }

		// set render texture as globally accessable 'glow map' texture
		// glowBuffer.SetGlobalTexture("_GlowMap", glowTemp);

		// add this command buffer to the pipeline
		// cam.AddCommandBuffer(CameraEvent.BeforeLighting, glowBuffer);
	}

	void LateUpdate() => glowBuffer.ClearRenderTarget(true, true, Color.clear);

	void OnPreCull()
	{
		glowBuffer.SetGlobalTexture("_GlowMap", glowTemp);
	}

}
