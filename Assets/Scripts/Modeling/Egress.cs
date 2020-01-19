using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSG
{
    /// <summary>
    /// A vertex on the edge of a triangle
    /// </summary>
    /// <remarks>
    /// <para>
    /// An Egress is a vertex found through intersection checking on the edge of a triangle that denotes an intersection
    /// with a bounding surface.
    /// </para>
    /// <para>
    /// Typically, an Egress denotes the beginning of a cut across the surface of the triangle, but sometimes
    /// it can be a part of multiple cuts.
    /// </para>
    /// </remarks>
    public class Egress : Vertex
    {
        /// <summary>
        /// A list of cuts across the associated triangle.
        /// </summary>
        /// <remarks>
        /// Typically, there will be only one cut, but occasionally there are multiple.
        /// </remarks>
        public List<Cut> cuts;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">Index in the mesh's vertex array</param>
        /// <param name="value">The location of the vertex in model space</param>
        /// <param name="containedByBound">Whether the vertex lies inside or outside the bounding shape</param>
        public Egress(int index, Vector3 value) : base(index, value)
        {
            cuts = new List<Cut>();
        }

        /// <summary>
        /// Creates an egress with the same properties as the given vertex
        /// </summary>
        /// <param name="vertex">The vertex to model this Egress after</param>
        /// <returns>The created Egress</returns>
        public static Egress CreateFromVertex(Vertex vertex)
        {
            Egress egress = new Egress(vertex.index, vertex.value);
            egress.usedInLoop = vertex.usedInLoop;
            egress.loops = vertex.loops;
            egress.triangles = vertex.triangles;

            return egress;
        }

        /// <summary>
        /// Finds and returns the cut among cuts which travels the furthest around the perimeter of the triangle
        /// </summary>
        /// <param name="perimeter">The perimeter in question</param>
        /// <returns>The cut that travels the furthest</returns>
        public Cut GetFurthestCut(List<Vertex> perimeter)
        {
            Cut bestCut = null;
            int bestCutIndex = -1;

            foreach (Cut cut in cuts)
            {
                if (cut.traversed == false)
                {
                    int perimeterIndex = perimeter.IndexOf(cut[cut.Count - 1]);
                    if (perimeterIndex > bestCutIndex)
                    {
                        bestCut = cut;
                        bestCutIndex = perimeterIndex;
                    }
                }
            }

            return bestCut;
        }

        /// <summary>
        /// Finds and returns the cut among cuts which travels the furthest around the perimeter of the triangle 
        /// while still terminating at a perimeter index between the given constraints.
        /// </summary>
        /// <param name="perimeter">The perimeter in question</param>
        /// <param name="ignore">A cut that should be ignored when searching for the furthest cut</param>
        /// <param name="minIndex">The minimum index a cut must arrive at to be returned</param>
        /// <param name="targetIndex">The index of the initial vertex of the current loop being traversed</param>
        /// <returns>The cut satisfying all constraints</returns>
        public Cut GetFurthestCut(List<Vertex> perimeter, Cut ignore, int minIndex, int targetIndex)
        {
            Cut bestCut = null;
            int bestCutIndex = -1;

            foreach (Cut cut in cuts)
            {
                if (cut.traversed == false && cut != ignore)
                {
                    int perimeterIndex = perimeter.IndexOf(cut[cut.Count - 1]);
                    if (perimeterIndex > bestCutIndex)
                    {
                        bestCut = cut;
                        bestCutIndex = perimeterIndex;
                    }
                }
            }

            if (bestCutIndex < minIndex && bestCutIndex != targetIndex)
            {
                //Debug.Log(bestCutIndex);
                return null;
            }

            return bestCut;
        }
    }
}

