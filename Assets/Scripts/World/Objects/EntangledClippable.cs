using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EntangledClippable : ClippableObject
{
	public ClippableObject heartVersion;
	public ClippableObject realVersion;
	public GameObject heartObject;
	public GameObject realObject;

	string entangledName = $"[|]"; // todo compound names

	private void Start()
	{
		foreach (Transform child in heartObject.transform)
		{
			child.gameObject.layer = heartObject.layer;
			if (child.GetComponent<MeshFilter>() != null)
			{
				if (!child.gameObject.GetComponent<ClippableObject>()) child.gameObject.AddComponent<ClippableObject>();
			}
		}

		foreach (Transform child in realObject.transform)
		{
			child.gameObject.layer = realObject.layer;
			if (child.GetComponent<MeshFilter>() != null)
			{
				if (!child.gameObject.GetComponent<ClippableObject>()) child.gameObject.AddComponent<ClippableObject>();
			}
		}
	}

	public bool Visable
	{
		get => realVersion.gameObject.GetComponent<MeshRenderer>().enabled;

		set
		{
			//heartVersion.gameObject.GetComponent<MeshRenderer>().enabled = value;
			realVersion.gameObject.GetComponent<MeshRenderer>().enabled = value;
		}
	}

	public override void UnionWith(CSG.Model model)
	{
		//heartVersion.UnionWith(other, operations);
		//realVersion.Subtract(other, operations);
	}

	public override void Revert()
	{
		//heartVersion.Revert();
		//realVersion.Revert();
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

	public void OnHeartChange(GameObject heartPrefab)
	{
		GameObject createdObject;
		if (heartObject != null)
		{
			createdObject = Instantiate(heartPrefab, heartObject.transform.position, heartObject.transform.rotation, transform);
			DestroyImmediate(heartObject);
		}
		else
		{
			createdObject = Instantiate(heartPrefab, transform);
		}
		createdObject.name += " [Heart]";
		heartObject = createdObject;
		heartVersion = ConfigureObject("Heart", createdObject);

	}

	private ClippableObject ConfigureObject(string layer, GameObject createdObject)
	{
		foreach (MeshFilter filter in createdObject.GetComponentsInChildren<MeshFilter>())
		{
			filter.gameObject.layer = LayerMask.NameToLayer(layer);
			if (filter.gameObject.GetComponent<MeshFilter>() != null && filter.gameObject.GetComponent<Collider>() == null)
				filter.gameObject.AddComponent<MeshCollider>();
			if (filter.gameObject.GetComponent<ClippableObject>() == null)
				filter.gameObject.AddComponent<ClippableObject>();
		}

		createdObject.layer = LayerMask.NameToLayer(layer);
		if (createdObject.GetComponent<MeshFilter>() != null)
		{
			if (createdObject.GetComponent<Collider>() == null) createdObject.AddComponent<MeshCollider>();
			if (createdObject.GetComponent<ClippableObject>() == null) createdObject.AddComponent<ClippableObject>();

			return createdObject.GetComponent<ClippableObject>();
		}

		return null;
	}
}
