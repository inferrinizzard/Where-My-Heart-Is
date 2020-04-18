using System;

using UnityEngine;

[System.Serializable, ExecuteInEditMode]
public class Outline : MonoBehaviour
{
	[SerializeField] Shader drawSolid = default;
	[SerializeField] Shader outline = default;
	Material outlineMat;
	Camera outlineCam;
	float[] kernel;

#if UNITY_EDITOR
	void OnEnable()
	{
		Start();
	}
#endif

	void Start()
	{
		outlineMat = new Material(outline);
		GameObject outlineCamChild = transform.Find("Outline Cam")?.gameObject;
		if (!outlineCamChild)
		{
			outlineCamChild = new GameObject();
			outlineCamChild.transform.SetParent(transform);
			outlineCamChild.name = "Outline Cam";
			outlineCam = outlineCamChild.AddComponent<Camera>();
		}
		else
			outlineCam = outlineCamChild.GetComponent<Camera>();

		kernel = GaussianKernel.Calculate(5, 21);
		Camera.main.depthTextureMode = Camera.main.depthTextureMode | DepthTextureMode.DepthNormals;
	}

	void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		outlineCam.CopyFrom(Camera.current);
		outlineCam.backgroundColor = Color.black;
		outlineCam.clearFlags = CameraClearFlags.Color;
		outlineCam.cullingMask = 1 << LayerMask.NameToLayer("Real");

		var outlineBuffer = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.R8);
		outlineCam.targetTexture = outlineBuffer;
		outlineCam.RenderWithShader(drawSolid, "");

		outlineMat.SetFloatArray("kernel", kernel);
		outlineMat.SetInt("_kernelWidth", kernel.Length);
		outlineMat.SetTexture("_SceneTex", source);

		outlineBuffer.filterMode = FilterMode.Point;

		Graphics.Blit(outlineBuffer, dest, outlineMat);
		outlineCam.targetTexture = source;
		RenderTexture.ReleaseTemporary(outlineBuffer);
	}

	public static class GaussianKernel
	{
		public static float[] Calculate(double sigma, int size)
		{
			float[] ret = new float[size];
			double sum = 0;
			int half = size / 2;
			for (int i = 0; i < size; i++)
			{
				ret[i] = (float) (1 / (Math.Sqrt(2 * Math.PI) * sigma) * Math.Exp(-(i - half) * (i - half) / (2 * sigma * sigma)));
				sum += ret[i];
			}
			return ret;
		}
	}
}
