using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SubWorld : MonoBehaviour
{
	int count = 0;

#if UNITY_EDITOR
	void Start()
	{
		count = transform.childCount;
	}

	void OnTransformChildrenChanged()
	{
		count = transform.childCount;
		foreach (Transform child in transform)
			if (child.GetComponent<WorldObject>() == null)
			{
				int numComponents = child.GetComponents<Component>().Length;
				var worldObjRef = child.gameObject.AddComponent<WorldObject>();
				for (int i = 0; i < numComponents; i++)
					UnityEditorInternal.ComponentUtility.MoveComponentUp(worldObjRef);
			}
	}
#endif
}
