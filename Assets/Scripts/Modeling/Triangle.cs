using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CSG
{
    /// <summary>
    /// An ordered triple of vertices representing a triangle of a Model
    /// </summary>
    public class Triangle
    {
        /// <summary>
        /// The three vertices of this triangle
        /// </summary>
        public List<Vertex> vertices;

        /// <summary>
        /// Creates a Triangle with the three given vertices
        /// </summary>
        /// <param name="a">The first Vertex of the triangle</param>
        /// <param name="b">The second Vertex of the triangle</param>
        /// <param name="c">The third Vertex of the triangle</param>
        public Triangle(Vertex a, Vertex b, Vertex c)
        {
            vertices = new List<Vertex> { a, b, c };

            // LINQ vertices.ForEach(v => v.triangles.Add(this));
            a.triangles.Add(this);
            b.triangles.Add(this);
            c.triangles.Add(this);
            // ENDLINQ
        }

        /// <summary>
        /// Determines whether the given Vertex is one of this Triangle's vertices
        /// </summary>
        /// <param name="vertex">The Vertex to compare against</param>
        /// <returns>True if the Vertex is contained, false if it is not</returns>
        public bool ContainsVertex(Vertex vertex)
        {
            return vertices.Contains(vertex);
        }

        /// <summary>
        /// Returns the normal vector of this Triangle as defined by the ordering of its vertices
        /// </summary>
        /// <returns>The calculated normal Vector3</returns>
        public Vector3 CalculateNormal()
        {
            //1>3 X 1>2
            return Vector3.Cross(vertices[2].value - vertices[0].value, vertices[1].value - vertices[0].value).normalized;
        }

        /// <summary>
        /// Flips the order of the vertex list in order to flip the triangle's facing
        /// </summary>
        public void FlipNormal()
        {
            vertices.Reverse();
        }

        public override string ToString() // public override string ToString() => $"{base.ToString()}::{String.Join("::",vertices.Select(v=>v.value))}";
        {
            return base.ToString() + " :: " + vertices[0].value + " :: " + vertices[1].value + " :: " + vertices[2].value;
        }
    }

}
