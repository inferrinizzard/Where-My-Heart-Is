using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSG
{

    public class Egress : Vertex
    {
        public List<Cut> cuts;

        public Egress(int index, Vector3 value, bool containedByBound) : base(index, value, containedByBound)
        {
            cuts = new List<Cut>();
        }

        public static Egress CreateFromVertex(Vertex vertex)
        {
            Egress egress = new Egress(vertex.index, vertex.value, vertex.containedByBound);
            egress.usedInLoop = vertex.usedInLoop;
            egress.loops = vertex.loops;
            egress.triangles = vertex.triangles;

            return egress;
        }

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

        public Cut GetFurthestCut(List<Vertex> perimeter, Cut ignore, int minIndex, int targetIndex)
        {
            //Debug.Log(ignore.ToString(perimeter));
            Cut bestCut = null;
            int bestCutIndex = -1;

            foreach (Cut cut in cuts)
            {
                //Debug.Log(cut.traversed);
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

