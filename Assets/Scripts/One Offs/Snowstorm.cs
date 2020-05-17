using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Snowstorm : MonoBehaviour
{
	[SerializeField] Texture2D transitionTex = default, betaTex = default;
	Material snowMat;
	int _CutoffID = Shader.PropertyToID("_Cutoff");
	float progress = 0;
	[SerializeField] float distance = 100;

	void Start()
	{
		var fadeSnowTex = new Texture2D(transitionTex.width, transitionTex.height);
		Graphics.CopyTexture(transitionTex, 0, 0, fadeSnowTex, 0, 0);
		var snowPixels = transitionTex.GetPixels();
		for (int i = 0; i < snowPixels.Length; i++)
			snowPixels[i] = new Color(1 - snowPixels[i].r, 1 - snowPixels[i].g, 1 - snowPixels[i].b, snowPixels[i].a);
		fadeSnowTex.SetPixels(snowPixels);
		fadeSnowTex.Apply();

		snowMat = new Material(Shader.Find("Dissolve/Transition"));
		snowMat.SetTexture("_TransitionTex", fadeSnowTex);
	}

	void Update()
	{
		progress = Player.Instance.transform.position.magnitude;
		if (distance - progress < .05)
			this.enabled = false;
	}
	void FixedUpdate() => snowMat.SetFloat(_CutoffID, EaseMethods.CubicEaseIn(1 - progress / distance, 0, 8, 2));
	void OnRenderImage(RenderTexture src, RenderTexture dest) => Graphics.Blit(src, dest, snowMat);
}
