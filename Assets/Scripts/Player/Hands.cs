using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Hands : MonoBehaviour
{
	Player player;
	Animator anim;
	[SerializeField] float heartAnimSpeed = 1;
	float heartAnimDuration;
	Vector3 heartStartPos, heartStartEulers;

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
		var heartTargetPos = new Vector3(.05f, -1.6f, 1.0f); // VS GHETTO
		var heartTargetEulers = new Vector3(0, -90, 10f); // VS GHETTO

		float start = Time.time;
		bool inProgress = true;
		while (inProgress)
		{
			if (!(player.State is Aiming))
			{
				StartCoroutine(Repos(.5f));
				yield break;
			}
			yield return null;
			float step = Time.time - start;
			// TODO: ease these
			anim.transform.localPosition = Vector3.Lerp(heartStartPos, heartTargetPos, step / heartAnimDuration);
			// anim.transform.localEulerAngles = Vector3.Lerp(heartStartEulers, heartTargetEulers, step / heartAnimDuration);

			if (step > heartAnimDuration)
				inProgress = false;
		}
		(player.State as Aiming).DeferredStart();
	}

	public void RevertAim() => StartCoroutine(Repos(heartAnimDuration / 1.5f));

	IEnumerator Repos(float time)
	{
		Vector3 startPos = anim.transform.localPosition;
		Vector3 startEulers = anim.transform.localEulerAngles;

		float start = Time.time;
		bool inProgress = true;
		while (inProgress)
		{
			yield return null;
			float step = Time.time - start;
			// TODO: ease these
			anim.transform.localPosition = Vector3.Lerp(heartStartPos, startPos, 1 - step / time);
			anim.transform.localEulerAngles = Vector3.Lerp(heartStartEulers, startEulers, 1 - step / time);

			if (step > time)
				inProgress = false;
		}
	}

	public void ToggleHands(bool on) => anim.SetBool("Aiming", on);
}
