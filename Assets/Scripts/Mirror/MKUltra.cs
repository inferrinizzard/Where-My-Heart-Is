using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[ExecuteInEditMode]
public class MKUltra : MonoBehaviour
{
	public GameObject mirror;
	Camera cam;
	Player player;
	Camera testCam;
	public Vector3 test;
	public float dist;
	public Matrix4x4 oblique;
	// Start is called before the first frame update
	void Start()
	{
		player = transform.parent.GetComponent<Player>();
		cam = GetComponent<Camera>();
		if (mirror.GetComponentInChildren<Camera>(true))
			testCam = mirror.GetComponentInChildren<Camera>(true);
		else
		{
			testCam = new GameObject().AddComponent<Camera>();
			testCam.name = "Mirror Cam";
			testCam.transform.parent = mirror.transform;
			testCam.CopyFrom(cam);
		}
	}

	// Update is called once per frame
	void Update()
	{
		// reflect pos
		// testCam.transform.position = mirror.transform.position + Vector3.Reflect(mirror.transform.position - transform.position, mirror.transform.forward);
		// testCam.transform.LookAt(mirror.transform.position); // remove x rot
		// testCam.transform.Rotate(new Vector3(-player.rotation.x, player.rotation.y, 0)); // transform with reflection

		// Vector3 mirrorF = cam.worldToCameraMatrix.MultiplyPoint(mirror.transform.up);
		// Vector3 mirrorF = -mirror.transform.up;
		Vector3 mirrorF = cam.worldToCameraMatrix.MultiplyPoint(test);
		// cam.projectionMatrix = cam.CalculateObliqueMatrix(new Vector4(mirrorF.x, mirrorF.y, mirrorF.z, Vector3.Distance(mirror.transform.position, cam.transform.position)));
		cam.projectionMatrix = oblique = cam.CalculateObliqueMatrix(new Vector4(mirrorF.x, mirrorF.y, mirrorF.z, dist));
	}

	// private void LateUpdate()
	// {
	// 	// Rotate testCam.transform 180 degrees so PortalCamera is mirror image of cam
	// 	Matrix4x4 destinationFlipRotation =
	// 		Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(180.0f, Vector3.up), Vector3.one);
	// 	Matrix4x4 sourceInvMat = destinationFlipRotation * testCam.transform.worldToLocalMatrix;

	// 	// Calculate translation and rotation of cam in testCam.transform space
	// 	var source4 = sourceInvMat * new Vector4(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z, 1);
	// 	Vector3 cameraPositionInSourceSpace = new Vector3(source4.x, source4.y, source4.z);
	// 	Quaternion cameraRotationInSourceSpace =
	// 		Quaternion.LookRotation(sourceInvMat.GetColumn(2), sourceInvMat.GetColumn(1)) * cam.transform.rotation;

	// 	// Transform Portal Camera to World Space relative to mirror.transform transform,
	// 	// matching the Main Camera position/orientation
	// 	testCam.transform.position = mirror.transform.TransformPoint(cameraPositionInSourceSpace);
	// 	testCam.transform.rotation = mirror.transform.rotation * cameraRotationInSourceSpace;

	// 	// Calculate clip plane for portal (for culling of objects in-between destination camera and portal)
	// 	Vector4 clipPlaneWorldSpace =
	// 		new Vector4(
	// 			mirror.transform.forward.x,
	// 			mirror.transform.forward.y,
	// 			mirror.transform.forward.z,
	// 			Vector3.Dot(mirror.transform.position, -mirror.transform.forward));

	// 	Vector4 clipPlaneCameraSpace =
	// 		Matrix4x4.Transpose(Matrix4x4.Inverse(testCam.worldToCameraMatrix)) * clipPlaneWorldSpace;

	// 	// Update projection based on new clip plane
	// 	// Note: http://aras-p.info/texts/obliqueortho.html and http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
	// 	testCam.projectionMatrix = cam.CalculateObliqueMatrix(clipPlaneCameraSpace);
	// }
}
