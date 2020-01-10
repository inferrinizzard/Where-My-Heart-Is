using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealConfig : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ConfigureChildren()
    {
        foreach(GameObject child in transform.GetComponentsInChildren<GameObject>())
        {
            child.layer = 8;// set layer to sculpture (real world)
            child.AddComponent<ClipableObject>();
        }
    }

    public List<GameObject> GetObjects()
    {
        return new List<GameObject>(transform.GetComponentsInChildren<GameObject>());
    }

    public List<ClipableObject> GetClipableObjects()
    {
        return new List<ClipableObject>(transform.GetComponentsInChildren<ClipableObject>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
