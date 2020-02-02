using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary Handles player movement and player interaction </summary>
[System.Serializable]
public class Player : MonoBehaviour
{
    [SerializeField] private Transform lastSpawn = default;

    /// <summary> Reference to player CharacterController. </summary>
    private CharacterController characterController;
    /// <summary> Reference to player Camera. </summary>
    private Camera cam;
    /// <summary> Empty GameObject for where to put a Pickupable object. </summary>
    [HideInInspector] public Transform heldObjectLocation;
    /// <summary> Reference to a Pickupable object that has been picked up. </summary>
    private InteractableObject heldObject;
    /// <summary> Vector3 to store and calculate move direction. </summary>
    private Vector3 moveDirection;
    /// <summary> Vector3 to store and calculate vertical velocity. </summary>
    private float verticalVelocity;
    /// <summary> Player's height. </summary>
    private float playerHeight;
    /// <summary> Whether the player can move or not. </summary>
    private bool playerCanMove = true;

    private bool jumping = false;
    private bool crouching = false;

    /// <summary> If the player is holding something or not. </summary>
    [HideInInspector] public bool holding = false;
    /// <summary> Whether the player is inspecting a Pickupable object or not. </summary>
    [HideInInspector] public bool looking = false;

    public GameObject heartWindow;

    public enum ObjectState
    {
        FREE,
        HOLDING,
        INSPECTING
    }

    public ObjectState state = ObjectState.FREE;

    /// <summary> Player move speed. </summary>
    [SerializeField] private float speed = 5f;

    /// <summary> Player gravity variable. </summary>
    [SerializeField] private float gravity = 25f;

    /// <summary> Player jump force. </summary>
    [SerializeField] private float jumpForce = 7f;

    /// <summary> Mouse sensitivity for camera rotation. </summary>
    [SerializeField] private float mouseSensitivity = 2f;

    /// <summary> How far the player can reach to pick something up. </summary>
    [SerializeField] private float playerReach = 4f;

    [Header("Keybinds")]
    /// <summary> KeyCode for player jump. </summary>
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    /// <summary> KeyCode for player crouch. </summary>
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftShift;
    /// <summary> KeyCode for inspecting and picking up objects. </summary>
    [SerializeField] private KeyCode pickUpKey = KeyCode.E;

    // Camera Variables
    /// <summary> Minimum angle the player can look upward. </summary>
    float minX = -90f;
    /// <summary> Minimum angle the player can look downward. </summary>
    float maxX = 90f;

    /// <summary> Stores the Y rotation of the player. </summary>
    float rotationY = 0f;
    /// <summary> Stores the X rotation of the player. </summary>
    float rotationX = 0f;
    /// <summary> Initializes variables before the game starts. </summary>
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();

        // Get reference to the player height using the CharacterController's height.
        playerHeight = characterController.height;

        // Creates an empty game object at the position where a held object should be.
        heldObjectLocation = new GameObject("HeldObjectLocation").transform;
        heldObjectLocation.position = cam.transform.position + cam.transform.forward;
        heldObjectLocation.parent = cam.transform;
    }

    /// <summary> Called once at the start. </summary>
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerCanMove = true;
        holding = false;
        looking = false;
    }

    /// <summary> Called once per frame. </summary>
    void Update()
    {
        if (playerCanMove) // If the player has movement enabled...
        {
            Move(); // Move the player.
            Crouch(); // Crouch.
            Rotate(); // Mouse based rotation for camera and player.
            Cut(); // Player cut powers.
        }

        PickUp(); // Ability to pick up is independent from player movement.
        Die();

    }

    private void Die()
    {
        if (characterController.velocity.y < -30)
        {
            transform.position = lastSpawn == null ? Vector3.zero : lastSpawn.position;
            verticalVelocity = 0;
        }
    }

    /// <summary> Moves and applies gravity to the player using Horizonal and Vertical Axes. </summary>
    private void Move()
    {
        // Get the Vertical and Horizontal Axes and scale them by movement speed.
        moveDirection = Input.GetAxis("Vertical") * transform.forward + Input.GetAxis("Horizontal") * transform.right;
        moveDirection.Normalize();
        GetComponent<PlayerAudioManager>().SetWalkingVelocity(Mathf.RoundToInt(characterController.velocity.magnitude) / speed);
        Debug.Log(Mathf.RoundToInt(characterController.velocity.magnitude) / speed);
        // Scale the moveDirection to account for different runtimes.
        moveDirection *= speed * Time.deltaTime;

        ApplyGravity();
        characterController.Move(moveDirection);

        if (characterController.velocity.y < -30)
        {
            transform.position = lastSpawn.position;
            verticalVelocity = 0;
        }
    }

    /// <summary> Applies gravity to the player and includes jump. </summary>
    void ApplyGravity()
    {
        if (!characterController.isGrounded)
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        Jump();

        // Scale the vertical velocity to account for different runtimes.
        moveDirection.y = verticalVelocity * Time.deltaTime;
    }

    /// <summary> Player jump function. </summary>
    void Jump()
    {
        PlayerAudioManager audioManager = GetComponent<PlayerAudioManager>();
        if (jumping)
        {
            RaycastHit hit;
            int mask = ~gameObject.layer;
            Physics.Raycast(new Ray(transform.position, Vector3.down), out hit, 5f, mask);
            if (verticalVelocity < 0 && hit.distance < audioManager.landingDistanceThreshold)
            {
                GetComponent<PlayerAudioManager>().PlayJumpLanding();
                jumping = false;
            }
        }

        if (characterController.isGrounded)
        {


            if (Input.GetKeyDown(jumpKey))
            {
                verticalVelocity = jumpForce;
                GetComponent<PlayerAudioManager>().PlayJumpLiftoff();
                jumping = true;
            }
        }

    }

    /// <summary> Rotates the player and camera based on mouse movement. </summary>
    private void Rotate()
    {
        // Get the rotation from the Mouse X and Mouse Y Axes and scale them by mouseSensitivity.
        rotationY += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationX += Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Limit the rotation along the x axis.
        rotationX = Mathf.Clamp(rotationX, minX, maxX);

        // Rotate the player along the y axis.
        transform.localEulerAngles = new Vector3(0, rotationY, 0);

        // Rotate the player camera along the x axis.
        // Done separately from player rotation so that movement is not hindered by looking up or down.
        cam.transform.localEulerAngles = new Vector3(-rotationX, 0, 0);

        // Allow the player to get out of the mouse lock.
        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    /// <summary> Player crouch function. </summary>
    /// <remarks> Also checks to see if the player can uncrouch. </remarks>
    private void Crouch()
    {
        // Ray looking straight up from the player's position.
        Ray crouchRay = new Ray(transform.position, Vector3.up);

        // Check if the player is pressing the crouch key.
        if (Input.GetKeyDown(crouchKey))
        {
            characterController.height = playerHeight / 2; // Make the player crouch.
            GetComponent<PlayerAudioManager>().PlayCrouchDown();
            crouching = true;
        }

        // Check if there is anything above the player before uncrouching.
        else if (!Input.GetKey(crouchKey) && !Physics.Raycast(crouchRay, out RaycastHit hit, playerHeight * 3 / 4))
        {
            if (crouching)
            {
                GetComponent<PlayerAudioManager>().PlayCrouchUp();
                characterController.height = playerHeight; // Make the player stand.
                crouching = false;
            }
        }
    }

    /// <summary> Handles player behavior with picking up and inspecting objects. </summary>
    private void PickUp()
    {
        // Check if the player is pressing the pick up key.
        if (Input.GetKeyDown(pickUpKey))
        {
            switch (state)
            {
                case ObjectState.FREE:
                    // Raycast for what the player is looking at.
                    RaycastHit hit;

                    int layerMask = 1 << 9;

                    // Raycast to see what the object's tag is. If it is a Pickupable object...
                    if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, playerReach, layerMask) && hit.transform.GetComponent<InteractableObject>() != null)
                    {
                        // Store the held object.
                        heldObject = hit.collider.gameObject.GetComponent<InteractableObject>();
                        heldObject.Interact();
                        heldObject.active = true;

                        state = ObjectState.HOLDING;
                        playerCanMove = true;
                    }
                    break;
                // If the player is holding something and inspecting it, when the player presses the pick up key...
                case ObjectState.INSPECTING:
                    // Stop inspecting the object
                    heldObject.Interact();

                    state = ObjectState.HOLDING;
                    playerCanMove = true;
                    break;
                // If the player is holding something and not inspecting, when the player presses the pick up key...
                case ObjectState.HOLDING:
                    // Drop the object.
                    heldObject.Interact();

                    heldObject.active = false;
                    state = ObjectState.FREE;
                    break;
            }
        }
    }

    /// <summary> Function to aim and apply player cut power. </summary>
    private void Cut()
    {
        if (Input.GetMouseButton(1) && !holding)
        {
            // Aiming...
            heartWindow.SetActive(true);
            if (Input.GetMouseButtonDown(0))
            {
                heartWindow.GetComponent<Window>().ApplyCut();
            }
        }
        else
        {
            // Not Aiming...
            heartWindow.SetActive(false);
        }
    }

    /// <summary> Function to get transform of where the held object should be. </summary>
    /// <returns> Returns a reference to the player's heldObjectLocation transform. </returns>
    public Transform GetHeldObjectLocation()
    {
        return heldObjectLocation.transform;
    }
}
