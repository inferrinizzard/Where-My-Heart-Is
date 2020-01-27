using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipableObject : MonoBehaviour
{
	private GameObject result;

	Mesh initialMesh;

	void Awake()
	{
		if (GetComponent<MeshFilter>() == null)
		{
			gameObject.AddComponent<MeshFilter>();
		}

		initialMesh = GetComponent<MeshFilter>().mesh;

	}

	public void UnionWith(GameObject other, CSG.Operations operations)
	{
		if (result != null)
		{
			Destroy(result);
		}

		result = GameObject.Instantiate(gameObject, transform.position, transform.rotation);
		result.transform.localScale = transform.localScale;
		result.layer = 9;

		result.GetComponent<MeshFilter>().mesh = operations.Union(gameObject, other);
		result.GetComponent<MeshCollider>().sharedMesh = result.GetComponent<MeshFilter>().mesh;
	}

	public void Subtract(GameObject other, CSG.Operations operations)
	{
		GetComponent<MeshFilter>().mesh = initialMesh;
		GetComponent<MeshFilter>().mesh = operations.Subtract(gameObject, other);
	}
}
