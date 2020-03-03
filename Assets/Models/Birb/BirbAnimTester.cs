using BansheeGz.BGSpline.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirbAnimTester : MonoBehaviour
{
    [SerializeField] private KeyCode idleKey = KeyCode.Alpha1;
    [SerializeField] private KeyCode flyKey = KeyCode.Alpha2;
    [SerializeField] private BGCcCursor[] curves;

	private Animator anim;

    private int currCurve;

	void Start()
	{
		anim = GetComponent<Animator>();
	}

	void Update()
	{
        CheckCurvePoints();
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
                anim.SetBool("IsFlying", true);
            }
            if (curves[currCurve].DistanceRatio > 0.99f)
            {
                anim.SetBool("IsFlying", false);
                currCurve++;
                StartCoroutine(NextCurve());
            }
        }
    }

    private IEnumerator NextCurve()
    {
        if (currCurve < curves.Length)
        {
            float tempSpeed = curves[currCurve].GetComponent<BGCcTrs>().Speed;
            curves[currCurve].GetComponent<BGCcTrs>().Speed = 0;
            curves[currCurve].DistanceRatio = 0;
            yield return new WaitForSeconds(3f);
            curves[currCurve].GetComponent<BGCcTrs>().Speed = tempSpeed;
        }
    }
}
