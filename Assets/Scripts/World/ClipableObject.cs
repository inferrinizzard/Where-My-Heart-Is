using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipableObject : MonoBehaviour
{
    public CSG.Operations operations;
    public GameObject ClippingResultPrefab;

    public void UnionWith(GameObject other)
    {
        GameObject result = GameObject.Instantiate(ClippingResultPrefab);
        result.transform.position = this.transform.position;
        result.transform.rotation = this.transform.rotation;

        operations.Union(this.gameObject, other, result);
    }

    public void Subtract(GameObject other)
    {
        //TODO: use subtract when available
        /*GameObject result = GameObject.Instantiate(ClippingResultPrefab);
        result.transform.position = this.transform.position;
        result.transform.rotation = this.transform.rotation;

        operations.Union(this.gameObject, other, result);*/
    }
}
