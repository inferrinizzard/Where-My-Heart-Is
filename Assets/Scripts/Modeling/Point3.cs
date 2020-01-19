using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSG
{
    /// <summary>
    /// A nullable wrapper for Vector3
    /// </summary>
    public class Point3
    {
        public Vector3 value;

        /// <summary>
        /// Creates a Point3 wrapper for the given Vector3
        /// </summary>
        /// <param name="value">The Vector3 to wrap</param>
        public Point3(Vector3 value)
        {
            this.value = value;
        }
    }
}


