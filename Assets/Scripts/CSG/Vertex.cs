using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CSG
{
    /// <summary>
    /// A wrapper for Vector3 that includes useful data for clipping meshes
    /// </summary>
    public class Vertex
    {
        /// <summary>
        /// Index in the mesh's vertex array
        /// </summary>
        public int index;

        /// <summary>
        /// Location in model space
        /// </summary>
        public Vector3 value;

        /// <summary>
        /// Whether this vertex has been identified to be in at least one loop
        /// </summary>
        public bool usedInLoop;

        /// <summary>
        /// A list of EdgeLoops that this vertex has been identified to be a part of
        /// </summary>
        public List<EdgeLoop> loops;

        /// <summary>
        /// A list of triangles this vertex appears in
        /// </summary>
        public List<Triangle> triangles;

        /// <summary>
        /// A list of cuts across the associated triangle.
        /// </summary>
        /// <remarks>
        /// Typically, there will be only one cut, but occasionally there are multiple.
        /// </remarks>
        //public List<Cut> cuts;
        public Cut cut;

        public Transform referenceFrame;

        public bool fromIntersection;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">Index in the mesh's vertex array</param>
        /// <param name="value">Location in model space</param>
        /// <param name="containedByBound">Whether the vertex lies within the bounding shape</param>
        public Vertex(int index, Vector3 value)
        {
            this.index = index;
            this.value = value;

            loops = new List<EdgeLoop>();
            triangles = new List<Triangle>();
            cut = null;
            usedInLoop = false;
            fromIntersection = false; 
        }

        /// <summary>
        /// Determines whether this vertex and the given vertex both appear on the same triangle
        /// </summary>
        /// <param name="vertex">The vertex to compare against</param>
        /// <returns>Whether this vertex and the given vertex both appear on the same triangle</returns>
        public bool SharesTriangle(Vertex vertex) => triangles.Any(t => vertex.triangles.Contains(t));

        public override string ToString()
        {
            return value.ToString("F4");
        }

        /// <summary>
        /// Determines whether this vertex lies inside the area of the given loop, assuming they share a plane
        /// </summary>
        /// <param name="loop">The loop to check for containment</param>
        /// <returns>Whether this vertex lies inside the area of the given loop, assuming they share a plane</returns>
        public bool LiesWithinLoop(EdgeLoop loop)
        {
            // collect intersection points
            Vector3 castDirection = (loop.vertices[0].value - loop.vertices[1].value).normalized;
            List<Vector3> positiveIntersections = new List<Vector3>();
            for (int i = 0; i < loop.vertices.Count; i++)
            {
                Point3 intersection = Raycast.RayToLineSegment(
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
                }
            }

            // remove duplicates
            RemoveDuplicates(positiveIntersections);

            // count #
            // if odd, return true, else return false
            return positiveIntersections.Count % 2 == 1;// && negativeIntersections.Count % 2 == 1;
        }

        public bool LiesOnEdge(Edge edge)
        {
            float error = 0.001f;
            return Raycast.PointLiesOnLineSegment(value, edge.vertices[0].value, edge.vertices[1].value, error);
        }

        /// <summary>
        /// Determines whether this vertex lies inside the area of the given triangle, assuming they share a plane
        /// </summary>
        /// <param name="triangle">The triangle to check for containment</param>
        /// <returns>Whether this vertex lies inside the area of the given triangle, assuming they share a plane</returns>
        public bool LiesWithinTriangle(Triangle triangle)
        {
            triangle.UpdateEdges();
            // starting with b->p
            for(int i = 0; i < 3; i++)
            {
                Vector3 edgeVector = triangle.edges[(i + 1) % 3].GetVector();
                Vector3 axis = Vector3.Cross(triangle.edges[i].GetVector(), edgeVector);
                Vector3 toPoint = value - triangle.vertices[(i + 1) % 3].value;

                if (Vector3.SignedAngle(edgeVector, toPoint, axis) <= 0)
                {
                    return false;
                }
            }

            return true;
            // collect intersection points
            /*Vector3 castDirection = (triangle.vertices[0].value - triangle.vertices[1].value).normalized;
            List<Vector3> positiveIntersections = new List<Vector3>();
            for (int i = 0; i < triangle.vertices.Count; i++)
            {
                Point3 intersection = Raycast.RayToLineSegment(
                    this.value,
                    castDirection,
                    triangle.vertices[i].value,
                    triangle.vertices[(i + 1) % triangle.vertices.Count].value);
                if (intersection != null)
                {
                    Vector3 directionToIntersection = (intersection.value - this.value).normalized;
                    if (Vector3.Dot(directionToIntersection, castDirection) > 0)
                    {
                        positiveIntersections.Add(intersection.value);
                    }
                }
            }

            // remove duplicates
            RemoveDuplicates(positiveIntersections);

            // count #
            // if odd, return true, else return false
            return positiveIntersections.Count % 2 == 1;// && negativeIntersections.Count % 2 == 1;*/
        }

        /// <summary>
        /// Takes a List and merges any vertices that are too similar
        /// </summary>
        /// <param name="list"> The list to remove duplicates from </param>
        /// <param name="margin"> The distance cutoff to clip duplicates </param>
        private void RemoveDuplicates(List<Vector3> list, double margin = .0001)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                for (int k = i; k >= 0; k--)
                {
                    if (Vector3.Distance(list[i], list[k]) < margin)
                    {
                        list.RemoveAt(i);
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Determines whether the given point is contained by the given model
        /// </summary>
        /// <param name="model">the model to check for containment of this point</param>
        /// <returns>True if the point is contained, false if it is not</returns>
        public bool ContainedBy(Model model, float error)
        {
            int intersectionsAbove = 0;
            int intersectionsBelow = 0;

            List<Vector3> intersections = model.triangles.Select(tri => Raycast.RayToTriangle(this.value, Vector3.up, tri)).Where(tri => tri != null).Select(tri => tri.value).ToList();

            // merge nearby points to eliminate double counting
            // duplicate logic as Vertex.RemoveDuplicates, merge?
            // intersections.RemoveAll(a => intersections.Any(b => Vector3.Distance(a, b) < error)); TODO fix
            for (int i = intersections.Count - 1; i > 0; i--)
            {
                for (int k = i - 1; k >= 0; k--)
                {
                    if (Vector3.Distance(intersections[i], intersections[k]) < error)
                    {
                        intersections.Remove(intersections[i]);
                        break;
                    }
                }
            }

            // count up intersections above and below
            // LINQ return intersections.GroupBy(intersection => intersection.y > point.y).All(l => l.Count() % 1 == 0); TODO

            for (int i = 0; i < intersections.Count; i++)
            {
                if (intersections[i].y > this.value.y)
                {
                    intersectionsAbove++;
                }
                else
                {
                    intersectionsBelow++;
                }
            }

            // If above and below are odd, vertex is contained
            // if they are even, vertex is not contained
            return intersectionsAbove % 2 == 1 && intersectionsBelow % 2 == 1;
            // ENDLINQ
        }

        /// <summary>
        /// Finds and returns the cut among cuts which travels the furthest around the perimeter of the triangle
        /// </summary>
        /// <param name="perimeter">The perimeter in question</param>
        /// <returns>The cut that travels the furthest</returns>
        /*public Cut GetFurthestCut(List<Vertex> perimeter)
        {
            Cut bestCut = null;
            int bestCutIndex = -1;

            foreach (Cut cut in cuts)
            {
                if (cut.traversed == false)
                {
                    int perimeterIndex = perimeter.IndexOf(cut[cut.Count - 1]);
                    if (perimeterIndex > bestCutIndex)
                    {
                        bestCut = cut;
                        bestCutIndex = perimeterIndex;
                    }
                }
            }

            return bestCut;
        }*/

        /// <summary>
        /// Finds and returns the cut among cuts which travels the furthest around the perimeter of the triangle 
        /// while still terminating at a perimeter index between the given constraints.
        /// </summary>
        /// <param name="perimeter">The perimeter in question</param>
        /// <param name="ignore">A cut that should be ignored when searching for the furthest cut</param>
        /// <param name="minIndex">The minimum index a cut must arrive at to be returned</param>
        /// <param name="targetIndex">The index of the initial vertex of the current loop being traversed</param>
        /// <returns>The cut satisfying all constraints</returns>
        /*public Cut GetFurthestCut(List<Vertex> perimeter, Cut ignore, int minIndex, int targetIndex)
        {
            Cut bestCut = null;
            int bestCutIndex = -1;

            foreach (Cut cut in cuts)
            {
                if (cut.traversed == false && cut != ignore)
                {
                    int perimeterIndex = perimeter.IndexOf(cut[cut.Count - 1]);
                    if (perimeterIndex > bestCutIndex)
                    {
                        bestCut = cut;
                        bestCutIndex = perimeterIndex;
                    }
                }
            }

            if (bestCutIndex < minIndex && bestCutIndex != targetIndex)
            {
                //Debug.Log(bestCutIndex);
                return null;
            }

            return bestCut;
        }*/

        public void Draw(float length, Vector3 direction, Color color)
        {
            Debug.DrawLine(value, value + direction.normalized * length, color);
        }
    }
}
