using System.Collections;
using System.Collections.Generic;
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
        public List<Triangle> Trianglulate()
        {
            List<Triangle> triangles = new List<Triangle>();

            for (int i = 1; i < this.vertices.Count - 1; i++)
            {
                triangles.Add(new Triangle(this.vertices[0], this.vertices[i], this.vertices[(i + 1) % this.vertices.Count]));
            }

            return triangles;
        }
    }

}


