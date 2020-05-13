using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class ApplyMask : MonoBehaviour
{
	public Shader testShader;
	public Material blackSkybox;

	///<summary> External RenderTexture for Mask TODO: to be consumed </summary>
	//public static RenderTexture mask;
	[Header("Image Effect Materials")]
	public Material rippleMat;
	public Material screenMat;

	///<summary> Reference to Heart World Cam, temp Mask Cam </summary>
	[HideInInspector] public Camera heartCam, maskCam, mainCam;
	[SerializeField] Camera depthCam;
	[SerializeField] Shader depthReplacement = default;

	///<summary> Generated RenderTexture for Heart World </summary>
	[HideInInspector] public RenderTexture heart;

	Texture2D curSave;
	int _HeartID;

	[Header("Ripple Behavior")]
	public float rippleLength;
	public float rippleTarget;
	public AnimationCurve rippleCurve;

	[HideInInspector] public RenderTexture mask;
	public RenderTexture depth;
	private RenderTexture depthBuffer;
	private Texture2D mask2D;
	private Texture2D depth2D;

	private bool rippleInProgress;
	private float rippleStartTime;

	void Start()
	{
		_HeartID = Shader.PropertyToID("_Heart");

		// get ref to heart world cam and assign generated RenderTexture
		mainCam = GetComponent<Camera>();
		mainCam.depthTextureMode = mainCam.depthTextureMode | DepthTextureMode.DepthNormals | DepthTextureMode.Depth;
		heartCam = this.GetComponentOnlyInChildren<Camera>();
		heart = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.Default);
		heartCam.depthTextureMode = heartCam.depthTextureMode | DepthTextureMode.DepthNormals | DepthTextureMode.Depth;
		heart.name = "Heart World";
		heartCam.targetTexture = heart;

		Player.Instance.OnApplyCut += StartRipple;

		mask = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.Default);
		mask.name = "Internal Mask";
		mask2D = new Texture2D(Screen.width, Screen.height);
		CreateMask();

		depth = new RenderTexture(Screen.width / 4, Screen.height / 4, 16, RenderTextureFormat.Default);
		depth.name = "DepthRT";
		depth2D = new Texture2D(Screen.width / 4, Screen.height / 4);

		Shader.SetGlobalFloat("_ScreenXToYRatio", Screen.width / Screen.height);

		depthCam.targetTexture = depth;
	}

	public void CopyInto(ApplyMask target)
	{
		target.screenMat = this.screenMat;
	}

	public void StartRipple()
	{
		rippleInProgress = true;
		rippleStartTime = Time.time;
	}

	public void CreateMask()
	{
		// spawn temp mask cam and configure transform
		maskCam = new GameObject("Mask Cam").AddComponent<Camera>();
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

		mask2D = new Texture2D(Screen.width, Screen.height);

		SetMask(mask);

		// remove temp cam
		maskCam.targetTexture = null;
		Destroy(maskCam.gameObject);
		//RenderTexture.ReleaseTemporary(mask);
	}

	void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		screenMat.SetTexture(_HeartID, heart);

		if (rippleInProgress == true)
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
		// spawn temp mask cam and configure transform
		// Camera depthCam = new GameObject("Depth Cam").AddComponent<Camera>();
		//(maskCam.transform.position, maskCam.transform.eulerAngles) = (Vector3.zero, Vector3.zero);
		// depthCam.transform.parent = transform;
		// depthCam.CopyFrom(GetComponent<Camera>());
		// (depthCam.transform.localPosition, depthCam.transform.localEulerAngles) = (Vector3.zero, Vector3.zero);
		// depthCam.enabled = false;

		// depthCam.backgroundColor = Color.black;
		// depthCam.clearFlags = CameraClearFlags.Skybox;
		// depthCam.targetTexture = depth;
		// depthCam.gameObject.AddComponent<Skybox>().material = blackSkybox;
		depthCam.SetReplacementShader(depthReplacement, "");
		//depthCam.RenderWithShader(depthReplacement, "");
		depthCam.Render();
		RenderTexture screen = RenderTexture.active;
		RenderTexture.active = depth;

		// copy to Texture2D and pass to shader
		depth2D.ReadPixels(new Rect(0, 0, depth.width, depth.height), 0, 0);
		depth2D.Apply();
		Shader.SetGlobalTexture("_DepthColor", depth2D);
		RenderTexture.active = screen;

		// remove temp cam
		// depthCam.targetTexture = null;
		// Destroy(depthCam.gameObject);
		//RenderTexture.ReleaseTemporary(mask);
	}

	public void SetMask(RenderTexture nextMask)
	{
		RenderTexture screen = RenderTexture.active;
		RenderTexture.active = nextMask;

		// copy to Texture2D and pass to shader
		mask2D.ReadPixels(new Rect(0, 0, nextMask.width, nextMask.height), 0, 0);
		mask2D.Apply();
		Shader.SetGlobalTexture("_Mask", mask2D);
		RenderTexture.active = screen;
	}

	void ClearRT(RenderTexture r, Camera cam)
	{
		RenderTexture rt = UnityEngine.RenderTexture.active;
		UnityEngine.RenderTexture.active = r;
		GL.ClearWithSkybox(true, cam);
		// GL.Clear(true, true, Color.clear);
		UnityEngine.RenderTexture.active = rt;
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
