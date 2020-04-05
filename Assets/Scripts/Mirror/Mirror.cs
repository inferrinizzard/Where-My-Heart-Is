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
		//mainCamera = Camera.main;

		renderTarget = new RenderTexture(Screen.width, Screen.height, 24);

		Vector3 normal = reflectionPlane.transform.up;
		reflectionCamera.projectionMatrix = reflectionCamera.CalculateObliqueMatrix(new Vector4(normal.x, normal.y, normal.z, reflectionPlane.transform.position.y));

		/*for(int i = 0; i < 100; i++)
		{
		    normal = reflectionPlane.transform.up;
		    reflectionCamera.projectionMatrix = reflectionCamera.CalculateObliqueMatrix(new Vector4(normal.x, normal.y, normal.z, reflectionPlane.transform.position.y));
		}*/
	}

	// Update is called once per frame
	void Update()
	{
		//RenderReflection();

		/*Vector3 directionToPlayer = (player.position - transform.position).normalized;
		Quaternion lookAtPlayer = Quaternion.LookRotation(directionToPlayer);

		lookAtPlayer.eulerAngles = transform.eulerAngles - lookAtPlayer.eulerAngles + new Vector3(0, 180, -90);

		cam.rotation = lookAtPlayer;


		Matrix4x4 mirrorPosition = transform.localToWorldMatrix * Matrix4x4.Scale(new Vector3(1, 1, -1)) * transform.worldToLocalMatrix;
		cam.position = mirrorPosition.MultiplyPoint(player.position);*/
	}

	void OnPreRender()
	{
		RenderReflection();
	}

	private void RenderReflection()
	{
		//TODO: only call when mirror is visable → put on mirror and call with OnWillRenderObject and check if facing right direction

		reflectionCamera.CopyFrom(mainCamera);
		Transform mainTransform = mainCamera.transform;

		//TODO: base this on normal vector instead of assumption that normal is in positive local y
		Matrix4x4 mirrorMatrix = reflectionPlane.transform.localToWorldMatrix * Matrix4x4.Scale(new Vector3(1, -1, 1)) * reflectionPlane.transform.worldToLocalMatrix;

		reflectionCamera.transform.position = mirrorMatrix.MultiplyPoint(mainTransform.position);
		reflectionCamera.transform.LookAt(reflectionCamera.transform.position + mirrorMatrix.MultiplyVector(mainTransform.forward), mirrorMatrix.MultiplyVector(mainTransform.up));

		reflectionCamera.targetTexture = renderTarget;

		// set custom frustum
		//Vector3 normal = reflectionPlane.transform.up;
		Vector3 intersection = CSG.Raycast.RayToPlane(reflectionCamera.transform.position, reflectionCamera.transform.forward, reflectionPlane.transform.position, reflectionPlane.transform.up);
		float distance = Vector3.Distance(reflectionPlane.transform.position, intersection);
		//intersection = reflectionPlane.transform.worldToLocalMatrix.MultiplyPoint(intersection);
		//Matrix4x4 customProjection = reflectionCamera.projectionMatrix;

		//Vector4 clipPlane = new Vector4(normal.x, normal.y, normal.z, Vector3.Distance(intersection, reflectionPlane.transform.worldToLocalMatrix.MultiplyPoint(reflectionCamera.transform.position)));

		/*CalculateObliqueMatrixOrtho(ref customProjection, clipPlane);
		reflectionCamera.projectionMatrix = customProjection;*/
		Debug.Log(distance);
		Vector3 normal = reflectionCamera.worldToCameraMatrix.MultiplyVector(reflectionPlane.transform.up);
		reflectionCamera.projectionMatrix = reflectionCamera.CalculateObliqueMatrix(new Vector4(normal.x, normal.y, normal.z, distance));
		//reflectionPlane.transform.position.y

		reflectionCamera.Render();
		mirrorMaterial.SetTexture("_ReflectionTex", renderTarget);
		//DrawQuad();
	}

	private void DrawQuad()
	{
		GL.PushMatrix();

		mirrorMaterial.SetPass(0);
		mirrorMaterial.SetTexture("_ReflectionTex", renderTarget);

		// Draw the quad itself
		GL.LoadOrtho();
		GL.Begin(GL.QUADS);
		GL.TexCoord2(1.0f, 0.0f);
		GL.Vertex3(0.0f, 0.0f, 0.0f);
		GL.TexCoord2(1.0f, 1.0f);
		GL.Vertex3(0.0f, 1.0f, 0.0f);
		GL.TexCoord2(0.0f, 1.0f);
		GL.Vertex3(1.0f, 1.0f, 0.0f);
		GL.TexCoord2(0.0f, 0.0f);
		GL.Vertex3(1.0f, 0.0f, 0.0f);
		GL.End();

		GL.PopMatrix();
	}

	// modifies projection matrix in place
	// clipPlane is in camera space
	// solution from http://aras-p.info/texts/obliqueortho.html
	private void CalculateObliqueMatrixOrtho(ref Matrix4x4 projection, Vector4 clipPlane)
	{
		Vector4 q = projection.inverse * new Vector4(
			Mathf.Sign(clipPlane.x),
			Mathf.Sign(clipPlane.y), -1.0f,
			1.0f
		);
		Vector4 c = clipPlane * (2.0F / (Vector4.Dot(clipPlane, q)));
		// third row = clip plane - fourth row
		projection[2] = c.x - projection[3];
		projection[6] = c.y - projection[7];
		projection[10] = c.z - projection[11];
		projection[14] = c.w - projection[15];
	}
}
