using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class ApplyMask : MonoBehaviour
{
	///<summary> Reference to Heart World Cam, temp Mask Cam </summary>
	Camera heartCam, maskCam, mainCam;
	///<summary> Shader that combines views </summary>
	[SerializeField] Shader merge = default, transition = default;
	///<summary> Generated material for screen shader </summary>
	public Material screenMat;
	[HideInInspector] public Material transitionMat;
	///<summary> Generated RenderTexture for Heart World </summary>\
	RenderTexture heart;
	[SerializeField] Texture2D dissolveTexture = default;
	[SerializeField] Texture2D hatchTexture = default;
	[SerializeField] Texture2D birdBackground = default;
	Texture2D curSave;
	int _HeartID;

	void Start()
	{
		screenMat = new Material(merge);
		_HeartID = Shader.PropertyToID("_Heart");

		// get ref to heart world cam and assign generated RenderTexture
		mainCam = GetComponent<Camera>();
		mainCam.depthTextureMode = mainCam.depthTextureMode | DepthTextureMode.DepthNormals | DepthTextureMode.Depth;
		heartCam = this.GetComponentOnlyInChildren<Camera>();
		heart = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.Default);
		heartCam.depthTextureMode = heartCam.depthTextureMode | DepthTextureMode.DepthNormals | DepthTextureMode.Depth;
		heart.name = "Heart World";
		heartCam.targetTexture = heart;

		CreateMask();
		screenMat.SetTexture("_HatchTex", hatchTexture);
		screenMat.SetTexture("_Background", birdBackground);
		// screenMat.SetColor("_DepthOutlineColour", Color.white);
	}

	public void CreateMask()
	{
		var mask = RenderTexture.GetTemporary(Screen.width, Screen.height, 16);
		mask.name = "Internal Mask";

		// spawn temp mask cam and configure transform
		maskCam = new GameObject("Mask Cam").AddComponent<Camera>();
		(maskCam.transform.position, maskCam.transform.eulerAngles) = (Vector3.zero, Vector3.zero);
		maskCam.transform.parent = transform;
		(maskCam.transform.localPosition, maskCam.transform.localEulerAngles) = (Vector3.zero, Vector3.zero);

		// configure mask Camera
		maskCam.cullingMask = 1 << LayerMask.NameToLayer("Mask");
		maskCam.clearFlags = CameraClearFlags.SolidColor;
		maskCam.backgroundColor = Color.clear;
		maskCam.targetTexture = mask;

		maskCam.Render();

		var screen = RenderTexture.active;
		RenderTexture.active = mask;

		// copy to Texture2D and pass to shader
		var mask2D = new Texture2D(mask.width, mask.height);
		mask2D.ReadPixels(new Rect(0, 0, mask.width, mask.height), 0, 0);
		mask2D.Apply();
		Shader.SetGlobalTexture("_Mask", Instantiate(mask2D));
		RenderTexture.active = screen;

		// remove temp cam
		Destroy(maskCam.gameObject);
		RenderTexture.ReleaseTemporary(mask);
	}

	void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		if (transitionMat == null)
		{ // pass both cameras to screen per render
			screenMat.SetTexture(_HeartID, heart);
			Graphics.Blit(source, dest, screenMat);
			// source.DiscardContents();
			// heart.DiscardContents();
			// source.Release();
			// heart.Release();
			// ClearRT(heart, heartCam);
			// ClearRT(source, mainCam);
		}
		else
		{
			Graphics.Blit(curSave, dest, transitionMat);
		}
	}

	void OnPreRender()
	{
		// GL.ClearWithSkybox(true, heartCam);
		// GL.ClearWithSkybox(true, mainCam);
	}

	void ClearRT(RenderTexture r, Camera cam)
	{
		RenderTexture rt = UnityEngine.RenderTexture.active;
		UnityEngine.RenderTexture.active = r;
		GL.ClearWithSkybox(true, cam);
		// GL.Clear(true, true, Color.clear);
		UnityEngine.RenderTexture.active = rt;
	}

	public IEnumerator PreTransition(Texture2D preview, string scene)
	{
		yield return new WaitForEndOfFrame();
		curSave = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		curSave.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		curSave.Apply();
		transitionMat = new Material(transition);
		transitionMat.SetTexture("_BackgroundTex", preview);
		transitionMat.SetTexture("_TransitionTex", dissolveTexture);
		GameManager.Instance.ChangeLevel(scene);
	}
}
