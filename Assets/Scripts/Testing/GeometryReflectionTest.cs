using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometryReflectionTest : MonoBehaviour
{
    public Mirror mirror;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("reflect");
            CSG.Model model = new CSG.Model(GetComponent<MeshFilter>().mesh);
            model.ApplyTransformation(mirror.reflectionMatrix * transform.localToWorldMatrix);
            model.FlipNormals();

            transform.position = mirror.reflectionMatrix.MultiplyPoint(transform.position);
            GetComponent<MeshFilter>().mesh = model.ToMesh(transform.worldToLocalMatrix);
            mirror.gameObject.SetActive(false);
        }
    }
}
