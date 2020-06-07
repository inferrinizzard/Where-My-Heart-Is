using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Fade : MonoBehaviour
{
	[SerializeField] Shader fadeShader = default;
	Material fadeMat;

	// Start is called before the first frame update
	void Start()
	{
		fadeMat = new Material(fadeShader);
	}

	// public void StartFade(bool fadingIn, float duration, bool white = true) => StartCoroutine(FadeRoutine(fadingIn, duration, white));

	public IEnumerator FadeRoutine(bool fadingIn, float time, bool white)
	{
		fadeMat.SetInt("_White", white ? 1 : 0);
		if (!enabled)
			yield break;
		for (var(start, step) = (Time.time, 0f); step <= time; step = Time.time - start)
		{
			yield return null;

			float alpha = fadingIn ? EaseMethods.CubicEaseOut(time - step, 0, 1, time) : EaseMethods.CubicEaseIn(step, 0, 1, time);
			fadeMat.SetFloat("_Alpha", alpha);
		}
		Player.Instance.playerCanRotate = true;
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, fadeMat);
	}
}
