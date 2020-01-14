using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CSG
{
    /// <summary>
    /// An ordered list of vertices that represents one contiguous sequence of intersections of a bounding object with a single triangle
    /// </summary>
    public class Cut : List<Vertex>
    {
        /// <summary>
        /// Whether this cut has been traversed during loop discovery yet
        /// </summary>
        public bool traversed;

        /// <summary>
        /// Creates a shallow copy of this Cut in reverse order
        /// </summary>
        /// <returns>The reversed copy</returns>

        // ↓ also kinda redundant

        public Cut GetReversedCopy() // public Cut GetReversedCopy() => this.Reverse();
        {
            Cut copy = new Cut();
            for (int i = this.Count - 1; i >= 0; i--)
            {
                copy.Add(this[i]);
            }

            return copy;
        }

        public string ToString(List<Vertex> perimeter) // public string ToString(List<Vertex> perimeter) => $"Cut: {String.Join(" ",this.Select(v=>$"{perimeter.IndexOf(v)}: {v.value}"))}";
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
