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
	public WorldManager worldManager;
	public GameObject fieldOfView;

	private CSG.Operations csgOperator;
    private CSG.Model fieldOfViewModel;

	void Start()
	{
		csgOperator = GetComponent<CSG.Operations>();
        fieldOfViewModel = new CSG.Model(fieldOfView.GetComponent<MeshFilter>().mesh);
    }

	public void ApplyCut()
	{
		worldManager.ResetCut();

        // real world objects get intersected with the bound
        foreach (ClipableObject clipableObject in worldManager.GetRealObjects())
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
		foreach (ClipableObject clipableObject in worldManager.GetDreamObjects())
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
		foreach (EntangledClippable clipableObject in worldManager.GetEntangledObjects())
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
	}
}
