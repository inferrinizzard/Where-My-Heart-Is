using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EntangledClipable))]
public class EntangledClippableGUI : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        EditorGUI.BeginChangeCheck();
        Object prefab = EditorGUILayout.ObjectField("Real Prefab", target, typeof(GameObject), true);
        if (EditorGUI.EndChangeCheck())
        {
            ((EntangledClipable)target).OnRealChange((GameObject)prefab);
        }

        EditorGUI.BeginChangeCheck();
        prefab = EditorGUILayout.ObjectField("Dream Prefab", target, typeof(GameObject), true);
        if (EditorGUI.EndChangeCheck())
        {
            ((EntangledClipable)target).OnDreamChange((GameObject)prefab);
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
