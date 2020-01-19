using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : MonoBehaviour
{
    private bool holding = false;
    private bool looking = false;
    private Vector3 targetPos;
    private PlayerMovement player;

    void Update()
    {
        if(holding) Holding();
        if(looking) Looking();
    }

    public void Holding()
    {
        targetPos = player.GetHeldObjectLocation().position;
        transform.position = targetPos;
        if(!looking)
        {
            // Rotate the object based on the player camera
            transform.parent = player.GetHeldObjectLocation();
        }
    }

    public void Looking()
    {
        float rotX = Input.GetAxis("Mouse X") * 2f;
        float rotY = Input.GetAxis("Mouse Y") * 2f;

        transform.rotation = Quaternion.AngleAxis(-rotX, player.GetHeldObjectLocation().up) * transform.rotation;
        transform.rotation = Quaternion.AngleAxis(rotY, player.GetHeldObjectLocation().right) * transform.rotation;
    }

    public void StopLooking()
    {
        looking = false;
    }

    public void PickUp(PlayerMovement player)
    {
        this.player = player;
        looking = true;
        holding = true;
    }

    public void Drop()
    {
        holding = false;
        transform.parent = null;
    }
}
