using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CSG
{
    /// <summary>
    /// Supplies methods for performing constructive solid geometry boolean operations
    /// </summary>
    public partial class Operations : MonoBehaviour
    {
        [Tooltip("Breakpoint for equality comparisons between floats")]
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
            combine[0].mesh = ClipAToB(shapeA, shapeB, true);
            combine[0].transform = Matrix4x4.identity;
            combine[1].mesh = ClipAToB(shapeB, shapeA, true);
            combine[1].transform = Matrix4x4.identity;

            ConvertMeshCoordinates(combine[1].mesh, shapeB, shapeA);

            Mesh completedMesh = new Mesh();
            completedMesh.CombineMeshes(combine);
            completedMesh.RecalculateNormals();
            completedMesh.RecalculateTangents();

            return completedMesh;
        }

        /// <summary>
        /// Generates the subtraction of two shapes
        /// </summary>
        /// <param name="shapeA">The first shape to subtract</param>
        /// <param name="shapeB">The second shape to subtract</param>
        /// <returns>The subtraction of the two shapes</returns>
        public Mesh Subtract(GameObject shapeA, GameObject shapeB)
        {
            CombineInstance[] combine = new CombineInstance[2];
            combine[0].mesh = ClipAToB(shapeA, shapeB, false);
            combine[0].transform = Matrix4x4.identity;
            combine[1].mesh = ClipAToB(shapeB, shapeA, true, true);
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
        private Mesh ClipAToB(GameObject toClip, GameObject bounds, bool clipInside = true, bool flipNormals = false)
        {
            Model bound = new Model(bounds.GetComponent<MeshFilter>().mesh);
            bound.ConvertCoordinates(bounds.transform, toClip.transform);

            Model modelToClip = new Model(toClip.GetComponent<MeshFilter>().mesh);

            // to create the triangles, we'll need a list of edge loops to triangulate
            List<EdgeLoop> edgeLoops;

            if (clipInside)
            {
                edgeLoops = modelToClip.triangles.SelectMany(triangle => ClipTriangleToBound(triangle, bound.triangles, modelToClip.vertices, PointContainedByBound)).ToList();
            }
            else
            {
                edgeLoops = modelToClip.triangles.SelectMany(triangle => ClipTriangleToBound(triangle, bound.triangles, modelToClip.vertices, PointExcludedByBound)).ToList();
            }

            // replace the list of triangles with the clipped version
            modelToClip.triangles.Clear();

            // fill the edge loops that need to be filled, add the result to the list of triangles
            foreach (EdgeLoop loop in edgeLoops)
            {
                if (loop.filled)modelToClip.triangles.AddRange(loop.Triangulate());
                EdgeLoop nestedLoop = loop.nestedLoop;
                while (nestedLoop != null)
                {
                    if (nestedLoop.filled)
                    {
                        modelToClip.triangles.AddRange(nestedLoop.Triangulate());
                    }
                    nestedLoop = nestedLoop.nestedLoop;
                }
            }

            if (flipNormals)
            {
                foreach (Triangle triangle in modelToClip.triangles)
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
        private List<EdgeLoop> ClipTriangleToBound(Triangle triangle, List<Triangle> boundsTriangles, List<Vertex> vertices, Func<Vector3, List<Triangle>, bool> ContainmentCheck)
        {
            List<Egress> aToBEgresses = new List<Egress>();
            List<Egress> bToCEgresses = new List<Egress>();
            List<Egress> cToAEgresses = new List<Egress>();
            List<Vertex> internalIntersections = new List<Vertex>();
            List<Egress>[] egressesList = { aToBEgresses, bToCEgresses, cToAEgresses };

            // find all intersections between the triangle and the bound
            foreach (Triangle boundsTriangle in boundsTriangles)
            {
                IdentifyEgress(triangle.vertices[0].value, triangle.vertices[1].value, boundsTriangle, aToBEgresses);
                IdentifyEgress(triangle.vertices[1].value, triangle.vertices[2].value, boundsTriangle, bToCEgresses);
                IdentifyEgress(triangle.vertices[2].value, triangle.vertices[0].value, boundsTriangle, cToAEgresses);
                internalIntersections.AddRange(Raycast.TriangleToTriangle(boundsTriangle, triangle, error));
            }

            // combine duplicate internal intersections
            // duplicate logic as Vertex.RemoveDuplicates, merge?
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

            // create an aggregate list of egresses for use creating cuts
            List<Egress> allEgresses = egressesList.SelectMany(egress => egress).ToList();

            // store the resulting intersections in the greater vertex list
            vertices.AddRange(allEgresses);
            vertices.AddRange(internalIntersections);

            // organize the intersections into cuts
            CreateCuts(allEgresses, internalIntersections);

            // sorting egresses to be in order of consecutive appearence around the edge of the triangle
            aToBEgresses.Sort((a, b) => Math.Sign(Vector3.Distance(a.value, triangle.vertices[0].value) - Vector3.Distance(b.value, triangle.vertices[0].value)));
            bToCEgresses.Sort((a, b) => Math.Sign(Vector3.Distance(a.value, triangle.vertices[1].value) - Vector3.Distance(b.value, triangle.vertices[1].value)));
            cToAEgresses.Sort((a, b) => Math.Sign(Vector3.Distance(a.value, triangle.vertices[2].value) - Vector3.Distance(b.value, triangle.vertices[2].value)));

            // organize all triangle vertices and egress intersections in an ordered list around the perimeter
            List<Vertex> perimeter = new List<Vertex>();

            perimeter.Add(triangle.vertices[0]);
            perimeter.AddRange(aToBEgresses);
            perimeter.Add(triangle.vertices[1]);
            perimeter.AddRange(bToCEgresses);
            perimeter.Add(triangle.vertices[2]);
            perimeter.AddRange(cToAEgresses);
            perimeter.Add(triangle.vertices[0]); // a duplicate of the first vertex as a sentinel

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
                // determine whether this loop should be filled or not
                EdgeLoop loop = new EdgeLoop();

                Vertex initialVertex = perimeter[currentVertexIndex];

                // the loop defines either a surface or a hole, and we can determine that by looking at our starting entry
                // the starting entry can be one of two cases:
                // 1. an egress which is already part of one loop
                if (initialVertex is Egress)
                {
                    // in this case, the new loop is the opposite of whatever the previous loop is
                    loop.filled = !initialVertex.loops.Last().filled; // TODO: sometimes empty
                }
                else // 2. a vertex of the original triangle
                {
                    // in this case, if the vertex is contained by the bound, the loop is a surface, otherwise it's a hole
                    loop.filled = ContainmentCheck(initialVertex.value, boundsTriangles);
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

                        if (perimeter[currentVertexIndex] == initialVertex) // if we arrived at the initial vertex
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
                                if (cut.Last() == furthestCut[0])
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

                            if (perimeter[currentVertexIndex] == initialVertex) // if we arrived at the initial vertex
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
            while (unusedVertices.Count > 2)
            {
                Vertex currentVertex = unusedVertices.Last();
                EdgeLoop result = DiscoverInternalLoop(currentVertex, unusedVertices, loops);
                unusedVertices = unusedVertices.Where(vertex => !result.vertices.Contains(vertex)).ToList();
            }

            // configure the filledness of the discovered nested loops
            foreach (EdgeLoop loop in loops)
            {
                EdgeLoop nestedLoop = loop.nestedLoop;
                EdgeLoop previousLoop = loop;
                while (nestedLoop != null)
                {
                    nestedLoop.filled = !previousLoop.filled;
                    previousLoop = nestedLoop;
                    nestedLoop = nestedLoop.nestedLoop;
                }
            }

            return loops;
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

                        if (cut.Count == 1) // if we're starting a new cut
                        {
                            if (egress.cuts.Any(existingCut => existingCut[1] == intersection))
                                alreadyExists = true;
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

                        if (cut.Count == 1) // if we're starting a new cut
                        {
                            if (egress.cuts.Any(newCut => newCut[1] == (Vertex)vertex))
                                alreadyExists = true;
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
            List<Vector3> newVertices = mesh.vertices.Select(vertex => ConvertPointCoordinates(vertex, from, to)).ToList();
            mesh.SetVertices(newVertices);
        }

    }
}
