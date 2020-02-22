using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipableObject : MonoBehaviour
{
	public bool volumeless;

	[HideInInspector] public InteractableObject tiedInteractable;
	[HideInInspector] public bool isClipped;
	[HideInInspector] public GameObject uncutCopy;

	private int oldLayer;
	private Mesh initialMesh;
	private MeshFilter meshFilter;

	void Awake()
	{
		isClipped = false;

		meshFilter = GetComponent<MeshFilter>() ?? gameObject.AddComponent<MeshFilter>();
		if (meshFilter)initialMesh = meshFilter.mesh;
		oldLayer = gameObject.layer;
	}

	public virtual bool IntersectsBound(Transform boundTransform, CSG.Model bound)
	{
		CSG.Model model = new CSG.Model(meshFilter.mesh);
		model.ConvertCoordinates(transform, boundTransform);
		return model.Intersects(bound, 0.001f);
	}

	public virtual void UnionWith(GameObject other, CSG.Operations operations)
	{
		isClipped = true;
		uncutCopy = GameObject.Instantiate(gameObject, transform.position, transform.rotation, transform);
		uncutCopy.transform.localScale = Vector3.one;
		oldLayer = gameObject.layer;
		gameObject.layer = 9;
		//GetComponent<Collider>().enabled = true;

		if (!volumeless)
		{
			meshFilter.mesh = operations.Intersect(gameObject, other);
		}
		else
		{
			meshFilter.mesh = operations.ClipAToB(gameObject, other);
		}

		if (GetComponent<MeshCollider>())
		{
			GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;
		}

		UpdateInteractable();
	}

	public virtual void Revert()
	{
        isClipped = false;
		gameObject.layer = oldLayer;
		meshFilter.mesh = initialMesh;
		//GetComponent<Collider>().enabled = false;
		if (uncutCopy != null)
		{
			DestroyImmediate(uncutCopy);
		}

		UpdateInteractable();
	}

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
		}

		UpdateInteractable();
	}

	private void UpdateInteractable()
	{
		if (tiedInteractable != null)
		{
			tiedInteractable.GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;
			tiedInteractable.gameObject.layer = gameObject.layer;
		}
	}
}
