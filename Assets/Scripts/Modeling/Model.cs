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
            // LINQ vertices = mesh.vertices.Select((v, i) => new Vertex(i, v));
            vertices = new List<Vertex>();
            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                vertices.Add(new Vertex(i, mesh.vertices[i]));
            }
            // ENDLINQ

            triangles = new List<Triangle>();

            int[] meshTriangles = mesh.triangles;

            for (int i = 0; i < meshTriangles.Length; i += 3)
            {
                Triangle triangle = new Triangle(vertices[meshTriangles[i]], vertices[meshTriangles[i + 1]], vertices[meshTriangles[i + 2]]);
                triangles.Add(triangle);
            }
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
            // LINQ List<Vector3> createdVertices = vertices.Select(v => v.value);
            List<Vector3> createdVertices = new List<Vector3>();

            // add vertices to mesh
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i].index = i; // vertices.ForEach((v, i) => v.index = i);
                createdVertices.Add(vertices[i].value);
            }
            //ENDLINQ

            mesh.SetVertices(createdVertices);

            // add triangles to mesh
            // LINQ int[] newTriangles = triangles.SelectMany(t => t.vertices.Select(v => v.index)).ToArray();
            int[] newTriangles = new int[triangles.Count * 3];
            int index = 0;

            foreach (Triangle triangle in triangles)
            {
                newTriangles[index++] = triangle.vertices[0].index;
                newTriangles[index++] = triangle.vertices[1].index;
                newTriangles[index++] = triangle.vertices[2].index;
            }
            //ENDLINQ

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
    }
}
