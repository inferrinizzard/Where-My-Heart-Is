using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Test summary
/// </summary>
public class TestUnion : MonoBehaviour
{
    public bool test;
    public bool testRolling;

    public GameObject model;
    public GameObject bounds;
    public GameObject output;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        if (test || testRolling)
        {
            test = false;
            Test();
        }
    }

    public void Test()
    {
        //model.GetComponent<MeshFilter>().mesh = model.GetComponent<MeshFilter>().mesh.
        /*output.GetComponent<MeshFilter>().mesh = CSG.Operations.Intersect(
            new CSG.Model(model.GetComponent<MeshFilter>().mesh, transform),
            new CSG.Model(bounds.GetComponent<MeshFilter>().mesh, transform));*/

        //output.GetComponent<MeshFilter>().mesh = result;

        //GameObject currentShape = new GameObject();
        /*output.GetComponent<MeshFilter>().sharedMesh = result.mesh;
        output.GetComponent<MeshRenderer>().sharedMaterials = result.materials.ToArray();*/
    }
}
