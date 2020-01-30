using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : Pickupable
{
	public WorldManager worldManager;
	public GameObject fieldOfView;

	CSG.Operations csgOperator;

	void Start()
	{
		csgOperator = GetComponent<CSG.Operations>();
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
			//TODO: Here's where we check if we should cut this one or not
			//clipableObject.gameObject.GetComponent<MeshRenderer>().enabled = false;
			clipableObject.UnionWith(fieldOfView, csgOperator);
		}

		foreach (ClipableObject clipableObject in worldManager.GetDreamObjects())
		{
			clipableObject.Subtract(fieldOfView, csgOperator);
		}

		foreach (EntangledClippable clipableObject in worldManager.GetEntangledObjects())
		{
			clipableObject.UnionWith(fieldOfView, csgOperator);
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
