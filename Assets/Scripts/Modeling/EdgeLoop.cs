using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CSG
{
    /// <summary>
    /// An ordered list of vertices representing an edge loop
    /// </summary>
    public class EdgeLoop
    {
        /// <summary>
        /// The vertices of this edge loop in order
        /// </summary>
        public List<Vertex> vertices;

        /// <summary>
        /// Some loops surround floating subloops, this points to that
        /// </summary>
        public EdgeLoop nestedLoop;

        /// <summary>
        /// Whether this edge loop should be filled during triangulation
        /// </summary>
        public bool filled;

        /// <summary>
        /// Creates an empty EdgeLoop
        /// </summary>
        public EdgeLoop()
        {
            vertices = new List<Vertex>();
        }

        /// <summary>
        /// Creates an EdgeLoop with a shallow copy of the given List of vertices
        /// </summary>
        /// <param name="vertices"></param>
        public EdgeLoop(List<Vertex> vertices)
        {
            this.vertices = new List<Vertex>(vertices);
        }

        /// <summary>
        /// Creates a series of Triangles that cover the surface of this EdgeLoop
        /// </summary>
        /// <returns>The created triangles</returns>
        public List<Triangle> Triangulate()
        {
            /*if(vertices.Count < 3)
            {
                return new List<Triangle>();
            }*/
            List<Triangle> triangles = new List<Triangle>();
            List<Vertex> currentVertices = new List<Vertex>(vertices);

            //ear method
            int i = 0;
            while (currentVertices.Count > 3)
            {
                i++;
                if(i > 300)
                {
                    throw new System.Exception();
                }
                triangles.Add(CreateNextEar(currentVertices));
            }
            triangles.Add(new Triangle(currentVertices[0], currentVertices[1], currentVertices[2]));

            return triangles;
        }

        private Triangle CreateNextEar(List<Vertex> currentVertices)
        {
            for (int i = 0; i < currentVertices.Count; i++)
            {
                Vector3 a = currentVertices[i].value - currentVertices[(i + 1) % currentVertices.Count].value;
                Vector3 b = currentVertices[(i + 2) % currentVertices.Count].value - currentVertices[(i + 1) % currentVertices.Count].value;
                if (Vector3.SignedAngle(a, b, Vector3.Cross(a, b)) > 0)
                {
                    Triangle resultingTriangle = new Triangle(currentVertices[i], currentVertices[(i + 1) % currentVertices.Count], currentVertices[(i + 2) % currentVertices.Count]);
                    if(!TriangleContainsAny(currentVertices, resultingTriangle))
                    {
                        currentVertices.RemoveAt((i + 1) % currentVertices.Count);
                        return resultingTriangle;
                    }

                }
            }

            return null;
        }

        private bool TriangleContainsAny(List<Vertex> vertices, Triangle triangle)
        {
            foreach(Vertex vertex in vertices)
            {
                if(!triangle.vertices.Contains(vertex) && vertex.LiesWithinTriangle(triangle))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Finds the unit vector normal to the plane defined by this EdgeLoop
        /// </summary>
        /// <returns>The normal vector</returns>
        public Vector3 GetNormal()
        {
            return Vector3.Cross(vertices[0].value - vertices[1].value, vertices[2].value - vertices[1].value).normalized;
        }

        public override string ToString() => $"{base.ToString()}::{string.Join("::", vertices.Select(v => v.value.ToString("F4")))}";
    }

}