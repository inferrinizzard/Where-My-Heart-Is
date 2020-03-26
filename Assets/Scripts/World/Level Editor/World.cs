using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class World : MonoBehaviour
{
	public static World Instance;

	public Transform realWorldContainer;
	public Transform dreamWorldContainer;
	public Transform entangledWorldContainer;

	public void Awake()
	{
		Instance = this;
	}

	public void Start()
	{
		realWorldContainer = transform.Find("Real World");
		dreamWorldContainer = transform.Find("Dream World");
		entangledWorldContainer = GetComponentInChildren<EntangledObjectManager>().transform;

		ConfigureWorld("Real", realWorldContainer);
		ConfigureWorld("Dream", dreamWorldContainer);
	}

	/*public void Initialize()
	{
	    realWorldContainer = transform.Find("Real World");
	    dreamWorldContainer = transform.Find("Dream World");
	    entangledWorldContainer = GetComponentInChildren<EntangledObjectManager>().transform;

	    ConfigureWorld("Real", realWorldContainer);
	    ConfigureWorld("Dream", dreamWorldContainer);
	}*/

	/*/// <summary> configures children and related clippables, interactables </summary>
	public void OnBeginTransition()
	{
        Initialize();
	}*/

	/*/// <summary> removes refs and deletes current to pass singleton to next world </summary>
	public void OnCompleteTransition()
	{
		realWorldContainer = null;
		dreamWorldContainer = null;
		entangledWorldContainer = null;
		instance = null;
		Destroy(gameObject);
	}*/

	private void ConfigureWorld(string layer, Transform worldContainer)
	{
		foreach (Transform child in worldContainer.transform)
		{
			// Debug.Log(child);
			if (child.GetComponent<MeshFilter>())
			{
				child.gameObject.layer = LayerMask.NameToLayer(layer);
				if (!child.GetComponent<ClippableObject>())
				{
					child.gameObject.AddComponent<ClippableObject>();
				}

				if (layer == "Real")
					child.GetComponent<MeshRenderer>().material.SetInt("_Dissolve", 1);
			}

			ConfigureWorld(layer, child); // do this recursively to hit everything in the given world
		}
	}

	public void ResetCut()
	{
		foreach (Transform child in realWorldContainer)
		{
			foreach (ClippableObject obj in child.GetComponentsInChildren<ClippableObject>())
			{
				if (obj.isClipped) obj.Revert();
			}
		}

		foreach (Transform child in dreamWorldContainer)
		{
			foreach (ClippableObject obj in child.GetComponentsInChildren<ClippableObject>())
			{
				if (obj.isClipped) obj.Revert();
			}
		}

		foreach (Transform child in entangledWorldContainer)
		{
			foreach (ClippableObject obj in child.GetComponentsInChildren<ClippableObject>())
			{
				if (obj.isClipped) obj.Revert();
			}
		}
	}

	public ClippableObject[] GetRealObjects()
	{
		return realWorldContainer.GetComponentsInChildren<ClippableObject>();
	}

	public ClippableObject[] GetDreamObjects()
	{
		return dreamWorldContainer.GetComponentsInChildren<ClippableObject>();
	}

	public ClippableObject[] GetEntangledObjects()
	{
		return entangledWorldContainer.GetComponentsInChildren<EntangledClippable>();
	}
}
