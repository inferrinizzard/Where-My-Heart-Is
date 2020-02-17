using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CSG
{
    /// <summary>
    /// Representation of a Mesh used for CSG operations
    /// </summary>
    public class Model
    {
        public List<Vertex> vertices;
        public List<Triangle> triangles;
        public List<Edge> edges;

        /// <summary>
        /// Initializes a new empty Model
        /// </summary>
        public Model()
        {
            vertices = new List<Vertex>();
            triangles = new List<Triangle>();
            edges = new List<Edge>();
        }

        /// <summary>
        /// Parses the given Mesh and creates a CSG.Model version for manipulation
        /// </summary>
        /// <param name="mesh">The Mesh to parse</param>
        public Model(Mesh mesh)
        {
            vertices = mesh.vertices.Select((vertex, index) => new Vertex(index, vertex)).ToList();
            List<Vertex> uniqueVertices = new List<Vertex>();
            foreach (Vertex vertex in vertices)
            {
                Vertex existingVertex = uniqueVertices.Find(testVertex => testVertex.value == vertex.value);
                if (existingVertex == null)
                {
                    uniqueVertices.Add(vertex);
                }
                else
                {
                    uniqueVertices.Add(existingVertex);
                }
            }
            vertices = uniqueVertices;
            

            triangles = new List<Triangle>();

            int[] meshTriangles = mesh.triangles;

            for (int i = 0; i < meshTriangles.Length; i += 3)
            {
                Triangle triangle = new Triangle(vertices[meshTriangles[i]], vertices[meshTriangles[i + 1]], vertices[meshTriangles[i + 2]]);
                triangles.Add(triangle);
            }

            uniqueVertices = new List<Vertex>();
            foreach (Vertex vertex in vertices)
            {
                Vertex existingVertex = uniqueVertices.Find(testVertex => testVertex == vertex);
                if (existingVertex == null)
                {
                    uniqueVertices.Add(vertex);
                }
            }

            vertices = uniqueVertices;

            CreateEdges();

            //Debug.Log(edges.Count);
        }

        /// <summary>
        /// Creates an edge object for each unique pair of vertices in this model that share a triangle
        /// </summary>
        private void CreateEdges()
        {
            edges = new List<Edge>();
            //Debug.Log(triangles.Count);
            foreach (Triangle triangle in triangles)
            {
                for (int i = 0; i < 3; i++)
                {
                    Edge preexistingEdge = edges.Find(edge =>
                        edge.vertices.Contains(triangle.vertices[i]) &&
                        edge.vertices.Contains(triangle.vertices[(i + 1) % 3])
                        );

                    if (preexistingEdge == null)// if there are no edges that contain both the current vertices
                    {
                        Edge createdEdge = new Edge(triangle.vertices[i], triangle.vertices[(i + 1) % 3]);
                        edges.Add(createdEdge);
                        triangle.edges.Add(createdEdge);
                        createdEdge.triangles.Add(triangle);
                    }
                    else// otherwise, the edge already exists
                    {
                        triangle.edges.Add(preexistingEdge);
                        preexistingEdge.triangles.Add(triangle);
                    }
                }
            }

        }

        /// <summary>
        /// Finds all intersections of this model's edges with the given model's triangles and stores them in this model
        /// </summary>
        /// <param name="other">The model to find intersections with</param>
        public List<Vertex> IntersectWith(Model other)
        {
            List<Vertex> createdVertices = new List<Vertex>();

            createdVertices.AddRange(IntersectAWithB(this, other, new Color(0, 0, 0, 0)));
            createdVertices.AddRange(IntersectAWithB(other, this, Color.white));


            return createdVertices;
        }

        private List<Vertex> IntersectAWithB(Model modelA, Model modelB, Color color)
        {
            List<Vertex> createdVertices = new List<Vertex>();
            foreach(Edge edge in modelA.edges)
            {
                foreach(Triangle triangle in modelB.triangles)
                {
                    //Debug.Log("Intersecting edge: " + edge + " with triangle: " + triangle);
                    Intersection intersection = edge.IntersectWithTriangle(triangle);

                    if (intersection != null)
                    {
                        intersection.vertex.referenceFrame = edges[0].referenceFrame;//TODO HAK
                        createdVertices.Add(intersection.vertex);
                        edge.intersections.Add(intersection);

                        triangle.internalIntersections.Add(intersection);
                    }
                }
            }
            //createdVertices.ForEach(vertex => vertex.Draw(0.02f, Vector3.back, color));
            return createdVertices;
        }
        

		/// <summary>
		/// Creates a unity Mesh from a list of CSG.Vertex and a list of CSG.Triangle
		/// </summary>
		/// <param name="vertices">The vertices of the mesh</param>
		/// <param name="triangles">The triangles of the mesh</param>
		/// <returns>The parsed Mesh</returns>
		public Mesh ToMesh()
		{
            //TODO: each face may need to have its vertices inputed as separate things
			Mesh mesh = new Mesh();

			// reindex vertices and add them to the mesh
			List<Vector3> createdVertices = vertices.Select((vertex, index) =>
			{
				vertex.index = index;
				return vertex.value;
			}).ToList();

			mesh.SetVertices(createdVertices);

			// add triangles to mesh
			int[] newTriangles = triangles.SelectMany(triangle => triangle.vertices.Select(vertex => vertex.index)).ToArray();

			mesh.SetTriangles(newTriangles, 0);

			return mesh;
		}

		/// <summary>
		/// Converts the location of each vertex of this Model to be expressed with respect with "to" instead of with respect to "from"
		/// </summary>
		/// <param name="from">The transform representing the model's current coordinate system</param>
		/// <param name="to">The transform representing the coordinate system to convert to</param>
		public void ConvertCoordinates(Transform from, Transform to)
		{
			foreach (Vertex vertex in vertices)
			{
				vertex.value = to.worldToLocalMatrix.MultiplyPoint3x4(from.localToWorldMatrix.MultiplyPoint3x4(vertex.value));
			}
		}

        public void ConvertToWorld(Transform referenceFrame)
        {
            vertices.ForEach(vertex => vertex.value = referenceFrame.localToWorldMatrix.MultiplyPoint3x4(vertex.value));
        }

        public void ConvertToLocal(Transform referenceFrame)
        {
            vertices.ForEach(vertex => vertex.value = referenceFrame.worldToLocalMatrix.MultiplyPoint3x4(vertex.value));
        }

        public bool Intersects(Model other, float error)
        {
            foreach (Vertex vertex in vertices)
            {
                if (vertex.ContainedBy(other, error))
                {
                    return true;
                }
            }

            return false;
        }

        public void AddTriangle(Triangle triangle)
        {
            triangles.Add(triangle);
            triangle.vertices.ForEach(vertex =>
            {
                if (!vertices.Contains(vertex))
                {
                    vertices.Add(vertex);
                }
            });
        }

        public void AddTriangles(List<Triangle> trianglesToAdd)
        {
            trianglesToAdd.ForEach(triangle =>
            {
                triangles.Add(triangle);
                triangle.vertices.ForEach(vertex =>
                {
                    if (!vertices.Contains(vertex))
                    {
                        vertices.Add(vertex);
                    }
                });
            });
        }

        public void FlipNormals()
        {
            triangles.ForEach(triangle => triangle.FlipNormal());
        }

        public static Model Combine(Model modelA, Model modelB)
        {
            Model result = new Model();

            modelA.triangles.ForEach(triangle =>
            {
                result.AddTriangle(triangle);
            }
            );

            modelB.triangles.ForEach(triangle =>
            {
                result.AddTriangle(triangle);
            }
            );

            return result;
        }
    }
}
