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

	public void StartFade(bool fadingIn, float duration) => StartCoroutine(FadeRoutine(fadingIn, duration));

	IEnumerator FadeRoutine(bool fadingIn, float time)
	{
		if (!enabled)
			yield break;
		float start = Time.time;
		bool inProgress = true;
		while (inProgress)
		{
			yield return null;
			float step = Time.time - start;

			float alpha = fadingIn ? EaseMethods.CubicEaseOut(time - step, 0, 1, time) : EaseMethods.CubicEaseIn(step, 0, 1, time);
			fadeMat.SetFloat("_Alpha", alpha);

			if (step > time)
				inProgress = false;
		}
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, fadeMat);
	}
}
