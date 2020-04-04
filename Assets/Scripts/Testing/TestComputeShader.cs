using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestComputeShader : MonoBehaviour
{
    public ComputeShader compute;

    public RenderTexture result;

    // Start is called before the first frame update
    void Start()
    {
        int kernel = compute.FindKernel("CSMain");

        result = new RenderTexture(512, 512, 24);

        result.enableRandomWrite = true;

        result.Create();

        compute.SetTexture(kernel, "Result", result);
        //compute.SetFloats();
        compute.SetVector("color", Color.red);
        compute.Dispatch(kernel, 512 / 8, 512 / 8, 1);
    }
}
