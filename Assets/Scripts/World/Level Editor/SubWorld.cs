using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[ExecuteInEditMode]
public class SubWorld : MonoBehaviour
{
#if UNITY_EDITOR
	void AddMeshCollider(Transform t)
	{
		if (t.GetComponent<MeshFilter>())
		{
			if (!t.GetComponent<MeshCollider>())
				t.gameObject.AddComponent<MeshCollider>();
			while (t.GetComponents<MeshCollider>().Length > 1)
				DestroyImmediate(t.GetComponent<MeshCollider>());
		}
		else if (t.GetComponent<MeshCollider>())
			DestroyImmediate(t.GetComponent<MeshCollider>());
		foreach (Transform child in t)
			AddMeshCollider(child);
	}

	void OnTransformChildrenChanged()
	{
		if (Application.isPlaying) return;
		foreach (Transform child in transform)
			AddMeshCollider(child);
	}

	void Start()
	{
		if (Application.isPlaying)
			Destroy(this);
	}
#endif
}
