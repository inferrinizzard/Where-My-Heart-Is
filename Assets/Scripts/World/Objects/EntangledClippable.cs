using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EntangledClippable : ClippableObject
{
	public GameObject heartObject, realObject;
	ClippableObject heartVersion, realVersion;

	private void Start()
	{
		foreach (Transform child in heartObject.transform)
		{
			child.gameObject.layer = heartObject.layer;
			if (child.TryComponent<MeshFilter>() && !child.gameObject.TryComponent<ClippableObject>()) child.gameObject.AddComponent<ClippableObject>();
		}

		foreach (Transform child in realObject.transform)
		{
			child.gameObject.layer = realObject.layer;
			if (child.TryComponent<MeshFilter>() && !child.gameObject.TryComponent<ClippableObject>()) child.gameObject.AddComponent<ClippableObject>();
		}
	}

	public bool Visable => realVersion.gameObject.GetComponent<MeshRenderer>().enabled;

	public override void IntersectWith(CSG.Model model)
	{
		//heartVersion.UnionWith(other, operations);
		//realVersion.Subtract(other, operations);
	}

    public void ClipMirrored(Window window, Bounds mirrorBound, CSG.Model mirrorBoundModel, Matrix4x4 reflectionMatrix, float frameLength)
    {
        isClipped = true; 
        mirroredCopy = Instantiate(gameObject, transform.parent);
        Destroy(mirroredCopy.GetComponent<EntangledClippable>().realObject);

        foreach (ClippableObject clippable in mirroredCopy.GetComponent<EntangledClippable>().heartObject.GetComponentsInChildren<ClippableObject>())
        {
            if (window.IntersectsBounds(clippable, mirrorBound))
            {
                clippable.GetComponent<ClippableObject>().StageIntersectMirroredInPlace(mirrorBoundModel);
            }
        }

        mirroredCopy.transform.position = reflectionMatrix.MultiplyPoint(mirroredCopy.transform.position);
        mirroredCopy.transform.LookAt(mirroredCopy.transform.position + reflectionMatrix.MultiplyVector(mirroredCopy.transform.forward),
            reflectionMatrix.MultiplyVector(mirroredCopy.transform.up));

        foreach (ClippableObject clippable in mirroredCopy.GetComponent<EntangledClippable>().heartObject.GetComponentsInChildren<ClippableObject>())
        {
            if (clippable.isClipped)
            {
                clippable.GetComponent<ClippableObject>().ApplyIntersectMirroredInPlace(reflectionMatrix);
            }
        }
    }

    /*public GameObject Clip()
    {
        isClipped = true;
        uncutCopy
    }*/

	public override void Revert()
	{
        isClipped = false;

        if(mirroredCopy)
        {
            Destroy(mirroredCopy);
        }
    }

	public void OnRealChange(GameObject realPrefab)
	{
		GameObject createdObject = realObject ?
			Instantiate(realPrefab, realObject.transform.position, realObject.transform.rotation, transform) :
			Instantiate(realPrefab, transform);

		if (realObject)
			DestroyImmediate(realObject);

		createdObject.name += " [Real]";
		realObject = createdObject;
		realVersion = ConfigureObject("Real", createdObject);
	}

	public void OnHeartChange(GameObject heartPrefab)
	{
		GameObject createdObject = heartObject ?
			Instantiate(heartPrefab, heartObject.transform.position, heartObject.transform.rotation, transform) :
			Instantiate(heartPrefab, transform);

		if (heartObject)
			DestroyImmediate(heartObject);

		createdObject.name += " [Heart]";
		heartObject = createdObject;
		heartVersion = ConfigureObject("Heart", createdObject);
	}

	private ClippableObject ConfigureObject(string layer, GameObject createdObject)
	{
		foreach (MeshFilter filter in createdObject.GetComponentsInChildren<MeshFilter>())
		{
			GameObject obj = filter.gameObject;
			obj.layer = LayerMask.NameToLayer(layer);
			if (obj.TryComponent<MeshFilter>() && !obj.TryComponent<Collider>()) obj.AddComponent<MeshCollider>();
			if (!obj.TryComponent<ClippableObject>()) obj.AddComponent<ClippableObject>();
		}

		createdObject.layer = LayerMask.NameToLayer(layer);
		if (createdObject.TryComponent<MeshFilter>())
		{
			if (!createdObject.TryComponent<Collider>()) createdObject.AddComponent<MeshCollider>();
			if (!createdObject.TryComponent<ClippableObject>(out ClippableObject clipComponent)) clipComponent = createdObject.AddComponent<ClippableObject>();
			return clipComponent;
		}

		return null;
	}
}
