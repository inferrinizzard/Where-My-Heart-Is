using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// [ExecuteInEditMode]
public class PageFlip : MonoBehaviour
{
	public void Init()
	{
		gameObject.SetActive(true);
		var cam = transform.parent.GetComponent<Camera>();
		var delta = (cam.ViewportToWorldPoint(Vector3.one) - cam.ViewportToWorldPoint(Vector3.forward)) / 10;
		transform.localScale = new Vector3(delta.x, 1, delta.y);
		gameObject.SetActive(false);
	}

	public IEnumerator Flip(Texture texture)
	{
		var renderer = GetComponent<Renderer>();
		var page = renderer.sharedMaterial;
		page.SetFloat(ShaderID._Theta, 1);
		page.SetFloat(ShaderID._Rho, 0);

		page.mainTexture = texture;
		gameObject.SetActive(true);
		float time = GameManager.Instance.transitionTime;

		Player.Instance.transform.rotation = Quaternion.identity;
		Player.Instance.cam.transform.rotation = Quaternion.identity;

		for (var(start, step) = (Time.time, 0f); step < time; step = Time.time - start)
		{
			yield return null;
			page.SetVector(ShaderID._BottomLeft, renderer.bounds.min);
			page.SetFloat(ShaderID._Theta, (1 - EaseMethods.CubicEaseOut(step / time, 0, 1, 1)) * .8f + .2f);
			page.SetFloat(ShaderID._Rho, EaseMethods.CubicEaseIn(step / time, 0, 1, 1) / 4f);

		}
		gameObject.SetActive(false);
	}
}
