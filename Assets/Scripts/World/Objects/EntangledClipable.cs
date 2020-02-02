using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntangledClipable : ClipableObject
{
	public ClipableObject realVersion;
	public ClipableObject dreamVersion;
	public GameObject realObject;
	public GameObject dreamObject;

	public bool Visable
	{
		get => dreamVersion.gameObject.GetComponent<MeshRenderer>().enabled;

		set
		{
			//realVersion.gameObject.GetComponent<MeshRenderer>().enabled = value;
			dreamVersion.gameObject.GetComponent<MeshRenderer>().enabled = value;
		}
	}

	public override void UnionWith(GameObject other, CSG.Operations operations)
	{
		realVersion.UnionWith(other, operations);
		dreamVersion.Subtract(other, operations);
	}

	public override void Revert()
	{
		realVersion.Revert();
		dreamVersion.Revert();
	}

	public void OnDreamChange(GameObject dreamPrefab)
	{
		GameObject createdObject;
		if (dreamObject != null)
		{
			createdObject = Instantiate(dreamPrefab, dreamObject.transform.position, dreamObject.transform.rotation, transform);
			DestroyImmediate(dreamObject);
		}
		else
		{
			createdObject = Instantiate(dreamPrefab, transform);
		}
		createdObject.name += " [Dream]";
		dreamObject = createdObject;
		dreamVersion = ConfigureObject("Dream", createdObject);
	}

	public void OnRealChange(GameObject realPrefab)
	{
		GameObject createdObject;
		if (realObject != null)
		{
			createdObject = Instantiate(realPrefab, realObject.transform.position, realObject.transform.rotation, transform);
			DestroyImmediate(realObject);
		}
		else
		{
			createdObject = Instantiate(realPrefab, transform);
		}
		createdObject.name += " [Real]";
		realObject = createdObject;
		realVersion = ConfigureObject("Real", createdObject);

	}

	private ClipableObject ConfigureObject(string layer, GameObject createdObject)
	{
		foreach (MeshFilter filter in createdObject.GetComponentsInChildren<MeshFilter>())
		{
			filter.gameObject.layer = LayerMask.NameToLayer(layer);
			if (filter.gameObject.GetComponent<MeshFilter>() != null)
				filter.gameObject.AddComponent<MeshCollider>();
			if (filter.gameObject.GetComponent<ClipableObject>() == null)
				filter.gameObject.AddComponent<ClipableObject>();
		}

		createdObject.layer = LayerMask.NameToLayer(layer);
		if (createdObject.GetComponent<MeshFilter>())
		{
			createdObject.AddComponent<MeshCollider>();
			return createdObject.AddComponent<ClipableObject>();
		}

		return null;
	}
}
