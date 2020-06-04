using System.Collections;
using System.Collections.Generic;

using FMODUnity;

using UnityEngine;
using UnityEngine.UI;

public class BirdcageNew : InteractableObject
{
	[FMODUnity.EventRef]
	public string CageEvent;
	
	//public GameObject distress;
	//public Image img;

	public override string prompt { get => "Press E to Cage Bird"; }
	public override void Interact()
	{
		// door close animation goes here

		Player.Instance.cam.gameObject.AddComponent<FadeOut>();
		FadeOut(-1);
		Player.Instance.prompt.Disable();
		FMODUnity.RuntimeManager.PlayOneShot(CageEvent);
		AudioMaster.Instance.StopAll();
		//distress.GetComponent<FMODUnity.StudioEventEmitter>().Stop();
		Player.Instance.canMove = false;

		//transition to next scene
     	Invoke("Transition", 2.0f);
	}

	void Transition()
	{
		StartCoroutine(Player.Instance.mask.PreTransition());
		FadeOut(1);
	}

	IEnumerator FadeOut(int fade)
	{
		float fadeTime = Player.Instance.cam.gameObject.GetComponent<FadeOut>().BeginFade(fade);
		yield return new WaitForSeconds(fadeTime);
	}

}
