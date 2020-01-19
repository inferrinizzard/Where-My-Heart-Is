using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableObject : InteractableObject
{
    public GameObject twin;
    public AudioClip pickupSound;
    public float threshold;
    public string interactText;

    private bool pickingUp;
    private Vector3 spacialTarget;
    private Vector3 rotationalTarget;

    private void Start()
    {
        pickingUp = false;        
    }

    private void Update()
    {
        if(pickingUp)
        {
            transform.position = Vector3.Lerp(transform.position, spacialTarget, 5f * Time.deltaTime);
            transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.rotation.eulerAngles, rotationalTarget, 4f * Time.deltaTime));

            if(Vector3.Distance(transform.position, spacialTarget) < threshold)
            {
                if(twin != null)
                {
                    twin.SetActive(false);
                }
                gameObject.SetActive(false);


            }
        }
    }

    public override void Interact(PlayerMovement player)
    {
        spacialTarget = player.transform.position;
        rotationalTarget = Quaternion.LookRotation(player.transform.forward, Vector3.up).eulerAngles;
        pickingUp = true;
    }
   
}
