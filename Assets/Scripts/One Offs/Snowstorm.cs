using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Snowstorm : MonoBehaviour
{
	[SerializeField] Shader transition = default;
	[SerializeField] Texture2D rawSnow = default;
	Material snowMat;
	int _CutoffID = Shader.PropertyToID("_Cutoff");

	bool snowOn = true;
	float progress = 0;
	[SerializeField] float distance = 100;

	void Start()
	{
		var fadeSnowTex = new Texture2D(rawSnow.width, rawSnow.height);
		Graphics.CopyTexture(rawSnow, 0, 0, fadeSnowTex, 0, 0);
		var snowPixels = rawSnow.GetPixels();
		for (int i = 0; i < snowPixels.Length; i++)
			snowPixels[i] = new Color(1 - snowPixels[i].r, 1 - snowPixels[i].g, 1 - snowPixels[i].b, snowPixels[i].a);
		fadeSnowTex.SetPixels(snowPixels);
		fadeSnowTex.Apply();

		snowMat = new Material(transition);
		snowMat.SetTexture("_TransitionTex", fadeSnowTex);
	}

	void Update()
	{
		progress = Player.Instance.transform.position.magnitude;
		if (progress >= distance)
			snowOn = false;
	}

	void FixedUpdate()
	{
		if (snowOn)
			snowMat.SetFloat(_CutoffID, 1 - progress / distance); // add curve here	
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest) => Graphics.Blit(src, dest, snowOn ? snowMat : null);
}
