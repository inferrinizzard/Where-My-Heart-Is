using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class BirdcageWinter : InteractableObject
{
	[FMODUnity.EventRef]
	public string CageEvent;
	public GameObject birdcage;


	public override string prompt { get => $"Press {InputManager.InteractKey} to Open Cage"; }
	public override void Interact()
	{
		birdcage.GetComponent<Animator>().SetBool("open", true);

		Player.Instance.prompt.Disable();
		FMODUnity.RuntimeManager.PlayOneShot(CageEvent, transform.position);
		//Player.VFX.StartFade(false, .05f, false);
		//GameManager.Instance.ChangeLevel();
	}
}
