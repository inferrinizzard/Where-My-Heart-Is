using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public KeyCode moveUp;
    public KeyCode moveDown;

    public float speed;
    public float grabDistance;

    public GameMaster gameMaster;

    private GameObject heldObject;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Grab();
        }

        Move();
        Rotate();
    }

    private void Grab()
    {
        if(heldObject == null)
        {
            Debug.Log("here");
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.forward, out hit, grabDistance))
            {
                heldObject = hit.collider.gameObject;
                Debug.Log(heldObject);
                if (heldObject.GetComponent<InteractableObject>())
                {
                    heldObject.GetComponent<InteractableObject>().Interact(this);
                }
                heldObject = null;
            }
        }
        else
        {
            //heldObject.transform.SetParent(null);
            heldObject = null;
        }
    }

    private void Move()
    {
        float verticalHeading = 0;
        if (Input.GetKey(moveUp))
        {
            verticalHeading += 1;
        }
        if (Input.GetKey(moveDown))
        {
            verticalHeading -= 1;
        }
        Vector3 heading = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal") + Vector3.up * verticalHeading;

        GetComponent<Rigidbody>().velocity = heading * speed;
    }

    private void Rotate()
    {
        //Vector3 rotationAngles = new Vector3(, Input.GetAxis("Mouse Y"));

        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X"), Space.World);
        transform.Rotate(Vector3.right * -Input.GetAxis("Mouse Y"), Space.Self);
    }
}
