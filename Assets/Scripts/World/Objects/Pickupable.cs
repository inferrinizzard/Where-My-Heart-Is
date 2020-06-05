using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary> Handles the behavior of an object that can be picked up. </summary>
public class Pickupable : InteractableObject
{
	[HideInInspector] public Transform oldParent;

	public bool birdTrigger;

	protected Vector3 initialPosition;
	protected Quaternion initialRotation;
	/// <summary> Whether or not this is the active item </summary>
	[HideInInspector] public bool active;
	public bool dissolves = false;
	Collider col;
	public override string prompt { get => "Press " + ParseKey(InputManager.interactKey.ToString()) + " to Pick Up"; }

	protected override void Start()
	{
		base.Start();
		col = GetComponent<Collider>();
	}

	// void Update()
	// {
	// 	if (active)
	// 	{
	// 		// If the object is being inspected, run Looking.
	// 		if (player.looking) Looking();
	// 	}
	// }

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
		// else if (player.looking)
		// 	player.looking = false;
		else if (!dissolves)
			PutDown();
		else
			StartCoroutine(DissolveOnDrop());
	}

	public virtual void PickUp()
	{
		initialPosition = transform.position;
		initialRotation = transform.rotation;

		(col as MeshCollider).convex = true;
		col.isTrigger = true;
		// col.enabled = false;
		oldParent = transform.parent;
		transform.parent = player.heldObjectLocation; // set the new parent to the hold object location object
		transform.localPosition = Vector3.zero; // set the position to local zero to match the position of the hold object location target

		if (birdTrigger)
		{
			GameObject.Find("/Bird").GetComponent<Bird>().StartNextCurve();
		}
	}

	public void PutDown()
	{
		transform.position = initialPosition;
		transform.rotation = initialRotation;

		transform.parent = oldParent;
		col.isTrigger = false;
		(col as MeshCollider).convex = false;
		// col.enabled = true;
		// if (Physics.Raycast(transform.position, player.transform.position - transform.position, out RaycastHit hit, (player.transform.position - transform.position).magnitude + 1))
		// 	if (hit.transform != player.transform)
		// 		Debug.Log(hit.transform.gameObject);
	}

	public IEnumerator DissolveOnDrop(float time = .25f)
	{
		transform.parent = oldParent;
		col.enabled = false;
		Material mat = GetComponent<MeshRenderer>().material;
		mat.EnableKeyword("DISSOLVE_MANUAL");

		float start = Time.time;
		bool inProgress = true;

		while (inProgress)
		{
			yield return null;
			float step = Time.time - start;
			mat.SetFloat(ShaderID._ManualDissolve, step / time);
			if (step > time)
				inProgress = false;
		}
		mat.DisableKeyword("DISSOLVE_MANUAL");
		mat.SetFloat(ShaderID._ManualDissolve, 1);

		PutDown();

		col.enabled = true;
		active = false;
	}
}
