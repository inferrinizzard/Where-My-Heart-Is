using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : Pickupable
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

	public override void Interact()
	{
		base.Interact();
		if (player.holding == false)
		{
			ApplyCut();
		}
	}

	public void ApplyCut()
	{
		worldManager.ResetCut();

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
		// Debug.Log(worldManager.GetDreamObjects().Count);
		/*foreach (ClipableObject clipableObject in worldManager.GetRealObjects())
		{
			clipableObject.UnionWith(fieldOfView, csgOperator);
		}

		foreach (ClipableObject clipableObject in worldManager.GetDreamObjects())
		{
			clipableObject.Subtract(fieldOfView, csgOperator);
		}

        foreach (ClipableObject clipableObject in worldManager.GetEntangledObjects())
        {
            clipableObject.UnionWith(fieldOfView, csgOperator);
        }*/
	}
}
