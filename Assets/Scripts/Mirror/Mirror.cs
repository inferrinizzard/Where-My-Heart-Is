using System.Collections;
using System.Collections.Generic;
using System.Linq;

using CSG;

using UnityEngine;

/// <summary>
/// Place on a mirror object to create a planar reflection for it to draw if it uses the mirror material
/// </summary>
public class Mirror : ClippableObject
{
	public Shader maskShader;
	Camera mainCamera;

	[HideInInspector] public Matrix4x4 reflectionMatrix;

	private Material mirrorMaterial;
	private Camera reflectionCamera;
	private RenderTexture renderTarget;

	// Start is called before the first frame update
	void Start()
	{
		mainCamera = Player.VFX.mainCam;
		mirrorMaterial = GetComponent<MeshRenderer>().material;
		if (!reflectionCamera) reflectionCamera = new GameObject("ReflectionCamera").AddComponent<Camera>();
		reflectionCamera.enabled = false;
		reflectionCamera.CopyFrom(Player.VFX.heartCam);
		reflectionCamera.cullingMask = reflectionCamera.cullingMask & ~(1 << LayerMask.NameToLayer("Hands"));

		renderTarget = new RenderTexture(Screen.width, Screen.height, 24);
		reflectionCamera.targetTexture = renderTarget;
	}

	void OnWillRenderObject()
	{
		if (Vector3.Dot(mainCamera.transform.forward, gameObject.transform.up) < 0)
		{
			RenderReflection();
		}
	}

	public override void ClipWith(CSG.Model other)
	{
		if (isClipped) return;
		base.ClipWith(other);
	}

	public override void IntersectWith(Model other)
	{
		base.IntersectWith(other);
		uncutCopy.GetComponent<Mirror>().reflectionCamera = reflectionCamera;
	}

	public override void IntersectMirrored(Model other, Matrix4x4 reflectionMatrix)
	{

	}

	public CSG.Model CreateBound(out Bounds bound)
	{
		CSG.Model sourceModel = new CSG.Model(GetComponent<MeshFilter>().mesh);
		sourceModel.ApplyTransformation(transform.localToWorldMatrix);

		// determine an ordering of vertices around the edge of the surface
		// start at an arbitrary vertex and traverse untraversed edges who belong to only one edge until you reach the starting vertex
		// save the order as a list of indices into the original model
		List<int> order = new List<int>();
		List<CSG.Edge> traversedEdges = new List<CSG.Edge>();
		CSG.Vertex initialVertex = sourceModel.vertices[0];

		CSG.Vertex currentVertex = sourceModel.vertices[0];
		order.Add(0);
		CSG.Edge currentEdge;

		int iterations = 0;
		do
		{
			currentEdge = sourceModel.edges.Find(edge => edge.triangles.Count == 1 && edge.vertices.Contains(currentVertex) && !traversedEdges.Contains(edge));
			if (currentEdge == null)
			{
				break;
			}

			traversedEdges.Add(currentEdge);
			currentVertex = currentEdge.vertices.Find(vertex => vertex != currentVertex);
			order.Add(sourceModel.vertices.IndexOf(currentVertex));

			if (iterations > 100)
			{
				Debug.LogError("ERROR: Too many iterations in mirror bound creation");
				throw new System.Exception("Too many iterations in mirror bound creation");
			}
		} while (currentVertex != initialVertex);

		// copy existing model faces

		CSG.Model farModel = new CSG.Model(GetComponent<MeshFilter>().mesh);
		farModel.ApplyTransformation(transform.localToWorldMatrix);
		Vector3 camPosition = reflectionCamera.transform.position;
		// project copy outwards to act as far side of the bound
		farModel.vertices.ForEach(vertex => vertex.value = camPosition + (vertex.value - camPosition).normalized * Player.Instance.window.fovDistance);

		// we'll use these to connect the two loops
		List<CSG.Vertex> closeLoop = new List<CSG.Vertex>();
		closeLoop = sourceModel.vertices;
		List<CSG.Vertex> farLoop = new List<CSG.Vertex>();
		farLoop = farModel.vertices;

		// flip the normals of the initial face so it faces outwards
		farModel.FlipNormals();

		CSG.Model result = CSG.Model.Combine(sourceModel, farModel);

		Vector3 testNormal = new CSG.Triangle(farLoop[order[0]], closeLoop[order[1]], farLoop[order[2]]).CalculateNormal();
		if (Vector3.Dot(testNormal, sourceModel.triangles[0].CalculateNormal()) <= 0)
		{
			order.Reverse();
		}

		// for every pair of vertices on the close face
		for (int i = 0; i < order.Count - 1; i++)
		{
			// Debug.Log(i);
			// create two new triangles that connect them to the corrosponding pair on the other surface
			result.AddTriangle(new CSG.Triangle(farLoop[order[i]], closeLoop[order[i]], farLoop[order[(i + 1) % order.Count]]));
			// result.triangles.Last().vertices.ForEach(edge => Debug.Log(i + " :: " + edge));
			result.AddTriangle(new CSG.Triangle(closeLoop[order[i]], closeLoop[order[(i + 1) % order.Count]], farLoop[order[(i + 1) % order.Count]]));
			// result.triangles.Last().vertices.ForEach(edge => Debug.Log(edge));
		}

		result.CreateEdges();
        /*Debug.Log(result.triangles[0].CalculateNormal());
        Debug.Log(reflectionCamera.transform.forward);
        Debug.Log(Vector3.Dot(result.triangles[0].CalculateNormal(), reflectionCamera.transform.forward));
        result.triangles[0].DrawNormal(Color.green);*/

        if (Vector3.Dot(result.triangles[0].CalculateNormal(), reflectionCamera.transform.forward) > 0)
		{
			result.FlipNormals();
		}

		/*GameObject test = new GameObject();
		test.AddComponent<MeshFilter>().mesh = result.ToMesh(test.transform.worldToLocalMatrix);
		test.AddComponent<MeshRenderer>();*/

		bound = new Bounds();
		Vector3 center = Vector3.zero;
		result.vertices.ForEach(vertex => center += vertex.value);
		center /= result.vertices.Count;
		bound.center = center;
		foreach (Vertex vertex in result.vertices)
		{
			bound.Encapsulate(vertex.value);
		}

		return result;
	}

	/// <summary>
	/// Renders the scene again from the perspective of a mirrored copy of the main camera and stores it in a render texture
	/// </summary>
	private void RenderReflection()
	{
		reflectionCamera.CopyFrom(Player.VFX.heartCam);
		reflectionCamera.cullingMask = reflectionCamera.cullingMask & ~(1 << LayerMask.NameToLayer("Hands"));
		Transform mainTransform = mainCamera.transform;

		//TODO: base this on normal vector instead of assumption that normal is in positive local y
		reflectionMatrix = gameObject.transform.localToWorldMatrix * Matrix4x4.Scale(new Vector3(1, -1, 1)) * gameObject.transform.worldToLocalMatrix;

		reflectionCamera.transform.position = reflectionMatrix.MultiplyPoint(mainTransform.position);
		reflectionCamera.transform.LookAt(reflectionCamera.transform.position + reflectionMatrix.MultiplyVector(mainTransform.forward), reflectionMatrix.MultiplyVector(mainTransform.up));

		Vector3 normal = gameObject.transform.up;
		Vector4 clipPlaneWorldSpace =
			new Vector4(
				normal.x,
				normal.y,
				normal.z,
				Vector3.Dot(gameObject.transform.position, -normal));

		Vector4 clipPlaneCameraSpace =
			Matrix4x4.Transpose(Matrix4x4.Inverse(reflectionCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;
		reflectionCamera.projectionMatrix = reflectionCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);

		reflectionCamera.targetTexture = renderTarget;

		reflectionCamera.Render();

		mirrorMaterial.SetTexture("_ReflectionTex", renderTarget);
	}
}
