using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace CSG
{
	/// <summary>
	/// An ordered list of vertices representing an edge loop
	/// </summary>
	public class EdgeLoop
	{
		/// <summary>
		/// The vertices of this edge loop in order
		/// </summary>
		public List<Vertex> vertices;

		/// <summary>
		/// Some loops surround floating subloops, this points to that
		/// </summary>
		public EdgeLoop nestedLoop;

		/// <summary>
		/// Whether this edge loop should be filled during triangulation
		/// </summary>
		public bool filled;

		public bool shadeFlat;

		public Triangle triangle;

		/// <summary>
		/// Creates an empty EdgeLoop
		/// </summary>
		public EdgeLoop()
		{
			vertices = new List<Vertex>();
		}

		/// <summary>
		/// Creates an EdgeLoop with a shallow copy of the given List of vertices
		/// </summary>
		/// <param name="vertices"></param>
		public EdgeLoop(List<Vertex> vertices)
		{
			this.vertices = new List<Vertex>(vertices);
		}

		/// <summary>
		/// Succesively removes the best candidate triangle from the edge loop until none remain.
		/// Currently unreliable.
		/// </summary>
		/// <returns>The created triangles</returns>
		public List<Triangle> TriangulateEarMethod()
		{
			vertices.ForEach(vertex => vertex.Draw(0.5f, new Vector3(Random.value, Random.value, 1).normalized, Color.magenta));
			List<Triangle> triangles = new List<Triangle>();
			List<Vertex> currentVertices = new List<Vertex>(vertices);
			Triangle nextEar;

			//ear method
			int i = 0;
			while (currentVertices.Count > 3)
			{
				i++;
				if (i > 100)
				{
					//triangles.ForEach(triangle => triangle.Draw(Color.green));
					currentVertices.ForEach(vertex =>
					{
						Debug.Log(vertex);
						vertex.Draw(0.05f, Vector3.up, Color.magenta);
					});
					Debug.LogError("Too many iterations while triangulating edge loop, aborting.");
					return triangles;
				}
				nextEar = CreateNextEar(currentVertices);

				if (nextEar != null) triangles.Add(nextEar);
			}
			triangles.Add(new Triangle(currentVertices[0], currentVertices[1], currentVertices[2]));

			return triangles;
		}

		/// <summary>
		/// Helper method for triangulateEar. Selects the best candidate for the next ear and creates the triangle for it, 
		/// removing the peak of the ear from the vertex list
		/// </summary>
		/// <param name="currentVertices">The remaining vertices to search for ears</param>
		/// <returns>The triangle created from the ear, or null if no ear candidates are found</returns>
		private Triangle CreateNextEar(List<Vertex> currentVertices)
		{
            Vector3 normal = GetNormal();

            int bestIndex = 0;
			Vector3 a = currentVertices[0].value - currentVertices[(1) % currentVertices.Count].value;
			Vector3 b = currentVertices[(2) % currentVertices.Count].value - currentVertices[(1) % currentVertices.Count].value;
            float bestAngle = Vector3.SignedAngle(a, b, normal);
            float currentAngle;
            for (int i = 1; i < currentVertices.Count; i++)
            {
                a = currentVertices[i].value - currentVertices[(i + 1) % currentVertices.Count].value;
                b = currentVertices[(i + 2) % currentVertices.Count].value - currentVertices[(i + 1) % currentVertices.Count].value;
                currentAngle = Vector3.SignedAngle(a, b, normal);
                if (currentAngle > 0 && currentAngle < bestAngle)
                {
                    bestAngle = currentAngle;
                    bestIndex = i;
                }
            }

            Triangle resultingTriangle = new Triangle(currentVertices[bestIndex], currentVertices[(bestIndex + 1) % currentVertices.Count], currentVertices[(bestIndex + 2) % currentVertices.Count]);
            currentVertices.RemoveAt((bestIndex + 1) % currentVertices.Count);
            return resultingTriangle;

            //for (int i = 0; i < currentVertices.Count; i++)
            //{

            //TODO
            /*if (SignedAngle(a, b) > 0)
            {
                Triangle resultingTriangle = new Triangle(currentVertices[i], currentVertices[(i + 1) % currentVertices.Count], currentVertices[(i + 2) % currentVertices.Count]);
                if (!TriangleContainsAny(vertices, resultingTriangle))
                {
                    currentVertices.RemoveAt((i + 1) % currentVertices.Count);
                    return resultingTriangle;
                }
            }*/
            //}

            return null;
		}

		/// <summary>
		/// Uses a simple fan method to triangulate this edge loop
		/// </summary>
		/// <returns>The created triangles</returns>
		public List<Triangle> TriangulateFan()
		{
			List<Triangle> triangles = new List<Triangle>();
			for (int i = 0; i < vertices.Count - 2; i++)
			{
				triangles.Add(new Triangle(vertices[0], vertices[i + 1], vertices[i + 2]));
			}

			return triangles;
		}

		/// <summary>
		/// Uses a simple strip method to triangulate this edge loop
		/// </summary>
		/// <returns>The created triangles</returns>
		public List<Triangle> TriangulateStrip()
		{
			List<Triangle> triangles = new List<Triangle>();

			List<int> indicies = new List<int>();

            indicies.Add(0);
            indicies.Add(0);
            indicies.Add(1);
            //indicies.Add(vertices.Count - 1);

            //triangles.Add(new Triangle(vertices[indicies[0]], vertices[indicies[1]], vertices[indicies[2]]));

            int i = 1;

            while (i < vertices.Count / 2)
            {
                indicies.RemoveAt(0);
                indicies.Add(vertices.Count - i);
                triangles.Add(new Triangle(vertices[indicies[0]], vertices[indicies[1]], vertices[indicies[2]]));

                i++;

                indicies.RemoveAt(0);// dequeue
                indicies.Add(i);
                triangles.Add(new Triangle(vertices[indicies[2]], vertices[indicies[1]], vertices[indicies[0]]));
            }

            if (vertices.Count % 2 == 1 )// count even
            {
                indicies.RemoveAt(0);
                indicies.Add(vertices.Count - i);
                triangles.Add(new Triangle(vertices[indicies[0]], vertices[indicies[1]], vertices[indicies[2]]));
            }

            
            /*
            indicies.Add(0);
			indicies.Add(1);
			indicies.Add(vertices.Count - 1);

			triangles.Add(new Triangle(vertices[indicies[0]], vertices[indicies[1]], vertices[indicies[2]]));

			int i = 2;
            //Debug.Log(Mathf.FloorToInt((vertices.Count) / 2));
            while (i < vertices.Count - 2)
            {
                indicies.RemoveAt(0); //dequeue
                indicies.Add(i);
                triangles.Add(new Triangle(vertices[indicies[0]], vertices[indicies[1]], vertices[indicies[2]]));

                if (i >= vertices.Count - 2) break;
                //if (i >= Mathf.CeilToInt((vertices.Count) / 2)) break;

                indicies.RemoveAt(0);
                indicies.Add(vertices.Count - i);
                triangles.Add(new Triangle(vertices[indicies[0]], vertices[indicies[1]], vertices[indicies[2]]));

                i++;
            }*/

            /*while (i < vertices.Count - 2)
			{
				indicies.RemoveAt(0); //dequeue
				indicies.Add(vertices.Count - 1 - i);
				triangles.Add(new Triangle(vertices[indicies[0]], vertices[indicies[1]], vertices[indicies[2]]));

				i++;
                if (i >= vertices.Count - 2) break;
                //if (i >= Mathf.CeilToInt((vertices.Count) / 2)) break;

                indicies.RemoveAt(0); //dequeue
				indicies.Add(i);
				triangles.Add(new Triangle(vertices[indicies[2]], vertices[indicies[1]], vertices[indicies[0]]));
			}*/

            return triangles;
		}

		/// <summary>
		/// Finds the unit vector normal to the plane defined by this EdgeLoop
		/// </summary>
		/// <returns>The normal vector</returns>
		public Vector3 GetNormal()
		{
			return Vector3.Cross(vertices[0].value - vertices[1].value, vertices[2].value - vertices[1].value).normalized;
		}

		/// <summary>
		/// Flips the normal of this edge loop by reversing the order of the vertex list
		/// </summary>
		public void FlipNormal()
		{
			vertices.Reverse();
		}

		/// <summary>
		/// Determines whether this edge loop's normal matches the normal of the given edge loop, and flips this edge
		/// loop's normal if they don't match
		/// </summary>
		/// <param name="toMatch">The edge loop whose normal should be matched</param>
		public void MatchNormal(EdgeLoop toMatch)
		{
			if (Vector3.Distance(this.GetNormal(), toMatch.GetNormal()) > 0.0001)
			{
				this.FlipNormal();
			}
		}

		public override string ToString() =>
			$"{base.ToString()}::{string.Join("::", vertices.Select(v => (v.value.ToString("F4") + " (" + (v.fromIntersection) + ")")))}";

		/// <summary>
		/// Draws each edge comprising this edge loop (as well as its vertices) in the editor window for debugging purposes.
		/// The edges will change color in a gradient, adding the secondary color.
		/// </summary>
		/// <param name="color">The initial color of edges drawn</param>
		/// <param name="secondaryColor">The added color to edges drawn</param>
		/// <param name="vertexColor">The color that the vertices of the edge loop are drawn in</param>
		public void Draw(Color color, Color secondaryColor, Color vertexColor)
		{
			float increment = 1f / vertices.Count;
			for (int i = 0; i < vertices.Count; i++)
			{
				vertices[i].Draw(0.05f, Vector3.forward, vertexColor);
				color += secondaryColor * increment;
				Debug.DrawLine(vertices[i].value, vertices[(i + 1) % vertices.Count].value, color, 60f);
			}
		}

	}

}
