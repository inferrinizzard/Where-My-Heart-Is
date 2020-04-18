using System;
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
    [Header("References")]
	public GameObject fieldOfViewSource;
	public GameObject fieldOfView;
    public GameObject mirror;
	public CSG.Model fieldOfViewModel;

    [Header("Behavior")]
	public int framerateTarget;// framerate to target while applying cut, effects whether the cut coroutine will yeild to next frame or not

    // references
	[HideInInspector] public World world;
	[HideInInspector] public Camera cam;

    // behavior
    [HideInInspector] public float fovDistance;// distance from camera to the far end of the fov object, calculated at the beginning of each puzzle
    [HideInInspector] public float cutStartTime;

    // state management
    private bool cutInProgress;

    public event Action OnBeginCut;
    public event Action<ClippableObject> OnClippableCut;
    public event Action OnCompleteCut;

    void Start()
	{
		fieldOfViewModel = new CSG.Model(fieldOfView.GetComponent<MeshFilter>().mesh);
	}

	public void ApplyCut()
	{
        OnBeginCut?.Invoke();

        if(cutInProgress)
        {
            Debug.Log("Cut attempted during other cut");
            return;
        }
        cutInProgress = true;
        world.ResetCut();
		fieldOfViewModel = new CSG.Model(fieldOfView.GetComponent<MeshFilter>().mesh, fieldOfView.transform);
		fieldOfViewModel.ConvertToWorld();
		StartCoroutine(ApplyCutCoroutine(1f / ((float) framerateTarget), new Bounds(fieldOfView.GetComponent<MeshCollider>().bounds.center, fieldOfView.GetComponent<MeshCollider>().bounds.size), fieldOfViewModel));
	}

	private IEnumerator ApplyCutCoroutine(float frameLength, Bounds bounds, CSG.Model boundModel)
	{
		float startTime = Time.realtimeSinceStartup;
        Matrix4x4 reflectionMatrix = new Matrix4x4(
            mirror.GetComponent<Mirror>().reflectionMatrix.GetColumn(0),
            mirror.GetComponent<Mirror>().reflectionMatrix.GetColumn(1),
            mirror.GetComponent<Mirror>().reflectionMatrix.GetColumn(2),
            mirror.GetComponent<Mirror>().reflectionMatrix.GetColumn(3)
            );

        if (mirror.GetComponent<ClippableObject>().CachedModel.Intersects(boundModel, 0.0001f, true))
        {
            mirror.GetComponent<ClippableObject>().ClipWith(boundModel);
            new CSG.Model(mirror.GetComponent<MeshFilter>().mesh).Draw(Color.cyan);

            Bounds mirrorBound;
            CSG.Model mirrorBoundModel = mirror.GetComponent<Mirror>().CreateBound(out mirrorBound);
            mirrorBoundModel.Draw(Color.green);

            foreach (ClippableObject clippable in world.heartWorldContainer.GetComponentsInChildren<ClippableObject>())
            {
                if (IntersectsBounds(clippable, mirrorBound))
                {
                    clippable.GetComponent<ClippableObject>().IntersectMirrored(mirrorBoundModel, reflectionMatrix);
                }
            }

            foreach(EntangledClippable entangled in world.entangledClippables)
            {
                entangled.ClipMirrored(this, mirrorBound, mirrorBoundModel, reflectionMatrix, frameLength);
            }
        }

		foreach (ClippableObject clippable in world.clippables)
		{
			if (IntersectsBounds(clippable, bounds))
			{
                clippable.ClipWith(boundModel);
                OnClippableCut?.Invoke(clippable);
			}
            
            if (Time.realtimeSinceStartup - startTime > frameLength)
			{
				yield return null;
				startTime = Time.realtimeSinceStartup;
			}
		}


        cutInProgress = false;
        OnCompleteCut?.Invoke();
    }

	public bool IntersectsBounds(ClippableObject clippableObject, Bounds bounds)
	{
		//return true;
		// less expensive, less accurate intersection check
		//TODO: checking the bound intersections doesn't consider world space, just model space, so it's basically trash
		//Debug.Log(clippableObject.GetComponent<MeshFilter>().mesh.bounds.extents);
		//Debug.Log(bounds.min + " :: " + bounds.max);
		//Debug.Log(clippableObject.GetComponent<MeshCollider>().bounds.min + " :: " + clippableObject.GetComponent<MeshCollider>().bounds.max);
		if (bounds.Intersects(clippableObject.GetComponent<MeshCollider>().bounds)) //true || 
		{
			//Debug.Log(clippableObject.IntersectsBound(fieldOfViewModel));
			// more expensive, more accurate intersection check
			if (clippableObject.IntersectsBound(fieldOfViewModel))
			{
				return true;
			}
		}

		return false;
	}

	public void CreateFoVMesh()
	{
        MeshFilter fovFilter = fieldOfView.GetComponent<MeshFilter>();

        Bounds sceneBound = GetSceneBounds();

		fovDistance = Mathf.Max(sceneBound.size.magnitude, 40f);

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

		model.ApplyTransformation(fieldOfView.transform.localToWorldMatrix);

		// project out the points of the original surface
		model.vertices.ForEach(vertex =>
		{
			vertex.value = GetComponent<Player>().cam.transform.position + (vertex.value - GetComponent<Player>().cam.transform.position).normalized * fovDistance;
		});
		// flip their normals

		// now create the sides of the view
		CSG.Vertex originVertex = new CSG.Vertex(0, cam.transform.position);

		model.edges.ForEach(edge =>
		{
			if (!toIgnore.Contains(edge.vertices[0]) && !toIgnore.Contains(edge.vertices[1]))
			{
				model.AddTriangle(new CSG.Triangle(originVertex, edge.vertices[1], edge.vertices[0]));
			}
		});
		model.edges.ForEach(edge => edge.Draw(Color.red));
		// convert to local space of the cam
		fovFilter.mesh = model.ToMesh(fieldOfView.transform.worldToLocalMatrix);
		fieldOfView.GetComponent<MeshCollider>().sharedMesh = fovFilter.mesh;
		fovFilter.mesh.RecalculateNormals();
	}

	private Bounds GetSceneBounds()
	{
		ClippableObject[] clippables = FindObjectsOfType<ClippableObject>();

        //Bounds bound = clippables[0].GetComponent<MeshCollider>().bounds;
        Bounds bound = new Bounds(Player.Instance.transform.position, clippables[0].GetComponent<MeshCollider>().bounds.size);

		for (int i = 0; i < clippables.Length; i++)
		{
			bound.Encapsulate(clippables[i].GetComponent<MeshCollider>().bounds);
		}

		return bound;
	}
}
