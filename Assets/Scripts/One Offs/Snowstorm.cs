using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Snowstorm : MonoBehaviour
{
	Texture2D transitionTex = default, backgroundTex = default;
	Material snowMat;
	float progress = 0;
	public static float walkDistance = 100;

	void Start()
	{
		transitionTex = Resources.Load<Texture2D>("Frost");

		var fadeSnowTex = new Texture2D(transitionTex.width, transitionTex.height);
		Graphics.CopyTexture(transitionTex, 0, 0, fadeSnowTex, 0, 0);
		var snowPixels = transitionTex.GetPixels();
		for (int i = 0; i < snowPixels.Length; i++)
			snowPixels[i] = new Color(1 - snowPixels[i].r, 1 - snowPixels[i].g, 1 - snowPixels[i].b, snowPixels[i].a);
		fadeSnowTex.SetPixels(snowPixels);
		fadeSnowTex.Apply();

		snowMat = new Material(Shader.Find("Dissolve/Transition"));
		snowMat.SetTexture("_TransitionTex", fadeSnowTex);
		if (backgroundTex != null)
			snowMat.SetTexture("_BackgroundTex", backgroundTex);
	}

	void Update()
	{
		progress = Player.Instance.transform.position.magnitude;
		if (walkDistance - progress < .05)
			Destroy(this);
	}
	void FixedUpdate() => snowMat.SetFloat(ShaderID._TransitionCutoff, EaseMethods.CubicEaseIn(1 - progress / walkDistance, 0, 1, 1));
	void OnRenderImage(RenderTexture src, RenderTexture dest) => Graphics.Blit(src, dest, snowMat);
}
