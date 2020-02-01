using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntangledClippable : ClipableObject
{
    public ClipableObject realVersion;
    public ClipableObject dreamVersion;

    public bool Visable
    {
        get
        {
            return dreamVersion.gameObject.GetComponent<MeshRenderer>().enabled;
        }

        set
        {
            //realVersion.gameObject.GetComponent<MeshRenderer>().enabled = value;
            dreamVersion.gameObject.GetComponent<MeshRenderer>().enabled = value;
        }
    }

    public override void UnionWith(GameObject other, CSG.Operations operations)
    {
        isClipped = true;

        realVersion.UnionWith(other, operations);
        dreamVersion.Subtract(other, operations);
    }

    public override void Revert()
    {
        realVersion.Revert();
        dreamVersion.Revert();
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
