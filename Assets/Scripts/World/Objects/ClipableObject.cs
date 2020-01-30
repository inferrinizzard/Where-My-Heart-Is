using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipableObject : MonoBehaviour
{
	//private GameObject result;

	private Mesh initialMesh;
    private GameObject uncutCopy;

    int oldLayer;

	void Awake()
	{
		if (GetComponent<MeshFilter>() == null)
		{
			gameObject.AddComponent<MeshFilter>();
		}

		initialMesh = GetComponent<MeshFilter>().mesh;
        oldLayer = gameObject.layer;
	}

    // TODO: APPLY IN PLACE
	public virtual void UnionWith(GameObject other, CSG.Operations operations)
	{
        uncutCopy = GameObject.Instantiate(gameObject, transform.position, transform.rotation, transform);
        oldLayer = gameObject.layer;
        gameObject.layer = 9;

		GetComponent<MeshFilter>().mesh = operations.Union(gameObject, other);
	}

    public virtual void Revert()
    {
        gameObject.layer = oldLayer;
        GetComponent<MeshFilter>().mesh = initialMesh;
        if(uncutCopy) Destroy(uncutCopy);
    }

    // TODO: APPLY IN PLACE
    public void Subtract(GameObject other, CSG.Operations operations)
	{
        oldLayer = gameObject.layer;
        GetComponent<MeshFilter>().mesh = operations.Subtract(gameObject, other);
	}
}
