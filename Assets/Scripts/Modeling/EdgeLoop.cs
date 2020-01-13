using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSG
{
    public class EdgeLoop
    {
        public List<Vertex> vertices;
        public EdgeLoop nestedLoop;
        public bool filled;

        public EdgeLoop()
        {
            vertices = new List<Vertex>();
        }

        public EdgeLoop(List<Vertex> vertices)
        {
            this.vertices = new List<Vertex>(vertices);
        }

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


