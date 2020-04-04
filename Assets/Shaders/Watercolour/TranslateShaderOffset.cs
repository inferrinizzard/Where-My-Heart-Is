using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TranslateShaderOffset : MonoBehaviour
{
	public string[] properties;
	public Vector2[] velocity;

	private List<Vector2> offset = new List<Vector2>();

	// Use this for initialization
	void Start()
	{
		foreach (string p in properties)
			offset.Add(GetComponent<Renderer>().material.GetTextureOffset(p));
		// offset = GetComponent<Renderer>().material.GetTextureOffset(properties);
	}

	// Update is called once per frame
	void Update()
	{
		for (int i = 0; i < offset.Count; i++)
		{
			offset[i] += velocity[i] * Time.deltaTime;
			GetComponent<Renderer>().material.SetTextureOffset(properties[i], offset[i]);
		}
	}
}
