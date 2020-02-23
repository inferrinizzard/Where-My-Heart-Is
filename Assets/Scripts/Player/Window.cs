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
	public GameObject fieldOfView;
	MeshCollider fovMeshCollider;
	public CSG.Model fieldOfViewModel;

	CSG.Operations csgOperator;

	void Start()
	{
		world = World.Instance;
		csgOperator = GetComponent<CSG.Operations>();
		fieldOfViewModel = new CSG.Model(fieldOfView.GetComponent<MeshFilter>().mesh);
		fovMeshCollider = fieldOfView.GetComponent<MeshCollider>();
	}

	public void ApplyCut()
	{
		world.ResetCut();

		// real world objects get intersected with the bound
		foreach (ClipableObject clipableObject in world.GetRealObjects())
		{
			// less expensive, less accurate intersection check
			if (fovMeshCollider.bounds.Intersects(clipableObject.GetComponent<Collider>().bounds))
			{
				// more expensive, more accurate intersection check
				if (clipableObject.IntersectsBound(fieldOfView.transform, fieldOfViewModel))
				{
					clipableObject.UnionWith(fieldOfView, csgOperator);
				}
			}
		}

		// dream world objects get the bound subtracted from them
		foreach (ClipableObject clipableObject in world.GetDreamObjects())
		{
			// less expensive, less accurate intersection check
			if (fovMeshCollider.bounds.Intersects(clipableObject.GetComponent<Collider>().bounds))
			{
				// more expensive, more accurate intersection check
				if (clipableObject.IntersectsBound(fieldOfView.transform, fieldOfViewModel))
				{
					clipableObject.Subtract(fieldOfView, csgOperator);
				}
			}
		}

		// entangled objects have both real and dream world components, which are cut properly by their entangled clipable
		foreach (EntangledClipable clipableObject in world.GetEntangledObjects())
		{
			foreach (ClipableObject clipable in clipableObject.transform.GetComponentsInChildren<ClipableObject>())
			{
				if (clipable == clipableObject)continue;
				// less expensive, less accurate intersection check
				if (fovMeshCollider.bounds.Intersects(clipable.GetComponent<MeshFilter>().mesh.bounds))
				{
					// more expensive, more accurate intersection check
					if (clipable.IntersectsBound(fieldOfView.transform, fieldOfViewModel))
					{
						if (clipable.gameObject.layer == 9)
						{
							clipable.Subtract(fieldOfView, csgOperator);
						}
						else
						{
							clipable.UnionWith(fieldOfView, csgOperator);
						}
					}
				}
			}
			/*// less expensive, less accurate intersection check
			if (fovMeshCollider.bounds.Intersects(clipableObject.realVersion.GetComponent<Collider>().bounds) ||
				fovMeshCollider.bounds.Intersects(clipableObject.dreamVersion.GetComponent<Collider>().bounds))
			{
				// more expensive, more accurate intersection check
				if (clipableObject.IntersectsBound(fieldOfView.transform, fieldOfViewModel))
				{
					clipableObject.UnionWith(fieldOfView, csgOperator);
				}
			}*/
		}
	}
}
