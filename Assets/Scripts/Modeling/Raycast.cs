using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSG
{
    public static class Raycast
    {
        /// <summary>
        /// Raycast to a line segment defined by two points
        /// </summary>
        /// <param name="origin">The start point for the raycast</param>
        /// <param name="direction">The direction to cast in</param>
        /// <param name="pointA">Half of the definition for the line segment</param>
        /// <param name="pointB">The other half of the defination of the line segment</param>
        /// <returns>The location where the ray and the line segment meet, or null if no intersection is found</returns>
        public static Point3 ToLineSegment(Vector3 origin, Vector3 direction, Vector3 pointA, Vector3 pointB)
        {
            Vector3 edgeDirection = (pointA - pointB).normalized;

            //TODO: unhardcode breakpoint
            // if the cast and the edge are parallel, there will be no intersection
            if (Vector3.Cross(direction, edgeDirection).magnitude < 0.0001)
            {
                return null;
            }

            float u = (pointA.x - origin.x - (direction.x / direction.y) * (pointA.y - origin.y)) /
                (((direction.x / direction.y) * edgeDirection.y) - edgeDirection.x);

            // sometimes we align with an axis and create NaN, so try a different axis if this happens
            if (float.IsNaN(u))
            {
                u = (pointA.x - origin.x - (direction.x / direction.z) * (pointA.z - origin.z)) /
                (((direction.x / direction.z) * edgeDirection.z) - edgeDirection.x);
            }

            Vector3 intersectionPoint = pointA + (edgeDirection * u);
            if (Vector3.Distance(pointA, intersectionPoint) + Vector3.Distance(intersectionPoint, pointB) - Vector3.Distance(pointA, pointB) < 0.0001)
            {
                return new Point3(intersectionPoint);
            }
            else return null;
        }
    }
}


