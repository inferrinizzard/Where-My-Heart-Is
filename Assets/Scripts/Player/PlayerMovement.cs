using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;
    private GameObject lookRoot;
    private Vector3 moveDirection;
    private float verticalVelocity;
    private float playerHeight;

    [SerializeField] private float speed;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpForce;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private KeyCode jumpKey;
    [SerializeField] private KeyCode crouchKey;

    // Camera Variables
    float minX = -85f;
    float maxX = 85f;

    float rotationY = 0f;
    float rotationX = 0f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        lookRoot = GameObject.FindGameObjectWithTag("LookRoot");
        playerHeight = characterController.height;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Move();
        Crouch();
        Rotate();
    }

    // Move Player using Horizontal and Vertical Axes
    private void Move()
    {
        moveDirection = Input.GetAxis("Vertical") * transform.forward + Input.GetAxis("Horizontal") * transform.right;
        moveDirection *= speed * Time.deltaTime;

        ApplyGravity();

        characterController.Move(moveDirection);
    }

    // Apply gravity to the player
    void ApplyGravity()
    {
        verticalVelocity -= gravity * Time.deltaTime;

        Jump();

        moveDirection.y = verticalVelocity * Time.deltaTime;
    }

    // Player jump function
    void Jump()
    {
        if (characterController.isGrounded && Input.GetKeyDown(jumpKey))
        {
            verticalVelocity = jumpForce;
        }
    }

    // Rotate player based on mouse movement
    // Code edited from Scripts/Player/PlayerManager.cs
    private void Rotate()
    {
        rotationY += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationX += Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationX = Mathf.Clamp(rotationX, minX, maxX);

        transform.localEulerAngles = new Vector3(0, rotationY, 0);
        lookRoot.transform.localEulerAngles = new Vector3(-rotationX, 0, 0);

        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Player crouch function
    private void Crouch()
    {
        Ray crouchRay = new Ray(transform.position, Vector3.up);

        if(Input.GetKey(crouchKey))
        {
            characterController.height = playerHeight/2;
        }
        else if (!Physics.Raycast(crouchRay, out RaycastHit hit, playerHeight * 3/4)) // Check if there is anything above the player
        {
            characterController.height = playerHeight;
        }
    }
}
