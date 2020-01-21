using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSG
{
    public class Operations : MonoBehaviour
    {
        public bool foo;

        public float egressSimilarity;
        public float error;
        public float intersectionError;

        public int triangleCount;

        public void Union(GameObject toClip, GameObject bounds, GameObject output)
        {
            //Debug.Log("==========================UNION============================");
            CombineInstance[] combine = new CombineInstance[2];
            combine[0].mesh = ClipAToB(toClip, bounds, output);
            combine[0].transform = Matrix4x4.identity;
            combine[1].mesh = ClipAToB(bounds, toClip, output);
            combine[1].transform = Matrix4x4.identity;

            ConvertCoords(bounds, toClip, combine[1].mesh);

            Mesh completedMesh = new Mesh();
            completedMesh.CombineMeshes(combine);
            output.GetComponent<MeshFilter>().mesh = completedMesh;
        }

        private void ConvertCoords(GameObject from, GameObject to, Mesh mesh)
        {
            List<Vector3> newVertices = new List<Vector3>();
            foreach (Vector3 vector in mesh.vertices)
            {
                newVertices.Add(BoundsSpaceToModelSpace(vector, to, from));
            }

            mesh.SetVertices(newVertices);
        }

        // classify existing verticies
        // perform new cut operation on each face of the bounded shape
        // add back to mesh

        public Mesh ClipAToB(GameObject toClip, GameObject bounds, GameObject result, bool flipNormals = false)
        {
            //Debug.Log("hello");
            // get a list of all triangles of the bounding mesh
            List<Triangle> boundsTriangles = GetBoundsTriangles(toClip, bounds);

            // get a list of all vertices of the clipping target
            List<Vertex> vertices = ClassifyVertices(toClip, bounds, boundsTriangles);

            // get a list of all triangles of the clipping target
            List<Triangle> meshTriangles = GetAllTriangles(toClip, vertices);

            // here is where the final triangles will be stored
            List<Triangle> triangles = new List<Triangle>();

            // to create the triangles, we'll need a list of edge loops to triangluate
            List<EdgeLoop> edgeLoops = new List<EdgeLoop>();

            //TODO: this is debug code that I need for now
            // fill the edge loops that need to be filled, add the result to the list of triangles
            if (triangleCount > -1 && triangleCount < meshTriangles.Count)
            {
                edgeLoops.AddRange(ClipTriangleToBound(meshTriangles[triangleCount], boundsTriangles, vertices, true));
                foreach (EdgeLoop loop in edgeLoops)
                {
                    if (loop.filled)triangles.AddRange(TriangulateEdgeLoop(loop));
                    EdgeLoop nestedLoop = loop.nestedLoop;
                    while (nestedLoop != null)
                    {
                        if (nestedLoop.filled)triangles.AddRange(TriangulateEdgeLoop(nestedLoop));
                        nestedLoop = nestedLoop.nestedLoop;
                    }

                }
            }
            else if (triangleCount < meshTriangles.Count)
            {
                foreach (Triangle triangle in meshTriangles)
                {
                    edgeLoops.AddRange(ClipTriangleToBound(triangle, boundsTriangles, vertices));
                }

                foreach (EdgeLoop loop in edgeLoops)
                {
                    if (loop.filled)triangles.AddRange(TriangulateEdgeLoop(loop));
                    EdgeLoop nestedLoop = loop.nestedLoop;
                    while (nestedLoop != null)
                    {
                        Debug.Log("here");
                        if (nestedLoop.filled)triangles.AddRange(TriangulateEdgeLoop(nestedLoop));
                        nestedLoop = nestedLoop.nestedLoop;
                    }
                }
            }
            else
            {
                return null;
            }

            if (flipNormals)
            {
                foreach (Triangle triangle in triangles)
                {
                    triangle.FlipNormal();
                }
            }

            Mesh completedMesh = new Mesh();

            result.GetComponent<MeshFilter>().mesh = completedMesh;

            // reindex vertices and add them to the mesh
            List<Vector3> createdVertices = new List<Vector3>();

            // add vertices to mesh
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i].index = i;
                createdVertices.Add(vertices[i].value);
            }
            completedMesh.SetVertices(createdVertices);

            // add triangles to mesh
            int[] newTriangles = new int[triangles.Count * 3];
            int index = 0;

            foreach (Triangle triangle in triangles)
            {
                foreach (Vertex vertex in triangle.vertices)
                {
                    if (!vertices.Contains(vertex))
                    {
                        Debug.Log("Vertex " + vertex + " not contained in vertices, :: " + vertex.containedByBound);
                    }
                }
            }

            //Debug.Log(triangles.Count);

            foreach (Triangle triangle in triangles)
            {
                newTriangles[index] = triangle.vertices[0].index;
                index++;
                newTriangles[index] = triangle.vertices[1].index;
                index++;
                newTriangles[index] = triangle.vertices[2].index;
                index++;
            }

            completedMesh.SetTriangles(newTriangles, 0);

            completedMesh.RecalculateTangents();
            completedMesh.RecalculateNormals();

            return completedMesh;
        }

        private List<Triangle> TriangulateEdgeLoop(EdgeLoop loop, bool debug = false)
        {
            List<Triangle> triangles = new List<Triangle>();

            for (int i = 1; i < loop.vertices.Count - 1; i++)
            {
                triangles.Add(new Triangle(loop.vertices[0], loop.vertices[i], loop.vertices[(i + 1) % loop.vertices.Count]));
            }

            return triangles;
        }

        private List<EdgeLoop> ClipTriangleToBound(Triangle triangle, List<Triangle> boundsTriangles, List<Vertex> vertices, bool debug = false)
        {

            List<Egress> aToBEgresses = new List<Egress>();
            List<Egress> bToCEgresses = new List<Egress>();
            List<Egress> cToAEgresses = new List<Egress>();
            List<Vertex> internalIntersections = new List<Vertex>();

            // find all intersections between the triangle and the bound
            foreach (Triangle boundsTriangle in boundsTriangles)
            {
                IntersectTriangleEdge(triangle.vertices[0].value, triangle.vertices[1].value, aToBEgresses, boundsTriangle, debug);
                IntersectTriangleEdge(triangle.vertices[1].value, triangle.vertices[2].value, bToCEgresses, boundsTriangle, debug);
                IntersectTriangleEdge(triangle.vertices[2].value, triangle.vertices[0].value, cToAEgresses, boundsTriangle, debug);
                internalIntersections.AddRange(FindTriangleIntersections(boundsTriangle, triangle));

                if (debug)
                {
                    foreach (Vertex intersection in internalIntersections)
                    {
                        Debug.DrawRay(transform.localToWorldMatrix.MultiplyPoint3x4(intersection.value), Vector3.back * 0.1f, Color.green, Time.deltaTime);
                    }
                }
            }

            if (debug)
            {
                foreach (Vertex vertex in triangle.vertices)
                {
                    Color color = Color.blue;
                    if (vertex.containedByBound)color = Color.cyan;
                    Debug.DrawRay(transform.localToWorldMatrix.MultiplyPoint3x4(vertex.value), Vector3.back * 0.1f, color, Time.deltaTime);
                }
            }

            List<Egress> allEgresses = new List<Egress>();
            allEgresses.AddRange(aToBEgresses);
            allEgresses.AddRange(bToCEgresses);
            allEgresses.AddRange(cToAEgresses);

            // combine duplicate internal intersections
            for (int i = internalIntersections.Count - 1; i > 0; i--)
            {
                for (int k = i - 1; k >= 0; k--)
                {
                    if (Vector3.Distance(internalIntersections[i].value, internalIntersections[k].value) < intersectionError)
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

            // organize the intersections into cuts
            CreateCuts(allEgresses, internalIntersections);

            //TODO: soon. deal with floating islands by collecting up loops without egresses
            // (they won't be a part of any cuts)

            // organize all triangle vertices and egress intersections in an ordered list around the perimeter
            List<Vertex> perimeter = new List<Vertex>();

            aToBEgresses.Sort((a, b) => (int)Mathf.Sign(Vector3.Distance(a.value, triangle.vertices[0].value) - Vector3.Distance(b.value, triangle.vertices[0].value)));
            bToCEgresses.Sort((a, b) => (int)Mathf.Sign(Vector3.Distance(a.value, triangle.vertices[1].value) - Vector3.Distance(b.value, triangle.vertices[1].value)));
            cToAEgresses.Sort((a, b) => (int)Mathf.Sign(Vector3.Distance(a.value, triangle.vertices[2].value) - Vector3.Distance(b.value, triangle.vertices[2].value)));

            perimeter.Add(triangle.vertices[0]);
            perimeter.AddRange(aToBEgresses);
            perimeter.Add(triangle.vertices[1]);
            perimeter.AddRange(bToCEgresses);
            perimeter.Add(triangle.vertices[2]);
            perimeter.AddRange(cToAEgresses);
            perimeter.Add(triangle.vertices[0]); // a duplicate of the first vertex as a sentinel

            // find all edge loops and classify whether they should be retopologized or not

            List<EdgeLoop> loops = new List<EdgeLoop>();
            // while there are still entries in that list for which we haven't identified all loops (1 for triangle vertices, 2 for egresses)
            //Debug.Log("Beginning classification of edge loops");
            foreach (Vertex vertex in triangle.vertices)
            {
                vertex.loops = new List<EdgeLoop>();
            }

            while (true)
            {
                // find the earliest entry whose loops aren't satisfied   
                int currentVertexIndex = 0;
                //Debug.Log("~~FINDING NEW STARTING POINT~~");
                for (currentVertexIndex = 0; currentVertexIndex < perimeter.Count - 2; currentVertexIndex++)
                {
                    //Debug.Log("Index: " + currentVertexIndex + ", Is Egress = " + (perimeter[currentVertexIndex] is Egress) + ", loop count: " + perimeter[currentVertexIndex].loops.Count);

                    // for egresses:
                    if (perimeter[currentVertexIndex] is Egress)
                    {
                        //Debug.Log(((Egress)perimeter[currentVertexIndex]).cuts.Count);

                        // if this one is unsatisfied, break. We've found the earliest
                        if (perimeter[currentVertexIndex].loops.Count < ((Egress)perimeter[currentVertexIndex]).cuts.Count + 1)
                        {
                            break;
                        }
                    }
                    else // for original vertices
                    {
                        // if this one is unsatisfied, break. We've found the earliest
                        if (perimeter[currentVertexIndex].loops.Count < 1)
                        {
                            break;
                        }
                    }
                }

                string parString = "perimeter:";
                foreach (Vertex vertex in perimeter)
                {
                    parString += " " + perimeter.IndexOf(vertex) + ": (" + (vertex is Egress) + ")" + vertex.value;
                }
                //Debug.Log(parString);
                //Debug.Log("finished with: " + currentVertexIndex);
                //Debug.Log("perimeter size: " + perimeter.Count);

                // we've satisfied all loops, which is our exit condition
                if (currentVertexIndex == perimeter.Count - 2)
                {
                    break;
                }

                // now that we've found the beginning of a loop, it's time to find the rest
                Vertex initialVertex = perimeter[currentVertexIndex];

                // determine whether this loop should be filled or not
                EdgeLoop loop = new EdgeLoop();

                // the loop defines either a surface or a hole, and we can determine that by looking at our starting entry
                // the starting entry can be one of two cases:
                // 1. an egress which is already part of one loop
                if (initialVertex is Egress)
                {
                    // in this case, the new loop is the opposite of whatever the previous loop is
                    loop.filled = !initialVertex.loops[initialVertex.loops.Count - 1].filled;

                    // also, if the initial vertex is an egress, we should add it and move on to the next perimeter value, 
                    // so as not to traverse backwards along the cut
                    /*Cut furthestCut = ((Egress)initialVertex).GetFurthestCut(perimeter);

                    loop.vertices.Add(perimeter[currentVertexIndex]);
                    perimeter[currentVertexIndex].loops.Add(loop);
                    currentVertexIndex++;*/
                }
                else // 2. a vertex of the original triangle
                {
                    // in this case, if the vertex is contained by the bound, the loop is a surface, otherwise it's a hole
                    loop.filled = initialVertex.containedByBound;
                }

                // traverse forward in the ordered list, following each cut, until we reach the initial point
                // once the inital entry is reached again, we have identified a loop, and can add it to the pile
                int tooMuch = 0;
                //Debug.Log("starting new loop");
                do
                {
                    // if the current vertex is an egress
                    tooMuch++;
                    if (tooMuch > 100)
                    {
                        Debug.Log("too long");
                        throw new System.ApplicationException();
                        break;
                    }

                    //Debug.Log(currentVertexIndex);

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

                        // if there's more than one cut on the vertex, we need to potentially
                        while (((Egress)perimeter[currentVertexIndex]).cuts.Count > 1)
                        {
                            //Debug.Log("Double traversal");
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
                                //Debug.Log("Couldn't find double traversal");
                                break;
                            }

                            if (perimeter[currentVertexIndex] == initialVertex) // if we arrived at the initial vertex
                            {
                                loop.vertices.RemoveAt(loop.vertices.Count - 1);
                                goto EndOfDoWhile;
                            }
                        }
                    }
                    else
                    {
                        // add current vertex to loop
                        loop.vertices.Add(perimeter[currentVertexIndex]);
                        perimeter[currentVertexIndex].loops.Add(loop);
                    }

                    // increment the current index by 1
                    currentVertexIndex++;

                    if (currentVertexIndex >= perimeter.Count)
                    {
                        string listString = "";
                        foreach (Vertex vertex in loop.vertices)
                        {
                            listString += " " + perimeter.IndexOf(vertex);
                        }
                        Debug.Log("Failed to create loop, (Filled: " + loop.filled + ") :: " + listString);
                    }
                    // once the current vertex loops around to the start, we're done
                } while (perimeter[currentVertexIndex] != initialVertex);
                EndOfDoWhile:

                    //Debug.Log("terminated");
                    loops.Add(loop);
            }
            if (debug)
            {
                foreach (EdgeLoop loop in loops)
                {
                    string listString = "";
                    foreach (Vertex vertex in loop.vertices)
                    {
                        listString += " " + perimeter.IndexOf(vertex);
                    }
                    Debug.Log("Created loop, (Filled: " + loop.filled + ") :: " + listString);
                }
            }

            // now that we've created all loops that intersect the edge of the triangle, we can start on loops that float as islands
            List<Vertex> unusedVertices = new List<Vertex>();
            foreach (Vertex intersection in internalIntersections)
            {
                if (!intersection.usedInLoop)
                {
                    unusedVertices.Add(intersection);
                }
            }
            int foo = 0;
            while (unusedVertices.Count > 0)
            {
                EdgeLoop currentLoop = new EdgeLoop();
                Vertex currentVertex = unusedVertices[unusedVertices.Count - 1];
                unusedVertices.RemoveAt(unusedVertices.Count - 1);

                foreach (EdgeLoop loop in loops)
                {
                    if (currentVertex.LiesWithinLoop(loop))
                    {
                        if (loop.nestedLoop == null)
                        {
                            loop.nestedLoop = currentLoop;
                        }
                        else
                        {
                            currentLoop.nestedLoop = loop.nestedLoop;
                            EdgeLoop previousLoop = loop;
                            do
                            {
                                if (currentVertex.LiesWithinLoop(currentLoop.nestedLoop))
                                {
                                    previousLoop.nestedLoop = currentLoop.nestedLoop;
                                    currentLoop.nestedLoop = currentLoop.nestedLoop.nestedLoop;
                                    previousLoop.nestedLoop.nestedLoop = currentLoop;
                                }
                                else
                                {
                                    break;
                                }
                            } while (currentLoop.nestedLoop != null);
                        }

                        // we've found the loop we're contained by, break out
                        break;
                    }
                }

                //TODO: discovery of loops
                currentLoop.vertices.Add(currentVertex);
                bool foundNext = false;
                do
                {
                    foundNext = false;
                    for (int i = unusedVertices.Count - 2; i >= 0; i--)
                    {
                        if (unusedVertices[i].SharesTriangle(currentVertex))
                        {
                            currentLoop.vertices.Add(unusedVertices[i]);
                            unusedVertices.RemoveAt(i);
                            currentVertex = unusedVertices[i];
                            foundNext = true;
                        }
                    }
                    foo++;
                    if (foo > 100)
                    {
                        Debug.Log("yeet");
                        return null;
                    }
                } while (foundNext);

            }

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

        private int TraverseCut(Cut cut, EdgeLoop loop, List<Vertex> perimeter, int currentVertexIndex)
        {
            cut.traversed = true;

            // add all vertices of the egress's cut to the loop
            loop.vertices.AddRange(cut);
            perimeter[currentVertexIndex].loops.Add(loop);

            // find the index of the last vertex in the cut (which is always an egress) and get its index in the perimeter
            currentVertexIndex = perimeter.IndexOf(cut[cut.Count - 1]);
            perimeter[currentVertexIndex].loops.Add(loop);

            string cutString = "";
            foreach (Vertex vertex in cut)
            {
                cutString += " ";
                if (perimeter.Contains(vertex))
                {
                    cutString += perimeter.IndexOf(vertex) + ": ";
                }
                cutString += vertex;
            }
            //Debug.Log("Traversing egress: " + cutString);
            //Debug.Log("Egress traversed. Arrived at " + currentVertexIndex);

            return currentVertexIndex;
        }

        private void IntersectTriangleEdge(Vector3 pointA, Vector3 pointB, List<Egress> egresses, Triangle boundsTriangle, bool debug = false)
        {
            Vertex intersectionPoint;

            intersectionPoint = EdgeIntersectsTriangle(pointA, pointB, boundsTriangle);

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

                if (debug)
                {
                    Debug.DrawRay(transform.localToWorldMatrix.MultiplyPoint3x4(intersectionPoint.value), Vector3.back * 0.1f, Color.red, Time.deltaTime);
                }
            }
        }

        private void CreateCuts(List<Egress> egresses, List<Vertex> intersections)
        {
            foreach (Egress egress in egresses)
            {
                Cut cut = new Cut();
                cut.Add(egress);
                Vertex currentVertex = egress;
                int tooMuch = 0;
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
                            foreach (Cut existingCut in egress.cuts) // check each of the previous cuts to ensure that it doesn't already exist
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

                        if (cut.Count == 1) // if we're starting a new cut
                        {
                            foreach (Cut existingCut in egress.cuts) // check each of the previous cuts to ensure that it doesn't already exist
                            {
                                if (existingCut[1] == vertex)
                                {
                                    alreadyExists = true;
                                }
                            }
                        }

                        if (vertex.SharesTriangle(currentVertex) && !cut.Contains(vertex) && !alreadyExists)
                        {
                            //Debug.Log("I share a triangle");
                            foundNext = true;
                            cut.Add(vertex);
                            currentVertex = vertex;
                            break;
                        }
                    }

                    tooMuch++;
                    if (tooMuch > 100)
                    {
                        throw new System.ApplicationException();
                    }
                } while (!(currentVertex is Egress) && foundNext);

                if (foundNext && currentVertex != egress)
                {
                    egress.cuts.Add(cut);
                    ((Egress)currentVertex).cuts.Add(cut.GetReversedCopy());
                    string cutString = "";
                    foreach (Vertex vertex in cut)
                    {
                        cutString += " ";
                        if (vertex is Egress)
                        {
                            cutString += "Egress: ";
                        }
                        cutString += vertex;
                    }
                }

            }
        }

        private Vector3 BoundsSpaceToModelSpace(Vector3 toTransform, GameObject toClip, GameObject bounds)
        {
            Vector3 point = bounds.transform.localToWorldMatrix.MultiplyPoint3x4(toTransform);
            point = toClip.transform.worldToLocalMatrix.MultiplyPoint3x4(point);
            return point;
        }

        // only finds the intersections of a's edges with b's surface
        private List<Vertex> FindTriangleIntersections(Triangle a, Triangle b)
        {
            List<Vertex> vertices = new List<Vertex>();

            // for each edge of a, raycast to b
            for (int i = 0; i < 3; i++)
            {
                Vertex vertex = EdgeIntersectsTriangle(a.vertices[i].value, a.vertices[(i + 1) % 3].value, b);
                if (vertex != null)
                {
                    //Debug.Log("point :: " + vertex.value);
                    vertex.triangles.Add(a);
                    vertices.Add(vertex);
                }
            }

            return vertices;
        }

        private Vertex EdgeIntersectsTriangle(Vector3 pointA, Vector3 pointB, Triangle triangle)
        {
            Vertex raycastIntersection = NewRaycastToTriangle(pointA, pointA - pointB, triangle);
            if (raycastIntersection != null)
            {
                if (PointLiesOnEdge(raycastIntersection.value, pointA, pointB))
                {
                    return raycastIntersection;
                }
            }

            return null;
        }

        private bool PointLiesOnEdge(Vector3 point, Vector3 edgeA, Vector3 edgeB)
        {
            float temp = Vector3.Distance(edgeA, point) + Vector3.Distance(point, edgeB);
            return Mathf.Abs(Vector3.Distance(edgeA, edgeB) - temp) < error;
        }

        private List<Vertex> GetBoundsVertices(GameObject bounds)
        {
            List<Vertex> vertices = new List<Vertex>();
            Vector3[] meshVertices = bounds.GetComponent<MeshFilter>().mesh.vertices;

            for (int i = 0; i < meshVertices.Length; i++)
            {
                vertices.Add(new Vertex(i, meshVertices[i], true));
            }

            return vertices;
        }

        // assumes that there is one
        private Vector3 FindIntersectionOfTwoLines(Vector3 oT, Vector3 dT, Vector3 oU, Vector3 dU)
        {
            float u = (oT.y + oU.x - oU.y - oT.x) / (1 - dU.x);
            return oU + dU * u;
        }

        private List<Vertex> ClassifyVertices(GameObject toClip, GameObject bounds, List<Triangle> boundsTriangles)
        {
            List<Vertex> vertices = new List<Vertex>();
            Vector3[] meshVertices = toClip.GetComponent<MeshFilter>().mesh.vertices;

            for (int i = 0; i < meshVertices.Length; i++)
            {
                vertices.Add(new Vertex(i, meshVertices[i], PointContainedByBounds(meshVertices[i], toClip, boundsTriangles)));
            }

            return vertices;
        }

        private bool PointContainedByBounds(Vector3 point, GameObject toClip, List<Triangle> boundsTriangles)
        {
            int intersectionsAbove = 0;
            int intersectionsBelow = 0;

            List<Vector3> intersections = new List<Vector3>();

            foreach (Triangle boundsTriangle in boundsTriangles)
            {
                Vertex intersectionPoint = NewRaycastToTriangle(point, Vector3.up, boundsTriangle);
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
                    if (Vector3.Distance(intersections[i], intersections[k]) < intersectionError)
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

        // solution from https://stackoverflow.com/questions/42740765/intersection-between-line-and-triangle-in-3d
        private Vertex NewRaycastToTriangle(Vector3 origin, Vector3 direction, Triangle triangle)
        {
            Vector3 q1 = origin + direction * 5;
            Vector3 q2 = origin - direction * 5;

            if (SignedVolume(q1, triangle.vertices[0].value, triangle.vertices[1].value, triangle.vertices[2].value) !=
                SignedVolume(q2, triangle.vertices[0].value, triangle.vertices[1].value, triangle.vertices[2].value) &&
                SignedVolume(q1, q2, triangle.vertices[0].value, triangle.vertices[1].value) ==
                SignedVolume(q1, q2, triangle.vertices[1].value, triangle.vertices[2].value) &&
                SignedVolume(q1, q2, triangle.vertices[1].value, triangle.vertices[2].value) ==
                SignedVolume(q1, q2, triangle.vertices[2].value, triangle.vertices[0].value)
            )
            {
                // determine equation of plane
                Vector3 normal = Vector3.Cross(triangle.vertices[0].value - triangle.vertices[1].value, triangle.vertices[1].value - triangle.vertices[2].value);
                Vector3 planePoint = triangle.vertices[0].value;

                // get ray intersection with plane,
                float numerator = normal.x * (planePoint.x - origin.x) + normal.y * (planePoint.y - origin.y) + normal.z * (planePoint.z - origin.z);
                float denominator = normal.x * direction.x + normal.y * direction.y + normal.z * direction.z;
                Vector3 intersectionPoint = ((numerator / denominator) * direction) + origin;

                return new Vertex(0, intersectionPoint, true);
            }
            else
            {
                return null;
            }
        }

        private int SignedVolume(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            return (int)Mathf.Sign(Vector3.Dot(Vector3.Cross(b - a, c - a), d - a));
        }

        // old triangle raycast
        /*
        private Vertex RaycastToTriangle(Vector3 origin, Vector3 direction, Triangle triangle)
        {
            // determine equation of plane
            Vector3 normal = Vector3.Cross(triangle.vertices[0].value - triangle.vertices[1].value, triangle.vertices[1].value - triangle.vertices[2].value);
            Vector3 planePoint = triangle.vertices[0].value;
            //Debug.Log(normal + " :: " + direction + " == " + (Vector3.Cross(normal, direction).magnitude < error));
            if(Mathf.Abs(Vector3.Dot(normal, direction)) < error)
            {
                //Debug.Log("this one's parellel");
                //Debug.Log(normal + " :: " + direction);
                return null;
            }
            //Debug.Log(origin);
            //Debug.Log(direction);

            // get ray intersection with plane,
            float numerator = normal.x * (planePoint.x - origin.x) + normal.y * (planePoint.y - origin.y) + normal.z * (planePoint.z - origin.z);
            float denominator = normal.x * direction.x + normal.y * direction.y + normal.z * direction.z;
            Vector3 intersectionPoint = ((numerator / denominator) * direction) + origin;

            // determine if that point is in the triangle
            float areaOfInternalTriangle = FindAreaOfTriangle(triangle.vertices[0].value, triangle.vertices[1].value, triangle.vertices[2].value);

            float areaOfPointTriangles = 0;

            for(int i = 0; i < 3; i++)
            {
                areaOfPointTriangles += FindAreaOfTriangle(intersectionPoint, triangle.vertices[i].value, triangle.vertices[(i + 1) % 3].value);
            }


            // if the point is on the triangle
            //Debug.Log(areaOfInternalTriangle + " :: " + areaOfPointTriangles);
            if(Mathf.Abs(areaOfInternalTriangle - areaOfPointTriangles) < error)
            {
                //Debug.Log("this one did");
                //Debug.Log("intersection: " + intersectionPoint);
                Vertex result = new Vertex(0, intersectionPoint, true);
                return result;
            }
            //Debug.Log("this one didn't");
            return null;// if we don't find anything, return null
        }

        private float FindAreaOfTriangle(Vector3 pointA, Vector3 pointB, Vector3 pointC)
        {
            //Debug.Log(pointA + " : " + pointB + " : " + pointC);
            float aToB = (pointB - pointA).magnitude;
            float bToC = (pointC - pointB).magnitude;
            float cToA = (pointA - pointC).magnitude;
            float s = (aToB + bToC + cToA) / 2;
            return Mathf.Sqrt(s * (s - aToB) * (s - bToC) * (s - cToA));
        }
        */

        private List<Triangle> GetAllTriangles(GameObject toClip, List<Vertex> vertices)
        {
            List<Triangle> triangles = new List<Triangle>();
            int[] meshTriangles = toClip.GetComponent<MeshFilter>().mesh.triangles;

            for (int i = 0; i < meshTriangles.Length; i += 3)
            {
                Triangle triangle = new Triangle(vertices[meshTriangles[i]], vertices[meshTriangles[i + 1]], vertices[meshTriangles[i + 2]]);
                triangles.Add(triangle);
            }

            return triangles;
        }

        private List<Triangle> GetBoundsTriangles(GameObject toClip, GameObject bounds)
        {
            List<Triangle> triangles = new List<Triangle>();
            Mesh mesh = bounds.GetComponent<MeshFilter>().mesh;

            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                Triangle triangle = new Triangle(
                    new Vertex(i, BoundsSpaceToModelSpace(mesh.vertices[mesh.triangles[i]], toClip, bounds), true),
                    new Vertex(i + 1, BoundsSpaceToModelSpace(mesh.vertices[mesh.triangles[i + 1]], toClip, bounds), true),
                    new Vertex(i + 2, BoundsSpaceToModelSpace(mesh.vertices[mesh.triangles[i + 2]], toClip, bounds), true));

                triangles.Add(triangle);
            }

            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                //Debug.Log(BoundsSpaceToModelSpace(mesh.vertices[i], toClip) + " :: " + mesh.vertices[i]);
            }

            return triangles;
        }
    }

}
