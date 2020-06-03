using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class HeartCamera : MonoBehaviour
{
	RenderTexture heartDepthNormalsTexture;
	Material heartDepthNormalsMat;
	void Start()
	{
		heartDepthNormalsTexture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.Default);
		heartDepthNormalsMat = new Material(Shader.Find("Screen/DepthNormals"));
	}
	void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, heartDepthNormalsTexture, heartDepthNormalsMat);
		Shader.SetGlobalTexture(ShaderID._HeartDepthNormals, heartDepthNormalsTexture);
		Graphics.Blit(src, dest);
	}
}
