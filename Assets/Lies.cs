using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lies : MonoBehaviour
{
    public List<GameObject> otherThing;
    public GameObject otherOtherThing;
    public GameObject otherThingParent;

    public Material fromReal;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            GameObject copy = Instantiate(gameObject);
            copy.GetComponentInChildren<BoxCollider>().enabled = false;
            gameObject.layer = 9;
            GetComponent<MeshRenderer>().material = fromReal;
            foreach(GameObject thing in otherThing)
            {
                thing.GetComponent<MeshRenderer>().material = fromReal;
                thing.layer = 9;
            }
            otherOtherThing.layer = 12;
        }
    }
}
