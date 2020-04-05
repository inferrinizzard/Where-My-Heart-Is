using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    /*private Transform cam;
    private Transform player;*/
    public GameObject reflectionPlane;
    public Camera mainCamera;
    public Material mirrorMaterial;

    private Camera reflectionCamera;

    private RenderTexture renderTarget;

    private Matrix4x4 initialProjection;


    // Start is called before the first frame update
    void Start()
    {
        reflectionCamera = new GameObject("ReflectionCamera").AddComponent<Camera>();
        reflectionCamera.enabled = false;

        renderTarget = new RenderTexture(Screen.width, Screen.height, 24);
    }

    void OnPreRender()
    {
        RenderReflection();
    }

    private void RenderReflection()
    {
        //TODO: only call when mirror is visable

        reflectionCamera.CopyFrom(mainCamera);
        Transform mainTransform = mainCamera.transform;

        //TODO: base this on normal vector instead of assumption that normal is in positive local y
        Matrix4x4 mirrorMatrix = reflectionPlane.transform.localToWorldMatrix * Matrix4x4.Scale(new Vector3(1, -1, 1)) * reflectionPlane.transform.worldToLocalMatrix;

        reflectionCamera.transform.position = mirrorMatrix.MultiplyPoint(mainTransform.position);
        reflectionCamera.transform.LookAt(reflectionCamera.transform.position + mirrorMatrix.MultiplyVector(mainTransform.forward), mirrorMatrix.MultiplyVector(mainTransform.up));

        reflectionCamera.targetTexture = renderTarget;

        // Calculate clip plane for portal (for culling of objects in-between destination camera and portal)
        Vector3 normal = reflectionPlane.transform.up;
        Vector4 clipPlaneWorldSpace =
            new Vector4(
                normal.x,
                normal.y,
                normal.z,
                Vector3.Dot(reflectionPlane.transform.position, -normal));

        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(reflectionCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;
        reflectionCamera.projectionMatrix = reflectionCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);

        reflectionCamera.Render();
        mirrorMaterial.SetTexture("_ReflectionTex", renderTarget);
    }
}
