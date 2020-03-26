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
	public static class Operations
	{
		[Tooltip("Breakpoint for equality comparisons between floats")]
        public static float error = 0.001f;

		[Header("Debug Variables")]
        public static int faceIndex = -1;
        public static int edgeLoopIndex = -1;

        public static int earToDraw = -1;

		/// <summary>
		/// Generates the intersection of two shapes
		/// </summary>
		/// <param name="shapeA">The first shape to intersect</param>
		/// <param name="shapeB">The second shape to intersect</param>
		/// <returns>The intersection of the two shapes</returns>
		public static Mesh Intersect(Model modelA, Model modelB)
		{
			/*Model modelA = new Model(shapeA.GetComponent<MeshFilter>().mesh);
			modelA.ConvertToWorld(shapeA.transform);

			Model modelB = new Model(shapeB.GetComponent<MeshFilter>().mesh);
			modelB.ConvertToWorld(shapeB.transform);*/

			modelA.IntersectWith(modelB); //generate all intersections

			Model clippedA = ClipModelAToModelB(modelA, modelB, true);
			Model clippedB = ClipModelAToModelB(modelB, modelA, true);

			Model result = Model.Combine(clippedA, clippedB);

			return result.ToMesh(modelA.worldToLocal);
		}

        /// <summary>
        /// Subtracts shapeB from shapeA
        /// </summary>
        public static Mesh Subtract(Model modelA, Model modelB)
		{
			/*Model modelA = new Model(shapeA.GetComponent<MeshFilter>().mesh);
			modelA.ConvertToWorld(shapeA.transform);

			Model modelB = new Model(shapeB.GetComponent<MeshFilter>().mesh);
			modelB.ConvertToWorld(shapeB.transform);*/

			modelA.IntersectWith(modelB); //generate all intersections

			Model clippedA = ClipModelAToModelB(modelA, modelB, false);
			Model clippedB = ClipModelAToModelB(modelB, modelA, true, true);

			Model result = Model.Combine(clippedA, clippedB);
			//result.ConvertToLocal(modelA.worldToLocal);

			return result.ToMesh(modelA.worldToLocal);
		}

        /// <summary>
        /// Clips the geometry of shapeA to just the component of it bounded by shapeB
        /// </summary>
        /// <param name="shapeA"></param>
        /// <param name="shapeB"></param>
        /// <param name="clipInside">If true, shapeA will be clipped to only geometry contained by shapeB, if false, it will be clipped only to geometry NOT contained by shapeB</param>
        /// <returns></returns>
        public static Mesh ClipAToB(Model modelA, Model modelB, bool clipInside = true)
		{
			/*Model modelA = new Model(shapeA.GetComponent<MeshFilter>().mesh);
			modelA.ConvertToWorld(shapeA.transform);

			Model modelB = new Model(shapeB.GetComponent<MeshFilter>().mesh);
			modelB.ConvertToWorld(shapeB.transform);*/

			modelA.IntersectWith(modelB); //generate all intersections

			Model clippedA = ClipModelAToModelB(modelA, modelB, clipInside);

			//clippedA.ConvertToLocal(modelA.worldToLocal);

			return clippedA.ToMesh(modelA.worldToLocal);
		}

        /// <summary>
        /// Generates a mesh that matches the portion of the given "toClip" object's mesh contained by the 
        /// bounding object "bounds"
        /// </summary>
        /// <param name="modelA">The GameObject containing the Mesh to clip</param>
        /// <param name="modelB">The GameObject containing the Mesh to clip "toClip" to</param>
        /// <param name="flipNormals">Whether the normals of the resulting mesh should be flipped</param>
        /// <returns>The clipped Mesh</returns>
        private static Model ClipModelAToModelB(Model modelA, Model modelB, bool clipInside = true, bool flipNormals = false)
		{
			// to create the triangles, we'll need a list of edge loops to triangulate
			List<EdgeLoop> edgeLoops = new List<EdgeLoop>();

			if (clipInside)
			{
				if (faceIndex <= -1)
				{
                    foreach (Triangle triangle in modelA.triangles)
                    {
                        try
                        {
                            edgeLoops.AddRange(IdentifyTriangleEdgeLoops(triangle, modelB, PointContainedByBound));
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Failed to find edge loop for triangle #" + modelA.triangles.IndexOf(triangle) + ", ERROR: " + e.Message);
                        }
                    }
                    
				}
				else// DEBUG
				{
                    edgeLoops = new List<EdgeLoop>();
                    if (faceIndex < modelA.triangles.Count)
                    {
                        edgeLoops.AddRange(IdentifyTriangleEdgeLoops(modelA.triangles[faceIndex], modelB, PointContainedByBound));
                        edgeLoops.ForEach(loop => loop.Draw(Color.red, Color.green, Color.blue));
                    }
                }
			}
			else
			{
				if (faceIndex <= -1)
				{
                    foreach (Triangle triangle in modelA.triangles)
                    {
                        edgeLoops.AddRange(IdentifyTriangleEdgeLoops(triangle, modelB, PointExcludedByBound));
                    }
                    
				}
				else
				{
                    edgeLoops = new List<EdgeLoop>();
                    if (faceIndex < modelA.triangles.Count)
                    {
                        edgeLoops.AddRange(IdentifyTriangleEdgeLoops(modelA.triangles[faceIndex], modelB, PointExcludedByBound));
                    }
                }
			}
			Model finalModel = new Model();

			edgeLoops.ForEach(loop =>
			{
				try
				{
					if (loop.filled)
					{
						finalModel.AddTriangles(loop.TriangulateStrip());
					}

					bool fillNested = !loop.filled;
					EdgeLoop nestedLoop = loop.nestedLoop;
					while (nestedLoop != null)
					{
						if (fillNested)finalModel.AddTriangles(nestedLoop.TriangulateStrip());
						fillNested = !fillNested;
						nestedLoop = nestedLoop.nestedLoop;
					}
				}
				catch (Exception e)
				{
					Debug.LogError("Failed to triangulate edge loop #" + edgeLoops.IndexOf(loop) + ", ERROR: " + e.Message);
					loop.Draw(Color.red, Color.green, Color.blue);
				}
			});

			if (flipNormals)finalModel.FlipNormals();

			return finalModel;
		}

        /// <summary>
        /// Clips a single Triangle to just its parts contained by the bound
        /// </summary>
        /// <param name="triangle">The triangle to clip</param>
        /// <param name="boundsTriangles">A list of triangles defining the bounding object</param>
        /// <param name="vertices">A list of the vertices of the object the triangle belongs to</param>
        /// <returns>A list of edge loops which define the clipped version of the triangle</returns>
        private static List<EdgeLoop> IdentifyTriangleEdgeLoops(Triangle triangle, Model bound, Func<Vertex, Model, bool> ContainmentCheck)
		{
			triangle.ClearLocalMetadata();
			// organize the intersections into cuts
			CreateCuts(triangle, bound);

			List<Vertex> perimeter = triangle.GetPerimeter();

			// find all edge loops and classify whether they should be retopologized or not
			List<EdgeLoop> loops = new List<EdgeLoop>();

			// while there are still entries in that list for which we haven't identified all loops (1 for triangle vertices, 2 for egresses)
			int currentVertexIndex = FindEarliestUnsatisfied(perimeter);
			int overflow = 0;

			while (currentVertexIndex != -1)
			{
				//Debug.Log(currentVertexIndex);
				// determine whether this loop should be filled or not
				EdgeLoop loop = new EdgeLoop();
				loop.triangle = triangle;

				Vertex initialVertex = perimeter[currentVertexIndex];

				// the loop defines either a surface or a hole, and we can determine that by looking at our starting entry
				// the starting entry can be one of two cases:
				// 1. an egress which is already part of one loop
				if (initialVertex.fromIntersection)
				{
					// in this case, the new loop is the opposite of whatever the previous loop is
					try
					{
						loop.filled = !initialVertex.loops.Last().filled;
					}
					catch
					{
						Debug.LogWarning("initial vertex does not have a previous loop to reference, using last found loop instead");
						loop.filled = !loops.Last().filled;
					}
				}
				else // 2. a vertex of the original triangle
				{
					// in this case, if the vertex is contained by the bound, the loop is a surface, otherwise it's a hole
					loop.filled = ContainmentCheck(initialVertex, bound);
				}

				// traverse forward in the ordered list, following each cut, until we reach the initial point
				// once the inital entry is reached again, we have identified a loop, and can add it to the pile

				loop.vertices.Add(perimeter[currentVertexIndex]);
				perimeter[currentVertexIndex].loops.Add(loop);

				currentVertexIndex = (currentVertexIndex + 1) % perimeter.Count;
				do
				{

					Cut cut = perimeter[currentVertexIndex].cut;
					if (cut != null)
					{
						// add all vertices of the egress's cut to the loop
						loop.vertices.AddRange(cut);
						perimeter[currentVertexIndex].loops.Add(loop);

						// find the index of the last vertex in the cut and get its index in the perimeter
						currentVertexIndex = perimeter.IndexOf(cut.Last());
						perimeter[currentVertexIndex].loops.Add(loop);

						if (perimeter[currentVertexIndex] == initialVertex)
						{
							// if we arrived at the beginning, we've added the initial vertex twice and should remove the second appearence
							loop.vertices.RemoveAt(loop.vertices.Count - 1);
							break;
						}
					}
					else
					{
						loop.vertices.Add(perimeter[currentVertexIndex]);
						perimeter[currentVertexIndex].loops.Add(loop);
					}
					overflow++;
					if (overflow > 100)throw new Exception("Too many iterations when defining loops");
					// once the current vertex loops around to the start, we're done
					currentVertexIndex = (currentVertexIndex + 1) % perimeter.Count;
				} while (perimeter[currentVertexIndex] != initialVertex);

				loops.Add(loop);

				overflow++;
				if (overflow > 100)throw new Exception("Too many iterations when defining loops");

				currentVertexIndex = FindEarliestUnsatisfied(perimeter);
			}

			List<Intersection> unusedInternalIntersections = triangle.internalIntersections.Where(intersection =>
			{
				return !loops.Exists(loop => loop.vertices.Contains(intersection.vertex));
			}).ToList();

			overflow = 0;
			while (unusedInternalIntersections.Count > 0)
			{
				DiscoverInternalLoop(unusedInternalIntersections, loops);

				overflow++;
				if (overflow > 100)throw new Exception("Too many iterations when defining loops");
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
        private static int FindEarliestUnsatisfied(List<Vertex> perimeter) =>
			perimeter.FindIndex(vertex => vertex.fromIntersection ? vertex.loops.Count < 2 : vertex.loops.Count < 1);

        /// <summary>
        /// Finds the loop associated with the given intersection with a triangle and places it as a child of the correct containing edge loop
        /// </summary>
        /// <param name="initialIntersection">A vertex on the loop to find</param>
        /// <param name="unusedIntersections">A list of vertices that could possibly be part of the loop</param>
        /// <param name="loops">A list of all non-internal loops found for the current triangle</param>
        /// <returns>An edge loop defining the loop associated with the given vertex</returns>
        private static EdgeLoop DiscoverInternalLoop(List<Intersection> unusedIntersections, List<EdgeLoop> loops)
		{
			EdgeLoop createdLoop = new EdgeLoop();

			Intersection initialIntersection = unusedIntersections.First();

			Triangle currentTriangle = null;
			Edge currentEdge = initialIntersection.edge;
			Edge finalEdge;

			createdLoop.vertices.Add(initialIntersection.vertex);
			unusedIntersections.Remove(initialIntersection);

			currentTriangle = currentEdge.triangles.Find(triangle => triangle != currentTriangle);

			Intersection nextIntersection = null;
			int overflow = 0;
			do
			{
				nextIntersection = null;
				foreach (Edge edge in currentTriangle.edges)
				{
					nextIntersection = edge.intersections.Find(intersection => unusedIntersections.Contains(intersection) && intersection != initialIntersection);
					if (nextIntersection != null)
					{
						unusedIntersections.Remove(nextIntersection);
						createdLoop.vertices.Add(nextIntersection.vertex);

						currentEdge = edge;
						currentTriangle = currentEdge.triangles.Find(triangle => triangle != currentTriangle);
						break;
					}
					else
					{
						finalEdge = edge;
					}
				}

				overflow++;
				if (overflow > 100)throw new Exception("Too many iterations in internal loop discovery");
			} while (nextIntersection != null);

			// if we haven't gotten back to the start, we don't have a valid loop
			if (!currentTriangle.edges.Exists(edge => edge.intersections.Exists(intersection => intersection == initialIntersection)) ||
				createdLoop.vertices.Count < 3)
			{
				Debug.LogWarning("Unable to complete internal loop");
				return null;
			}

			//TODO: THERE CAN BE MULTIPLE NESTED LOOPS!!!!!
			EdgeLoop parentLoop = loops.Find(loop => initialIntersection.vertex.LiesWithinLoop(loop));
			createdLoop.MatchNormal(parentLoop);
			while (parentLoop.nestedLoop != null && initialIntersection.vertex.LiesWithinLoop(parentLoop.nestedLoop))
			{
				parentLoop = parentLoop.nestedLoop;
			}

			createdLoop.nestedLoop = parentLoop.nestedLoop;
			parentLoop.nestedLoop = createdLoop;

			return createdLoop;
		}

        /// <summary>
        /// Takes a list of Egresses and finds all Cuts that join them
        /// </summary>
        /// <param name="egresses">The list of Egresses to find cuts for</param>
        /// <param name="intersections">The list of internal intersections that form the intermediate points in the created cuts</param>
        private static void CreateCuts(Triangle triangle, Model bounds)
		{
			for (int i = 0; i < 3; i++)
			{
				//TODO do this somewhere where it won't get called multiple times on the same edges
				triangle.edges[i].intersections.Sort(
					(a, b) => Math.Sign(Vector3.Distance(a.vertex.value, triangle.vertices[i].value) -
						Vector3.Distance(b.vertex.value, triangle.vertices[i].value)));
				int overflow = 0;

				// for each edge forming the parimeter, find its cuts
				foreach (Intersection intersection in triangle.edges[i].intersections)
				{
					// if there's already a cut for this intersection, pass
					if (intersection.vertex.cut != null)
					{
						//Debug.LogWarning("Vertex: " + intersection.vertex + " already has cut, passing on finding it again");
						//intersection.vertex.Draw(0.05f, Vector3.left, (Color.red / 2) + (Color.yellow / 2));
						continue;
					}

					Cut createdCut = new Cut();
					createdCut.Add(intersection.vertex); // add the starting point of the cut

					Edge currentEdge = null;
					Triangle previousTriangle = intersection.triangle;
					Triangle currentTriangle = intersection.triangle;

					bool done = false;

					// while I haven't gotten to a triangle who is intersected by my perimeter
					while (!done)
					{
						// the current edge is the specific edge that intersects the initial triangle
						Edge nextEdge = currentTriangle.edges.Find(edge => edge.intersections.Exists(intersect => intersect.triangle == triangle) &&
							edge != currentEdge);
						// if we found another edge, we can try to continue to proceed
						if (nextEdge != null)
						{
							currentEdge = nextEdge;
							createdCut.Add(currentEdge.intersections.Find(intersect => intersect.triangle == triangle).vertex);

							// the previous triangle is now the one we found most recently
							previousTriangle = currentTriangle;

							// the current triangle is the first one we can find that shares an edge with the previous triangle 
							// that has an additional edge intersection the initial triangle
							currentTriangle = currentEdge.triangles.Find(tri => tri != previousTriangle &&
								tri.edges.Exists(edge => edge.intersections.Exists(intersect => intersect.triangle == triangle) && edge != currentEdge));
						}
						else
						{
							currentTriangle = null;
						}

						if (currentTriangle == null) // if we can't find a way to proceed, with internal intersections, it's time to check for egress intersections
						{
							Intersection finalIntersection = null;

							if (currentEdge == null) // this happens when we have no internal intersections as part of the cut
							{
								foreach (Edge edge in triangle.edges)
								{
									finalIntersection = edge.intersections.Find(intersect => intersection.triangle == intersect.triangle && intersect != intersection);
									if (finalIntersection != null)
									{
										break;
									}
								}
							}
							else
							{
								foreach (Edge edge in triangle.edges)
								{
									finalIntersection = edge.intersections.Find(intersect => currentEdge.triangles.Contains(intersect.triangle) && intersect != intersection);
									if (finalIntersection != null)
									{
										break;
									}
								}
							}

							if (finalIntersection != null)
							{
								createdCut.Add(finalIntersection.vertex);
								createdCut.First().cut = createdCut;
								createdCut.Last().cut = createdCut.GetReversedCopy();
							}
							else
							{
								Debug.LogWarning("Unable to complete cut. Aborting and proceeding to next cut.");
							}

							done = true;
						}

						overflow++;
						if (overflow > 100)throw new Exception("Maxiumum iterations exceeded");
					}
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
        private static Vector3 ConvertPointCoordinates(Vector3 toTransform, GameObject from, GameObject to)
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
        private static void ConvertMeshCoordinates(Mesh mesh, GameObject from, GameObject to)
		{
			List<Vector3> newVertices = mesh.vertices.Select(vertex => ConvertPointCoordinates(vertex, from, to)).ToList();
			mesh.SetVertices(newVertices);
		}

        /// <summary>
        /// Determines whether the given point is contained by the given bound
        /// </summary>
        /// <param name="point">The point to check for containment</param>
        /// <param name="boundsTriangles">A List of triangles representing the bounding shape</param>
        /// <returns>True if the point is contained, false if it is not</returns>
        private static bool PointContainedByBound(Vertex point, Model bound)
		{
			return point.ContainedBy(bound, error);
		}

        private static bool PointExcludedByBound(Vertex point, Model bound)
		{
			return !PointContainedByBound(point, bound);
		}
	}
}
