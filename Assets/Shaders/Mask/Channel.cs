using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Channel : MonoBehaviour
{
	public Texture2D tex;
	void Start()
	{
		var temp = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
		Graphics.CopyTexture(tex, 0, 0, temp, 0, 0);
		var pixels = tex.GetPixels();
		for (int i = 0; i < pixels.Length; i++)
			pixels[i] = new Color(pixels[i].r, pixels[i].g, pixels[i].b, 1 - pixels[i].r);
		temp.SetPixels(pixels);
		temp.Apply();

		var bytes = ImageConversion.EncodeToPNG(temp);
		System.IO.File.WriteAllBytes("channel.png", bytes);
	}
}
