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

		public Cut GetReversedCopy()
		{
			// AsEnumerable returns a shallow copy of the list
			//return (Cut)this.AsEnumerable().Reverse().ToList();

			Cut copy = new Cut();
			for (int i = this.Count - 1; i >= 0; i--)
			{
				copy.Add(this[i]);
			}

			return copy;
		}

		public string ToString(List<Vertex> perimeter) => $"Cut: {string.Join(" ",this.Select(v=>$"{perimeter.IndexOf(v)}: {v.value}"))}";
	}

}
