using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMask : MonoBehaviour
{
	RenderTexture maskRT;
	Camera maskCam;

	void Start()
	{
		maskCam = GetComponent<Camera>();
		// maskCam.cullingMask = 1 << LayerMask.NameToLayer("Mask");
		// maskCam.clearFlags = CameraClearFlags.SolidColor;
		// maskCam.depth = 0;
		maskCam.backgroundColor = new Color(0, 0, 0, 0);

		// maskRT = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.R8);
		// maskCam.targetTexture = maskRT;

		var currentScreen = RenderTexture.active;
		RenderTexture.active = maskCam.targetTexture;
		maskCam.Render();

		Texture2D mask = new Texture2D(maskCam.targetTexture.width, maskCam.targetTexture.height);
		mask.ReadPixels(new Rect(0, 0, maskCam.targetTexture.width, maskCam.targetTexture.height), 0, 0);
		mask.Apply();
		GetComponentInParent<ApplyMask>().AssignMask(mask);

		RenderTexture.active = currentScreen;

		StartCoroutine(WaitFrames());
	}

	IEnumerator WaitFrames(int frames = 2)
	{
		int i = 0;
		while (i++ < frames)
			yield return null;
		gameObject.SetActive(false);
		// Destroy(gameObject);
	}
}
