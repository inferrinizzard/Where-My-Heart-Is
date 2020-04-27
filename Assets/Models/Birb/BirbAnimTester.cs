using System.Collections;
using System.Collections.Generic;
using System.Linq;

using CurveTRS = BansheeGz.BGSpline.Components.BGCcTrs;
using CurveCursor = BansheeGz.BGSpline.Components.BGCcCursor;
// using CurveObject = BansheeGz.BGSpline.Components.BGCcCursorObjectTranslate;

using UnityEngine;
public class BirbAnimTester : MonoBehaviour
{
	[SerializeField] Transform curveHolder = default;
	CurveCursor[] curves;
	[SerializeField] Transform triggerHolder = default;
	List<PlayerTrigger> pathTriggers;
	[SerializeField] float flySpeed;
	[FMODUnity.EventRef] public string FlapEvent;
	[FMODUnity.EventRef] public string ChirpEvent;
	private FMOD.Studio.EventInstance flapInstance;
	private FMOD.Studio.EventInstance chirpInstance;
	private Animator anim;
	private int curveIndex = 0;

	void Start()
	{
		anim = GetComponent<Animator>();
		if (triggerHolder)
			pathTriggers = triggerHolder?.GetComponentsInChildren<PlayerTrigger>().Select(trigger => { trigger.OnPlayerEnterID += StartNextCurveID; return trigger; }).ToList();

		curves = curveHolder.GetComponentsInChildren<CurveCursor>();
		curves.Select((curve, i) =>
		{
			var trs = curve.GetComponent<CurveTRS>();
			trs.DistanceRatio = 0;
			trs.Speed = 0;
			// trs.GetComponent<CurveObject>().objectToManipulate = null;
			curve.DistanceRatio = 0;
			return curve;
		});

		flapInstance = FMODUnity.RuntimeManager.CreateInstance(FlapEvent);
		chirpInstance = FMODUnity.RuntimeManager.CreateInstance(ChirpEvent);
		foreach (var i in new [] { flapInstance, chirpInstance })
		{
			FMODUnity.RuntimeManager.AttachInstanceToGameObject(i, transform, GetComponent<Rigidbody>());
			i.start();
		}

		flapInstance.setParameterByName("Flying", 0);

		StartNextCurve();
		//StartCoroutine(NextCurve());
	}

	void Update()
	{
		// CheckCurvePoints();
	}

	private void OnDestroy()
	{
		flapInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		chirpInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
	}

	// private void CheckCurvePoints()
	// {
	// 	/*
	// 	        Debug.Log(curveIndex);
	// 	        Debug.Log(curves[curveIndex].DistanceRatio);*/
	// 	if (curveIndex < curves.Length)
	// 	{
	// 		if (curves[curveIndex].DistanceRatio > 0)
	// 		{
	// 			Debug.Log(curves[curveIndex].DistanceRatio);
	// 		}
	// 		if (curves[curveIndex].DistanceRatio > 0.999f)
	// 		{
	// 			anim.SetBool("IsFlying", false);
	// 			flapInstance.setParameterByName("Flying", 0);

	// 			curves[curveIndex].GetComponent<CurveTRS>().Speed = 0;
	// 		}
	// 	}
	// }

	public void StartNextCurveID(PlayerTrigger trigger)
	{
		if (pathTriggers.Contains(trigger))
		{
			trigger.OnPlayerEnterID -= StartNextCurveID;
			StartNextCurve();
		}
	}

	// public void StartNextCurve() => StartCoroutine(NextCurve());
	public void StartNextCurve()
	{
		var currentCurve = curves[curveIndex++];
		var currentTRS = currentCurve.GetComponent<CurveTRS>();
		currentCurve.enabled = true;
		currentTRS.Speed = flySpeed;
		flapInstance.setParameterByName("Flying", 1);
		anim.SetBool("IsFlying", true);
		Debug.Log(transform.position);
	}

	private IEnumerator NextCurve(float time = .8f)
	{
		var currentCurve = curves[curveIndex++];
		var currentTRS = currentCurve.GetComponent<CurveTRS>();
		currentCurve.enabled = true;
		// currentTRS.GetComponent<CurveObject>().objectToManipulate = transform;
		currentTRS.Speed = flySpeed;

		flapInstance.setParameterByName("Flying", 1);
		anim.SetBool("IsFlying", true);
		Debug.Log(transform.position);

		// yield return new WaitForSeconds(0.8f);

		float start = Time.time;
		bool inProgress = true;
		while (inProgress)
		{
			float step = Time.time - start;
			currentTRS.DistanceRatio = step / time;
			Debug.Log(currentTRS.DistanceRatio);

			if (step > time)
				yield break;
		}

		// currentTRS.GetComponent<CurveObject>().objectToManipulate = null;
		currentCurve.DistanceRatio = 1;
	}

	public void StopChirps()
	{
		chirpInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
	}
}
