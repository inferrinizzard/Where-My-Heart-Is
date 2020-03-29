using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

/**
 * <summary>
 * Class that uses CSG.Operations on clippableObjects to create the cut
 * </summary>
 * */
public class Window : MonoBehaviour
{
	[HideInInspector] public World world;
	public new Camera camera;
	public GameObject fieldOfViewSource;
	public GameObject fieldOfView;
	public CSG.Model fieldOfViewModel;
	public Material tempMaterial;
	public int framerateTarget;

	void Start()
	{
		fieldOfViewModel = new CSG.Model(fieldOfView.GetComponent<MeshFilter>().mesh);
		//Invoke("CreateFoVMesh", 0.5f); //TODO: extreme hack
	}

	public void ApplyCut()
	{
		world.ResetCut();
		fieldOfViewModel = new CSG.Model(fieldOfView.GetComponent<MeshFilter>().mesh, fieldOfView.transform);
		fieldOfViewModel.ConvertToWorld();
		fieldOfViewModel.Draw(Color.red);
		StartCoroutine(ApplyCutCoroutine(1f / ((float) framerateTarget)));
	}

	private void ApplyCutSynchronous()
	{
		world.GetHeartObjects().ToList().ForEach(clippable => { if (IntersectsBounds(clippable)) clippable.UnionWith(fieldOfViewModel); });
		world.GetRealObjects().ToList().ForEach(clippable => { if (IntersectsBounds(clippable)) clippable.Subtract(fieldOfViewModel); });

		foreach (EntangledClippable entangled in world.GetEntangledObjects())
		{
			// clip the immediate children of entangled
			//if (IntersectsBounds(entangled.heartVersion)) entangled.heartVersion.UnionWith(fieldOfView, csgOperator);
			//if (IntersectsBounds(entangled.realVersion)) entangled.realVersion.Subtract(fieldOfView, csgOperator);

			// clip any children below them to the correct world
			entangled.heartObject.GetComponentsInChildren<ClippableObject>().ToList().ForEach(
				clippable => { if (IntersectsBounds(clippable)) clippable.UnionWith(fieldOfViewModel); });
			entangled.realObject.GetComponentsInChildren<ClippableObject>().ToList().ForEach(
				clippable => { if (IntersectsBounds(clippable)) clippable.Subtract(fieldOfViewModel); });
		}
	}

	private IEnumerator ApplyCutCoroutine(float frameLength)
	{
		float startTime = Time.realtimeSinceStartup;

		foreach (ClippableObject clippable in world.GetHeartObjects())
		{
			if (IntersectsBounds(clippable))
			{
				clippable.UnionWith(fieldOfViewModel);
			}

			if (Time.realtimeSinceStartup - startTime > frameLength)
			{
				yield return null;
				startTime = Time.realtimeSinceStartup;
			}
		}

		foreach (ClippableObject clippable in world.GetRealObjects())
		{
			if (IntersectsBounds(clippable))
			{
				clippable.Subtract(fieldOfViewModel);
			}

			if (Time.realtimeSinceStartup - startTime > frameLength)
			{
				yield return null;
				startTime = Time.realtimeSinceStartup;
			}
		}

		foreach (EntangledClippable entangled in world.GetEntangledObjects())
		{
			// clip the immediate children of entangled
			//if (IntersectsBounds(entangled.heartVersion)) entangled.heartVersion.UnionWith(fieldOfView, csgOperator);
			//if (IntersectsBounds(entangled.realVersion)) entangled.realVersion.Subtract(fieldOfView, csgOperator);

			// clip any children below them to the correct world
			foreach (ClippableObject clippable in entangled.heartObject.GetComponentsInChildren<ClippableObject>())
			{
				if (IntersectsBounds(clippable))
				{
					clippable.UnionWith(fieldOfViewModel);
				}

				if (Time.realtimeSinceStartup - startTime > frameLength)
				{
					yield return null;
					startTime = Time.realtimeSinceStartup;
				}
			}
			foreach (ClippableObject clippable in entangled.realObject.GetComponentsInChildren<ClippableObject>())
			{
				if (IntersectsBounds(clippable))
				{
					clippable.Subtract(fieldOfViewModel);
				}

				if (Time.realtimeSinceStartup - startTime > frameLength)
				{
					yield return null;
					startTime = Time.realtimeSinceStartup;
				}
			}
		}
	}

	private bool IntersectsBounds(ClippableObject clippableObject)
	{
		return true;
		// less expensive, less accurate intersection check
		//TODO: checking the bound intersections doesn't consider world space, just model space, so it's basically trash
		if (true || fieldOfView.GetComponent<MeshCollider>().bounds.Intersects(clippableObject.GetComponent<MeshFilter>().mesh.bounds))
		{
			// more expensive, more accurate intersection check
			if (clippableObject.IntersectsBound(fieldOfView.transform, fieldOfViewModel))
			{
				return true;
			}
		}

		return false;
	}

	public void CreateFoVMesh()
	{
		Bounds sceneBound = GetSceneBounds();

		float distance = sceneBound.extents.magnitude * 2;

		Mesh sourceMesh = fieldOfViewSource.GetComponent<MeshFilter>().mesh;

		CSG.Model model = new CSG.Model(fieldOfViewSource.GetComponent<MeshFilter>().mesh);
		List<CSG.Vertex> toIgnore = new List<CSG.Vertex>();
		model.vertices.ForEach(vertex =>
		{
			if (vertex.triangles.Count > 2)
			{
				toIgnore.Add(vertex);
			}
		});

		model.ConvertToWorld(fieldOfView.transform.localToWorldMatrix);

		// project out the points of the original surface
		model.vertices.ForEach(vertex =>
		{
			vertex.value = camera.transform.position + (vertex.value - camera.transform.position).normalized * distance;
		});
		// flip their normals

		// now create the sides of the view
		CSG.Vertex originVertex = new CSG.Vertex(0, camera.transform.position);

		model.edges.ForEach(edge =>
		{
			if (!toIgnore.Contains(edge.vertices[0]) && !toIgnore.Contains(edge.vertices[1]))
			{
				model.AddTriangle(new CSG.Triangle(originVertex, edge.vertices[1], edge.vertices[0]));
			}
		});
		model.edges.ForEach(edge => edge.Draw(Color.red));
		// convert to local space of the camera
		fieldOfView.GetComponent<MeshFilter>().mesh = model.ToMesh(fieldOfView.transform.worldToLocalMatrix);
		fieldOfView.GetComponent<MeshCollider>().sharedMesh = fieldOfView.GetComponent<MeshFilter>().mesh;
		fieldOfView.GetComponent<MeshFilter>().mesh.RecalculateNormals();
	}

	private Bounds GetSceneBounds()
	{
		ClippableObject[] clippables = FindObjectsOfType<ClippableObject>();

		Bounds bound = clippables[0].GetComponent<MeshCollider>().bounds;

		for (int i = 1; i < clippables.Length; i++)
		{
			bound.Encapsulate(clippables[i].GetComponent<MeshCollider>().bounds);
		}

		return bound;
	}
}
