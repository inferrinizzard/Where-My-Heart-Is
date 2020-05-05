using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace CSG
{
	/// <summary>
	/// Supplies methods for intersection and raycasting
	/// </summary>
	public static class Raycast
	{
		/// <summary>
		/// Raycast to a line segment defined by two points
		/// </summary>
		/// <remarks>http://mathforum.org/library/drmath/view/62814.html</remarks>
		/// <param name="origin">The start point for the raycast</param>
		/// <param name="direction">The direction to cast in</param>
		/// <param name="pointA">Half of the definition for the line segment</param>
		/// <param name="pointB">The other half of the defination of the line segment</param>
		/// <returns>The location where the ray and the line segment meet, or null if no intersection is found</returns>
		public static Vector3? RayToLineSegment(Vector3 origin, Vector3 direction, Vector3 pointA, Vector3 pointB)
		{
			Vector3 edgeDirection = (pointA - pointB).normalized;

			//TODO: unhardcode breakpoint
			// if the cast and the edge are parallel, there will be no intersection
			if (Vector3.Cross(direction, edgeDirection).magnitude < 0.0001)
			{
				return null;
			}
			Vector3 closestPoint = (Vector3.Dot((origin - pointA), edgeDirection) * edgeDirection) + pointA;

			float h = Vector3.Distance(closestPoint, origin) / Mathf.Cos(Vector3.Angle((closestPoint - origin).normalized, direction) * Mathf.Deg2Rad);

			Vector3 intersectionPoint = origin + (direction * h);
			if (Vector3.Distance(pointA, intersectionPoint) + Vector3.Distance(intersectionPoint, pointB) - Vector3.Distance(pointA, pointB) < 0.0001)
			{
				return intersectionPoint;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Finds and returns the intersections between each edge of a and the surface of b
		/// </summary>
		/// <param name="a">The Triangle whose edges should be cast from</param>
		/// <param name="b">The Triangle whose surface should be cast to</param>
		/// <param name="error">Equality comparison breakpoint value</param>
		/// <returns>The List of all found intersections</returns>
		public static List<Intersection> TriangleToTriangle(Triangle a, Triangle b, float error)
		{
			List<Intersection> intersections = new List<Intersection>();

			// for each edge of a, raycast to b
			for (int i = 0; i < 3; i++)
			{
				Intersection intersection = a.edges[i].IntersectWithTriangle(b);
				if (intersection != null)
				{
					intersections.Add(intersection);
				}
			}

			return intersections;
		}

		/// <summary>
		/// Finds and returns the intersection of the line segment defined by the two given points with the given triangle, or null if there is none
		/// </summary>
		/// <param name="pointA">The first point defining the line segment</param>
		/// <param name="pointB">The second point defining the line segment</param>
		/// <param name="triangle">The triangle to cast to</param>
		/// <param name="error">Equality comparison breakpoint value</param>
		/// <returns>A vertex representing the intersection if there is one, null otherwise</returns>
		public static Vertex LineSegmentToTriangle(Vector3 pointA, Vector3 pointB, Triangle triangle, float error)
		{
			Vertex raycastIntersection = RayToTriangle(pointA, pointA - pointB, triangle);
			if (raycastIntersection != null)
			{
				if (PointLiesOnLineSegment(raycastIntersection.value, pointA, pointB, error))
				{
					return raycastIntersection;
				}
			}

			return null;
		}

		/// <summary>
		/// Casts a ray to the triangle to find the point of intersection between the two
		/// </summary>
		/// <param name="origin">The origin of the ray</param>
		/// <param name="direction">The direction of the ray</param>
		/// <param name="triangle">The triangle to cast to</param>
		/// <returns>A vertex with the location of the triangle, or null if none can be found</returns>
		public static Vertex RayToTriangle(Vector3 origin, Vector3 direction, Triangle triangle)
		{
			// first, determine whether the ray intersects the triangle
			if (PointLiesOnTriangle(origin, direction, triangle))
			{
				// if the ray intersects the triangle, we can find the specific point at which it does

				// determine equation of plane
				Vector3 normal = Vector3.Cross(triangle.vertices[0].value - triangle.vertices[1].value, triangle.vertices[1].value - triangle.vertices[2].value);
				Vector3 planePoint = triangle.vertices[0].value;

				return new Vertex(0, RayToPlane(origin, direction, planePoint, normal));
			}
			else
			{
				return null;
			}
		}

        /// <summary>
        /// Casts a ray to the given plane and returns the point of intersection
        /// </summary>
        /// <param name="origin">The origin of the ray</param>
        /// <param name="direction">The direction of the ray</param>
        /// <param name="planePoint">An arbirtrary point lying on the plane</param>
        /// <param name="planeNormal">The normal vector of the plane</param>
        /// <returns>The point of intersection between the ray and the plane</returns>
        public static Vector3 RayToPlane(Vector3 origin, Vector3 direction, Vector3 planePoint, Vector3 planeNormal)
        {
            // get ray intersection with plane,
            float numerator = planeNormal.x * (planePoint.x - origin.x) + planeNormal.y * (planePoint.y - origin.y) + planeNormal.z * (planePoint.z - origin.z);
            float denominator = planeNormal.x * direction.x + planeNormal.y * direction.y + planeNormal.z * direction.z;

            return ((numerator / denominator) * direction) + origin;
        }

		/// <summary>
		/// Determines whether the given point lies on the given triangle
		/// </summary>
		/// <remarks>
		/// solution taken from https://stackoverflow.com/questions/42740765/intersection-between-line-and-triangle-in-3d
		/// </remarks>
		/// <param name="point">The point to check</param>
		/// <param name="offsetDirection">A direction that should not be parallel to the triangle</param>
		/// <param name="triangle">The triangle to check the point against</param>
		/// <returns>True if the point lies on the triangle, false otherwise</returns>
		public static bool PointLiesOnTriangle(Vector3 point, Vector3 offsetDirection, Triangle triangle)
		{
			Vector3 q1 = point + offsetDirection * 50;
			Vector3 q2 = point - offsetDirection * 50;

			return SignedVolume(q1, triangle.vertices[0].value, triangle.vertices[1].value, triangle.vertices[2].value) !=
				SignedVolume(q2, triangle.vertices[0].value, triangle.vertices[1].value, triangle.vertices[2].value) &&
				SignedVolume(q1, q2, triangle.vertices[0].value, triangle.vertices[1].value) ==
				SignedVolume(q1, q2, triangle.vertices[1].value, triangle.vertices[2].value) &&
				SignedVolume(q1, q2, triangle.vertices[1].value, triangle.vertices[2].value) ==
				SignedVolume(q1, q2, triangle.vertices[2].value, triangle.vertices[0].value);
		}

		/// <summary>
		/// Helper for RayToTriangle. Finds the signed volume of the tetrahedron defined by the four given points
		/// </summary>
		/// <param name="a">The first point of the tetrahedron</param>
		/// <param name="b">The second point of the tetrahedron</param>
		/// <param name="c">The third point of the tetrahedron</param>
		/// <param name="d">The fourth point of the tetrahedron</param>
		/// <returns></returns>
		private static int SignedVolume(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
		{
			return (int) Mathf.Sign(Vector3.Dot(Vector3.Cross(b - a, c - a), d - a));
		}

		/// <summary>
		/// Returns whether the given point lies between the line segment defined by the other two points
		/// </summary>
		/// <param name="point">The point to check</param>
		/// <param name="edgePointA">The first point defining the line segment</param>
		/// <param name="edgePointB">The second point defining the line segment</param>
		/// <param name="error">Equality comparison breakpoint value</param>
		/// <returns>True of the point lies between the two others, false if it does not</returns>
		public static bool PointLiesOnLineSegment(Vector3 point, Vector3 edgePointA, Vector3 edgePointB, float error)
		{
			float temp = Vector3.Distance(edgePointA, point) + Vector3.Distance(point, edgePointB);
			return Mathf.Abs(Vector3.Distance(edgePointA, edgePointB) - temp) < error;
		}
	}
}
