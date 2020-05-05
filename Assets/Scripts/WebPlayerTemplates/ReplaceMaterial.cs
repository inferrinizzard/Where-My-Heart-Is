using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class ReplaceMaterial : MonoBehaviour
{
    public bool replace;
    public Material toReplace;
    public Material newMaterial;

    // Update is called once per frame
    void Update()
    {
        if(replace)
        {
            replace = false;
            FindObjectsOfType<Renderer>().ToList().ForEach(renderer =>
            {
                if(renderer.sharedMaterial == toReplace)
                {
                    renderer.sharedMaterial = newMaterial;
                }
            });
        }
    }
}
