﻿using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class FlavorObject : InteractableObject
{
	[FMODUnity.EventRef]
	public string InteractSoundEvent;

	public override void Interact()
	{
		base.Interact();
		if (InteractSoundEvent != "")
		{
			FMODUnity.RuntimeManager.PlayOneShot(InteractSoundEvent, transform.position);
		}
	}
}
