using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TestPip : MonoBehaviour
{

	private Camera _camera;
	//public Texture2D _screenshot;
	public RenderTexture rt;

	private int resWidth = 1920;
	private int resHeight = 1080;

	private void Awake()
	{
		_camera = GetComponent<Camera>();
	}

	public IEnumerator TakeScreenshot()
	{
		yield return new WaitForEndOfFrame();

		//RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
		_camera.targetTexture = rt;
		//_screenshot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
		_camera.Render();
		RenderTexture.active = rt;
		//_screenshot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
		//_screenshot.Apply();
		_camera.targetTexture = null;
		/*Destroy(rt);

		Sprite tempSprite = Sprite.Create(_screenshot, new Rect(0, 0, resWidth, resHeight), new Vector2(0, 0));
		GameObject.Find("SpriteObject").GetComponent<SpriteRenderer>().sprite = tempSprite;*/

	}

}
