using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parabox.CSG;

public class TestUnion : MonoBehaviour
{
    public GameObject model;
    public GameObject output;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Test()
    {
        //model.GetComponent<MeshFilter>().mesh = model.GetComponent<MeshFilter>().mesh.
        Parabox.CSG.CSG_Model result = Parabox.CSG.Boolean.Intersect(model, gameObject);
        result.mesh.RecalculateNormals();
        result.mesh.RecalculateBounds();

        //GameObject currentShape = new GameObject();
        output.GetComponent<MeshFilter>().sharedMesh = result.mesh;
        output.GetComponent<MeshRenderer>().sharedMaterials = result.materials.ToArray();
    }
}
