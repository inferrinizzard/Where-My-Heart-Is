using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipableObject : MonoBehaviour
{
    //private GameObject result;
    public bool volumeless;

    public bool isClipped;

	private Mesh initialMesh;
    private GameObject uncutCopy;

    int oldLayer;

	void Awake()
	{
        isClipped = false;

        if (GetComponent<MeshFilter>() == null)
		{
			gameObject.AddComponent<MeshFilter>();
		}

		initialMesh = GetComponent<MeshFilter>().mesh;
        oldLayer = gameObject.layer;
	}

    public bool IntersectsBound(Transform boundTransform, CSG.Model bound)
    {
        CSG.Model model = new CSG.Model(GetComponent<MeshFilter>().mesh);
        model.ConvertCoordinates(transform, boundTransform);
        return model.Intersects(bound, 0.001f);
    }

    // TODO: APPLY IN PLACE
	public virtual void UnionWith(GameObject other, CSG.Operations operations)
	{
        isClipped = true;
        uncutCopy = GameObject.Instantiate(gameObject, transform.position, transform.rotation, transform);
        uncutCopy.transform.localScale = new Vector3(1,1,1);
        oldLayer = gameObject.layer;
        gameObject.layer = 9;
        GetComponent<Collider>().enabled = true;

        if (!volumeless)
        {
            GetComponent<MeshFilter>().mesh = operations.Union(gameObject, other);
        }
        else
        {
            GetComponent<MeshFilter>().mesh = operations.ClipAToB(gameObject, other);
        }
    }

    public virtual void Revert()
    {
        gameObject.layer = oldLayer;
        GetComponent<MeshFilter>().mesh = initialMesh;
        GetComponent<Collider>().enabled = false;
        if (uncutCopy) Destroy(uncutCopy);
    }

    // TODO: APPLY IN PLACE
    public void Subtract(GameObject other, CSG.Operations operations)
	{
        isClipped = true;

        oldLayer = gameObject.layer;
        
        if (!volumeless)
        {
            GetComponent<MeshFilter>().mesh = operations.Subtract(gameObject, other);
        }
        else
        {
            GetComponent<MeshFilter>().mesh = operations.ClipAToB(gameObject, other, false);
            //GetComponent<MeshFilter>().mesh.RecalculateNormals();
            //GetComponent<MeshFilter>().mesh.RecalculateTangents();
        }
    }
}
