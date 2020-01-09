using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSG
{

    public class Triangle
    {
        public List<Vertex> vertices;

        public Triangle(Vertex a, Vertex b, Vertex c)
        {
            vertices = new List<Vertex>();

            vertices.Add(a);
            vertices.Add(b);
            vertices.Add(c);

            a.triangles.Add(this);
            b.triangles.Add(this);
            c.triangles.Add(this);
        }

        public bool ContainsVertex(Vertex vertex)
        {
            return vertex == vertices[0] || vertex == vertices[1] || vertex == vertices[2];
        }

        public override string ToString()
        {
            return base.ToString() + " :: " + vertices[0].value + " :: " + vertices[1].value + " :: " + vertices[2].value;
        }

        public Vector3 CalculateNormal()
        {
            //1>3 X 1>2
            return Vector3.Cross(vertices[2].value - vertices[0].value, vertices[1].value - vertices[0].value).normalized;
        }

        public void FlipNormal()
        {
            vertices.Reverse();
        }
        /*
        public Triangle(Vertex a, Vertex b, Vertex c, bool aContained, bool bContained, bool cContained)
        {
            vertices.Add(a);
            vertices.Add(b);
            vertices.Add(c);

            vertices[0].containedByBound = aContained;
            vertices[1].containedByBound = bContained;
            vertices[2].containedByBound = cContained;
        }*/
    }

}

