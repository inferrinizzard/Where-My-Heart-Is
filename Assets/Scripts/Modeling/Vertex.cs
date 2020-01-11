using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CSG
{
    public class Vertex
    {
        public int index;
        public Vector3 value;
        public bool containedByBound;
        public bool usedInLoop;
        public List<EdgeLoop> loops; // how many loops have been found to belong to this point
        public List<Triangle> triangles;

        public Vertex(int index, Vector3 value, bool containedByBound)
        {
            this.index = index;
            this.value = value;
            this.containedByBound = containedByBound;

            loops = new List<EdgeLoop>();
            usedInLoop = false;
            triangles = new List<Triangle>();
        }

        public bool SharesTriangle(Vertex vertex)
        {
            //Debug.Log("Do I even have a triangle? " + triangles.Count);
            foreach (Triangle myTriangle in triangles)
            {
                foreach (Triangle theirTriangle in vertex.triangles)
                {
                    if (myTriangle == theirTriangle)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override string ToString()
        {
            return value.ToString("F4");
        }

        public bool LiesWithinLoop(EdgeLoop loop)
        {
            //Debug.Log("kjshfd");


            // collect intersection points
            Vector3 castDirection = (loop.vertices[0].value - loop.vertices[1].value).normalized;
            List<Vector3> positiveIntersections = new List<Vector3>();
            List<Vector3> negativeIntersections = new List<Vector3>();
            for (int i = 0; i < loop.vertices.Count; i++)
            {
                Point3 intersection = IntersectLineWithEdge(
                    this.value, 
                    castDirection, 
                    loop.vertices[i].value, 
                    loop.vertices[(i + 1) % loop.vertices.Count].value);
                if (intersection != null)
                {
                    Vector3 directionToIntersection = (intersection.value - this.value).normalized;
                    if (Vector3.Dot(directionToIntersection, castDirection) > 0)
                    {
                        positiveIntersections.Add(intersection.value);
                    }
                    else
                    {
                        negativeIntersections.Add(intersection.value);
                    }
                }
            }

            // remove duplicates
            RemoveDuplicates(positiveIntersections);
            RemoveDuplicates(negativeIntersections);

            // count # above, below
            // if both odd, return true, else return false
            return positiveIntersections.Count % 2 == 1 && negativeIntersections.Count % 2 == 1;
        }

        //TODO: this should be elsewhere
        private void RemoveDuplicates(List<Vector3> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                for (int k = i - 1; k >= 0; k--)
                {
                    // TODO: unhardcode
                    if (Vector3.Distance(list[i], list[k]) < 0.0001)
                    {
                        list.RemoveAt(i);
                    }
                }
            }
        }

        //TODO: this should be elsewhere
        // assumes the line and edge lie on the same plane
        private Point3 IntersectLineWithEdge(Vector3 origin, Vector3 direction, Vector3 pointA, Vector3 pointB)
        {
            Vector3 edgeDirection = (pointA - pointB).normalized;

            //TODO: unhardcode breakpoint
            if(Vector3.Cross(direction, edgeDirection).magnitude < 0.0001)
            {
                //Debug.Log("sup");
                return null;
            }

            float u = (pointA.x - origin.x - (direction.x / direction.y) * (pointA.y - origin.y)) /
                (((direction.x / direction.y) * edgeDirection.y) - edgeDirection.x);
            if (float.IsNaN(u))
            {
                u = (pointA.x - origin.x - (direction.x / direction.z) * (pointA.z - origin.z)) /
                (((direction.x / direction.z) * edgeDirection.z) - edgeDirection.x);
            }

            Vector3 intersectionPoint = pointA + (edgeDirection * u);
            //Debug.Log(direction.y);
            //TODO: unhardcode
            //Debug.Log(Vector3.Distance(pointA, intersectionPoint) + Vector3.Distance(intersectionPoint, pointB) - Vector3.Distance(pointA, pointB));
            if (Vector3.Distance(pointA, intersectionPoint) + Vector3.Distance(intersectionPoint, pointB) - Vector3.Distance(pointA, pointB) < 0.0001)
            {
                return new Point3(intersectionPoint);
            }
            else return null;
        }
    }
}

