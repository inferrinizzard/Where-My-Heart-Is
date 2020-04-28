using System.Collections;
using System.Collections.Generic;
using System.Linq;

using CurveTRS = BansheeGz.BGSpline.Components.BGCcTrs;
using CurveCursor = BansheeGz.BGSpline.Components.BGCcCursor;
using Curve = BansheeGz.BGSpline.Curve.BGCurve;
// using CurveObject = BansheeGz.BGSpline.Components.BGCcCursorObjectTranslate;

using UnityEngine;
public class BirbAnimTester : MonoBehaviour
{
	[SerializeField] Transform curveHolder = default;
	List<CurveCursor> cursors;
	List<Curve> curves;
	[SerializeField] Transform triggerHolder = default;
	List<PlayerTrigger> pathTriggers;
	[SerializeField] float flySpeed;
	[FMODUnity.EventRef] public string FlapEvent;
	[FMODUnity.EventRef] public string ChirpEvent;
	private FMOD.Studio.EventInstance flapInstance;
	private FMOD.Studio.EventInstance chirpInstance;
	private Animator anim;
	private int curveIndex = -1;

	void Start()
	{
		anim = GetComponent<Animator>();
		if (triggerHolder)
			pathTriggers = triggerHolder?.GetComponentsInChildren<PlayerTrigger>().Select(trigger => { trigger.OnPlayerEnterID += StartNextCurveID; return trigger; }).ToList();

		curves = curveHolder.GetComponentsInChildren<Curve>().ToList();
		cursors = curveHolder.GetComponentsInChildren<CurveCursor>().ToList();
		curves.ForEach(c => Debug.Log(c.Points.AsString()));
		cursors.ForEach(cursor =>
		{
			var trs = cursor.GetComponent<CurveTRS>();
			trs.DistanceRatio = 0;
			trs.Speed = 0;
			cursor.DistanceRatio = 0;
		});

		transform.position = curves[0].Points.First().PositionWorld;

		flapInstance = FMODUnity.RuntimeManager.CreateInstance(FlapEvent);
		chirpInstance = FMODUnity.RuntimeManager.CreateInstance(ChirpEvent);
		foreach (var i in new [] { flapInstance, chirpInstance })
		{
			FMODUnity.RuntimeManager.AttachInstanceToGameObject(i, transform, GetComponent<Rigidbody>());
			i.start();
		}

		flapInstance.setParameterByName("Flying", 0);
	}

	void Update()
	{
		// CheckCurvePoints();
		if (Input.GetMouseButtonDown(0))
			StartNextCurve();

		if (transform.position == curves[curveIndex].Points.Last().PositionWorld)
			ReachedEnd();
	}

	void ReachedEnd()
	{
		flapInstance.setParameterByName("Flying", 0);
		anim.SetBool("IsFlying", false);
		// cursors[curveIndex - 1].GetComponent<CurveTRS>().Speed = 0;
	}

	private void OnDestroy()
	{
		flapInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		chirpInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
	}

	public void StartNextCurveID(PlayerTrigger trigger)
	{
		if (pathTriggers.Contains(trigger))
		{
			trigger.OnPlayerEnterID -= StartNextCurveID;
			StartNextCurve();
		}
	}

	public void StartNextCurve()
	{
		var currentCurve = cursors[++curveIndex];
		var currentTRS = currentCurve.GetComponent<CurveTRS>();
		currentCurve.enabled = true;
		currentTRS.Speed = flySpeed;
		flapInstance.setParameterByName("Flying", 1);
		anim.SetBool("IsFlying", true);
		Debug.Log(transform.position);
	}

	public void StopChirps()
	{
		chirpInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
	}
}
