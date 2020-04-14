using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ClippableObject : MonoBehaviour
{
	[SerializeField] bool volumeless = default;
	[HideInInspector] public InteractableObject tiedInteractable;
	[HideInInspector] public bool isClipped;
	[HideInInspector] public GameObject uncutCopy;

    private GameObject mirroredCopy;

    public CSG.Model CachedModel
	{
		get
		{
			if (model == null || transform.position != previousCutPosition)
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

		if (!this.TryComponent(out meshFilter))
			meshFilter = gameObject.AddComponent<MeshFilter>();
		if (!this.TryComponent<MeshCollider>())
			gameObject.AddComponent<MeshCollider>();
		initialMesh = meshFilter.mesh;
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

		uncutCopy = Instantiate(gameObject, transform.position, transform.rotation, transform);
		uncutCopy.transform.localScale = Vector3.one;
		oldLayer = gameObject.layer;
		gameObject.layer = 9;

		if (!volumeless)
			meshFilter.mesh = CSG.Operations.Intersect(CachedModel, other);
		else
			meshFilter.mesh = CSG.Operations.ClipAToB(CachedModel, other);

		if (this.TryComponent(out MeshCollider col))
			col.sharedMesh = meshFilter.mesh;

		UpdateInteractable();
	}

    public virtual void UnionMirrored(CSG.Model other, Matrix4x4 reflectionMatrix)
    {
        CSG.Model model = CachedModel;
        mirroredCopy = Instantiate(gameObject, transform.position, transform.rotation);
        mirroredCopy.transform.position = reflectionMatrix.MultiplyPoint(transform.position);
        //mirroredCopy.transform.localScale = reflectionMatrix.MultiplyVector(transform.localScale);

        if (!volumeless)
            mirroredCopy.GetComponent<MeshFilter>().mesh = CSG.Operations.Intersect(model, other, true, mirroredCopy.transform.worldToLocalMatrix * reflectionMatrix);
        else
            mirroredCopy.GetComponent<MeshFilter>().mesh = CSG.Operations.ClipAToB(model, other, true, true, mirroredCopy.transform.worldToLocalMatrix * reflectionMatrix);

        other.Draw(Color.red);

        if (this.TryComponent(out MeshCollider col))
            col.sharedMesh = meshFilter.mesh;

        Debug.Log(mirroredCopy);

        mirroredCopy.layer = 9;
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
            uncutCopy.GetComponent<ClippableObject>().Revert();
			DestroyImmediate(uncutCopy);
		}
        if(mirroredCopy)
        {
            DestroyImmediate(mirroredCopy);
        }

		UpdateInteractable();

		if (this.TryComponent(out MeshCollider col))
			col.sharedMesh = meshFilter.mesh;
	}

	public void Subtract(CSG.Model other)
	{
		isClipped = true;
		mat.SetInt(_DissolveID, 0);

		oldLayer = gameObject.layer;

		if (!volumeless)
			meshFilter.mesh = CSG.Operations.Subtract(CachedModel, other);
		else
			meshFilter.mesh = CSG.Operations.ClipAToB(CachedModel, other, false);

		UpdateInteractable();

		if (this.TryComponent(out MeshCollider col))
			col.sharedMesh = meshFilter.mesh;
	}

	private void UpdateInteractable()
	{
		if (tiedInteractable)
			tiedInteractable.gameObject.layer = gameObject.layer;
	}
}
