using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * <summary>
 * Class that uses CSG.Operations on ClipableObjects to create the cut
 * </summary>
 * */
public class Window : MonoBehaviour
{
	public World world;
	public GameObject fieldOfView;
	public CSG.Model fieldOfViewModel;

	private CSG.Operations csgOperator;

	void Start()
	{
		csgOperator = GetComponent<CSG.Operations>();
		fieldOfViewModel = new CSG.Model(fieldOfView.GetComponent<MeshFilter>().mesh);
	}

	public void ApplyCut()
	{
		world.ResetCut();

		// real world objects get intersected with the bound
		foreach (ClipableObject clipableObject in world.GetRealObjects())
		{
			// less expensive, less accurate intersection check
			if (fieldOfView.GetComponent<MeshCollider>().bounds.Intersects(clipableObject.GetComponent<Collider>().bounds))
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
			if (fieldOfView.GetComponent<MeshCollider>().bounds.Intersects(clipableObject.GetComponent<Collider>().bounds))
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
			// less expensive, less accurate intersection check
			if (fieldOfView.GetComponent<MeshCollider>().bounds.Intersects(clipableObject.realVersion.GetComponent<Collider>().bounds) ||
				fieldOfView.GetComponent<MeshCollider>().bounds.Intersects(clipableObject.dreamVersion.GetComponent<Collider>().bounds))
			{
				// more expensive, more accurate intersection check
				if (clipableObject.IntersectsBound(fieldOfView.transform, fieldOfViewModel))
				{
					clipableObject.UnionWith(fieldOfView, csgOperator);
				}
			}
		}
	}
}
