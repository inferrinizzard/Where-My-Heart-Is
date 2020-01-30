using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EntangledClipable))]
public class EntangledClipableGUI : Editor
{
	Object realPrefab;
	Object dreamPrefab;
	private void Awake()
	{
		realPrefab = null;
		dreamPrefab = null;
	}

	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI();
		EditorGUI.BeginChangeCheck();
		realPrefab = EditorGUILayout.ObjectField("Real Prefab", realPrefab, typeof(GameObject), true);
		if (EditorGUI.EndChangeCheck())
		{
			((EntangledClipable)target).OnRealChange((GameObject)realPrefab);
		}

		EditorGUI.BeginChangeCheck();
		dreamPrefab = EditorGUILayout.ObjectField("Dream Prefab", dreamPrefab, typeof(GameObject), true);
		if (EditorGUI.EndChangeCheck())
		{
			((EntangledClipable)target).OnDreamChange((GameObject)dreamPrefab);
		}

	}

	/*
	public void OnValidate()
	{
	    if (dreamPrefab != null && dreamPrefab != previousDreamPrefab)
	    {
	        GameObject createdObject;
	        if (dreamVersion != null)
	        {
	            createdObject = Instantiate(dreamPrefab, dreamVersion.transform.position, dreamVersion.transform.rotation, transform);
	            UnityEditor.EditorApplication.delayCall += () => DestroyImmediate(dreamVersion.gameObject);
	        }
	        else
	        {
	            createdObject = Instantiate(dreamPrefab, transform);
	        }
	        dreamVersion = createdObject.AddComponent<ClipableObject>();
	        previousDreamPrefab = dreamPrefab;
	    }

	    if (realPrefab != null && realPrefab != previousRealPrefab)
	    {
	        GameObject createdObject;
	        if (realVersion != null)
	        {
	            createdObject = Instantiate(realPrefab, realVersion.transform.position, realVersion.transform.rotation, transform);
	            UnityEditor.EditorApplication.delayCall += () => DestroyImmediate(realVersion.gameObject);
	        }
	        else
	        {
	            createdObject = Instantiate(realPrefab, transform);
	        }
	        realVersion = createdObject.AddComponent<ClipableObject>();
	        previousRealPrefab = realPrefab;
	    }
	}*/
}
