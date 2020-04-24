using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace CSG
{
	/// <summary>
	/// An ordered triple of vertices representing a triangle of a Model
	/// </summary>
	public class Triangle
	{
		/// <summary>
		/// The three vertices of this triangle
		/// </summary>
		public List<Vertex> vertices;

		public List<Edge> edges; // always 3 of these// to be found externally

		public List<Intersection> internalIntersections;

		/// <summary>
		/// Creates a Triangle with the three given vertices
		/// </summary>
		/// <param name="a">The first Vertex of the triangle</param>
		/// <param name="b">The second Vertex of the triangle</param>
		/// <param name="c">The third Vertex of the triangle</param>
		public Triangle(Vertex a, Vertex b, Vertex c)
		{
			vertices = new List<Vertex> { a, b, c };
			vertices.ForEach(v => v.triangles.Add(this));
			edges = new List<Edge>();
			internalIntersections = new List<Intersection>();
		}

		public List<Vertex> GetPerimeter()
		{
			List<Vertex> perimeter = new List<Vertex>();

			for (int i = 0; i < 3; i++)
			{
				perimeter.Add(vertices[i]);
				List<Vertex> edgeIntersections = edges[i].intersections.Select(intersection => intersection.vertex).ToList();

				edgeIntersections.Sort((a, b) => Math.Sign(Vector3.Distance(a.value, vertices[i].value) - Vector3.Distance(b.value, vertices[i].value)));

				perimeter.AddRange(edgeIntersections);
			}

			return perimeter;
		}

		public void UpdateEdges()
		{
			edges.Add(new Edge(vertices[0], vertices[1]));
			edges.Add(new Edge(vertices[1], vertices[2]));
			edges.Add(new Edge(vertices[2], vertices[0]));
		}

		/// <summary>
		/// Determines whether the given Vertex is one of this Triangle's vertices
		/// </summary>
		/// <param name="vertex">The Vertex to compare against</param>
		/// <returns>True if the Vertex is contained, false if it is not</returns>
		public bool ContainsVertex(Vertex vertex)
		{
			return vertices.Contains(vertex);
		}

		/// <summary>
		/// Returns the normal vector of this Triangle as defined by the ordering of its vertices
		/// </summary>
		/// <returns>The calculated normal Vector3</returns>
		public Vector3 CalculateNormal()
		{
			//1>3 X 1>2
			return Vector3.Cross(vertices[2].value - vertices[0].value, vertices[1].value - vertices[0].value).normalized;
		}

		/// <summary>
		/// Flips the order of the vertex list in order to flip the triangle's facing
		/// </summary>
		public void FlipNormal()
		{
			vertices.Reverse();
		}

		/// <summary>
		/// Clears cut metadata for all edges of this triangle
		/// </summary>
		public void ClearLocalMetadata()
		{
			vertices.ForEach(vertex => vertex.loops = new List<EdgeLoop>());

			foreach (Edge edge in edges)
			{
				edge.intersections.ForEach(intersection =>
				{
					intersection.vertex.cut = null;
					intersection.vertex.loops = new List<EdgeLoop>();
				});
			}
		}

		public void ClearCutMetadata()
		{
			internalIntersections.Clear();
		}

		public override string ToString() => $"{base.ToString()}::{string.Join("::", vertices.Select(v=>v.value.ToString("F4")))}";

		public void Draw(Color color)
		{
			if (edges.Count == 0)
			{
				edges.Add(new Edge(vertices[0], vertices[1]));
				edges.Add(new Edge(vertices[1], vertices[2]));
				edges.Add(new Edge(vertices[2], vertices[0]));
			}
			edges.ForEach(edge => edge.Draw(color));
		}

		public void DrawNormal(Color color)
		{
			Vector3 center = (vertices[0].value + vertices[1].value + vertices[2].value) / 3;
			Debug.DrawLine(center, center + (0.5f * CalculateNormal()), color, 60f);
		}
	}

}
