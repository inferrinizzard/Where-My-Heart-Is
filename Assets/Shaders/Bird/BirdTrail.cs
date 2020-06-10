using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;

// [ExecuteInEditMode]
public class BirdTrail : MonoBehaviour
{
	[SerializeField] Shader drawShader = default;
	Material drawMat;
	[SerializeField] int count = 3;
	[SerializeField] int length = 20;
	List<Ghost> copies = new List<Ghost>();
	int step = 0;

	Camera cam;
	CommandBuffer birdBuffer;
	int birdTemp = Shader.PropertyToID("_BirdTemp");
	SkinnedMeshRenderer[] smrs;

	void Start()
	{
		smrs = GetComponentsInChildren<SkinnedMeshRenderer>();
		drawMat = new Material(drawShader);
		drawMat.SetColor("_Colour", new Color(0, 1, 1, 1));

		cam = Player.Instance.GetComponentInChildren<Camera>() ?? Camera.main; // TODO: fix this reference
		birdBuffer = new CommandBuffer();
		birdBuffer.GetTemporaryRT(birdTemp, -1, -1, 24, FilterMode.Bilinear);
		birdBuffer.SetRenderTarget(birdTemp);
		birdBuffer.ClearRenderTarget(true, true, Color.clear);
		birdBuffer.SetGlobalTexture("_BirdMask", birdTemp);
		birdBuffer.name = "Bird Trail Buffer";

		cam.AddCommandBuffer(CameraEvent.BeforeSkybox, birdBuffer);
	}

	void Cleanup()
	{
		if (birdBuffer != null && cam)
			cam.RemoveCommandBuffer(CameraEvent.BeforeSkybox, birdBuffer);
	}

	public void OnDisable() => Cleanup();

	public void OnEnable() => Cleanup();

	// class Ghost : IEnumerable
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

        public void ReleaseMeshes()
        {
            meshes.ForEach(mesh => Destroy(mesh));
        }
	}

	/*void Update()
	{
		foreach (Ghost t in copies)
			this.DrawCube(t.position, 1, t.rotation, Color.blue, depthCheck : true);
	}*/

	void FixedUpdate()
	{
		step = ++step % length;
		if (step == 0)
        {
			copies.Add(new Ghost(transform, smrs));
        }

		if (copies.Count > count)
        {
            copies[0].ReleaseMeshes();
			copies.RemoveAt(0);
        }

		// if (copies.Count > 0)
		// {
		// 	deltaPos = copies[copies.Count - 1].position - transform.position;
		// 	deltaRot = copies[copies.Count - 1].rotation * Quaternion.Inverse(transform.rotation);
		// }
	}

	void LateUpdate() => birdBuffer.ClearRenderTarget(true, true, Color.clear);
	int colourID = Shader.PropertyToID("_Colour");
	void OnWillRenderObject()
	{
		var properties = new MaterialPropertyBlock();
		for (int i = 0; i < copies.Count; i++)
		{
			float step = 0.5f / (copies.Count - i);
            //properties.SetColor(colourID, new Color(1, 1, step, 1));
            properties.SetColor(colourID, Color.cyan);
			foreach (var(mesh, pos, rot) in copies[i].Data())
				birdBuffer.DrawMesh(mesh, Matrix4x4.TRS(pos, rot, transform.localScale * (step + 0.5f) * i), drawMat, 0, 0, properties);
		}
	}
	void OnPreCull() => birdBuffer.SetGlobalTexture("_BirdMask", birdTemp);
}
