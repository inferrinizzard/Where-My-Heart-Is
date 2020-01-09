using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : InteractableObject
{
    public bool opening;
    public GameObject toRotate;
    private float targetRotation = 90;
    private float currentRotation;
    private Vector3 initialRotation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(opening)
        {
            //toRotate.transform.rotation = Quaternion.Euler(Vector3.Lerp(currentRotation, targetRotation, 2 * Time.deltaTime));
            currentRotation = Mathf.Lerp(currentRotation, targetRotation, 1f * Time.deltaTime);
            if(Mathf.Abs(currentRotation - targetRotation) <= 10)
            {
                opening = false;
                currentRotation = targetRotation;
            }
            Debug.Log(currentRotation);
            toRotate.transform.rotation = Quaternion.Euler(initialRotation + new Vector3(0, currentRotation, 0));

        }
    }

    public void Open()
    {
        opening = true;
        currentRotation = 0;
        initialRotation = toRotate.transform.rotation.eulerAngles;
    }

    public override void Interact(PlayerManager player)
    {
        Open();
    }
}
