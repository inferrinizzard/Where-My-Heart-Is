using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
	public Transform realWorldContainer;
	public Transform dreamWorldContainer;
	public Transform entangledWorldContainer;
	public PlayerMovement player; // TODO: phase out by using player object
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
		// preserving certain changes selectively must be done here

		// delete previous copy set
		//TODO: all changes made to the state of these objects done during the cut should be 
		// applied to their source objects before the cut versions are removed

		/*foreach (GameObject obj in realObjects)
		{
		    Destroy(obj);
		}
		foreach (GameObject obj in dreamObjects)
		{
		    Destroy(obj);
		}
		foreach (GameObject obj in entangledObjects)
		{
		    Destroy(obj);
		}
		realObjects = new List<GameObject>();
		dreamObjects = new List<GameObject>();
		entangledObjects = new List<GameObject>();*/

		// get top level children for each world

		foreach (Transform child in realWorldContainer)
		{
			foreach (ClipableObject obj in child.GetComponentsInChildren<ClipableObject>())
			{
				obj.Revert();
			}
		}

		foreach (Transform child in dreamWorldContainer)
		{
			foreach (ClipableObject obj in child.GetComponentsInChildren<ClipableObject>())
			{
				obj.Revert();
			}
		}

		foreach (Transform child in entangledWorldContainer)
		{
			foreach (EntangledClipable obj in child.GetComponentsInChildren<EntangledClipable>())
			{
				obj.Revert();
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
