using System;

using UnityEngine;

// [ExecuteInEditMode]
public class ScreenOutline : MonoBehaviour
{
	[SerializeField] Material outlineMat = default;
	// [SerializeField] string layer = "";

	// float[] kernel;

	float[] Gauss(float sigma, int size)
	{
		float[] ret = new float[size];
		int half = size / 2;
		for (int i = 0; i < size; i++)
			ret[i] = 1f / (Mathf.Sqrt(2 * Mathf.PI) * sigma) * Mathf.Exp(-(i - half) * (i - half) / (2 * sigma * sigma));
		return ret;
	}

	void Start()
	{
		// Camera.main.depthTextureMode = Camera.main.depthTextureMode | DepthTextureMode.DepthNormals;
		// kernel = Gauss(5, 7);
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		// if (layer != "")
		// {
		// 	Camera current = gameObject.GetComponent<Camera>();
		// 	int cullingMask = current.cullingMask;
		// 	current.cullingMask = 1 << LayerMask.NameToLayer(layer);
		// 	Graphics.Blit(source, destination, outlineMat);
		// 	current.cullingMask = cullingMask;
		// }
		// else

		// outlineMat.SetFloatArray("Kernel", kernel);
		// outlineMat.SetInt("_KernelWidth", kernel.Length);
		// outlineMat.SetTexture("_SceneTex", source);

		Graphics.Blit(source, destination, outlineMat);
	}
}
