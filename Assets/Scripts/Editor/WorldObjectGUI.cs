using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(WorldObject))]
public class WorldObjectGUI : Editor
{
	string yes = "✔";

	string no = "✘";

	WorldObject obj = null;

	void OnEnable() => obj = (WorldObject) target;

	string Is(bool status) => status ? yes : no;
	public override void OnInspectorGUI()
	{
		GUILayout.BeginHorizontal();
		// SummonButton("Interactable", obj.interactable, obj.ToggleInteractable);
		SummonButton("Clippable", obj.clippable, obj.ToggleClippable);
		SummonButton("Pickupable", obj.pickupable, obj.TogglePickupable);
		SummonButton("Collectable", obj.collectable, obj.ToggleCollectable);
		GUILayout.EndHorizontal();
	}

	void SummonButton(string label, bool flag, System.Action effect)
	{
		Color temp = GUI.backgroundColor;
		if (flag)
			GUI.backgroundColor = Color.green;
		if (GUILayout.Button($"{label} {Is(flag)}"))
			effect.Invoke();
		GUI.backgroundColor = temp;
	}

}
