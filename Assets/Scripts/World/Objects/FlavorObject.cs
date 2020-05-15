 using System.Collections.Generic;
 using System.Collections;

 using FMODUnity;

 using UnityEngine;

 public class FlavorObject : InteractableObject
 {
 	[FMODUnity.EventRef] public string InteractSoundEvent;

 	public override void Interact()
 	{
 		base.Interact();
 		if (InteractSoundEvent != "")
 		{
 			FMODUnity.RuntimeManager.PlayOneShot(InteractSoundEvent, transform.position);
 		}
 	}

 	void OnMouseEnter()
 	{
 		if (!player.heldObject && !this.TryComponent<OutlineObject>() && (transform.position - player.transform.position).sqrMagnitude < player.playerReach * player.playerReach)
 			Effects.Instance.SetGlow(this);
 	}

 	void OnMouseExit() => Effects.Instance.SetGlow(null);
 }
