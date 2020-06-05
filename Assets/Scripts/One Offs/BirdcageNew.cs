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

		Invoke("Fade", 1.0f);

		//transition to next scene
		Invoke("Transition", 3.0f);
	}

	void Fade()
	{
		Player.Instance.cam.gameObject.AddComponent<FadeOut>();
		FadeOut(-1);
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
