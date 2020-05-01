using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[ExecuteInEditMode]
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
	}

	void Update()
	{
		page.SetVector("_BottomLeft", GetComponent<Renderer>().bounds.min);
	}

	void AssignTexture(Texture tex) => texture = tex;

	IEnumerator Flip(float time = 3f)
	{
		// page.mainTexture = texture;
		gameObject.SetActive(true);
		for (float start = Time.time; Time.time - start > time;)
		{
			yield return null;
		}
		gameObject.SetActive(false);
	}
}
