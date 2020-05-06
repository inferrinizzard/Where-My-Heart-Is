using UnityEditor;

using UnityEngine;

[ExecuteInEditMode]
public class LevelInABox : MonoBehaviour
{
#if UNITY_EDITOR
	void OnEnable()
	{
		PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
		while (transform.childCount > 0)
			transform.GetChild(0).parent = null;
	}
	void OnTransformChildrenChanged()
	{
		if (transform.childCount == 0)
			DestroyImmediate(gameObject);
	}
#endif
}
