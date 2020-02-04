using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipableObject : MonoBehaviour
{
	//private GameObject result;
	public bool volumeless;

	public bool isClipped;

	private Mesh initialMesh;
	public GameObject uncutCopy;

	int oldLayer;

	MeshFilter meshFilter;

	void Awake()
	{
		isClipped = false;

		meshFilter = GetComponent<MeshFilter>() ?? gameObject.AddComponent<MeshFilter>();
		// if (GetComponent<MeshFilter>() == null)
		// {
		// 	meshFilter = gameObject.AddComponent<MeshFilter>();
		// }

		initialMesh = meshFilter.mesh;
		oldLayer = gameObject.layer;
	}

	public bool IntersectsBound(Transform boundTransform, CSG.Model bound)
	{
		CSG.Model model = new CSG.Model(meshFilter.mesh);
		model.ConvertCoordinates(transform, boundTransform);
		return model.Intersects(bound, 0.001f);
	}

	// TODO: APPLY IN PLACE
	public virtual void UnionWith(GameObject other, CSG.Operations operations)
	{
		isClipped = true;
		uncutCopy = GameObject.Instantiate(gameObject, transform.position, transform.rotation, transform);
		uncutCopy.transform.localScale = Vector3.one;
		oldLayer = gameObject.layer;
		gameObject.layer = 9;
		GetComponent<Collider>().enabled = true;

		if (!volumeless)
		{
			meshFilter.mesh = operations.Intersect(gameObject, other);
		}
		else
		{
			meshFilter.mesh = operations.ClipAToB(gameObject, other);
		}
	}

	public virtual void Revert()
	{
		gameObject.layer = oldLayer;
		meshFilter.mesh = initialMesh;
		GetComponent<Collider>().enabled = false;
		if (uncutCopy)Destroy(uncutCopy);
	}

	// TODO: APPLY IN PLACE
	public void Subtract(GameObject other, CSG.Operations operations)
	{
		isClipped = true;

		oldLayer = gameObject.layer;

		if (!volumeless)
		{
			meshFilter.mesh = operations.Subtract(gameObject, other);
		}
		else
		{
			meshFilter.mesh = operations.ClipAToB(gameObject, other, false);
			//meshFilter.mesh.RecalculateNormals();
			//meshFilter.mesh.RecalculateTangents();
		}
	}
}
