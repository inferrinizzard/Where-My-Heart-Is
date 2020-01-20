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
		/// <param name="perimeter"> The perimeter to examine for incomplete vertices </param>
		/// <returns>The index of the earliest unsatisfied vertex in the loop, or -1 if none are left.</returns>
		private int FindEarliestUnsatisfied(List<Vertex> perimeter) =>
			perimeter.FindIndex(vertex => vertex is Egress ? vertex.loops.Count < ((Egress)vertex).cuts.Count + 1 : vertex.loops.Count < 1);

		/// <summary>
		/// Finds the loop associated with the given intersection with a triangle and places it as a child of the correct containing edge loop
		/// </summary>
		/// <param name="initialVertex"> A vertex on the loop to find </param>
		/// <param name="unusedVertices"> A list of vertices that could possibly be part of the loop </param>
		/// <param name="loops"> A list of all non-internal loops found for the current triangle </param>
		/// <returns>An edge loop defining the loop associated with the given vertex</returns>
		private EdgeLoop DiscoverInternalLoop(Vertex initialVertex, List<Vertex> unusedVertices, List<EdgeLoop> loops)
		{
			Queue<EdgeLoop> potentialLoops = new Queue<EdgeLoop>();
			List<EdgeLoop> completedLoops = new List<EdgeLoop>();

			potentialLoops.Enqueue(new EdgeLoop());
			potentialLoops.Peek().vertices.Add(initialVertex);
			while (potentialLoops.Count > 0)
			{
				Vertex secondVertex = null;
				Vertex nextVertex;
				do
				{
					nextVertex = null;
					List<Vertex> loopVertices = potentialLoops.Peek().vertices;
					for (int i = unusedVertices.Count - 1; i >= 0; i--)
					{
						if (unusedVertices[i].SharesTriangle(loopVertices.Last()) &&
							(!loopVertices.Contains(unusedVertices[i]) || (unusedVertices[i] == initialVertex && secondVertex != null)))
						{
							if (nextVertex == null)
							{
								nextVertex = unusedVertices[i];

								if (secondVertex == null)
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

				EdgeLoop potentialLoop = potentialLoops.Dequeue();
				if (nextVertex != null)completedLoops.Add(potentialLoop);
			}

			// the loop with the greatest number of vertices is the correct loop
			EdgeLoop finalLoop = completedLoops.OrderBy(loop => loop.vertices.Count).First(); // TODO: sometimes completedLoops is empty

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
						bool liesWithinLoop = initialVertex.LiesWithinLoop(finalLoop.nestedLoop);
						do
						{
							previousLoop.nestedLoop = finalLoop.nestedLoop;
							finalLoop.nestedLoop = finalLoop.nestedLoop.nestedLoop;
							previousLoop.nestedLoop.nestedLoop = finalLoop;
						} while (finalLoop.nestedLoop != null && liesWithinLoop);
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
		/// <param name="cut"> The Cut being traversed </param>
		/// <param name="loop"> The EdgeLoop being traversed </param>
		/// <param name="perimeter"> The parimeter of the current triangle </param>
		/// <param name="currentVertexIndex"> The index of the vertex at the beginning of the Cut </param>
		/// <returns>The index of the vertex arrived at by traversing the Cut</returns>
		private int TraverseCut(Cut cut, EdgeLoop loop, List<Vertex> perimeter, int currentVertexIndex)
		{
			cut.traversed = true;

			// add all vertices of the egress's cut to the loop
			loop.vertices.AddRange(cut);
			perimeter[currentVertexIndex].loops.Add(loop);

			// find the index of the last vertex in the cut (which is always an egress) and get its index in the perimeter
			currentVertexIndex = perimeter.IndexOf(cut.Last());
			perimeter[currentVertexIndex].loops.Add(loop);

			return currentVertexIndex;
		}

		/// <summary>
		/// Identifies a new egress created by the intersection of the edge defined by points A and B and a given bounds triangle if there is one
		/// </summary>
		/// <remarks>
		/// Sometimes an Egress is identified multiple times. In these cases, the Egress is not double counted
		/// </remarks>
		/// <param name="pointA"> The first point of the intersecting edge </param>
		/// <param name="pointB"> The second point of the intersecting edge </param>
		/// <param name="boundsTriangle"> The Triangle to intersect the edge with </param>
		/// <param name="egresses"> A list of existing egresses to store the new Egress in </param>
		private void IdentifyEgress(Vector3 pointA, Vector3 pointB, Triangle boundsTriangle, List<Egress> egresses)
		{
			Vertex intersectionPoint;

			intersectionPoint = Raycast.LineSegmentToTriangle(pointA, pointB, boundsTriangle, error);

			if (intersectionPoint != null)
			{
				bool alreadyExists = false;
				//TODO
				Egress duplicate = egresses.Find(egress => Vector3.Distance(intersectionPoint.value, egress.value) < error);
				if (duplicate != null)
				{
					alreadyExists = true;
					duplicate.triangles.Add(boundsTriangle);
				}
				// duplicate logic as below

				if (!alreadyExists)
				{
					intersectionPoint.triangles.Add(boundsTriangle);
					egresses.Add(Egress.CreateFromVertex(intersectionPoint));
				}
			}
		}

		/// <summary>
		/// Determines whether the given point is contained by the given bound
		/// </summary>
		/// <param name="point"> The point to check for containment </param>
		/// <param name="boundsTriangles"> A List of triangles representing the bounding shape </param>
		/// <returns>True if the point is contained, false if it is not</returns>
		private bool PointContainedByBound(Vector3 point, List<Triangle> boundsTriangles)
		{
			int intersectionsAbove = 0;
			int intersectionsBelow = 0;

			List<Vector3> intersections = boundsTriangles.Select(tri => Raycast.RayToTriangle(point, Vector3.up, tri)).Where(tri => tri != null).Select(tri => tri.value).ToList();

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
			// LINQ return intersections.GroupBy(intersection => intersection.y > point.y).All(l => l.Count() & 1 == 0); TODO

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
			// ENDLINQ
		}
	}

	private bool PointExcludedByBound(Vector3 point, List<Triangle> boundsTriangles)
	{
		return !PointContainedByBound(point, boundsTriangles);
	}
}
