using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTriangleCast : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CSG.Triangle tri = new CSG.Triangle(
            new CSG.Vertex(0, new Vector3(0, 0, 0)), 
            new CSG.Vertex(0, new Vector3(0, 1, 0)), 
            new CSG.Vertex(0, new Vector3(1, 1, 0)));

        CSG.Vertex vertex = new CSG.Vertex(0, new Vector3(0.2f, 0.5f, 0));
        Debug.Log(vertex.LiesWithinTriangle(tri));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
