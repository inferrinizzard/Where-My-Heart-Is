using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class Hands : MonoBehaviour
{
	Player player;
	Animator anim;
	[SerializeField] float heartAnimSpeed = 1;
	float heartAnimDuration;
	Vector3 heartStartPos, heartStartEulers;
	[SerializeField] AnimationCurve animCurve = default;
	public bool leftHandOn
	{
		get => leftHand.activeSelf;
		set
		{
			leftHand.SetActive(value);
			leftArm.SetActive(value);
		}
	}

	[SerializeField] GameObject leftHand = default, leftArm = default;

	void Start()
	{
		player = Player.Instance;
		anim = GetComponentInChildren<Animator>();
		anim.SetFloat("Speed", heartAnimSpeed);
		heartAnimDuration = anim.runtimeAnimatorController.animationClips[0].length / heartAnimSpeed;
		heartStartPos = anim.transform.localPosition;
		heartStartEulers = anim.transform.localEulerAngles;
	}

	public IEnumerator WaitAndAim()
	{
		anim.SetBool("Aiming", true);

		Vector3 heartTargetPos = new Vector3(.19f, -1.6f, 1.3f); // VS GHETTO
		Vector3 heartTargetEulers = new Vector3(0.2f, -111f, 0f); // VS GHETTO

		// anim.Play("Heart Cut");
		for (var(start, step) = (Time.time, 0f); step <= heartAnimDuration; step = Time.time - start)
		{
			if (!(player.State is Aiming))
			{
				StartCoroutine(Repos(.5f));
				yield break;
			}
			yield return null;
			// TODO: ease these

			anim.speed = animCurve.Evaluate(step / heartAnimDuration);
			/*anim.transform.localPosition = Vector3.Lerp(heartStartPos, heartTargetPos, step / heartAnimDuration);
			anim.transform.localEulerAngles = Vector3.Lerp(heartStartEulers, heartTargetEulers, step / heartAnimDuration);*/
		}

		(player.State as Aiming).DeferredStart();
	}

	public void RevertAim() => StartCoroutine(Repos(heartAnimDuration / 1.5f));

	IEnumerator Repos(float time)
	{
		Vector3 startPos = anim.transform.localPosition;
		Vector3 startEulers = anim.transform.localEulerAngles;

		for (var(start, step) = (Time.time, 0f); step <= time; step = Time.time - start)
		{
			yield return null;
			// TODO: ease these
			anim.transform.localPosition = Vector3.Lerp(heartStartPos, startPos, 1 - step / time);
			anim.transform.localEulerAngles = Vector3.Lerp(heartStartEulers, startEulers, 1 - step / time);
		}
	}

	public void ToggleHands(bool on)
	{
		anim.SetBool("Aiming", on);
	}

}
