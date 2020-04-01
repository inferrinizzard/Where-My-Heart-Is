using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ClippableObject : MonoBehaviour
{
	public bool volumeless;

	[HideInInspector] public InteractableObject tiedInteractable;
	[HideInInspector] public bool isClipped;
	[HideInInspector] public GameObject uncutCopy;

    public CSG.Model CachedModel
    {
        get
        {
            if(model == null || transform.position != previousCutPosition)
            {
                model = new CSG.Model(initialMesh, transform);
                model.ConvertToWorld();
                previousCutPosition = transform.position;
            }

            return model;
        }
    }

	Material mat;
	int _DissolveID = Shader.PropertyToID("_Dissolve");

	private int oldLayer;
	private Mesh initialMesh;
	private MeshFilter meshFilter;
    private CSG.Model model;
    private Vector3 previousCutPosition;

	void Awake()
	{
		mat = GetComponentInChildren<MeshRenderer>().material;
		isClipped = false;

		meshFilter = GetComponent<MeshFilter>() ?? gameObject.AddComponent<MeshFilter>();
		if (GetComponent<MeshCollider>() == null)
		{
			gameObject.AddComponent<MeshCollider>();
		}
		if (meshFilter) initialMesh = meshFilter.mesh;
		oldLayer = gameObject.layer;
	}

	public virtual bool IntersectsBound(CSG.Model bound)
	{
		return CachedModel.Intersects(bound, 0.001f);
	}

	public virtual void UnionWith(CSG.Model other)
	{
		isClipped = true;
		mat.SetInt(_DissolveID, 0);
		uncutCopy = GameObject.Instantiate(gameObject, transform.position, transform.rotation, transform);
		uncutCopy.transform.localScale = Vector3.one;
		oldLayer = gameObject.layer;
		gameObject.layer = 9;

		if (!volumeless)
		{
            meshFilter.mesh = CSG.Operations.Intersect(CachedModel, other);
		}
		else
		{
			meshFilter.mesh = CSG.Operations.ClipAToB(CachedModel, other);
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

	public void Subtract(CSG.Model other)
	{
		isClipped = true;
		mat.SetInt(_DissolveID, 0);

		oldLayer = gameObject.layer;

		if (!volumeless)
		{
			meshFilter.mesh = CSG.Operations.Subtract(CachedModel, other);
		}
		else
		{
			meshFilter.mesh = CSG.Operations.ClipAToB(CachedModel, other);
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
