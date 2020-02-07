using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	public Transform realWorldContainer;
	public Transform dreamWorldContainer;
	public Transform entangledWorldContainer;
	public Player player; // TODO: phase out by using player object
	public static GameObject playerReference;

	void Awake()
	{
		realWorldContainer = transform.Find("Real World");
		dreamWorldContainer = transform.Find("Dream World");

		ConfigureWorld("Real", realWorldContainer);
		ConfigureWorld("Dream", dreamWorldContainer);
		ConfigureInteractables(transform);
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

	void ConfigureInteractables(Transform parent)
	{
		foreach (Transform child in parent)
		{
			if (child.childCount > 0)
				ConfigureInteractables(child);
			var childInteractable = child.GetComponent<InteractableObject>();
			if (childInteractable != null)
				childInteractable.player = player;
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
			foreach (ClipableObject obj in child.GetComponentsInChildren<ClipableObject>())
			{
				if (obj.isClipped) obj.Revert();
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
