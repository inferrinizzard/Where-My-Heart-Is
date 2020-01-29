using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntangledClippable : ClipableObject
{
    //[SerializeField] public GameObject dreamPrefab;
    //[SerializeField] public GameObject realPrefab;

    public ClipableObject realVersion;
    public ClipableObject dreamVersion;

    //private GameObject previousDreamPrefab;
    //private GameObject previousRealPrefab;

    public override void UnionWith(GameObject other, CSG.Operations operations)
    {
        dreamVersion.UnionWith(other, operations);
        realVersion.Subtract(other, operations);
    }

    public void OnDreamChange(GameObject dreamPrefab)
    {
        GameObject createdObject;
        if (dreamVersion != null)
        {
            createdObject = Instantiate(dreamPrefab, dreamVersion.transform.position, dreamVersion.transform.rotation, transform);
            DestroyImmediate(dreamVersion.gameObject);
        }
        else
        {
            createdObject = Instantiate(dreamPrefab, transform);
        }
        dreamVersion = ConfigureObject("Dream", createdObject);
    }

    public void OnRealChange(GameObject realPrefab)
    {
        GameObject createdObject;
        if (realVersion != null)
        {
            createdObject = Instantiate(realPrefab, realVersion.transform.position, realVersion.transform.rotation, transform);
            DestroyImmediate(realVersion.gameObject);
        }
        else
        {
            createdObject = Instantiate(realPrefab, transform);
        }
        realVersion = ConfigureObject("Real", createdObject);

    }

    private ClipableObject ConfigureObject(string layer, GameObject createdObject)
    {
        if (createdObject.GetComponent<MeshFilter>())
        {
            createdObject.gameObject.layer = LayerMask.NameToLayer(layer);
            return createdObject.gameObject.AddComponent<ClipableObject>();
        }
        Debug.Log(":(");

        return null;
    }
}
