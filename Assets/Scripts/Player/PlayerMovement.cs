using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Handles player movement and player interaction.
/// </summary>
[System.Serializable]
public class PlayerMovement : MonoBehaviour
{
    // Reference to player CharacterController
    private CharacterController characterController;

    // Reference to player Camera
    private Camera cam;
    private GameObject heldObjectLocation;
    private GameObject heldObject;
    private Vector3 moveDirection;
    private float verticalVelocity;
    private float playerHeight;
    private bool playerMovement = true;
    private bool holding = false;
    private bool looking = false;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float gravity = 25f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float playerReach = 4f;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode pickUpKey = KeyCode.E;

    // Camera Variables
    float minX = -90f;
    float maxX = 90f;

    float rotationY = 0f;
    float rotationX = 0f;

    /// <summary>
    /// Initializes variables before the game starts.
    /// </summary>
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
        playerHeight = characterController.height;

        // Creates an empty game object at the position where a held item should be
        heldObjectLocation = new GameObject("HeldObjectLocation");
        heldObjectLocation.transform.parent = cam.transform;
        heldObjectLocation.transform.position = cam.transform.position + Vector3.forward;

    }

    /// <summary>
    /// Called when the script is enabled.
    /// </summary>
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerMovement = true;
        holding = false;
        looking = false;
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    void Update()
    {
        if(playerMovement)
        {
            Move();
            Crouch();
            Rotate();
        }

        PickUp();
    }

    /// <summary>
    /// Moves and applies gravity to the player using Horizonal and Vertical Axes.
    /// </summary>
    private void Move()
    {
        moveDirection = Input.GetAxis("Vertical") * transform.forward + Input.GetAxis("Horizontal") * transform.right;
        moveDirection *= speed * Time.deltaTime;

        ApplyGravity();

        characterController.Move(moveDirection);
    }

    /// <summary>
    /// Applies gravity to the player and includes jump.
    /// </summary>
    void ApplyGravity()
    {
        if(!characterController.isGrounded)
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        Jump();

        moveDirection.y = verticalVelocity * Time.deltaTime;
    }

    /// <summary>
    /// Player jump function.
    /// </summary>
    void Jump()
    {
        if(characterController.isGrounded && Input.GetKeyDown(jumpKey))
        {
            verticalVelocity = jumpForce;
        }
    }

    /// <summary>
    /// Rotates the player and camera based on mouse movement.
    /// </summary>
    private void Rotate()
    {
        rotationY += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationX += Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationX = Mathf.Clamp(rotationX, minX, maxX);

        transform.localEulerAngles = new Vector3(0, rotationY, 0);
        cam.transform.localEulerAngles = new Vector3(-rotationX, 0, 0);

        if(Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// Player crouch function.
    /// </summary>
    /// <remarks>
    /// Also checks to see if the player can uncrouch.
    /// </remarks>
    private void Crouch()
    {
        Ray crouchRay = new Ray(transform.position, Vector3.up);

        if(Input.GetKey(crouchKey))
        {
            characterController.height = playerHeight/2;
        }
        else if(!Physics.Raycast(crouchRay, out RaycastHit hit, playerHeight * 3 / 4)) // Check if there is anything above the player
        {
            characterController.height = playerHeight;
        }
    }

    /// <summary>
    /// Handles player behavior with picking up and inspecting items.
    /// </summary>
    private void PickUp()
    {
        RaycastHit hit;
        if(Input.GetKeyDown(pickUpKey))
        {
            if(!holding)
            {
                if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, playerReach) && hit.transform.tag == "Pickupable")
                {
                    //Debug.Log(hit.transform.tag);
                    //Debug.DrawRay(cam.transform.position, cam.transform.forward * playerReach, Color.red, 2);
                    heldObject = hit.collider.gameObject;
                    heldObject.GetComponent<Pickupable>().PickUp(this);
                    looking = true;
                    holding = true;
                    playerMovement = false;
                }
            }
            else if(looking)
            {
                heldObject.GetComponent<Pickupable>().StopLooking();
                looking = false;
                playerMovement = true;
            }
            else
            {
                heldObject.GetComponent<Pickupable>().Drop();
                holding = false;
            }
        }
    }

    /// <summary>
    /// Function to get transform of where the held object should be.
    /// </summary>
    /// <returns>
    /// Returns a reference to the player's heldObjectLocation transform.
    /// </returns>
    public Transform GetHeldObjectLocation()
    {
        return heldObjectLocation.transform;
    }    
}
