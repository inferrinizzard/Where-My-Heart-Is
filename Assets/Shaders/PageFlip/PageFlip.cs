using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// [ExecuteInEditMode]
public class PageFlip : MonoBehaviour
{
	Camera cam;
	Texture texture;
	Material page;
	int progress = Shader.PropertyToID("");

	void Start()
	{
		cam = transform.parent.GetComponent<Camera>();
		var delta = (cam.ViewportToWorldPoint(Vector3.one) - cam.ViewportToWorldPoint(Vector3.forward)) / 10;
		transform.localScale = new Vector3(delta.x, 1, delta.y);

		page = GetComponent<Renderer>().sharedMaterial;

		StartCoroutine(Flip());
	}

	void Update()
	{
		page.SetVector("_BottomLeft", GetComponent<Renderer>().bounds.min);

		var progress = Mathf.PingPong(Time.time / 10, 1);
		// page.SetFloat("_Theta", .25f); // 1 - > 25 fast, .25-> 0 slow
		// page.SetFloat("_Rho", progress); 0->.25
	}

	void AssignTexture(Texture tex) => texture = tex;

	IEnumerator Flip(float time = 3f)
	{
		int posID = Shader.PropertyToID("_BottomLeft");
		var renderer = GetComponent<Renderer>();
		int thetaID = Shader.PropertyToID("_Theta"), rhoID = Shader.PropertyToID("_Rho");
		page.SetFloat(thetaID, 1);
		page.SetFloat(rhoID, 0);

		// page.mainTexture = texture;
		gameObject.SetActive(true);
		for (var(start, step) = (Time.time, 0f); step < time; step = Time.time - start)
		{
			yield return null;
			page.SetVector(posID, renderer.bounds.min);
			page.SetFloat(thetaID, (1 - EaseMethods.CubicEaseOut(step / time, 0, 1, 1)) * .8f + .2f);
			page.SetFloat(rhoID, EaseMethods.CubicEaseIn(step / time, 0, 1, 1) / 4f);

		}
		gameObject.SetActive(false);
	}
}
