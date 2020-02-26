using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public float rotationAngle;
    public Transform leftDoor;
    public Transform rightDoor;
    public GameObject keyHole;

    private bool open;

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
    }

    public void Open()
    {
        if (open) return;
        open = true;

        leftDoor.Rotate(Vector3.up, rotationAngle);
        rightDoor.Rotate(Vector3.up, -rotationAngle);
    }
}
