using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public Transform realWorldContainer;
    public Transform dreamWorldContainer;

    // Start is called before the first frame update
    void Start()
    {
        ConfigureWorld("Real", realWorldContainer);
        ConfigureWorld("Dream", dreamWorldContainer);
    }

    private void ConfigureWorld(string layer, Transform worldContainer)
    {
        for (int i = 0; i < worldContainer.childCount; i++)
        {
            if (worldContainer.GetChild(i).GetComponent<MeshFilter>())
            {
                worldContainer.GetChild(i).gameObject.layer = LayerMask.NameToLayer(layer);
                worldContainer.GetChild(i).gameObject.AddComponent<ClipableObject>();
            }
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
