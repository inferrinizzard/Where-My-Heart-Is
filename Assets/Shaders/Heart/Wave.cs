using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Wave : MonoBehaviour
{
	// [SerializeField] Shader waveShader = default;
	[SerializeField] Color waveColour = default;
	[HideInInspector] public float waveDistance = 0;
	// Material waveMat;

	void Start()
	{
		// Camera cam = GetComponent<Camera>();
		// cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth;
		// waveMat = new Material(waveShader);
		// Player.Instance.mask.screenMat.SetColor("_WaveColour", waveColour);
		// Player.Instance.mask.screenMat.SetFloat("_WaveDistance", waveDistance);
	}

	// void OnRenderImage(RenderTexture source, RenderTexture destination)
	// {
	// 	waveMat.SetFloat("_WaveDistance", waveDistance);
	// 	Graphics.Blit(source, destination, waveMat);
	// }
}
