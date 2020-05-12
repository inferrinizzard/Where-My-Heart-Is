using System.Collections;
using System.Collections.Generic;
using System.Linq;

using CurveTRS = BansheeGz.BGSpline.Components.BGCcTrs;
using CurveCursor = BansheeGz.BGSpline.Components.BGCcCursor;
using Curve = BansheeGz.BGSpline.Curve.BGCurve;
// using CurveObject = BansheeGz.BGSpline.Components.BGCcCursorObjectTranslate;

using UnityEngine;
public class Bird : MonoBehaviour
{
	[SerializeField] Transform curveHolder = default;
	List<CurveCursor> cursors;
	List<Curve> curves;
	[SerializeField] Transform triggerHolder = default;
	List<PlayerTrigger> pathTriggers;
	[SerializeField] float flySpeed = 3;
	[FMODUnity.EventRef] public string FlapEvent;
	[FMODUnity.EventRef] public string ChirpEvent;
	private FMOD.Studio.EventInstance flapInstance;
	private FMOD.Studio.EventInstance chirpInstance;
	private Animator anim;
	private int curveIndex = -1;
	[HideInInspector] public bool flying = false;

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

		if (!flying)
			anim.SetFloat("IdleBlend", Mathf.PingPong(Time.time, 1));
		if (curveIndex < curves.Count - 1)
			if (transform.position == curves[curveIndex].Points.Last().PositionWorld)
				ReachedEnd();
	}

	void ReachedEnd()
	{
		flying = false;
		flapInstance.setParameterByName("Flying", 0);
		anim.SetTrigger("Land");
		StartCoroutine(DriveBlend("LandingBlend", .25f)); // TODO: do beforehand
		anim.ResetTrigger("Takeoff");
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
		if (curveIndex < cursors.Count)
		{
			var currentCurve = cursors[++curveIndex];
			var currentTRS = currentCurve.GetComponent<CurveTRS>();
			currentCurve.enabled = true;
			currentTRS.Speed = flySpeed;
			flapInstance.setParameterByName("Flying", 1);
			anim.SetTrigger("Takeoff");
			StartCoroutine(DriveBlend("TakeoffBlend"));
			// Debug.Log(transform.position);
			flying = true;
		}
	}

	IEnumerator DriveBlend(string blend, float time = 1f) //TODO: add Ease support
	{
		for (float start = Time.time; Time.time - start < time;)
		{
			anim.SetFloat(blend, (Time.time - start) / time);
			yield return null;
		}
	}

	public void StopChirps()
	{
		chirpInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
	}
}
