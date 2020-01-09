using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreLies : MonoBehaviour
{
    public Mesh myMesh;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            GetComponent<MeshFilter>().mesh = myMesh;
            GetComponent<MeshCollider>().sharedMesh = myMesh;

        }
    }
}
