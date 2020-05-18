using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ApplyMask : MonoBehaviour
{
	///<summary> External RenderTexture for Mask TODO: to be consumed </summary>
	[Header("Image Effect Materials")]
	public Material rippleMat;
	public Material screenMat;

	///<summary> Reference to Heart World Cam, temp Mask Cam </summary>
	[HideInInspector] public Camera heartCam, mainCam;
	[SerializeField] Camera depthCam = default;
	[SerializeField] Shader depthReplacement = default;

	///<summary> Generated RenderTexture for Heart World </summary>
	RenderTexture heart;
	Texture2D curSave;
	int _HeartID = Shader.PropertyToID("_Heart");

	[Header("Ripple Behavior")]
	[SerializeField] float rippleLength = 1, rippleTarget = 10;
	[SerializeField] AnimationCurve rippleCurve = default;

	[HideInInspector] public RenderTexture mask;
	RenderTexture depth;

	private bool rippleInProgress;
	private float rippleStartTime;

	void Start()
	{
		// get ref to heart world cam and assign generated RenderTexture
		mainCam = GetComponent<Camera>();
		mainCam.depthTextureMode = mainCam.depthTextureMode | DepthTextureMode.DepthNormals | DepthTextureMode.Depth;
		heartCam = this.GetComponentOnlyInChildren<Camera>();
		heartCam.depthTextureMode = heartCam.depthTextureMode | DepthTextureMode.DepthNormals | DepthTextureMode.Depth;
		heart = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.Default);
		heart.name = "Heart World";
		heartCam.targetTexture = heart;

		Player.Instance.OnApplyCut += StartRipple;

		mask = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.Default);
		mask.name = "Internal Mask";
		CreateMask();

		depth = new RenderTexture(Screen.width / 4, Screen.height / 4, 16, RenderTextureFormat.Default);
		depth.name = "DepthRT";
		depthCam.targetTexture = depth;

		Shader.SetGlobalFloat("_ScreenXToYRatio", Screen.width / Screen.height);
	}

	public void StartRipple()
	{
		rippleInProgress = true;
		rippleStartTime = Time.time;
	}

	public void CreateMask()
	{
		// spawn temp mask cam and configure transform
		var maskCam = new GameObject("Mask Cam").AddComponent<Camera>();
		//(maskCam.transform.position, maskCam.transform.eulerAngles) = (Vector3.zero, Vector3.zero);
		maskCam.transform.parent = transform;
		(maskCam.transform.localPosition, maskCam.transform.localEulerAngles) = (Vector3.zero, Vector3.zero);
		maskCam.enabled = false;

		// configure mask Camera
		maskCam.cullingMask = 1 << LayerMask.NameToLayer("Mask");
		maskCam.clearFlags = CameraClearFlags.SolidColor;
		maskCam.backgroundColor = Color.clear;
		maskCam.targetTexture = mask;

		maskCam.Render();
		SetMask(mask);

		// remove temp cam
		maskCam.targetTexture = null;
		Destroy(maskCam.gameObject);
	}

	void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		screenMat.SetTexture(_HeartID, heart);

		if (rippleInProgress)
		{
			RenderTexture temp = RenderTexture.GetTemporary(Screen.width, Screen.height, 16);
			Graphics.Blit(source, temp, screenMat);
			Graphics.Blit(temp, dest, rippleMat);
			RenderTexture.ReleaseTemporary(temp);
		}
		else
			Graphics.Blit(source, dest, screenMat);
	}

	void OnPreRender()
	{
		// render depth buffer to sample from
		RenderDepth();

		if (rippleInProgress)
		{
			//float t = () / rippleLength;
			if (Time.time - rippleStartTime < rippleLength)
			{
				if (Time.time - rippleStartTime > 0)
				{
					rippleMat.SetFloat("_Offset", rippleCurve.Evaluate((Time.time - rippleStartTime) / rippleLength) * rippleTarget);
				}
			}
			else
			{
				rippleInProgress = false;
			}
		}
	}

	public void RenderDepth()
	{
		depthCam.SetReplacementShader(depthReplacement, "");
		depthCam.Render();
		Shader.SetGlobalTexture("_DepthColor", depth);
	}

	public void SetMask(RenderTexture nextMask)
	{
		RenderTexture screen = RenderTexture.active;
		RenderTexture.active = nextMask;

		// copy to Texture2D and pass to shader
		var mask2D = new Texture2D(Screen.width, Screen.height);
		mask2D.ReadPixels(new Rect(0, 0, nextMask.width, nextMask.height), 0, 0);
		mask2D.Apply();
		Shader.SetGlobalTexture("_Mask", mask2D);
		RenderTexture.active = screen;
	}

	public IEnumerator PreTransition()
	{
		GameManager.Instance.pause.gameObject.SetActive(false); // TODO: not gameobject but just gameplayUI
		yield return new WaitForEndOfFrame();

		curSave = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		curSave.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		curSave.Apply();

		foreach (var r in World.Instance.GetComponentsInChildren<Renderer>()) // TODO: better?
			r.enabled = false;

		var pageFlip = StartCoroutine(Player.Instance.GetComponentInChildren<PageFlip>(true).Flip(curSave));
		var load = StartCoroutine(GameManager.Instance.ChangeLevelManual());

		yield return pageFlip;
		yield return load;
	}
}
