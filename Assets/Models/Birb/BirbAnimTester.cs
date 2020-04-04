using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BansheeGz.BGSpline.Components;

using UnityEngine;
public class BirbAnimTester : MonoBehaviour
{
	// [SerializeField] private KeyCode idleKey = KeyCode.Alpha1;
	// [SerializeField] private KeyCode flyKey = KeyCode.Alpha2;
	[SerializeField] private BGCcCursor[] curves = default;
	public List<PlayerTrigger> pathTriggers;
	public float flySpeed;

	[FMODUnity.EventRef]
	public string FlapEvent;
	[FMODUnity.EventRef]
	public string ChirpEvent;

	private FMOD.Studio.EventInstance flapInstance;
	private FMOD.Studio.EventInstance chirpInstance;

	private Animator anim;

	private int currCurve;

	void Start()
	{
		currCurve = 0;
		anim = GetComponent<Animator>();
		curves[currCurve].GetComponent<BGCcTrs>().Speed = 0;
		curves.ToList().ForEach(curve =>
		{
			curve.GetComponent<BGCcTrs>().DistanceRatio = 0;
			curve.GetComponent<BGCcTrs>().Speed = 0;
			curve.DistanceRatio = 0;
		});
		curves[currCurve].DistanceRatio = 0;

		flapInstance = FMODUnity.RuntimeManager.CreateInstance(FlapEvent);
		flapInstance.setParameterByName("Flying", 0);
		chirpInstance = FMODUnity.RuntimeManager.CreateInstance(ChirpEvent);
		FMODUnity.RuntimeManager.AttachInstanceToGameObject(flapInstance, transform, GetComponent<Rigidbody>());
		FMODUnity.RuntimeManager.AttachInstanceToGameObject(chirpInstance, transform, GetComponent<Rigidbody>());
		flapInstance.start();
		chirpInstance.start();
		pathTriggers.ForEach(trigger => trigger.OnPlayerEnterID += StartNextCurveID);

		currCurve = -1;
	}

	void Update()
	{
		CheckCurvePoints();
	}

	private void OnDestroy()
	{
		flapInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		chirpInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
	}

	private void CheckCurvePoints()
	{
		/*
		        Debug.Log(currCurve);
		        Debug.Log(curves[currCurve].DistanceRatio);*/
		if (currCurve < curves.Length && currCurve != -1)
		{
			if (curves[currCurve].DistanceRatio > 0) { }
			if (curves[currCurve].DistanceRatio > 0.999f)
			{
				anim.SetBool("IsFlying", false);
				flapInstance.setParameterByName("Flying", 0);

				curves[currCurve].GetComponent<BGCcTrs>().Speed = 0;
			}
		}
	}

	public void StartNextCurve()
	{
		currCurve++;
		curves[currCurve].enabled = true;
		StartCoroutine(NextCurve());
	}

	public void StartNextCurveID(PlayerTrigger trigger)
	{
		if (pathTriggers.Contains(trigger))
		{
			trigger.OnPlayerEnterID -= StartNextCurveID;
			StartNextCurve();
		}
	}

	private IEnumerator NextCurve()
	{
		if (currCurve < curves.Length && currCurve != -1)
		{
			curves[currCurve].DistanceRatio = 0;
			flapInstance.setParameterByName("Flying", 1);

			anim.SetBool("IsFlying", true);
			yield return new WaitForSeconds(0.8f);
			curves[currCurve].GetComponent<BGCcTrs>().Speed = flySpeed;
		}
	}

	public void StopChirps()
	{
		chirpInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
	}
}
