using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestImageEffect : MonoBehaviour
{
    public Material maskMaterial;
    public RenderTexture replacement;
    public bool replace;

    public Camera spaghettiHelper;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        

        if(replace)
        {
            Graphics.Blit(replacement, destination);
            /*replacement.width = Camera.main.pixelWidth;
            replacement.height = Camera.main.pixelHeight;*/
            RenderTextureDescriptor replacementDescriptor = replacement.descriptor;
            replacementDescriptor.width = Camera.main.pixelWidth;
            replacementDescriptor.height = Camera.main.pixelHeight;
            RenderTexture doubleReplacement = new RenderTexture(replacementDescriptor);
            replacement = doubleReplacement;
            spaghettiHelper.targetTexture = replacement;
        }
        else
        {
            /*RenderTextureDescriptor maskDescriptor = new RenderTextureDescriptor();
            maskDescriptor.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.A10R10G10B10_XRSRGBPack32;
            RenderTexture mask = new RenderTexture(maskDescriptor);*/
            Graphics.Blit(source, destination, maskMaterial);
        }
        /*if(shader != null)
        {
            GetComponent<Camera>().SetReplacementShader(shader, "RenderType");
        }*/
    }
}  
