using System;
using UnityEngine;

[ExecuteInEditMode]
public class ScreenOutline : MonoBehaviour
{
	[SerializeField] Material outlineMat = default;

	void Start()
	{
		Camera.main.depthTextureMode = Camera.main.depthTextureMode | DepthTextureMode.DepthNormals;
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, outlineMat);
	}
}
