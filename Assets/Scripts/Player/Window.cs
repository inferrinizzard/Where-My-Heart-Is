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
	// [Header("References")]
	[HideInInspector] public GameObject fieldOfViewSource;
	[HideInInspector] public GameObject fieldOfView;
	[HideInInspector] public ClippableObject mirrorObj;
	Mirror mirror;
	public CSG.Model fieldOfViewModel;

	[Header("Behavior")]
	public int framerateTarget; // framerate to target while applying cut, effects whether the cut coroutine will yeild to next frame or not

	// references
	[HideInInspector] public World world;
	[HideInInspector] public Camera cam;

	// behavior
	[HideInInspector] public float fovDistance; // distance from camera to the far end of the fov object, calculated at the beginning of each puzzle
	[HideInInspector] public float cutStartTime;

	// state management
	private bool cutInProgress;
	private bool mirrorCutApplied;

	public event Action OnBeginCut;
	public event Action<ClippableObject> OnClippableCut;
	public event Action OnCompleteCut;

	void Start()
	{
		fieldOfViewSource = Player.Instance.heartWindow;
		fieldOfView = fieldOfViewSource.GetComponentOnlyInChildren<MeshFilter>().gameObject;
		FindMirror();
		fieldOfViewModel = new CSG.Model(fieldOfView.GetComponent<MeshFilter>().mesh);
	}

	public void FindMirror()
	{
		mirrorObj = world.heartWorldContainer.GetComponentInChildren<Mirror>()?.GetComponent<ClippableObject>();
		mirror = mirrorObj?.GetComponent<Mirror>();
	}

	public bool ApplyCut()
	{
		OnBeginCut?.Invoke();

		if (cutInProgress)
		{
			Debug.Log("Cut attempted during other cut");
			return false;
		}
		cutInProgress = true;
		world.ResetCut();
		fieldOfViewModel = new CSG.Model(fieldOfView.GetComponent<MeshFilter>().mesh, fieldOfView.transform);
		fieldOfViewModel.ConvertToWorld();
		StartCoroutine(ApplyCutCoroutine(1f / ((float) framerateTarget), new Bounds(fieldOfView.GetComponent<MeshRenderer>().bounds.center, fieldOfView.GetComponent<MeshRenderer>().bounds.size), fieldOfViewModel));
		return true;
	}

	private IEnumerator ApplyCutCoroutine(float frameLength, Bounds bounds, CSG.Model boundModel)
	{
		float startTime = Time.realtimeSinceStartup;
        //float monitorStartTime = startTime;
		CSG.Model mirrorBoundModel = null;
		Matrix4x4 reflectionMatrix = Matrix4x4.identity;

		if (mirrorObj && mirrorObj.CachedModel.Intersects(boundModel, 0.0001f, true))
		{
			mirrorCutApplied = true;

			reflectionMatrix = new Matrix4x4(
				mirror.reflectionMatrix.GetColumn(0),
				mirror.reflectionMatrix.GetColumn(1),
				mirror.reflectionMatrix.GetColumn(2),
				mirror.reflectionMatrix.GetColumn(3)
			);

			mirrorObj.ClipWith(boundModel);

			Bounds mirrorBound;
			mirrorBoundModel = mirror.CreateBound(out mirrorBound);

			foreach (EntangledClippable entangled in world.EntangledClippables)
			{
				entangled.ClipMirrored(this, mirrorBound, mirrorBoundModel, reflectionMatrix);
				// Debug.Log(entangled.gameObject);
			}

			foreach (ClippableObject clippable in world.heartWorldContainer.GetComponentsInChildren<ClippableObject>())
			{
				if (IntersectsBounds(clippable, mirrorBound, mirrorBoundModel) && !(clippable is Mirror))
				{
					clippable.GetComponent<ClippableObject>().IntersectMirrored(mirrorBoundModel, reflectionMatrix);
					// Debug.Log(clippable.gameObject);
				}
			}
		}
		else mirrorCutApplied = false;

		// cut away the stuff behind the mirror
		foreach (ClippableObject clippable in world.Clippables)
		{
			// TODO: could we just check if they were already clipped?
			// since things that are clipped have already intersected the fovmodel, so we've already checked for this
			if (IntersectsBounds(clippable, bounds, fieldOfViewModel))
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

		if (mirrorCutApplied)
		{
			//reflect csg model
			mirrorBoundModel.ApplyTransformation(reflectionMatrix);

			foreach (ClippableObject clippable in world.Clippables)
			{
				if (clippable.IntersectsBound(mirrorBoundModel))
				{
					// Debug.Log(clippable.gameObject);
					clippable.Subtract(mirrorBoundModel, false);
					OnClippableCut?.Invoke(clippable);
				}

				if (Time.realtimeSinceStartup - startTime > frameLength)
				{
					yield return null;
					startTime = Time.realtimeSinceStartup;
				}
			}
		}

        //Debug.Log(Time.realtimeSinceStartup - monitorStartTime);
		cutInProgress = false;
		OnCompleteCut?.Invoke();
	}

	public bool IntersectsBounds(ClippableObject clippableObject, Bounds bounds, CSG.Model boundsModel)
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
			if (clippableObject.IntersectsBound(boundsModel))
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

		if (Vector3.Dot(transform.forward, model.triangles[0].CalculateNormal()) < 0)
		{
            model.FlipNormals();
		}
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

		// convert to local space of the cam
		fovFilter.mesh = model.ToMesh(fieldOfView.transform.worldToLocalMatrix);
		fieldOfView.GetComponent<MeshFilter>().sharedMesh = fovFilter.mesh;
		fovFilter.mesh.RecalculateNormals();
	}

	private Bounds GetSceneBounds()
	{
		ClippableObject[] clippables = FindObjectsOfType<ClippableObject>();

		//Bounds bound = clippables[0].GetComponent<MeshCollider>().bounds;
		Bounds bound = clippables.Length > 0 ? new Bounds(Player.Instance.transform.position, clippables[0].GetComponent<MeshCollider>().bounds.size) : Player.Instance.heartWindow.GetComponentInChildren<MeshRenderer>().bounds;

		for (int i = 0; i < clippables.Length; i++)
		{
			bound.Encapsulate(clippables[i].GetComponent<MeshCollider>().bounds);
		}

		return bound;
	}
}
