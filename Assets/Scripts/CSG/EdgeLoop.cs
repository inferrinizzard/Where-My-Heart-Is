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
		/// Creates a series of Triangles that cover the surface of this EdgeLoop
		/// </summary>
		/// <returns>The created triangles</returns>
		public List<Triangle> Triangulate(int earToDraw)
		{
			List<Triangle> triangles = new List<Triangle>();
			List<Vertex> currentVertices = new List<Vertex>(vertices);
            Triangle nextEar;

			//ear method
			int i = 0;
            //Debug.Log("Edge LOPOP");
			while (currentVertices.Count > 3)
			{
				i++;
				if (i > 100)
				{
                    //Debug.Log(triangles[0]);
                    //currentVertices.ForEach(vertex => vertex.Draw(0.2f, Vector3.up, Color.red));
                    triangles.ForEach(triangle => triangle.Draw(Color.green));
					throw new System.Exception("Triangle overflow");
				}
                nextEar = CreateNextEar(currentVertices, true);// i == earToDraw

                if (nextEar != null) triangles.Add(nextEar);
			}
			triangles.Add(new Triangle(currentVertices[0], currentVertices[1], currentVertices[2]));

            triangles.ForEach(triangle => triangle.Draw(Color.white));

			return triangles;
		}

        public List<Triangle> TriangulateFanMethod()
        {
            List<Triangle> triangles = new List<Triangle>();
            for(int i = 0; i < vertices.Count - 2; i++)
            {
                triangles.Add(new Triangle(vertices[0], vertices[i+1], vertices[i+2]));
            }

            return triangles;
        }

		private Triangle CreateNextEar(List<Vertex> currentVertices, bool draw)
		{
			for (int i = 0; i < currentVertices.Count; i++)
			{
				Vector3 a = currentVertices[i].value - currentVertices[(i + 1) % currentVertices.Count].value;
				Vector3 b = currentVertices[(i + 2) % currentVertices.Count].value - currentVertices[(i + 1) % currentVertices.Count].value;
				/*Debug.Log(currentVertices[i] + " :: " + currentVertices[(i + 1) % currentVertices.Count] + " :: " + currentVertices[(i + 2) % currentVertices.Count]);
				Debug.Log((currentVertices[i].value - currentVertices[(i + 1) % currentVertices.Count].value).ToString("F4"));
				Debug.Log(currentVertices[(i + 2) % currentVertices.Count].value - currentVertices[(i + 1) % currentVertices.Count].value);
				Debug.Log(SignedAngle(a, b));*/
				//if (Vector3.SignedAngle(a, b, Vector3.Cross(a, b)) > 0)
				if (SignedAngle(a, b) > 0)
				{
					Triangle resultingTriangle = new Triangle(currentVertices[i], currentVertices[(i + 1) % currentVertices.Count], currentVertices[(i + 2) % currentVertices.Count]);
					if (!TriangleContainsAny(currentVertices, resultingTriangle, draw))
					{
						currentVertices.RemoveAt((i + 1) % currentVertices.Count);
						return resultingTriangle;
					}
                    else
                    {
                        if(draw) resultingTriangle.Draw(Color.cyan);
                    }
				}
			}

			return null;
		}

		private float SignedAngle(Vector3 a, Vector3 b)
		{
			//Vector3 cross = Vector3.Cross(a, b);
			//int  = Mathf.Sign
			//Debug.Log(Mathf.Sign(Mathf.Asin(Vector3.Cross(a, b).magnitude / (a.magnitude * b.magnitude))));
			return Mathf.Asin(Vector3.Cross(a, b).magnitude / (a.magnitude * b.magnitude));
		}

		private bool TriangleContainsAny(List<Vertex> vertices, Triangle triangle, bool draw)
		{
			foreach (Vertex vertex in vertices)
			{
				if (!triangle.vertices.Contains(vertex) && vertex.LiesWithinTriangle(triangle))
				{
                    if(draw) vertex.Draw(0.8f, Vector3.up, Color.cyan);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Finds the unit vector normal to the plane defined by this EdgeLoop
		/// </summary>
		/// <returns>The normal vector</returns>
		public Vector3 GetNormal()
		{
			return Vector3.Cross(vertices[0].value - vertices[1].value, vertices[2].value - vertices[1].value).normalized;
		}

        public void FlipNormal()
        {
            vertices.Reverse();
        }

        public void MatchNormal(EdgeLoop toMatch)
        {
            if (Vector3.Distance(this.GetNormal(), toMatch.GetNormal()) > 0.0001)
            {
                this.FlipNormal();
            }
        }

		public void RemoveDuplicates()
		{
			for (int i = vertices.Count - 1; i > 0; i--)
			{
				for (int k = i - 1; k >= 0; k--)
				{
					/*Debug.Log(vertices[i]);
					Debug.Log(vertices[k]);
					Debug.Log(Vector3.Distance(vertices[i].value, vertices[k].value));*/
					if (Vector3.Distance(vertices[i].value, vertices[k].value) < 0.0001)
					{
						//Debug.Log("removing " + i);
						vertices.RemoveAt(i);
						break;
					}
				}
			}
		}

		public override string ToString() =>
			$"{base.ToString()}::{string.Join("::", vertices.Select(v => (v.value.ToString("F4") + " (" + (v.fromIntersection) + ")")))}";

        public void Draw(Color color, Color secondaryColor, Color vertexColor)
		{

            float increment = 1f / vertices.Count;
            for (int i = 0; i < vertices.Count; i++)
			{
                vertices[i].Draw(0.05f, Vector3.forward, vertexColor);
                vertexColor = Color.blue;
                color += secondaryColor * increment;
                //color = new Color(i * increment, i * increment, i * increment);
                //Debug.Log(i * increment);
                Debug.DrawLine(vertices[i].value, vertices[(i + 1) % vertices.Count].value, color);
			}
		}

	}

}
