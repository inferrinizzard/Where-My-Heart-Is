﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class ApplyOutline : MonoBehaviour
{
	public Camera cam;
	CommandBuffer glowBuffer;
	public Transform root;

	OutlineObject[] glowObjects = new OutlineObject[0];

	[Range(0, 10)] public float intensity = 1;
	[Range(0, 10)] public float threshold = 1;
	[Range(0, 1)] public float softThreshold = 0.5f;
	// private Dictionary<Camera, CommandBuffer> cameras = new Dictionary<Camera, CommandBuffer>();

	void Start()
	{
		glowObjects = root.GetComponentsInChildren<OutlineObject>();

		glowBuffer = new CommandBuffer();
		int tempID = Shader.PropertyToID("_Temp1");
		glowBuffer.GetTemporaryRT(tempID, -1, -1, 24, FilterMode.Bilinear);
		glowBuffer.SetRenderTarget(tempID);
		glowBuffer.ClearRenderTarget(true, true, Color.black);
		glowBuffer.SetGlobalTexture("_GlowMap", tempID);

		float knee = threshold * softThreshold;
		Vector4 filter;
		filter.x = threshold;
		filter.y = filter.x - knee;
		filter.z = 2f * knee;
		filter.w = 0.25f / (knee + 0.00001f);
		glowBuffer.SetGlobalVector("_Filter", filter);
		glowBuffer.SetGlobalFloat("_Intensity", Mathf.GammaToLinearSpace(intensity));

		cam.AddCommandBuffer(CameraEvent.BeforeLighting, glowBuffer);
	}

	private void Cleanup()
	{
		// foreach (var cam in cameras)
		// {
		// 	if (cam.Key)
		// 		cam.Key.RemoveCommandBuffer(CameraEvent.BeforeLighting, cam.Value);
		// }
		// cameras.Clear();

		if (glowBuffer != null && cam)
			cam.RemoveCommandBuffer(CameraEvent.BeforeLighting, glowBuffer);
	}

	public void OnDisable()
	{
		Cleanup();
	}

	public void OnEnable()
	{
		Cleanup();
	}

	public void OnWillRenderObject()
	{
		var render = gameObject.activeInHierarchy && enabled;
		if (!render)
		{
			Cleanup();
			return;
		}

		// var cam = Camera.current;
		// if (!cam || cameras.ContainsKey(cam))
		// 	return;

		// create new command buffer
		var tempBuffer = new CommandBuffer();
		tempBuffer.name = "Glow Map buffer";
		glowBuffer = tempBuffer;

		// cameras[cam] = tempBuffer;

		// create render texture for glow map
		int tempID = Shader.PropertyToID("_Temp1");
		tempBuffer.GetTemporaryRT(tempID, -1, -1, 24, FilterMode.Bilinear);
		tempBuffer.SetRenderTarget(tempID);
		tempBuffer.ClearRenderTarget(true, true, Color.black); // clear before drawing to it each frame!!

		// draw all glow objects to it
		foreach (OutlineObject o in glowObjects)
		{
			Renderer r = o.GetComponent<Renderer>();
			Material glowMat = o.glowMaterial;
			if (r && glowMat)
				tempBuffer.DrawRenderer(r, glowMat);
		}

		// set render texture as globally accessable 'glow map' texture
		tempBuffer.SetGlobalTexture("_GlowMap", tempID);

		// add this command buffer to the pipeline
		cam.AddCommandBuffer(CameraEvent.BeforeLighting, tempBuffer);
	}
}
