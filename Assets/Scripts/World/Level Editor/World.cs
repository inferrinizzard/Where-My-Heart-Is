using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class World : MonoBehaviour
{
	public static World Instance;

	public Transform heartWorldContainer;
	public Transform realWorldContainer;
	public Transform entangledWorldContainer;

	public void Awake()
	{
		Instance = this;
	}

	public void Start()
	{
		heartWorldContainer = transform.Find("Heart World");
		realWorldContainer = transform.Find("Real World");
		entangledWorldContainer = GetComponentInChildren<EntangledObjectManager>().transform;

		ConfigureWorld("Heart", heartWorldContainer);
		ConfigureWorld("Real", realWorldContainer);
	}

	/*public void Initialize()
	{
	    heartWorldContainer = transform.Find("Heart World");
	    realWorldContainer = transform.Find("Real World");
	    entangledWorldContainer = GetComponentInChildren<EntangledObjectManager>().transform;

	    ConfigureWorld("Heart", heartWorldContainer);
	    ConfigureWorld("Real", realWorldContainer);
	}*/

	/*/// <summary> configures children and related clippables, interactables </summary>
	public void OnBeginTransition()
	{
        Initialize();
	}*/

	/*/// <summary> removes refs and deletes current to pass singleton to next world </summary>
	public void OnCompleteTransition()
	{
		heartWorldContainer = null;
		realWorldContainer = null;
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

				if (layer == "Heart")
					child.GetComponent<MeshRenderer>().material.SetInt("_Dissolve", 1);
			}

			ConfigureWorld(layer, child); // do this recursively to hit everything in the given world
		}
	}

	public void ResetCut()
	{
		foreach (Transform child in heartWorldContainer)
		{
			foreach (ClippableObject obj in child.GetComponentsInChildren<ClippableObject>())
			{
				if (obj.isClipped) obj.Revert();
			}
		}

		foreach (Transform child in realWorldContainer)
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

	public ClippableObject[] GetHeartObjects()
	{
		return heartWorldContainer.GetComponentsInChildren<ClippableObject>();
	}

	public ClippableObject[] GetRealObjects()
	{
		return realWorldContainer.GetComponentsInChildren<ClippableObject>();
	}

	public ClippableObject[] GetEntangledObjects()
	{
		return entangledWorldContainer.GetComponentsInChildren<EntangledClippable>();
	}
}
