using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[ExecuteInEditMode]
public class Wave : MonoBehaviour
{
	[SerializeField] Shader waveShader = default;
	[SerializeField] Color waveColour = default;
	[HideInInspector] public float waveDistance = 0;
	Material waveMat;

	void Start()
	{
		Camera cam = GetComponent<Camera>();
		cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth;
		waveMat = new Material(waveShader);
		waveMat.SetColor("_WaveColour", waveColour);
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		waveMat.SetFloat("_WaveDistance", waveDistance);
		Graphics.Blit(source, destination, waveMat);
	}
}
