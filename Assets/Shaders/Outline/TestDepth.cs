using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[ExecuteInEditMode]
public class TestDepth : MonoBehaviour
{
	public Shader depthShader;
	public Material depthMat;
	// Start is called before the first frame update
	void OnEnable()
	{
		depthMat = depthMat ?? new Material(depthShader);
		Camera cam = GetComponent<Camera>();
		cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.DepthNormals | DepthTextureMode.Depth;
	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, depthMat);
	}
}
