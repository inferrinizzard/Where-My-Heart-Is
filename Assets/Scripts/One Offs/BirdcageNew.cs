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

	public override string prompt { get => "Press E to Cage Bird"; }
	public override void Interact()
	{
		birdcage.GetComponent<Animator>().SetBool("close", true);

		Player.Instance.prompt.Disable();
		FMODUnity.RuntimeManager.PlayOneShot(CageEvent);
		AudioMaster.Instance.StopAll();
		//distress.GetComponent<FMODUnity.StudioEventEmitter>().Stop();
		Player.Instance.canMove = false;
		Effects.Instance.StartFade(false, .05f, false);
		GameManager.Instance.ChangeLevel();
	}
}
