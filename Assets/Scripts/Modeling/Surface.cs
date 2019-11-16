using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surface
{
    public List<Vertex> vertices;
    public Vector3 normal;

    public Surface()
    {
        vertices = new List<Vertex>();
    }

    public void RemoveDuplicates()
    {
        for(int i = vertices.Count - 1; i > 1; i--)
        {
            for (int k = i - 1; k > 0; k--)
            {
                if(vertices[i].value == vertices[k].value)
                {
                    //Debug.Log("Vertex " + vertices[i].value + " is the same as vertex " + vertices[k].value + ", removing...");
                    vertices.Remove(vertices[i]);
                    break;
                }
            }
        }
    }

    public void RecalculateOrder(GameObject toClip)
    {
        Vector3 average = Vector3.zero;

        //Debug.Log(vertices.Count);
        foreach(Vertex vertex in vertices)
        {
            //Debug.Log(toClip.transform.localToWorldMatrix.MultiplyPoint3x4(vertex.value));
            //Debug.DrawLine(toClip.transform.localToWorldMatrix.MultiplyPoint3x4(vertex.value), toClip.transform.localToWorldMatrix.MultiplyPoint3x4(vertex.value) + Vector3.up * 0.5f, Color.green, 60);

            average = average + vertex.value;
        }

        average /= vertices.Count;
        //Debug.DrawLine(toClip.transform.localToWorldMatrix.MultiplyPoint3x4(average), toClip.transform.localToWorldMatrix.MultiplyPoint3x4(average) + Vector3.up * 0.2f , Color.blue, 60);

        Vector3 begin = vertices[0].value - average;
        List<Vertex> reordering = new List<Vertex>();

        Vector3 current = begin;

        while (vertices.Count > 0)
        {
            Vertex bestVertex = vertices[0];

            foreach(Vertex vertex in vertices)
            {
                if(Vector3.Angle(current, vertex.value - average) < Vector3.Angle(current, bestVertex.value - average))
                {
                    bestVertex = vertex;
                }
            }

            current = bestVertex.value - average;
            reordering.Add(bestVertex);
            vertices.Remove(bestVertex);
        }

        vertices.AddRange(reordering);

        // split these into surfaces by shared triangles
        /*
        Vertex currentVertex;// = vertices[0];
        List<Vertex> reordering = new List<Vertex>();
        while (vertices.Count > 0)
        {
            currentVertex = vertices[0];
            reordering.Add(currentVertex);
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
                        reordering.Add(currentVertex);
                        foundNext = true;
                        break;
                    }
                }
            }
        }

        vertices.AddRange(reordering);*/
    }
    
    public Vertex GetSuccessor(int index)
    {
        if (index == vertices.Count - 1) return null;
        int i = index + 1;
        Vertex currentVertex = vertices[i];
        while(!currentVertex.active)
        {
            i++;
            if (i == vertices.Count - 1) return null;
            currentVertex = vertices[i];
        }

        return currentVertex;
    }
    
    /*public Vertex GetSuccessor(int index)
    {
        int successorIndex = GetSuccessorIndex(index);
        if (successorIndex != -1)
        {
            return vertices[successorIndex];
        }
        else
        {
            return null;
        }
    }
    */
    public int GetSuccessorIndex(int index)
    {
        if (index == vertices.Count - 1) return -1;
        int i = index + 1;
        int currentVertex = i;
        while (!vertices[currentVertex].active)
        {
            i++;
            if (i == vertices.Count - 1) return -1;
            currentVertex = i;
        }

        return currentVertex;
    }

    public Vertex GetPredecessor(int index)
    {
        if (index <= 0) return null;
        int i = index - 1;
        Vertex currentVertex = vertices[i];
        while (!currentVertex.active)
        {
            i--;
            if (i <= 0)
            {
                return vertices[0];
            }
            currentVertex = vertices[i];
        }

        return currentVertex;
    }
}
