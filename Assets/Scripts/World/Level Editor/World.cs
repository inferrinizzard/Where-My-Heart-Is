using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class World : MonoBehaviour
{
	public static World Instance;

	public Transform heartWorldContainer;
	public Transform realWorldContainer;
	public Transform entangledWorldContainer;

	public List<ClippableObject> heartClippables, realClippables;

    [HideInInspector] public List<EntangledClippable> entangledClippables
    {
        get
        {
            List<EntangledClippable> objs = entangledWorldContainer.GetComponentsInChildren<EntangledClippable>().ToList();

            return objs.OrderBy(clippable => (clippable.transform.position - Player.Instance.transform.position).sqrMagnitude).ToList();
        }
    }
	[HideInInspector] public List <ClippableObject> clippables
	{
		get
		{
			List <ClippableObject> objs = new List <ClippableObject>();
            objs.AddRange(realClippables);
            objs.AddRange(heartClippables);

            return objs.OrderBy(clippable => (clippable.transform.position - Player.Instance.transform.position).sqrMagnitude).ToList();
		}
	}

	public ClippableObject[] GetEntangledObjects()
	{
		return entangledWorldContainer.GetComponentsInChildren<EntangledClippable>();
	}

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

		heartClippables = heartWorldContainer.GetComponentsInChildren<ClippableObject>().ToList();
		realClippables = realWorldContainer.GetComponentsInChildren<ClippableObject>().ToList();
        foreach (EntangledClippable entangled in entangledWorldContainer.GetComponentsInChildren<EntangledClippable>().ToList())
		{
			heartClippables.AddRange(entangled.heartObject.GetComponentsInChildren<ClippableObject>());
			realClippables.AddRange(entangled.realObject.GetComponentsInChildren<ClippableObject>());
		}
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
		foreach (MeshFilter meshFilter in worldContainer.GetComponentsInChildren<MeshFilter>())
		{
			meshFilter.gameObject.layer = LayerMask.NameToLayer(layer);
			if (!meshFilter.TryComponent(out MeshRenderer meshRenderer)) meshRenderer = meshFilter.gameObject.AddComponent<MeshRenderer>();
			if (!meshFilter.TryComponent<MeshCollider>()) meshFilter.gameObject.AddComponent<MeshCollider>();
			if (!meshFilter.TryComponent<ClippableObject>()) meshFilter.gameObject.AddComponent<ClippableObject>();

            if (layer == "Heart") meshFilter.gameObject.GetComponent<ClippableObject>().worldType = ClippableObject.WorldType.Heart;
            else if (layer == "Real") meshFilter.gameObject.GetComponent<ClippableObject>().worldType = ClippableObject.WorldType.Real;
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
}
