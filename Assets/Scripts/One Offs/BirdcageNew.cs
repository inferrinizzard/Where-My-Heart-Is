using System.Collections;
using System.Collections.Generic;

using FMODUnity;

using UnityEngine;
using UnityEngine.UI;

public class BirdcageNew : InteractableObject
{
	[FMODUnity.EventRef]
	public string CageEvent;
	//public Animator animator;
	public GameObject birdcage;
	//public GameObject distress;
	//public Image img;

	public override string prompt { get => $"Press {InputManager.InteractKey} to Cage Bird"; }
	public override void Interact()
	{
		birdcage.GetComponent<Animator>().SetBool("close", true);

		Player.Instance.prompt.Disable();
		FMODUnity.RuntimeManager.PlayOneShot(CageEvent, transform.position);
		AudioMaster.Instance.StopAll();
		//distress.GetComponent<FMODUnity.StudioEventEmitter>().Stop();
		Player.VFX.StartFade(false, .05f, false);
		GameManager.Instance.ChangeLevel();
	}
}
