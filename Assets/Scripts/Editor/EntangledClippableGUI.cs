using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(EntangledClippable))]
public class EntangledClippableGUI : Editor
{
	Object realPrefab;
	Object dreamPrefab;
	EntangledClippable entangledObject;

	private void OnEnable()
	{
		entangledObject = (EntangledClippable) target;
		realPrefab = entangledObject.realObject;
		dreamPrefab = entangledObject.dreamObject;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		EditorGUI.BeginChangeCheck();
		realPrefab = EditorGUILayout.ObjectField("Real Prefab", realPrefab, typeof(GameObject), true);
		if (EditorGUI.EndChangeCheck())
		{
			entangledObject.OnRealChange((GameObject) realPrefab);
		}

		EditorGUI.BeginChangeCheck();
		dreamPrefab = EditorGUILayout.ObjectField("Dream Prefab", dreamPrefab, typeof(GameObject), true);
		if (EditorGUI.EndChangeCheck())
		{
			entangledObject.OnDreamChange((GameObject) dreamPrefab);
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
	        dreamVersion = createdObject.AddComponent<ClippableObject>();	
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
	        realVersion = createdObject.AddComponent<ClippableObject>();	
	        previousRealPrefab = realPrefab;	
	    }	
	}*/
}
