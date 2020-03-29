using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(EntangledClippable))]
public class EntangledClippableGUI : Editor
{
	Object heartPrefab;
	Object realPrefab;
	EntangledClippable entangledObject;

	private void OnEnable()
	{
		entangledObject = (EntangledClippable) target;
		heartPrefab = entangledObject.heartObject;
		realPrefab = entangledObject.realObject;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		EditorGUI.BeginChangeCheck();
		heartPrefab = EditorGUILayout.ObjectField("Heart Prefab", heartPrefab, typeof(GameObject), true);
		if (EditorGUI.EndChangeCheck())
		{
			entangledObject.OnRealChange((GameObject) heartPrefab);
		}

		EditorGUI.BeginChangeCheck();
		realPrefab = EditorGUILayout.ObjectField("Real Prefab", realPrefab, typeof(GameObject), true);
		if (EditorGUI.EndChangeCheck())
		{
			entangledObject.OnRealChange((GameObject) realPrefab);
		}

	}

	/*	
	public void OnValidate()	
	{	
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
	        realVersion = createdObject.AddComponent<ClippableObject>();	
	        previousRealPrefab = realPrefab;	
	    }	
	    if (heartPrefab != null && heartPrefab != previousRealPrefab)	
	    {	
	        GameObject createdObject;	
	        if (heartVersion != null)	
	        {	
	            createdObject = Instantiate(heartPrefab, heartVersion.transform.position, heartVersion.transform.rotation, transform);	
	            UnityEditor.EditorApplication.delayCall += () => DestroyImmediate(heartVersion.gameObject);	
	        }	
	        else	
	        {	
	            createdObject = Instantiate(heartPrefab, transform);	
	        }	
	        heartVersion = createdObject.AddComponent<ClippableObject>();	
	        previousRealPrefab = heartPrefab;	
	    }	
	}*/
}
