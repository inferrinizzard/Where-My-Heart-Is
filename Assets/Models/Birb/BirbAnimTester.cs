using BansheeGz.BGSpline.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class BirbAnimTester : MonoBehaviour
{
    [SerializeField] private KeyCode idleKey = KeyCode.Alpha1;
    [SerializeField] private KeyCode flyKey = KeyCode.Alpha2;
    [SerializeField] private BGCcCursor[] curves;

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
        chirpInstance = FMODUnity. RuntimeManager.CreateInstance(ChirpEvent);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(flapInstance, GetComponent<Transform>(), GetComponent<Rigidbody>());
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(chirpInstance, GetComponent<Transform>(), GetComponent<Rigidbody>());
        flapInstance.start();
        chirpInstance.start();
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
    {/*
        Debug.Log(currCurve);
        Debug.Log(curves[currCurve].DistanceRatio);*/
        if(currCurve < curves.Length)
        {
            if (curves[currCurve].DistanceRatio > 0)
            {
                Debug.Log("called!");
            }
            if (curves[currCurve].DistanceRatio > 0.99f)
            {
                anim.SetBool("IsFlying", false);
                flapInstance.setParameterByName("Flying", 0);

                curves[currCurve].GetComponent<BGCcTrs>().Speed = 0;
                currCurve++;
            }
        }
    }

    public void StartNextCurve()
    {
        curves[currCurve].enabled = true;
        StartCoroutine(NextCurve());
    }

    private IEnumerator NextCurve()
    {
        if (currCurve < curves.Length)
        {
            curves[currCurve].DistanceRatio = 0;
            flapInstance.setParameterByName("Flying", 1);

            anim.SetBool("IsFlying", true);
            yield return new WaitForSeconds(0.8f);
            curves[currCurve].GetComponent<BGCcTrs>().Speed = 3;
        }
    }
}
