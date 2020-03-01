using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ApplyMask : MonoBehaviour
{
	///<summary> Reference to Real World Cam, temp Mask Cam </summary>
	Camera realCam, maskCam, mainCam;
	///<summary> Shader that combines views </summary>
	[SerializeField] Shader merge = default;
	///<summary> Generated material for merge shader </summary>
	Material screenMat;
	///<summary> Generated RenderTexture for Real World </summary>
	RenderTexture real;
	///<summary> External RenderTexture for Mask TODO: to be consumed </summary>
	public RenderTexture mask;

	void Start()
	{
		screenMat = new Material(merge);

		// get ref to real world cam and assign generated RenderTexture
		mainCam = GetComponent<Camera>();
		realCam = this.GetComponentOnlyInChildren<Camera>();
		real = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.Default);
		real.name = "Real World";
		realCam.targetTexture = real;

		// same as above, does not work
		// mask = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.R8);
		// mask = RenderTexture.GetTemporary(Screen.width, Screen.height, 16, RenderTextureFormat.R8);
		// mask.Create();
		// mask.name = "Internal Mask";

		// spawn temp mask cam and configure transform
		maskCam = new GameObject("Mask Cam").AddComponent<Camera>();
		maskCam.transform.position = Vector3.zero;
		maskCam.transform.eulerAngles = Vector3.zero;
		maskCam.transform.parent = transform;
		maskCam.transform.localEulerAngles = Vector3.zero;
		maskCam.transform.localPosition = Vector3.zero;

		// configure mask Camera
		maskCam.cullingMask = 1 << LayerMask.NameToLayer("Mask");
		maskCam.clearFlags = CameraClearFlags.SolidColor;
		maskCam.backgroundColor = new Color(0, 0, 0, 0);
		maskCam.targetTexture = mask;

		maskCam.Render();

		var screen = RenderTexture.active;
		RenderTexture.active = mask;

		// copy to Texture2D and pass to shader
		var mask2D = new Texture2D(mask.width, mask.height);
		mask2D.ReadPixels(new Rect(0, 0, mask.width, mask.height), 0, 0);
		mask2D.Apply();
		Shader.SetGlobalTexture("_Mask", mask2D);

		RenderTexture.active = screen;

		// remove temp cam
		Destroy(maskCam.gameObject);
		// RenderTexture.ReleaseTemporary(mask);
	}

	void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		// pass both cameras to screen per render
		screenMat.SetTexture("_Dream", source);
		screenMat.SetTexture("_Real", real);
		Graphics.Blit(source, dest, screenMat);
		ClearRT(real, realCam);
		ClearRT(source, mainCam);
	}

	void ClearRT(RenderTexture r, Camera cam)
	{
		RenderTexture rt = UnityEngine.RenderTexture.active;
		UnityEngine.RenderTexture.active = r;
		GL.ClearWithSkybox(true, cam);
		// GL.Clear(true, true, Color.clear);
		UnityEngine.RenderTexture.active = rt;
	}

	// void OnEnable()
	// {
	// 	if (ReplacementShader != null)
	// 		realCam.SetReplacementShader(ReplacementShader, "Dissolve");
	// }

	// void OnDisable() => realCam.ResetReplacementShader();
}
