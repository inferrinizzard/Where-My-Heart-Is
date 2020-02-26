using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Handles the behavior of an object that can be picked up. </summary>
public class Pickupable : InteractableObject
{
	protected Transform oldParent;

	protected Vector3 initialPosition;
	protected Quaternion initialRotation;

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

	public void PickUp()
	{
		player.holding = true;

		initialPosition = transform.position;
		initialRotation = transform.rotation;

		oldParent = transform.parent;
		transform.parent = player.heldObjectLocation; // set the new parent to the hold object location object
		transform.localPosition = Vector3.zero; // set the position to local zero to match the position of the hold object location target
	}

	public void PutDown()
	{
		ClipableObject clipable = GetComponent<ClipableObject>();

		if (clipable != null && !clipable.IntersectsBound(player.window.fieldOfView.transform, player.window.fieldOfViewModel))
		{
			if (clipable.uncutCopy != null)
			{
				transform.position = initialPosition;
				transform.rotation = initialRotation;
			}
		}

		player.holding = false;
		transform.parent = oldParent;
	}

	public override void Interact()
	{
		if (!player.holding)
		{
			PickUp();
		}
		else if (player.looking)
		{
			player.looking = false;
		}
		else
		{
			PutDown();
		}
	}
}
