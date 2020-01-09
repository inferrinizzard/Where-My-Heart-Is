using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSG
{
    public class Cut : List<Vertex>
    {
        public bool traversed;

        public Cut()
        {

        }

        public Cut GetReversedCopy()
        {
            Cut copy = new Cut();
            for (int i = this.Count - 1; i >= 0; i--)
            {
                copy.Add(this[i]);
            }

            return copy;
        }

        public string ToString(List<Vertex> perimeter)
        {
            string output = "Cut:";
            foreach (Vertex vertex in this)
            {
                output += " " + perimeter.IndexOf(vertex) + ": " + vertex.value;
            }

            return output;
        }
    }

}


