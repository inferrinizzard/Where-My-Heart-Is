using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class ApplyOutline : MonoBehaviour
{
	public Camera cam;
	CommandBuffer glowBuffer;
	public Transform root;
	private Dictionary<Camera, CommandBuffer> m_Cameras = new Dictionary<Camera, CommandBuffer>();

	private void Cleanup()
	{
		foreach (var cam in m_Cameras)
		{
			if (cam.Key)
				cam.Key.RemoveCommandBuffer(CameraEvent.BeforeLighting, cam.Value);
		}
		m_Cameras.Clear();
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

		var cam = Camera.current;
		if (!cam)
			return;

		if (m_Cameras.ContainsKey(cam))
			return;

		// create new command buffer
		glowBuffer = new CommandBuffer();
		glowBuffer.name = "Glow Map buffer";
		m_Cameras[cam] = glowBuffer;

		// create render texture for glow map
		int tempID = Shader.PropertyToID("_Temp1");
		glowBuffer.GetTemporaryRT(tempID, -1, -1, 24, FilterMode.Bilinear);
		glowBuffer.SetRenderTarget(tempID);
		glowBuffer.ClearRenderTarget(true, true, Color.black); // clear before drawing to it each frame!!

		// draw all glow objects to it
		foreach (OutlineObject o in root.GetComponentsInChildren<OutlineObject>())
		{
			Renderer r = o.GetComponent<Renderer>();
			Material glowMat = o.glowMaterial;
			if (r && glowMat)
				glowBuffer.DrawRenderer(r, glowMat);
		}

		// set render texture as globally accessable 'glow map' texture
		glowBuffer.SetGlobalTexture("_GlowMap", tempID);

		// add this command buffer to the pipeline
		cam.AddCommandBuffer(CameraEvent.BeforeLighting, glowBuffer);
	}
}
