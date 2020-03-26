using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * <summary>
 * Class that uses CSG.Operations on ClipableObjects to create the cut
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
        StartCoroutine(ApplyCutCoroutine(1f/ ((float)framerateTarget)));
    }

    private void ApplyCutSynchronous()
    {
        world.GetRealObjects().ToList().ForEach(clipable => { if (IntersectsBounds(clipable)) clipable.UnionWith(fieldOfViewModel); });
        world.GetDreamObjects().ToList().ForEach(clipable => { if (IntersectsBounds(clipable)) clipable.Subtract(fieldOfViewModel); });

        foreach (EntangledClipable entangled in world.GetEntangledObjects())
        {
            // clip the immediate children of entangled
            //if (IntersectsBounds(entangled.realVersion)) entangled.realVersion.UnionWith(fieldOfView, csgOperator);
            //if (IntersectsBounds(entangled.dreamVersion)) entangled.dreamVersion.Subtract(fieldOfView, csgOperator);

            // clip any children below them to the correct world
            entangled.realObject.GetComponentsInChildren<ClipableObject>().ToList().ForEach(
                clipable => { if (IntersectsBounds(clipable)) clipable.UnionWith(fieldOfViewModel); });
            entangled.dreamObject.GetComponentsInChildren<ClipableObject>().ToList().ForEach(
                clipable => { if (IntersectsBounds(clipable)) clipable.Subtract(fieldOfViewModel); });
        }
    }

    private IEnumerator ApplyCutCoroutine(float frameLength)
    {
        float startTime = Time.realtimeSinceStartup;

		foreach(ClipableObject clipable in world.GetRealObjects())
        {
            if (IntersectsBounds(clipable))
            {
                clipable.UnionWith(fieldOfViewModel);
            }

            if(Time.realtimeSinceStartup - startTime > frameLength)
            {
                yield return null;
                startTime = Time.realtimeSinceStartup;
            }
        }

        foreach (ClipableObject clipable in world.GetDreamObjects())
        {
            if (IntersectsBounds(clipable))
            {
                clipable.Subtract(fieldOfViewModel);
            }

            if (Time.realtimeSinceStartup - startTime > frameLength)
            {
                yield return null;
                startTime = Time.realtimeSinceStartup;
            }
        }

        foreach (EntangledClipable entangled in world.GetEntangledObjects())
        {
            // clip the immediate children of entangled
            //if (IntersectsBounds(entangled.realVersion)) entangled.realVersion.UnionWith(fieldOfView, csgOperator);
            //if (IntersectsBounds(entangled.dreamVersion)) entangled.dreamVersion.Subtract(fieldOfView, csgOperator);

            // clip any children below them to the correct world
            foreach (ClipableObject clipable in entangled.realObject.GetComponentsInChildren<ClipableObject>())
            {
                if (IntersectsBounds(clipable))
                {
                    clipable.UnionWith(fieldOfViewModel);
                }

                if (Time.realtimeSinceStartup - startTime > frameLength)
                {
                    yield return null;
                    startTime = Time.realtimeSinceStartup;
                }
            }
            foreach (ClipableObject clipable in entangled.dreamObject.GetComponentsInChildren<ClipableObject>())
            {
                if (IntersectsBounds(clipable))
                {
                    clipable.Subtract(fieldOfViewModel);
                }

                if (Time.realtimeSinceStartup - startTime > frameLength)
                {
                    yield return null;
                    startTime = Time.realtimeSinceStartup;
                }
            }
        }
    }

    private bool IntersectsBounds(ClipableObject clipableObject)
	{
		return true;
		// less expensive, less accurate intersection check
		//TODO: checking the bound intersections doesn't consider world space, just model space, so it's basically trash
		if (true || fieldOfView.GetComponent<MeshCollider>().bounds.Intersects(clipableObject.GetComponent<MeshFilter>().mesh.bounds))
		{
			// more expensive, more accurate intersection check
			if (clipableObject.IntersectsBound(fieldOfView.transform, fieldOfViewModel))
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
		ClipableObject[] clipables = FindObjectsOfType<ClipableObject>();

		Bounds bound = clipables[0].GetComponent<MeshCollider>().bounds;

		for (int i = 1; i < clipables.Length; i++)
		{
			bound.Encapsulate(clipables[i].GetComponent<MeshCollider>().bounds);
		}

        return bound;
	}
}
