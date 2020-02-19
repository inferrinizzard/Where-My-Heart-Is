using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSG
{
    public class Edge
    {
        public List<Vertex> vertices;// the vertices that define this edge
        public List<Intersection> intersections;// a place to store the intersections this edge has with the other shape
        public List<Triangle> triangles;

        public Transform referenceFrame;

        public Edge(Vertex vertexA, Vertex vertexB)
        {
            vertices = new List<Vertex>();
            vertices.Add(vertexA);
            vertices.Add(vertexB);
            intersections = new List<Intersection>();
            triangles = new List<Triangle>();
        }

        public Intersection IntersectWithTriangle(Triangle triangle)
        {
            float error = 0.0001f;//TODO
            Vertex vertex = Raycast.LineSegmentToTriangle(vertices[0].value, vertices[1].value, triangle, error);
            if (vertex != null) vertex.fromIntersection = true;
            return vertex != null ? new Intersection(triangle, vertex, this) : null;
        }

        public Vector3 GetVector()
        {
            return vertices[1].value - vertices[0].value;
        }

        public override string ToString()
        {
            return base.ToString() + ": " + vertices[0] + " :: " + vertices[1];
        }

        public void Draw(Color color)
        {
            Debug.DrawLine(vertices[0].value, vertices[1].value, color);
        }
    }
}


