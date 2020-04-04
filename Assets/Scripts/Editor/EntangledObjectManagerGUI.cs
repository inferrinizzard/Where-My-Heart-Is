using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(EntangledObjectManager))]
public class EntangledObjectManagerGUI : Editor
{

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		GUILayout.BeginVertical();
		if (GUILayout.Button("Add Shared Object"))
		{
			((EntangledObjectManager) target).AddObject();
		}
		GUILayout.EndVertical();
	}

}
