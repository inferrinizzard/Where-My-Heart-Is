using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex
{
    public int index;
    public Vector3 value;
    public bool containedByBound;
    public bool active;
    public List<Triangle> triangles;

    public Vertex(int index, Vector3 value, bool containedByBound)
    {
        this.index = index;
        this.value = value;
        this.containedByBound = containedByBound;
        active = true;
        triangles = new List<Triangle>();
    }

    public bool SharesTriangle(Vertex vertex)
    {
        foreach(Triangle myTriangle in triangles)
        {
            foreach(Triangle theirTriangle in vertex.triangles)
            {
                if(myTriangle == theirTriangle)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
