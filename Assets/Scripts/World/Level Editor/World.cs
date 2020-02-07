using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : Singleton<World>, IResetable
{
	public Transform realWorldContainer;
	public Transform dreamWorldContainer;
	public Transform entangledWorldContainer;

	public void Start()
	{
		Init();
	}

	/// <summary> configures children and related clipables, interactables </summary>
	public void Init()
	{
		realWorldContainer = transform.Find("Real World");
		dreamWorldContainer = transform.Find("Dream World");
		entangledWorldContainer = GetComponentInChildren<EntangledObjectManager>().transform;

		ConfigureWorld("Real", realWorldContainer);
		ConfigureWorld("Dream", dreamWorldContainer);
	}

	/// <summary> removes refs and deletes current to pass singleton to next world </summary>
	public void Reset()
	{
		realWorldContainer = null;
		dreamWorldContainer = null;
		entangledWorldContainer = null;
		instance = null;
		Destroy(gameObject);
	}

	private void ConfigureWorld(string layer, Transform worldContainer)
	{
		foreach (Transform child in worldContainer.transform)
		{
			if (child.GetComponent<MeshFilter>())
			{
				child.gameObject.layer = LayerMask.NameToLayer(layer);
				if (child.GetComponent<ClipableObject>() == null)
				{
					child.gameObject.AddComponent<ClipableObject>();
				}
			}

			ConfigureWorld(layer, child); // do this recursively to hit everything in the given world
		}
	}

	public void ResetCut()
	{
		foreach (Transform child in realWorldContainer)
		{
			foreach (ClipableObject obj in child.GetComponentsInChildren<ClipableObject>())
			{
				if (obj.isClipped)obj.Revert();
			}
		}

		foreach (Transform child in dreamWorldContainer)
		{
			foreach (ClipableObject obj in child.GetComponentsInChildren<ClipableObject>())
			{
				if (obj.isClipped)obj.Revert();
			}
		}

		foreach (Transform child in entangledWorldContainer)
		{
			foreach (ClipableObject obj in child.GetComponentsInChildren<EntangledClipable>())
			{
				if (obj.isClipped)obj.Revert();
			}
		}
	}

	public ClipableObject[] GetRealObjects()
	{
		return realWorldContainer.GetComponentsInChildren<ClipableObject>();
	}

	public ClipableObject[] GetDreamObjects()
	{
		return dreamWorldContainer.GetComponentsInChildren<ClipableObject>();
	}

	public ClipableObject[] GetEntangledObjects()
	{
		return entangledWorldContainer.GetComponentsInChildren<EntangledClipable>();
	}
}
