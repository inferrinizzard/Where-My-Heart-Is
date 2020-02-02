using System;
using UnityEngine;

[ExecuteInEditMode]
public class OldOutline : MonoBehaviour
{
	[SerializeField] Material postprocessMaterial;

	void Start()
	{
		Camera.main.depthTextureMode = Camera.main.depthTextureMode | DepthTextureMode.DepthNormals;
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, postprocessMaterial);
	}
}
