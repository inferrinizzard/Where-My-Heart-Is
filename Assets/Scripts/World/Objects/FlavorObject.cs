using System.Collections;
using System.Collections.Generic;

using FMODUnity;

using UnityEngine;

public class FlavorObject : InteractableObject
{
	[FMODUnity.EventRef] public string InteractSoundEvent;
	string flavorText = "";
	DialogueSystem dialogue;

	protected override void Start()
	{
		base.Start();
		dialogue = GameManager.Instance.dialogue;
	}
	public override void Interact()
	{
		// if (flavorText != "")
		// 	StartCoroutine(dialogue.WriteDialogue(flavorText));
		if (InteractSoundEvent != "")
		{
			FMODUnity.RuntimeManager.PlayOneShot(InteractSoundEvent, transform.position);
		}
	}

	void OnMouseEnter()
	{
		if (!player.heldObject && !this.TryComponent<OutlineObject>() && (transform.position - player.transform.position).sqrMagnitude < player.playerReach * player.playerReach)
			Player.VFX.SetGlow(this);
	}

	void OnMouseExit() => Player.VFX.SetGlow(null);
}
