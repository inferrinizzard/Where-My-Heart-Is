﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CSG
{
    /// <summary>
    /// Supplies methods for performing constructive solid geometry boolean operations
    /// </summary>
    public class Operations : MonoBehaviour
    {
        [Tooltip("breakpoint for equality comparisons between floats")]
        [SerializeField] private float error;

        /// <summary>
        /// Generates the union of two shapes
        /// </summary>
        /// <param name="shapeA">The first shape to union</param>
        /// <param name="shapeB">The second shape to union</param>
        /// <returns>The union of the two shapes</returns>
        public Mesh Union(GameObject shapeA, GameObject shapeB)
        {
            CombineInstance[] combine = new CombineInstance[2];
            combine[0].mesh = ClipAToB(shapeA, shapeB);
            combine[0].transform = Matrix4x4.identity;
            combine[1].mesh = ClipAToB(shapeB, shapeA);
            combine[1].transform = Matrix4x4.identity;

            ConvertMeshCoordinates(combine[1].mesh, shapeB, shapeA);

            Mesh completedMesh = new Mesh();
            completedMesh.CombineMeshes(combine);
            completedMesh.RecalculateNormals();
            completedMesh.RecalculateTangents();

            return completedMesh;
        }

        /// <summary>
        /// Generates a mesh that matches the portion of the given "toClip" object's mesh contained by the 
        /// bounding object "bounds"
        /// </summary>
        /// <param name="toClip">The GameObject containing the Mesh to clip</param>
        /// <param name="bounds">The GameObject containing the Mesh to clip "toClip" to</param>
        /// <param name="flipNormals">Whether the normals of the resulting mesh should be flipped</param>
        /// <returns>The clipped Mesh</returns>
        private Mesh ClipAToB(GameObject toClip, GameObject bounds, bool flipNormals = false)
        {
            Model bound = new Model(bounds.GetComponent<MeshFilter>().mesh);
            bound.ConvertCoordinates(bounds.transform, toClip.transform);

            Model modelToClip = new Model(toClip.GetComponent<MeshFilter>().mesh);

            // to create the triangles, we'll need a list of edge loops to triangluate
            List<EdgeLoop> edgeLoops = new List<EdgeLoop>();

            // clip each triangle to only the portion contained by the bound
            foreach (Triangle triangle in modelToClip.triangles)
            {
                edgeLoops.AddRange(ClipTriangleToBound(triangle, bound.triangles, modelToClip.vertices));
            }

            // replace the list of triangles with the clipped version
            modelToClip.triangles.Clear();

            // fill the edge loops that need to be filled, add the result to the list of triangles
            foreach (EdgeLoop loop in edgeLoops)
            {
                if (loop.filled) modelToClip.triangles.AddRange(loop.Trianglulate());
                EdgeLoop nestedLoop = loop.nestedLoop;
                while (nestedLoop != null)
                {
                    if (nestedLoop.filled) modelToClip.triangles.AddRange(nestedLoop.Trianglulate());
                    nestedLoop = nestedLoop.nestedLoop;
                }
            }

            if(flipNormals)
            {
                foreach(Triangle triangle in modelToClip.triangles)
                {
                    triangle.FlipNormal();
                }
            }

            return modelToClip.ToMesh();
        }

        /// <summary>
        /// Clips a single Triangle to just its parts contained by the bound
        /// </summary>
        /// <param name="triangle">The triangle to clip</param>
        /// <param name="boundsTriangles">A list of triangles defining the bounding object</param>
        /// <param name="vertices">A list of the vertices of the object the triangle belongs to</param>
        /// <returns>A list of edge loops which define the clipped version of the triangle</returns>
        private List<EdgeLoop> ClipTriangleToBound(Triangle triangle, List<Triangle> boundsTriangles, List<Vertex> vertices)
        {
            List<Egress> aToBEgresses = new List<Egress>();
            List<Egress> bToCEgresses = new List<Egress>();
            List<Egress> cToAEgresses = new List<Egress>();
            List<Vertex> internalIntersections = new List<Vertex>();

            // find all intersections between the triangle and the bound
            foreach (Triangle boundsTriangle in boundsTriangles)
            {
                IdentifyEgress(triangle.vertices[0].value, triangle.vertices[1].value, boundsTriangle, aToBEgresses);
                IdentifyEgress(triangle.vertices[1].value, triangle.vertices[2].value, boundsTriangle, bToCEgresses);
                IdentifyEgress(triangle.vertices[2].value, triangle.vertices[0].value, boundsTriangle, cToAEgresses);
                internalIntersections.AddRange(Raycast.TriangleToTriangle(boundsTriangle, triangle, error));
            }

            // combine duplicate internal intersections
            for (int i = internalIntersections.Count - 1; i > 0; i--)
            {
                for (int k = i - 1; k >= 0; k--)
                {
                    if (Vector3.Distance(internalIntersections[i].value, internalIntersections[k].value) < error)
                    {
                        internalIntersections[k].triangles.AddRange(internalIntersections[i].triangles);
                        internalIntersections.RemoveAt(i);
                        break;
                    }
                }
            }

            // store the resulting intersections in the greater vertex list
            vertices.AddRange(aToBEgresses);
            vertices.AddRange(bToCEgresses);
            vertices.AddRange(cToAEgresses);
            vertices.AddRange(internalIntersections);

            // create an aggregate list of egresses for use creating cuts
            List<Egress> allEgresses = new List<Egress>();
            allEgresses.AddRange(aToBEgresses);
            allEgresses.AddRange(bToCEgresses);
            allEgresses.AddRange(cToAEgresses);

            // organize the intersections into cuts
            CreateCuts(allEgresses, internalIntersections);


            // sorting egresses to be in order of consecutive appearence around the edge of the triangle
            aToBEgresses.Sort((a, b) => (int)Mathf.Sign(Vector3.Distance(a.value, triangle.vertices[0].value) - Vector3.Distance(b.value, triangle.vertices[0].value)));
            bToCEgresses.Sort((a, b) => (int)Mathf.Sign(Vector3.Distance(a.value, triangle.vertices[1].value) - Vector3.Distance(b.value, triangle.vertices[1].value)));
            cToAEgresses.Sort((a, b) => (int)Mathf.Sign(Vector3.Distance(a.value, triangle.vertices[2].value) - Vector3.Distance(b.value, triangle.vertices[2].value)));

            // organize all triangle vertices and egress intersections in an ordered list around the perimeter
            List<Vertex> perimeter = new List<Vertex>();
            
            perimeter.Add(triangle.vertices[0]);
            perimeter.AddRange(aToBEgresses);
            perimeter.Add(triangle.vertices[1]);
            perimeter.AddRange(bToCEgresses);
            perimeter.Add(triangle.vertices[2]);
            perimeter.AddRange(cToAEgresses);
            perimeter.Add(triangle.vertices[0]);// a duplicate of the first vertex as a sentinel


            // find all edge loops and classify whether they should be retopologized or not
            List<EdgeLoop> loops = new List<EdgeLoop>();

            foreach (Vertex vertex in triangle.vertices)
            {
                vertex.loops = new List<EdgeLoop>();
            }

            // while there are still entries in that list for which we haven't identified all loops (1 for triangle vertices, 2 for egresses)
            int currentVertexIndex = FindEarliestUnsatisfied(perimeter);
            while (currentVertexIndex != -1)
            {
                // we've satisfied all loops, which is our exit condition
                if (currentVertexIndex == -1)
                {
                    break;
                }

                // determine whether this loop should be filled or not
                EdgeLoop loop = new EdgeLoop();

                Vertex initialVertex = perimeter[currentVertexIndex];

                // the loop defines either a surface or a hole, and we can determine that by looking at our starting entry
                // the starting entry can be one of two cases:
                // 1. an egress which is already part of one loop
                if (initialVertex is Egress)
                {
                    // in this case, the new loop is the opposite of whatever the previous loop is
                    loop.filled = !initialVertex.loops[initialVertex.loops.Count - 1].filled;
                }
                else // 2. a vertex of the original triangle
                {
                    // in this case, if the vertex is contained by the bound, the loop is a surface, otherwise it's a hole
                    loop.filled = PointContainedByBound(initialVertex.value, boundsTriangles);
                }

                // traverse forward in the ordered list, following each cut, until we reach the initial point
                // once the inital entry is reached again, we have identified a loop, and can add it to the pile
                do
                {
                    Cut furthestCut = null;

                    if (perimeter[currentVertexIndex] is Egress)
                    {
                        furthestCut = ((Egress)perimeter[currentVertexIndex]).GetFurthestCut(perimeter);
                    }

                    if (furthestCut != null)
                    {
                        currentVertexIndex = TraverseCut(furthestCut, loop, perimeter, currentVertexIndex);

                        if (perimeter[currentVertexIndex] == initialVertex)// if we arrived at the initial vertex
                        {
                            loop.vertices.RemoveAt(loop.vertices.Count - 1);
                            break;
                        }

                        while (((Egress)perimeter[currentVertexIndex]).cuts.Count > 1)
                        {
                            // select cut
                            Cut toIgnore = null;
                            foreach (Cut cut in ((Egress)perimeter[currentVertexIndex]).cuts)
                            {
                                if (cut[cut.Count - 1] == furthestCut[0])
                                {
                                    toIgnore = cut;
                                    break;
                                }
                            }
                            furthestCut = ((Egress)perimeter[currentVertexIndex]).GetFurthestCut(perimeter, toIgnore, currentVertexIndex, perimeter.IndexOf(initialVertex));

                            if (furthestCut != null)
                            {
                                currentVertexIndex = TraverseCut(furthestCut, loop, perimeter, currentVertexIndex);
                            }
                            else
                            {
                                break;
                            }

                            if (perimeter[currentVertexIndex] == initialVertex)// if we arrived at the initial vertex
                            {
                                loop.vertices.RemoveAt(loop.vertices.Count - 1);
                                goto FinalStep;
                            }
                        }
                    }
                    else
                    {
                        // add current vertex to loop
                        loop.vertices.Add(perimeter[currentVertexIndex]);
                        perimeter[currentVertexIndex].loops.Add(loop);
                    }

                    currentVertexIndex++;

                    // once the current vertex loops around to the start, we're done
                } while (perimeter[currentVertexIndex] != initialVertex);

                FinalStep:

                loops.Add(loop);

                currentVertexIndex = FindEarliestUnsatisfied(perimeter);
            }

            // now that we've created all loops that intersect the edge of the triangle, we can start on loops that float as islands
            List<Vertex> unusedVertices = internalIntersections.Where(intersection => !intersection.usedInLoop).ToList();

            while(unusedVertices.Count > 0)
            {
                Vertex currentVertex = unusedVertices[unusedVertices.Count - 1];
                EdgeLoop result = DiscoverInternalLoop(currentVertex, unusedVertices, loops);
                unusedVertices = (unusedVertices.Where(vertex => !result.vertices.Contains(vertex))).ToList();
            }

            // configure the filledness of the discovered nested loops
            foreach(EdgeLoop loop in loops)
            {
                EdgeLoop nestedLoop = loop.nestedLoop;
                EdgeLoop previousLoop = loop;
                while(nestedLoop != null)
                {
                    nestedLoop.filled = !previousLoop.filled;
                    previousLoop = nestedLoop;
                    nestedLoop = nestedLoop.nestedLoop;
                }
            }


            return loops;
        }

        /// <summary>
        /// Finds the earliest appearing Vertex in a triangle's perimeter that hasn't had all it's loops identified.
        /// Returns -1 if no unsatisfied vertices are left
        /// </summary>
        /// <remarks>
        /// <para>
        /// Each of the original vertices of a triangle always appear in exactly 1 loop each
        /// </para>
        /// <para>
        /// Each Egress appears in one more loop than it has Cuts
        /// </para>
        /// </remarks>
        /// <param name="perimeter">The perimeter to examine for incomplete vertices</param>
        /// <returns>The index of the earliest unsatisfied vertex in the loop, or -1 if none are left.</returns>
        private int FindEarliestUnsatisfied(List<Vertex> perimeter)
        {
            // find the earliest entry whose loops aren't satisfied   
            int index = 0;
            for (index = 0; index < perimeter.Count - 2; index++)
            {
                // for egresses:
                if (perimeter[index] is Egress)
                {
                    // if this one is unsatisfied, break. We've found the earliest
                    if (perimeter[index].loops.Count < ((Egress)perimeter[index]).cuts.Count + 1)
                    {
                        break;
                    }
                }
                else // for original vertices
                {
                    // if this one is unsatisfied, break. We've found the earliest
                    if (perimeter[index].loops.Count < 1)
                    {
                        break;
                    }
                }
            }

            if (index == perimeter.Count - 2)
            {
                return -1;
            }

            return index;
        }

        /// <summary>
        /// Finds the loop associated with the given intersection with a triangle and places it as a child of the correct containing edge loop
        /// </summary>
        /// <param name="initialVertex">A vertex on the loop to find</param>
        /// <param name="unusedVertices">A list of vertices that could possibly be part of the loop</param>
        /// <param name="loops">A list of all non-internal loops found for the current triangle</param>
        /// <returns>An edge loop defining the loop associated with the given vertex</returns>
        private EdgeLoop DiscoverInternalLoop (Vertex initialVertex, List<Vertex> unusedVertices, List<EdgeLoop> loops)
        {
            Queue<EdgeLoop> potentialLoops = new Queue<EdgeLoop>();
            List<EdgeLoop> completedLoops = new List<EdgeLoop>();

            potentialLoops.Enqueue(new EdgeLoop());
            potentialLoops.Peek().vertices.Add(initialVertex);
            while(potentialLoops.Count > 0)
            {
                Vertex secondVertex = null;
                Vertex nextVertex;
                do
                {
                    nextVertex = null;
                    List<Vertex> loopVertices = potentialLoops.Peek().vertices;
                    for (int i = unusedVertices.Count - 1; i >= 0; i--)
                    {
                        if (unusedVertices[i].SharesTriangle(loopVertices[loopVertices.Count - 1]) && 
                            (!loopVertices.Contains(unusedVertices[i]) || (unusedVertices[i] == initialVertex && secondVertex != null)))
                        {
                            if(nextVertex == null)
                            {
                                nextVertex = unusedVertices[i];

                                if(secondVertex == null)
                                {
                                    secondVertex = nextVertex;
                                }
                            }
                            else
                            {
                                EdgeLoop newLoop = new EdgeLoop(loopVertices);
                                newLoop.vertices.Add(unusedVertices[i]);
                                potentialLoops.Enqueue(newLoop);
                            }
                        }
                    }
                    if (nextVertex != null && nextVertex != initialVertex)
                    {
                        loopVertices.Add(nextVertex);
                    }
                } while (nextVertex != null && nextVertex != initialVertex);

                if (nextVertex != null)
                {
                    completedLoops.Add(potentialLoops.Dequeue());
                }
                else
                {
                    potentialLoops.Dequeue();// we did not arrive back at the initial vertex, discard the potential loop
                }
            }

            // the loop with the greatest number of vertices is the correct loop
            EdgeLoop finalLoop = completedLoops[0];
            foreach(EdgeLoop loop in completedLoops)
            {
                if(loop.vertices.Count > finalLoop.vertices.Count)
                {
                    finalLoop = loop;
                }
            }

            // determine which external loop contains this new loop
            foreach (EdgeLoop loop in loops)
            {
                if (initialVertex.LiesWithinLoop(loop))
                {
                    if (loop.nestedLoop == null)
                    {
                        loop.nestedLoop = finalLoop;

                    }
                    else
                    {
                        finalLoop.nestedLoop = loop.nestedLoop;
                        EdgeLoop previousLoop = loop;
                        do
                        {
                            if (initialVertex.LiesWithinLoop(finalLoop.nestedLoop))
                            {
                                previousLoop.nestedLoop = finalLoop.nestedLoop;
                                finalLoop.nestedLoop = finalLoop.nestedLoop.nestedLoop;
                                previousLoop.nestedLoop.nestedLoop = finalLoop;
                            }
                            else
                            {
                                break;
                            }
                        } while (finalLoop.nestedLoop != null);
                    }

                    // we've found the loop we're contained by, break out
                    break;
                }
            }

            return finalLoop;
        }

        /// <summary>
        /// Marks the given Cut as traversed and adds its vertices to the greater loop being traversed
        /// </summary>
        /// <param name="cut">The Cut being traversed</param>
        /// <param name="loop">The EdgeLoop being traversed</param>
        /// <param name="perimeter">The parimeter of the current triangle</param>
        /// <param name="currentVertexIndex">The index of the vertex at the beginning of the Cut</param>
        /// <returns>The index of the vertex arrived at by traversing the Cut</returns>
        private int TraverseCut(Cut cut, EdgeLoop loop, List<Vertex> perimeter, int currentVertexIndex)
        {
            cut.traversed = true;

            // add all vertices of the egress's cut to the loop
            loop.vertices.AddRange(cut);
            perimeter[currentVertexIndex].loops.Add(loop);

            // find the index of the last vertex in the cut (which is always an egress) and get its index in the perimeter
            currentVertexIndex = perimeter.IndexOf(cut[cut.Count - 1]);
            perimeter[currentVertexIndex].loops.Add(loop);

            return currentVertexIndex;
        }

        /// <summary>
        /// Identifies a new egress created by the intersection of the edge defined by points A and B and a given bounds triangle if there is one
        /// </summary>
        /// <remarks>
        /// Sometimes an Egress is identified multiple times. In these cases, the Egress is not double counted
        /// </remarks>
        /// <param name="pointA">The first point of the intersecting edge</param>
        /// <param name="pointB">The second point of the intersecting edge</param>
        /// <param name="boundsTriangle">The Triangle to intersect the edge with</param>
        /// <param name="egresses">A list of existing egresses to store the new Egress in</param>
        private void IdentifyEgress(Vector3 pointA, Vector3 pointB, Triangle boundsTriangle, List<Egress> egresses)
        {
            Vertex intersectionPoint;

            intersectionPoint = Raycast.LineSegmentToTriangle(pointA, pointB, boundsTriangle, error);

            if (intersectionPoint != null)
            {
                bool alreadyExists = false;
                foreach (Egress egress in egresses)
                {
                    if (Vector3.Distance(intersectionPoint.value, egress.value) < error)
                    {
                        egress.triangles.Add(boundsTriangle);
                        alreadyExists = true;
                        break;
                    }
                }
                if (!alreadyExists)
                {
                    intersectionPoint.triangles.Add(boundsTriangle);
                    egresses.Add(Egress.CreateFromVertex(intersectionPoint));
                }
            }
        }

        /// <summary>
        /// Takes a list of Egresses and finds all Cuts that join them
        /// </summary>
        /// <param name="egresses">The list of Egresses to find cuts for</param>
        /// <param name="intersections">The list of internal intersections that form the intermediate points in the created cuts</param>
        private void CreateCuts(List<Egress> egresses, List<Vertex> intersections)
        {
            foreach (Egress egress in egresses)
            {
                Cut cut = new Cut();
                cut.Add(egress);
                Vertex currentVertex = egress;
                bool foundNext = false;
                do
                {
                    foundNext = false;

                    // prioritize progressing along internal intersections first
                    foreach (Vertex intersection in intersections)
                    {
                        bool alreadyExists = false;

                        if (cut.Count == 1)// if we're starting a new cut
                        {
                            foreach (Cut existingCut in egress.cuts)// check each of the previous cuts to ensure that it doesn't already exist
                            {
                                if (existingCut[1] == intersection)
                                {
                                    alreadyExists = true;
                                }
                            }
                        }

                        if (intersection.SharesTriangle(currentVertex) && !cut.Contains(intersection) && !alreadyExists)
                        {
                            foundNext = true;
                            cut.Add(intersection);
                            intersection.usedInLoop = true;
                            currentVertex = intersection;
                        }
                    }

                    // then find egresses that will terminate the cut
                    foreach (Egress vertex in egresses)
                    {
                        bool alreadyExists = false;

                        if (cut.Count == 1)// if we're starting a new cut
                        {
                            foreach (Cut existingCut in egress.cuts)// check each of the previous cuts to ensure that it doesn't already exist
                            {
                                if (existingCut[1] == vertex)
                                {
                                    alreadyExists = true;
                                }
                            }
                        }

                        if (vertex.SharesTriangle(currentVertex) && !cut.Contains(vertex) && !alreadyExists)
                        {
                            foundNext = true;
                            cut.Add(vertex);
                            currentVertex = vertex;
                            break;
                        }
                    }
                } while (!(currentVertex is Egress) && foundNext);

                if (foundNext && currentVertex != egress)
                {
                    egress.cuts.Add(cut);
                    ((Egress)currentVertex).cuts.Add(cut.GetReversedCopy());
                }
            }
        }

        /// <summary>
        /// Converts a Vector 3 from the local coordinate system defined by "from" to a target coordinate system defined by "to"
        /// </summary>
        /// <param name="toTransform">The point to convert</param>
        /// <param name="from">The GameObject defining the initial coordinate space</param>
        /// <param name="to">The GameObject defining the target coordinate space</param>
        /// <returns>The converted point</returns>
        private Vector3 ConvertPointCoordinates(Vector3 toTransform, GameObject from, GameObject to)
        {
            Vector3 point = from.transform.localToWorldMatrix.MultiplyPoint3x4(toTransform);
            point = to.transform.worldToLocalMatrix.MultiplyPoint3x4(point);
            return point;
        }

        /// <summary>
        /// Converts all vertex locations of the given mesh from their local coordinate system defined by "from" to a
        /// target coordinate system defined by "to"
        /// </summary>
        /// <param name="mesh">The Mesh whose coordinates will be converted</param>
        /// <param name="from">The GameObject defining the initial coordinate space</param>
        /// <param name="to">The GameObject defining the target coordinate space</param>
        private void ConvertMeshCoordinates(Mesh mesh, GameObject from, GameObject to)
        {
            List<Vector3> newVertices = new List<Vector3>();
            foreach (Vector3 vector in mesh.vertices)
            {
                newVertices.Add(ConvertPointCoordinates(vector, from, to));
            }

            mesh.SetVertices(newVertices);
        }

        /// <summary>
        /// Determines whether the given point is contained by the given bound
        /// </summary>
        /// <param name="point">The point to check for containment</param>
        /// <param name="boundsTriangles">A List of triangles representing the bounding shape</param>
        /// <returns>True if the point is contained, false if it is not</returns>
        private bool PointContainedByBound(Vector3 point, List<Triangle> boundsTriangles)
        {
            int intersectionsAbove = 0;
            int intersectionsBelow = 0;

            List<Vector3> intersections = new List<Vector3>();

            foreach (Triangle boundsTriangle in boundsTriangles)
            {
                Vertex intersectionPoint = Raycast.RayToTriangle(point, Vector3.up, boundsTriangle);
                if (intersectionPoint != null)
                {
                    intersections.Add(intersectionPoint.value);
                }
            }

            // merge nearby points to eliminate double counting
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
            for (int i = 0; i < intersections.Count; i++)
            {
                if (intersections[i].y > point.y)
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
        }
    }
}


