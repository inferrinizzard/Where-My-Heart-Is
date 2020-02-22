using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ApplyMask : MonoBehaviour
{
	[SerializeField] Camera realCam = default;
	Camera maskCam;
	[SerializeField] Shader merge = default;
	Material screenMat;

	RenderTexture real;

	public RenderTexture mask;
	void Start()
	{
		screenMat = new Material(merge);

		real = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.Default);
		real.name = "Real World";
		realCam.targetTexture = real;

		// mask = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.R8);
		// mask = RenderTexture.GetTemporary(Screen.width, Screen.height, 16, RenderTextureFormat.R8);
		// mask.Create();
		// mask.name = "Internal Mask";

		maskCam = new GameObject("Mask Cam").AddComponent<Camera>();
		maskCam.transform.position = Vector3.zero;
		maskCam.transform.eulerAngles = Vector3.zero;
		maskCam.transform.parent = transform;
		maskCam.transform.localEulerAngles = Vector3.zero;
		maskCam.transform.localPosition = Vector3.zero;

		maskCam.cullingMask = 1 << LayerMask.NameToLayer("Mask");
		maskCam.clearFlags = CameraClearFlags.SolidColor;
		maskCam.backgroundColor = new Color(0, 0, 0, 0);
		maskCam.targetTexture = mask;

		maskCam.Render();

		var screen = RenderTexture.active;
		RenderTexture.active = mask;

		var mask2D = new Texture2D(mask.width, mask.height);
		mask2D.ReadPixels(new Rect(0, 0, mask.width, mask.height), 0, 0);
		mask2D.Apply();
		screenMat.SetTexture("_Mask", mask2D);

		RenderTexture.active = screen;

		Destroy(maskCam.gameObject);
		// RenderTexture.ReleaseTemporary(mask);
	}

	void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		screenMat.SetTexture("_Dream", source);
		screenMat.SetTexture("_Real", real);
		Graphics.Blit(source, dest, screenMat);
	}
}
