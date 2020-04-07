using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;

// [ExecuteInEditMode]
public class BirdTrail : MonoBehaviour
{
	public Material mat;
	[SerializeField] int count = 3;
	[SerializeField] int length = 20;
	List<Trans> copies = new List<Trans>();
	int step = 0;

	Camera cam;
	CommandBuffer birdBuffer;
	int birdTemp = Shader.PropertyToID("_BirdTemp");

	void Start()
	{
		cam = Camera.main;
		birdBuffer = new CommandBuffer();
		birdBuffer.GetTemporaryRT(birdTemp, -1, -1, 24, FilterMode.Bilinear);
		birdBuffer.SetRenderTarget(birdTemp);
		birdBuffer.ClearRenderTarget(true, true, Color.clear);
		birdBuffer.SetGlobalTexture("_BirdTrail", birdTemp);
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

	struct Trans
	{
		public Vector3 position;
		public Quaternion rotation;
		public Vector3 eulerAngles => rotation.eulerAngles;
		public Vector3 scale;
		public Matrix4x4 TRS { get => Matrix4x4.TRS(position, rotation, scale); }

		public Trans(Transform t)
		{
			position = t.position;
			rotation = t.rotation;
			scale = t.localScale;
		}
	}

	void Update()
	{
		foreach (Trans t in copies)
			this.DrawCube(t.position, 1, t.rotation, Color.red, depthCheck : true);
	}

	void FixedUpdate()
	{
		step = ++step % length;
		if (step == 0)
			copies.Add(new Trans(transform));
		if (copies.Count > count)
			copies.RemoveAt(0);
	}

	void LateUpdate() => birdBuffer.ClearRenderTarget(true, true, Color.clear);
	void OnWillRenderObject()
	{
		foreach (SkinnedMeshRenderer r in GetComponentsInChildren<SkinnedMeshRenderer>())
			// birdBuffer.DrawMesh(r.sharedMesh, r.transform.TRS(), mat, 0, 0);
			for (int i = 0; i < copies.Count; i++)
				birdBuffer.DrawMesh(r.sharedMesh,
					Matrix4x4.TRS(r.transform.position + (copies[i].position - transform.position),
						// r.transform.rotation * (copies[i].rotation * Quaternion.Inverse(transform.rotation)),
						Quaternion.Euler(r.transform.rotation.eulerAngles + (copies[i].eulerAngles - transform.eulerAngles)),
						r.transform.localScale * (i + 1) / count), mat, 0, 0);
	}
	void OnPreCull() => birdBuffer.SetGlobalTexture("_BirdTrail", birdTemp);
}
