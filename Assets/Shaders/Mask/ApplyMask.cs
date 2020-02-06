using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ApplyMask : MonoBehaviour
{
	[SerializeField] Camera realCam = default;
	Camera dreamCam;
	[SerializeField] Shader merge = default;
	Material screenMat;

	public Texture2D mask;
	RenderTexture real;
	void Start()
	{
		screenMat = new Material(merge);
		real = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.Default);
		realCam.targetTexture = real;
	}

	public void AssignMask(Texture2D mask)
	{
		this.mask = mask;
		screenMat.SetTexture("_Mask", mask);
	}

	void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		screenMat.SetTexture("_Dream", source);
		screenMat.SetTexture("_Real", real);
		Graphics.Blit(source, dest, screenMat);
	}
}
