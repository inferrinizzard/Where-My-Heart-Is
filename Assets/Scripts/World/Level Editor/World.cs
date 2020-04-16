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
		entangledWorldContainer = GetComponentInChildren<EntangledObjectManager>().transform; // TODO: decouple EntangledObjectManager

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
		foreach (MeshFilter mf in worldContainer.GetComponentsInChildren<MeshFilter>())
		{
			mf.gameObject.layer = LayerMask.NameToLayer(layer);
			if (!mf.TryComponent(out MeshRenderer mr)) mr = mf.gameObject.AddComponent<MeshRenderer>();
			if (!mf.TryComponent<MeshCollider>()) mf.gameObject.AddComponent<MeshCollider>();
			if (!mf.TryComponent<ClippableObject>()) mf.gameObject.AddComponent<ClippableObject>();

			if (layer == "Heart")
				mr.material.SetInt("_Dissolve", 1);
		}

		// foreach (Transform child in worldContainer.transform)
		// {
		// 	// Debug.Log(child);
		// 	if (child.TryComponent<MeshFilter>())
		// 	{
		// 		child.gameObject.layer = LayerMask.NameToLayer(layer);
		// 		if (!child.TryComponent(out MeshRenderer mr)) mr = child.gameObject.AddComponent<MeshRenderer>();
		// 		if (!child.TryComponent<MeshCollider>()) child.gameObject.AddComponent<MeshCollider>();
		// 		if (!child.TryComponent<ClippableObject>()) child.gameObject.AddComponent<ClippableObject>();

		// 		if (layer == "Heart")
		// 			mr.material.SetInt("_Dissolve", 1);
		// 	}

		// 	ConfigureWorld(layer, child); // do this recursively to hit everything in the given world
		// }
	}

	public void ResetCut()
	{
		foreach (ClippableObject obj in GetComponentsInChildren<ClippableObject>())
			if (obj.isClipped) obj.Revert();

		// foreach (Transform child in heartWorldContainer)
		// 	foreach (ClippableObject obj in child.GetComponentsInChildren<ClippableObject>())
		// 		if (obj.isClipped) obj.Revert();

		// foreach (Transform child in realWorldContainer)
		// 	foreach (ClippableObject obj in child.GetComponentsInChildren<ClippableObject>())
		// 		if (obj.isClipped) obj.Revert();

		// foreach (Transform child in entangledWorldContainer)
		// 	foreach (ClippableObject obj in child.GetComponentsInChildren<ClippableObject>())
		// 		if (obj.isClipped) obj.Revert();
	}

	public ClippableObject[] GetHeartObjects()
	{
		return heartWorldContainer.GetComponentsInChildren<ClippableObject>(); // TODO: do these ever change?
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
