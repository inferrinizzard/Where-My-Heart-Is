using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipableObject : MonoBehaviour
{
	public bool volumeless;

	[HideInInspector] public InteractableObject tiedInteractable;
	[HideInInspector] public bool isClipped;
	[HideInInspector] public GameObject uncutCopy;

	Material mat;
	int _DissolveID = Shader.PropertyToID("_Dissolve");

	private int oldLayer;
	private Mesh initialMesh;
	private MeshFilter meshFilter;

	void Awake()
	{
		mat = GetComponentInChildren<MeshRenderer>().material;
		isClipped = false;

		meshFilter = GetComponent<MeshFilter>() ?? gameObject.AddComponent<MeshFilter>();
		if (GetComponent<MeshCollider>() == null)
		{
			gameObject.AddComponent<MeshCollider>();
		}
		if (meshFilter)initialMesh = meshFilter.mesh;
		oldLayer = gameObject.layer;
	}

	public virtual bool IntersectsBound(Transform boundTransform, CSG.Model bound)
	{
		if (meshFilter.mesh == null)
		{
			return false;
		}
		CSG.Model model = new CSG.Model(meshFilter.mesh);
		model.ConvertCoordinates(transform, boundTransform);
		return model.Intersects(bound, 0.001f);
	}

	public virtual void UnionWith(GameObject other, CSG.Operations operations)
	{
		isClipped = true;
		mat.SetInt(_DissolveID, 0);
		uncutCopy = GameObject.Instantiate(gameObject, transform.position, transform.rotation, transform);
		uncutCopy.transform.localScale = Vector3.one;
		oldLayer = gameObject.layer;
		gameObject.layer = 9;

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
		mat.SetInt(_DissolveID, 1);
		gameObject.layer = oldLayer;
		meshFilter.mesh = initialMesh;
		//GetComponent<Collider>().enabled = false;
		if (uncutCopy)
		{
			DestroyImmediate(uncutCopy);
		}

		UpdateInteractable();

		if (GetComponent<MeshCollider>())
		{
			GetComponent<MeshCollider>().sharedMesh = initialMesh;
		}
	}

	public void Subtract(GameObject other, CSG.Operations operations)
	{
		isClipped = true;
		mat.SetInt(_DissolveID, 0);

		oldLayer = gameObject.layer;

		if (!volumeless)
		{
			meshFilter.mesh = operations.Subtract(gameObject, other);
		}
		else
		{
			meshFilter.mesh = operations.ClipAToB(gameObject, other);
		}

		UpdateInteractable();

		if (GetComponent<MeshCollider>())
		{
			GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;
		}
	}

	private void UpdateInteractable()
	{
		if (tiedInteractable != null)
		{
			tiedInteractable.gameObject.layer = gameObject.layer;
		}
	}
}
