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

		var currentSCreen = RenderTexture.active;
		RenderTexture.active = maskCam.targetTexture;
		maskCam.Render();

		Texture2D mask = new Texture2D(maskCam.targetTexture.width, maskCam.targetTexture.height);
		mask.ReadPixels(new Rect(0, 0, maskCam.targetTexture.width, maskCam.targetTexture.height), 0, 0);
		mask.Apply();

		RenderTexture.active = currentSCreen;

		byte[] bytes = mask.EncodeToPNG();
		System.IO.File.WriteAllBytes("Assets/Resources/Mask.png", bytes);
		gameObject.SetActive(false);
	}
}
