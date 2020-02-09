using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSG
{
    public class Intersection
    {
        public Triangle triangle;
        public Vertex vertex;

        // this is an alternative to below I believe
        public Edge edge;

        // maybe this?
        public List<Intersection> adjacentIntersections;// only two of these allowed
        //(or three in the special case of an edge-edge intersection)

        public Intersection(Triangle triangle, Vertex vertex, Edge edge)
        {
            this.triangle = triangle;
            this.vertex = vertex;
            this.edge = edge;
        }
    }
}

