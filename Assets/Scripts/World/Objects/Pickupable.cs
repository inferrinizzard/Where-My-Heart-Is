using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary> Handles the behavior of an object that can be picked up. </summary>
public class Pickupable : InteractableObject
{
	[HideInInspector] public Transform oldParent;

	protected Vector3 initialPosition;
	protected Quaternion initialRotation;
	public bool dissolves = false;
	Collider col;
	public override string prompt { get => "Press E to Pick Up"; }

	protected override void Start()
	{
		base.Start();
		col = GetComponent<Collider>();
	}

	void Update()
	{
		if (active)
		{
			// If the object is being inspected, run Looking.
			if (player.looking) Looking();
		}
	}

	/// <summary> Manages behavior of the object when being inspected. </summary>
	public void Looking()
	{
		// Set the rotations based on the mouse movement.
		float rotX = Input.GetAxis("Mouse X") * 2f;
		float rotY = Input.GetAxis("Mouse Y") * 2f;

		// Rotate the object based on previous rotations.
		transform.rotation = Quaternion.AngleAxis(-rotX, player.heldObjectLocation.up) * transform.rotation;
		transform.rotation = Quaternion.AngleAxis(rotY, player.heldObjectLocation.forward) * transform.rotation;
	}

	public override void Interact()
	{
		if (!player.heldObject)
			PickUp();
		else if (player.looking)
			player.looking = false;
		else
		{
			if (!dissolves)
				PutDown();
			else
				StartCoroutine(Dissolve());
		}
	}

	public void PickUp()
	{
		initialPosition = transform.position;
		initialRotation = transform.rotation;

		(col as MeshCollider).convex = true;
		oldParent = transform.parent;
		transform.parent = player.heldObjectLocation; // set the new parent to the hold object location object
		transform.localPosition = Vector3.zero; // set the position to local zero to match the position of the hold object location target
	}

	public void PutDown()
	{
		transform.position = initialPosition;
		transform.rotation = initialRotation;

		transform.parent = oldParent;
		(col as MeshCollider).convex = false;
	}

	public void Disintegrate() => StartCoroutine(Dissolve(true));

	public IEnumerator Dissolve(bool destroy = false, float time = .25f)
	{
		transform.parent = World.Instance.realWorldContainer;
		col.enabled = false;
		Material mat = GetComponent<MeshRenderer>().material;
		mat.EnableKeyword("DISSOLVE_MANUAL");
		int ManualDissolveID = Shader.PropertyToID("_ManualDissolve");

		for (var(start, step) = (Time.time, 0f); step <= time; step = Time.time - start)
		{
			yield return null;
			// Debug.Log("dissolving");
			mat.SetFloat(ManualDissolveID, EaseMethods.CubicEaseOut(step / time, 0, 1, 1));
		}
		yield return new WaitForEndOfFrame();

		mat.DisableKeyword("DISSOLVE_MANUAL");
		mat.SetFloat(ManualDissolveID, 1);

		if (destroy)
			Destroy(gameObject);
		else
			PutDown();

		col.enabled = true;
		active = false;
	}
}
