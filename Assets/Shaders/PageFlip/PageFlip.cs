using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// [ExecuteInEditMode]
public class PageFlip : MonoBehaviour
{
	void Start()
	{
		gameObject.SetActive(true);
		var cam = transform.parent.GetComponent<Camera>();
		var delta = (cam.ViewportToWorldPoint(Vector3.one) - cam.ViewportToWorldPoint(Vector3.forward)) / 10;
		transform.localScale = new Vector3(delta.x, 1, delta.y);
		gameObject.SetActive(false);
	}

	public IEnumerator Flip(Texture texture, float time = 3f)
	{
		int posID = Shader.PropertyToID("_BottomLeft");
		int thetaID = Shader.PropertyToID("_Theta"), rhoID = Shader.PropertyToID("_Rho");

		var renderer = GetComponent<Renderer>();
		var page = renderer.sharedMaterial;
		page.SetFloat(thetaID, 1);
		page.SetFloat(rhoID, 0);

		page.mainTexture = texture;
		gameObject.SetActive(true);

		Player.Instance.transform.rotation = Quaternion.identity;
		Player.Instance.cam.transform.rotation = Quaternion.identity;

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
