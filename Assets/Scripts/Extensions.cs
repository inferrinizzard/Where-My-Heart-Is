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
}
