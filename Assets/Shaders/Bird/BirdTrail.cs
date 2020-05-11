using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;

// [ExecuteInEditMode]
public class BirdTrail : MonoBehaviour
{
	Material drawMat;
	[SerializeField] int count = 3;
	[SerializeField] int length = 20;
	List<Ghost> copies = new List<Ghost>();
	int step = 0;

	Camera cam;
	CommandBuffer birdBuffer;
	int birdTempID = Shader.PropertyToID("_BirdTemp"), birdMaskID = Shader.PropertyToID("_BirdMask");
	SkinnedMeshRenderer[] smrs;

	void Start()
	{
		smrs = GetComponentsInChildren<SkinnedMeshRenderer>();
		drawMat = new Material(Shader.Find("Outline/GlowObject"));
		drawMat.color = new Color(0, 1, 1, 1);

		cam = Effects.Instance.mainCam;
		birdBuffer = new CommandBuffer();
		birdBuffer.name = "Bird Trail Buffer";

		cam.AddCommandBuffer(CameraEvent.BeforeSkybox, birdBuffer);
		ApplyOutline.ResetScreenBuffer(birdBuffer, birdTempID);
	}

	public void OnEnable() => Cleanup();
	public void OnDisable() => Cleanup();
	void Cleanup()
	{
		if (birdBuffer != null && cam)
			cam.RemoveCommandBuffer(CameraEvent.BeforeSkybox, birdBuffer);
	}

	class Ghost
	{
		public Vector3 position;
		public List<Vector3> rendererPositions;
		public Quaternion rotation;
		public List<Quaternion> rendererRotations;
		public Vector3 eulerAngles => rotation.eulerAngles;
		public Vector3 scale;
		public Matrix4x4 TRS { get => Matrix4x4.TRS(position, rotation, scale); }
		public List<Mesh> meshes;

		public Ghost(Transform t, SkinnedMeshRenderer[] renderers)
		{
			position = t.position;
			rotation = t.rotation;
			scale = t.localScale;
			meshes = new List<Mesh>();
			rendererPositions = new List<Vector3>();
			rendererRotations = new List<Quaternion>();
			foreach (SkinnedMeshRenderer r in renderers)
			{
				Mesh temp = new Mesh();
				r.BakeMesh(temp);
				meshes.Add(temp);
				rendererPositions.Add(r.transform.position);
				rendererRotations.Add(r.transform.rotation);
			}
		}
		// public(Mesh, Vector3, Quaternion) this [int i] => (meshes[i], rendererPositions[i], rendererRotations[i]);
		public IEnumerable < (Mesh, Vector3, Quaternion) > Data()
		{
			for (int i = 0; i < meshes.Count; i++)
				yield return (meshes[i], rendererPositions[i], rendererRotations[i]);
		}
	}

	// void Update()
	// {
	// 	foreach (Ghost t in copies)
	// 		this.DrawCube(t.position, 1, t.rotation, Color.red, depthCheck : true);
	// }

	void FixedUpdate()
	{
		step = ++step % length;
		if (step == 0)
			copies.Add(new Ghost(transform, smrs));
		if (copies.Count > count)
			copies.RemoveAt(0);

		// if (copies.Count > 0)
		// {
		// 	deltaPos = copies[copies.Count - 1].position - transform.position;
		// 	deltaRot = copies[copies.Count - 1].rotation * Quaternion.Inverse(transform.rotation);
		// }
	}

	void LateUpdate() => ApplyOutline.ResetScreenBuffer(birdBuffer, birdTempID);
	int colourID = Shader.PropertyToID("_Color");
	void OnWillRenderObject()
	{
		if (Camera.current == Effects.Instance.mainCam)
		{
			var properties = new MaterialPropertyBlock();
			for (int i = 0; i < copies.Count; i++)
			{
				float step = 1f / (copies.Count - i);
				properties.SetColor(colourID, new Color(0, 1, step, 1));
				foreach (var(mesh, pos, rot) in copies[i].Data())
					birdBuffer.DrawMesh(mesh, Matrix4x4.TRS(pos, rot, transform.localScale * step), drawMat, 0, 0, properties);
			}
		}
	}
	void OnPreCull() => birdBuffer.SetGlobalTexture(birdMaskID, birdTempID);
}
