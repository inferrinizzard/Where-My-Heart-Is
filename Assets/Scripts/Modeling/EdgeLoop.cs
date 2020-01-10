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
    }

}


