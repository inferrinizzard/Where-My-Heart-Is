using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public Transform realWorldContainer;
    public Transform dreamWorldContainer;

    public PlayerMovement player;

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
                child.gameObject.AddComponent<ClipableObject>();
            }
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

    public List<ClipableObject> GetRealObjects()
    {
        return new List<ClipableObject>(realWorldContainer.GetComponentsInChildren<ClipableObject>());
    }

    public List<ClipableObject> GetDreamObjects()
    {
        return new List<ClipableObject>(dreamWorldContainer.GetComponentsInChildren<ClipableObject>());
    }
}
