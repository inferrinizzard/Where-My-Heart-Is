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
        }

        /// <summary>
        /// Parses the given Mesh and creates a CSG.Model version for manipulation
        /// </summary>
        /// <param name="mesh">The Mesh to parse</param>
        public Model(Mesh mesh)
        {
            vertices = mesh.vertices.Select((vertex, index) => new Vertex(index, vertex)).ToList();

            triangles = new List<Triangle>();

            int[] meshTriangles = mesh.triangles;

            for (int i = 0; i < meshTriangles.Length; i += 3)
            {
                Triangle triangle = new Triangle(vertices[meshTriangles[i]], vertices[meshTriangles[i + 1]], vertices[meshTriangles[i + 2]]);
                triangles.Add(triangle);
            }

            CreateEdges();
        }

        /// <summary>
        /// Creates an edge object for each unique pair of vertices in this model that share a triangle
        /// </summary>
        private void CreateEdges()
        {
            edges = new List<Edge>();
            foreach (Triangle triangle in triangles)
            {
                for (int i = 0; i < 3; i++)
                {
                    List<Edge> preexistingEdges = edges.Where(edge =>
                        !edge.vertices.Contains(triangle.vertices[i]) &&
                        !edge.vertices.Contains(triangle.vertices[(i + 1) % 3])
                        ).ToList();
                    if (preexistingEdges.Count == 0)// if there are no edges that contain both the current vertices
                    {
                        Edge createdEdge = new Edge(triangle.vertices[i], triangle.vertices[(i + 1) % 3]);
                        edges.Add(createdEdge);
                        triangle.edges.Add(createdEdge);
                    }
                    else// otherwise, the edge already exists
                    {
                        triangle.edges.Add(preexistingEdges[0]);
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

            createdVertices.AddRange(IntersectAWithB(this, other));
            createdVertices.AddRange(IntersectAWithB(other, this));

           //Debug.Log("Intersections: " + createdVertices.Count);

            return createdVertices;
        }

        private List<Vertex> IntersectAWithB(Model modelA, Model modelB)
        {
            List<Vertex> createdVertices = new List<Vertex>();
            foreach(Edge edge in modelA.edges)
            {
                foreach(Triangle triangle in modelB.triangles)
                {
                    Intersection intersection = edge.IntersectWithTriangle(triangle);

                    if (intersection != null)
                    {
                        createdVertices.Add(intersection.vertex);
                        edge.intersections.Add(intersection);

                        bool foundEdgeEdge = false;
                        foreach (Edge triangleEdge in triangle.edges)// check if this intersection intersects an edge of the triangle
                        {
                            if (intersection.vertex.LiesOnEdge(triangleEdge))
                            {
                                triangleEdge.intersections.Add(intersection);
                                foundEdgeEdge = true;
                                break;
                            }
                        }

                        if (!foundEdgeEdge)// if it does not intersect an edge of the trangle, it can be saved as an internal intersection
                        {
                            triangle.internalIntersections.Add(intersection);
                        }
                    }
                }
            }

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
	}
}
