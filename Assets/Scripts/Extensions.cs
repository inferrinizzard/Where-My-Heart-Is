using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class Extensions
{
	///<summary> Same as GetComponentInChildren but does not return parent </summary>
	///<returns> Component of type <typeparamref name="T"/> or null </returns>
	///<remarks> also works on interfaces </remarks>
	public static T GetComponentOnlyInChildren<T>(this MonoBehaviour script) where T : Component
	{
		if (typeof(T).IsInterface || typeof(T).IsSubclassOf(typeof(Component)) || typeof(T) == typeof(Component))
			foreach (Transform child in script.transform)
			{
				var component = child.GetComponentInChildren<T>();
				if (component != null)
					return component;
			}
		return default(T);
	}

	///<summary> Same as GetComponentsInChildren but does not return parent </summary>
	///<returns> Array of Component type <typeparamref name="T"/> or null </returns>
	///<remarks> also works on interfaces </remarks>
	public static T[] GetComponentsOnlyInChildren<T>(this MonoBehaviour script) where T : Component
	{
		List<T> group = new List<T>();
		if (typeof(T).IsInterface || typeof(T).IsSubclassOf(typeof(Component)) || typeof(T) == typeof(Component))
			foreach (Transform child in script.transform)
				group.AddRange(child.GetComponentsInChildren<T>());
		return group.ToArray();
	}

	public static dynamic Enable(this MonoBehaviour @this)
	{
		@this.enabled = true;
		return @this;
	}

	public static void Print(this MonoBehaviour @this, params object[] args) => UnityEngine.Debug.Log(string.Join(" ", args));
	public static dynamic Log(this Object @this)
	{
		Debug.Log(@this);
		return @this;
	}
	public static string AsString<T>(this IEnumerable<T> args) => $"[{string.Join(", ", args)}]";
	public static string AsString<T>(this T[] args) => $"[{string.Join(", ", args)}]";

	public static bool TryComponent<T>(this MonoBehaviour @this, out T c) where T : Component => @this.TryGetComponent(out c);
	public static bool TryComponent<T>(this MonoBehaviour @this) where T : Component => @this.TryGetComponent(out T c);
	public static bool TryComponent<T>(this GameObject @this, out T c) where T : Component => @this.TryGetComponent(out c);
	public static bool TryComponent<T>(this GameObject @this) where T : Component => @this.TryGetComponent(out T c);
	public static bool TryComponent<T>(this Component @this, out T c) where T : Component => @this.TryGetComponent(out c);
	public static bool TryComponent<T>(this Component @this) where T : Component => @this.TryGetComponent(out T c);
	public static bool TryComponent<T>(this Transform @this, out T c) where T : Component => @this.TryGetComponent(out c);
	public static bool TryComponent<T>(this Transform @this) where T : Component => @this.TryGetComponent(out T c);

	public static void DrawCube(this MonoBehaviour @this, Vector3 pos, float size = 1, Quaternion rot = default(Quaternion), Color colour = default(Color), float duration = 0, bool depthCheck = false)
	{
		Debug.DrawLine(pos + rot * new Vector3(size / 2, size / 2, size / 2), pos + rot * new Vector3(-size / 2, size / 2, size / 2), colour, duration, depthCheck);
		Debug.DrawLine(pos + rot * new Vector3(size / 2, size / 2, size / 2), pos + rot * new Vector3(size / 2, -size / 2, size / 2), colour, duration, depthCheck);
		Debug.DrawLine(pos + rot * new Vector3(size / 2, size / 2, size / 2), pos + rot * new Vector3(size / 2, size / 2, -size / 2), colour, duration, depthCheck);
		Debug.DrawLine(pos + rot * new Vector3(-size / 2, size / 2, size / 2), pos + rot * new Vector3(-size / 2, -size / 2, size / 2), colour, duration, depthCheck);
		Debug.DrawLine(pos + rot * new Vector3(size / 2, -size / 2, size / 2), pos + rot * new Vector3(size / 2, -size / 2, -size / 2), colour, duration, depthCheck);
		Debug.DrawLine(pos + rot * new Vector3(size / 2, size / 2, -size / 2), pos + rot * new Vector3(-size / 2, size / 2, -size / 2), colour, duration, depthCheck);
		Debug.DrawLine(pos + rot * new Vector3(-size / 2, size / 2, size / 2), pos + rot * new Vector3(-size / 2, size / 2, -size / 2), colour, duration, depthCheck);
		Debug.DrawLine(pos + rot * new Vector3(size / 2, -size / 2, size / 2), pos + rot * new Vector3(-size / 2, -size / 2, size / 2), colour, duration, depthCheck);
		Debug.DrawLine(pos + rot * new Vector3(size / 2, size / 2, -size / 2), pos + rot * new Vector3(size / 2, -size / 2, -size / 2), colour, duration, depthCheck);
		Debug.DrawLine(pos + rot * new Vector3(-size / 2, -size / 2, size / 2), pos + rot * new Vector3(-size / 2, -size / 2, -size / 2), colour, duration, depthCheck);
		Debug.DrawLine(pos + rot * new Vector3(size / 2, -size / 2, -size / 2), pos + rot * new Vector3(-size / 2, -size / 2, -size / 2), colour, duration, depthCheck);
		Debug.DrawLine(pos + rot * new Vector3(-size / 2, size / 2, -size / 2), pos + rot * new Vector3(-size / 2, -size / 2, -size / 2), colour, duration, depthCheck);
	}

	public static Matrix4x4 TRS(this Transform @this) => Matrix4x4.TRS(@this.position, @this.rotation, @this.localScale);
}
