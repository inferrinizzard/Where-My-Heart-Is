using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode]
public class Mask : MonoBehaviour
{
	public Camera realCam;
	[SerializeField] Shader drawSolid = default;
	[SerializeField] Shader merge = default;

	Material mat;

	private void OnEnable()
	{
		Start();
		Debug.Log("start");
	}

	void Start()
	{
		mat = new Material(merge);
	}
	private void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		realCam.CopyFrom(Camera.current);
		realCam.backgroundColor = Color.black;
		realCam.clearFlags = CameraClearFlags.Depth;
		realCam.cullingMask = 1 << LayerMask.NameToLayer("Mask");

		var mask = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.R8);
		realCam.targetTexture = mask;
		realCam.RenderWithShader(drawSolid, "");

		mat.SetTexture("_Real", source);
		mat.SetTexture("_Mask", mask);

		Texture2D ToTexture2D(RenderTexture rTex)
		{
			Texture2D tex = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
			RenderTexture.active = rTex;
			tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
			tex.Apply();
			return tex;
		}

		byte[] bytes = ToTexture2D(mask).EncodeToPNG();
		System.IO.File.WriteAllBytes("SavedScreen.png", bytes);

		//pass buffer into lookup shader

		Graphics.Blit(source, dest, mat);
		realCam.targetTexture = null;
		RenderTexture.ReleaseTemporary(mask);
	}
}
