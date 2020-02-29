using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public float rotationAngle;
    public float rotationTime;
    public Transform leftDoor;
    public Transform rightDoor;
    public GameObject keyHole;

    private float currentRotation;

    private bool open;
    private bool opening;
    private float rotationDelta;

    private void Start()
    {
        open = false;
    }

    private void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.Space))
        {
            Open();
        }*/
        if(opening)
        {
            Debug.Log("opening");
            leftDoor.Rotate(Vector3.up, rotationDelta * Time.deltaTime);
            rightDoor.Rotate(Vector3.up, -rotationDelta * Time.deltaTime);
        }
    }

    private void DoneOpening()
    {
        Debug.Log("end");

        opening = false;
    }

    public void Open()
    {
        if (open) return;
        open = true;
        opening = true;
        Debug.Log("start");
        rotationDelta = rotationAngle / rotationTime;

        currentRotation = 0;
        Invoke("DoneOpening", rotationTime);
    }
}
