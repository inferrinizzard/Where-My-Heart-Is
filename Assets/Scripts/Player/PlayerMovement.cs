using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;
    private Vector3 moveDirection;
    private float verticalVelocity;

    [SerializeField] private float speed;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpForce;
    [SerializeField] private KeyCode jumpKey;
    [SerializeField] private float mouseSensitivity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
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

    // Player jump funtion
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
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivity, Space.World);
        transform.Rotate(Vector3.right * -Input.GetAxis("Mouse Y") * mouseSensitivity, Space.Self);
    }
}
