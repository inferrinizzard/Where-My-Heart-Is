using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMask : MonoBehaviour
{
	Camera maskCam;
	// Start is called before the first frame update
	void Start()
	{
		maskCam = GetComponent<Camera>();
		maskCam.backgroundColor = Color.black;

		var currentScreen = RenderTexture.active;
		// var capture = RenderTexture.GetTemporary(Screen.width, Screen.height, 16, RenderTextureFormat.R8);
		// maskCam.targetTexture = capture;
		RenderTexture.active = maskCam.targetTexture;
		maskCam.Render();

		Texture2D mask = new Texture2D(maskCam.targetTexture.width, maskCam.targetTexture.height);
		mask.ReadPixels(new Rect(0, 0, maskCam.targetTexture.width, maskCam.targetTexture.height), 0, 0);
		mask.Apply();

		// maskCam.targetTexture = null;
		// RenderTexture.ReleaseTemporary(capture);
		RenderTexture.active = currentScreen;
		maskCam.Render();

		byte[] bytes = mask.EncodeToPNG();
		System.IO.File.WriteAllBytes("Assets/Resources/Mask.png", bytes);
		StartCoroutine(WaitFrames());

	}

	IEnumerator WaitFrames(int frames = 2)
	{
		int i = 0;
		while (i++ < frames)
			yield return null;
		gameObject.SetActive(false);
		Destroy(this);
	}
}
