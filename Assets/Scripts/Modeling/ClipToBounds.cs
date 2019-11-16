using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipToBounds : MonoBehaviour
{
    public float error;

    //public GameObject bounds;

    public int testVal;
    public bool limitDisplay;


    public void Clip(GameObject toClip, GameObject bounds, GameObject output)
    {


        /*CombineInstance[] combine = new CombineInstance[2];
        combine[0].mesh = ClipAToB(toClip, bounds, output);
        combine[0].transform = Matrix4x4.identity;
        combine[1].mesh = ClipAToB(bounds, toClip, output);
        combine[1].transform = Matrix4x4.identity;

        ConvertCoords(bounds, toClip, combine[1].mesh);

        Mesh completedMesh = new Mesh();
        completedMesh.CombineMeshes(combine);
        output.GetComponent<MeshFilter>().mesh = completedMesh;*/


        output.GetComponent<MeshFilter>().mesh = ClipAToB(toClip, bounds, output);
        output.GetComponent<MeshCollider>().sharedMesh = ClipAToB(toClip, bounds, output);
    }

    private void ConvertCoords(GameObject from, GameObject to, Mesh mesh)
    {
        List<Vector3> newVertices = new List<Vector3>();
        foreach(Vector3 vector in mesh.vertices)
        {
            newVertices.Add(BoundsSpaceToModelSpace(vector, to, from));
        }

        mesh.SetVertices(newVertices);
    }

    private Mesh ClipAToB(GameObject toClip, GameObject bounds, GameObject result)
    {
        List<Triangle> boundsTriangles;

        // update bounds triangles
        boundsTriangles = GetBoundsTriangles(toClip, bounds);

        // get a list of external vertices
        List<Vertex> vertices = ClassifyVertices(toClip, bounds, boundsTriangles);

        // use that list to find all triangles intersecting the bounding volume
        List<Triangle> intersectingTriangles = GetAllTriangles(toClip, vertices);

        // use this list of triangles of interest to generate the geometry of the cut, covering the hole
        // and the replacement faces, which are triangles cut short by the volume
        List<Surface> surfaces = FindReplacementSurfaces(intersectingTriangles, boundsTriangles, vertices, toClip);
        List<Triangle> triangles = new List<Triangle>();

        // now turn these surfaces into triangles
        //Debug.Log(surfaces.Count);
        
        if(limitDisplay && testVal < surfaces.Count)
        {
            surfaces[testVal].RemoveDuplicates();
            surfaces[testVal].RecalculateOrder(toClip);
            triangles.AddRange(TriangulateSurface(surfaces[testVal]));
        }
        else
        {
            foreach (Surface surface in surfaces)
            {
                surface.RemoveDuplicates();
                surface.RecalculateOrder(toClip);
                triangles.AddRange(TriangulateSurface(surface));
            }
        }
        /**/
        //triangles.Add(new Triangle(surfaces[0].vertices[0], surfaces[0].vertices[1], surfaces[0].vertices[2]));



        //triangles.AddRange(TriangulateSurface(surfaces[2]));

        if(testVal < surfaces.Count)
        {
            int integer = 1;
            foreach (Vertex vertex in surfaces[testVal].vertices)
            {
                Vector3 point = toClip.transform.localToWorldMatrix.MultiplyPoint3x4(vertex.value) + result.transform.localPosition;

                Debug.DrawLine(point, point - surfaces[testVal].normal * 0.2f * integer, Color.red, 0f);
                integer++;
            }
            integer = 1;
        }
        
        /*foreach (Vertex vertex in surfaces[testval+1].vertices)
        {
            Debug.DrawLine(toClip.transform.localToWorldMatrix.MultiplyPoint3x4(vertex.value), toClip.transform.localToWorldMatrix.MultiplyPoint3x4(vertex.value) + Vector3.back * 0.2f * integer, Color.red, 60);
            integer++;
        }*/

        // and combine with the fully contained and unaffected triangles inside the bounding mesh
        triangles.AddRange(GetIncludedTriangles(toClip, vertices));// these define the final mesh!


        Mesh completedMesh = new Mesh();

        result.GetComponent<MeshFilter>().mesh = completedMesh;

        // reindex vertices and add them to the mesh
        List<Vector3> createdVertices = new List<Vector3>();

        PruneExternalVertices(vertices);
        //PruneUnusedVertices(vertices, triangles);

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

    private void PruneExternalVertices(List<Vertex> vertices)
    {
        for(int i = vertices.Count - 1; i > 0; i--)
        {
            if(!vertices[i].containedByBound)
            {
                vertices.Remove(vertices[i]);
            }
        }
    }

    private void PruneUnusedVertices(List<Vertex> vertices, List<Triangle> triangles)
    {
        for (int i = vertices.Count - 1; i > 0; i--)
        {
            if (vertices[i].triangles.Count == 0)
            {
                vertices.Remove(vertices[i]);
            }
        }
    }


    private List<Surface> FindReplacementSurfaces(List<Triangle> intersectingTriangles, List<Triangle> boundsTriangles, List<Vertex> modelVertices, GameObject toClip)
    {
        //List<Triangle> boundsTriangles = GetAllTriangles(bounds, GetBoundsVertices());

        List<Surface> reconfiguredTriangles = new List<Surface>();
        List<Surface> newFaces = new List<Surface>();

        List<Vertex> vertices = new List<Vertex>();

        Surface currentSurface = new Surface();

        // for each trangle of the model, find all intersections with the bound's edges, 
        foreach (Triangle modelTriangle in intersectingTriangles)
        {
            // the current reconfigured model triangle
            currentSurface = new Surface();

            // vertices of the model triangle contained by the bound
            foreach (Vertex vertex in modelTriangle.vertices)
            {
                if (vertex.containedByBound)
                {
                    currentSurface.vertices.Add(vertex);
                }
            }

            // vertices from the intersections of the two triangles at hand
            foreach (Triangle boundsTriangle in boundsTriangles)
            {
                List<Vertex> intersections = FindTriangleIntersections(modelTriangle, boundsTriangle, toClip);
                foreach(Vertex vertex in intersections)
                {
                    vertex.triangles.Add(modelTriangle);
                }

                if (intersections.Count > 0)
                {
                    Surface newSurface = new Surface();
                    newSurface.vertices.AddRange(intersections);
                    newSurface.normal = boundsTriangle.CalculateNormal();
                    newFaces.Add(newSurface);
                }

                currentSurface.vertices.AddRange(intersections);
                modelVertices.AddRange(intersections);
            }

            //currentSurface.RemoveDuplicates();
            //currentSurface.RecalculateOrder();

            currentSurface.normal = modelTriangle.CalculateNormal();
            if(currentSurface.vertices.Count != 0) reconfiguredTriangles.Add(currentSurface);
        }


        /*foreach (Triangle boundsTriangle in boundsTriangles)
        {
            List<Vertex> intersections = FindTriangleIntersections(boundsTriangle, boundsTriangle, toClip);
            foreach (Vertex vertex in intersections)
            {
                vertex.triangles.Add(boundsTriangle);
            }
            Debug.Log("hey");
            currentSurface.vertices.AddRange(intersections);
            
            modelVertices.AddRange(intersections);
        }*/
        // and find all intersections of the triangle's edges with the bound
        // these new vertices represent the new surface 


        // split these into surfaces by shared triangles

        /*Vertex currentVertex;// = vertices[0];
        Debug.Log(vertices.Count);
        
        while (vertices.Count > 0)
        {
            currentVertex = vertices[0];
            currentSurface = new Surface();
            currentSurface.vertices.Add(currentVertex);
            vertices.Remove(currentVertex);

            bool foundNext = true;

            while (foundNext)
            {
                foundNext = false;
                foreach (Vertex vertex in vertices)
                {
                    if (currentVertex.SharesTriangle(vertex))
                    {
                        currentVertex = vertex;
                        vertices.Remove(currentVertex);
                        currentSurface.vertices.Add(currentVertex);
                        foundNext = true;
                        break;
                    }
                }
            }
            Debug.Log(currentSurface.vertices.Count);
            reconfiguredTriangles.Add(currentSurface);
        }*/
        reconfiguredTriangles.AddRange(newFaces);
        return reconfiguredTriangles;
    }


    private Vector3 BoundsSpaceToModelSpace(Vector3 toTransform, GameObject toClip, GameObject bounds)
    {
        Vector3 point = bounds.transform.localToWorldMatrix.MultiplyPoint3x4(toTransform);
        point = toClip.transform.worldToLocalMatrix.MultiplyPoint3x4(point);
        return point;
    }

    private List<Vertex> FindTriangleIntersections(Triangle a, Triangle b, GameObject toClip)
    {
        List<Vertex> vertices = new List<Vertex>();

        // for each edge of a, raycast to b
        for(int i = 0; i < 3; i++)
        {
            Vertex vertex = EdgeIntersectsTriangle(a.vertices[i].value, a.vertices[(i + 1) % 3].value, b);
            if(vertex != null)
            {
                //Debug.Log("point :: " + vertex.value);
                //vertex.triangles.Add(a);
                vertices.Add(vertex);
            }
        }

        // for each edge of b, raycast to a
        for (int i = 0; i < 3; i++)
        {
            Vertex vertex = EdgeIntersectsTriangle(b.vertices[i].value, b.vertices[(i + 1) % 3].value, a);
            if (vertex != null)
            {
                //vertex.triangles.Add(b);
                vertices.Add(vertex);
            }
        }

        return vertices;
    }
    
    private Vertex EdgeIntersectsTriangle(Vector3 pointA, Vector3 pointB, Triangle triangle)
    {
        Vertex raycastIntersection = RaycastToTriangle(pointA, pointA - pointB, triangle);
        if(raycastIntersection != null)
        {
            float temp = Vector3.Distance(pointA, raycastIntersection.value) + Vector3.Distance(raycastIntersection.value, pointB);
            // if the point lies on the edge
            if (Mathf.Abs(Vector3.Distance(pointA, pointB) - temp) < error)
            {
                return raycastIntersection;
            }
        }

        return null;
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

    private List<Triangle> TriangulateSurface(Surface surface)
    {
        List<Triangle> triangles = new List<Triangle>();
        for(int i = 1; i < surface.vertices.Count - 1; i++)
        {
            if(false)//IntersectsLines(surface, i))
            {
                Debug.Log("here");
                Triangle triangle = new Triangle(surface.GetPredecessor(i), surface.vertices[i], surface.GetSuccessor(i));
                if(Vector3.Distance(triangle.CalculateNormal(), surface.normal) > error)
                {
                    triangle.FlipNormal();
                }
                triangles.Add(triangle);
                surface.vertices[i].active = false;
            }
            else
            {
                Triangle triangle = new Triangle(surface.vertices[0], surface.vertices[i], surface.GetSuccessor(i));

                if (Vector3.Distance(triangle.CalculateNormal(), surface.normal) > error)
                {
                    triangle.FlipNormal();
                }
                triangles.Add(triangle);
            }
        }

        return triangles;
    }

    private bool IntersectsLines(Surface surface, int index)
    {
        // for each pair of vertices (each edge) on the surface preceeding the vertex, check if there is an intersection with the
        // proposed new edge and return the result
        Vector3 proposedOrigin = surface.vertices[index].value;
        Vector3 proposedDirection = surface.vertices[0].value - surface.vertices[index].value;

        for (int i = 0; i < index; i++)
        {
            Vector3 testSegmentOrigin = surface.vertices[i].value;
            Vector3 testSegmentDirection = surface.vertices[i + 1].value - surface.vertices[i].value;
            Vector3 lineIntersection = FindIntersectionOfTwoLines(proposedOrigin, proposedDirection, testSegmentOrigin, testSegmentDirection);
            float sum = (lineIntersection - surface.vertices[i].value).magnitude + (surface.vertices[i + 1].value - lineIntersection).magnitude;

            if (Mathf.Abs(testSegmentDirection.magnitude - sum) < error)
            {
                return true;
            }
        }

        return false;
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

        //Debug.Log(meshVertices.Length);

        for(int i = 0; i < meshVertices.Length; i++)
        {
            vertices.Add(new Vertex(i, meshVertices[i], PointContainedByBounds(meshVertices[i], toClip, boundsTriangles)));
        }

        return vertices;
    }

    private bool PointContainedByBounds(Vector3 point, GameObject toClip, List<Triangle> boundsTriangles)
    {
        int intersectionsAbove = 0;
        int intersectionsBelow = 0;

        // for each triangle, do a raycast along Vector3.up
        foreach (Triangle boundsTriangle in boundsTriangles)
        {
            Vertex intersectionPoint = RaycastToTriangle(point, Vector3.up, boundsTriangle);

            if (intersectionPoint != null)
            {
                // if the ray intersects with the face, determine if the intersection is above or below the vertex
                if(intersectionPoint.value.y > point.y)
                {
                    intersectionsAbove++;
                }
                else
                {
                    intersectionsBelow++;
                }
            }
            // keep a tally of above and below. 
        }
        // If above and below are odd, vertex is contained
        // if they are even, vertex is not contained
        return intersectionsAbove % 2 == 1 && intersectionsBelow % 2 == 1;
    }

    private Vertex RaycastToTriangle(Vector3 origin, Vector3 direction, Triangle triangle)
    {
        // determine equation of plane
        Vector3 normal = Vector3.Cross(triangle.vertices[0].value - triangle.vertices[1].value, triangle.vertices[1].value - triangle.vertices[2].value);
        Vector3 planePoint = triangle.vertices[0].value;
        //Debug.Log(origin);
        //Debug.Log(direction);

        // get ray intersection with plane,
        float numerator = normal.x * (planePoint.x - origin.x) + normal.y * (planePoint.y - origin.y) + normal.z * (planePoint.z - origin.z);
        float denominator = normal.x * direction.x + normal.y * direction.y + normal.z * direction.z;
        Vector3 intersectionPoint = ((numerator / denominator) * direction) + origin;
        //Debug.Log(numerator);
        //Debug.Log(denominator);
        //Debug.Log(intersectionPoint);
        // determine if that point is in the triangle
        float areaOfInternalTriangle = FindAreaOfTriangle(triangle.vertices[0].value, triangle.vertices[1].value, triangle.vertices[2].value);

        float areaOfPointTriangles = 0;

        for(int i = 0; i < 3; i++)
        {
            areaOfPointTriangles += FindAreaOfTriangle(intersectionPoint, triangle.vertices[i].value, triangle.vertices[(i + 1) % 3].value);
        }


        // if the point is on the triangle
        if(Mathf.Abs(areaOfInternalTriangle - areaOfPointTriangles) < error)
        {
        //Debug.Log("this one did");
            Vertex result = new Vertex(0, intersectionPoint, true);
            return result;
        }
        //Debug.Log("this one didn't");
        return null;// if we don't find anything, return null
    }

    private float FindAreaOfTriangle(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        float aToB = (pointB - pointA).magnitude;
        float bToC = (pointC - pointB).magnitude;
        float cToA = (pointA - pointC).magnitude;
        float s = (aToB + bToC + cToA) / 2;
        return Mathf.Sqrt(s * (s - aToB) * (s - bToC) * (s - cToA));
    }

    private List<Triangle> GetIntersectingTriangles(GameObject toClip, List<Vertex> vertices)
    {
        List<Triangle> intersectingTriangles = new List<Triangle>();
        int[] meshTriangles = toClip.GetComponent<MeshFilter>().mesh.triangles;


        for (int i = 0; i < meshTriangles.Length; i += 3)
        {
            // if any one of the vertices of this triangle exists outside of the bound
            if (!vertices[meshTriangles[i]].containedByBound || !vertices[meshTriangles[i + 1]].containedByBound || !vertices[meshTriangles[i + 2]].containedByBound)
            {
                // but only if at least one resides inside the bound
                if(vertices[meshTriangles[i]].containedByBound || vertices[meshTriangles[i + 1]].containedByBound || vertices[meshTriangles[i + 2]].containedByBound)
                {
                    Triangle intersectingTriangle = new Triangle(vertices[meshTriangles[i]], vertices[meshTriangles[i + 1]], vertices[meshTriangles[i + 2]]);
                    intersectingTriangles.Add(intersectingTriangle);
                }
            }
        }

        return intersectingTriangles;
    }

   /* private List<Triangle> GetExcludedTriangles(GameObject toClip, List<Vertex> vertices)
    {
        List<Triangle> excludedTriangles = new List<Triangle>();
        int[] meshTriangles = toClip.GetComponent<MeshFilter>().mesh.triangles;

        for (int i = 0; i < meshTriangles.Length; i += 3)
        {
            // if all vertices of the triangle are contained by the bound
            if (!vertices[i].containedByBound && !vertices[i + 1].containedByBound && !vertices[i + 2].containedByBound)
            {
                Triangle excludedTriangle = new Triangle(vertices[i], vertices[i + 1], vertices[i + 2]);
                excludedTriangles.Add(excludedTriangle);
            }
        }

        return excludedTriangles;
    }
    */
    private List<Triangle> GetIncludedTriangles(GameObject toClip, List<Vertex> vertices)
    {
        List<Triangle> includedTriangles = new List<Triangle>();
        int[] meshTriangles = toClip.GetComponent<MeshFilter>().mesh.triangles;

        for (int i = 0; i < meshTriangles.Length; i += 3)
        {
            // if all vertices of the triangle are contained by the bound
            if (vertices[meshTriangles[i]].containedByBound && vertices[meshTriangles[i + 1]].containedByBound && vertices[meshTriangles[i + 2]].containedByBound)
            {
                Triangle includedTriangle = new Triangle(vertices[meshTriangles[i]], vertices[meshTriangles[i + 1]], vertices[meshTriangles[i + 2]]);
                includedTriangles.Add(includedTriangle);
            }
        }

        return includedTriangles;
    }

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

        for(int i = 0; i < mesh.vertices.Length; i++)
        {
            //Debug.Log(BoundsSpaceToModelSpace(mesh.vertices[i], toClip) + " :: " + mesh.vertices[i]);
        }

        return triangles;
    }
}


/*
private List<Surface> FindNewSurfaces(List<Triangle> intersectingTriangles, GameObject toClip)
{
    Mesh mesh = bounds.GetComponent<MeshFilter>().mesh;

    List<Vertex> vertices = new List<Vertex>();

    // for each face of the bound,
    foreach (Triangle boundsTriangle in boundsTriangles)
    {
        // and for each triangle intersecting the bound
        foreach (Triangle triangle in intersectingTriangles)
        {
            //find all intersections of the triangles edges with the bound face
            // and find all intersections of the bound with the triangle
            vertices.AddRange(FindTriangleIntersections(boundsTriangle, triangle));
        }
    }
    // split these into surfaces by shared triangles

    Vertex currentVertex;// = vertices[0];
    Surface currentSurface;
    List<Surface> surfaces = new List<Surface>();
    while(vertices.Count > 0)
    {
        currentVertex = vertices[0];
        currentSurface = new Surface();
        currentSurface.vertices.Add(currentVertex);
        vertices.Remove(currentVertex);

        bool foundNext = true;

        while (foundNext)
        {
            foundNext = false;
            foreach (Vertex vertex in vertices)
            {
                if (currentVertex.SharesTriangle(vertex))
                {
                    currentVertex = vertex;
                    vertices.Remove(currentVertex);
                    currentSurface.vertices.Add(currentVertex);
                    foundNext = true;
                    break;
                }
            }
        }
    }
    // return the surfaces

    return surfaces;
}*/
