using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static event Action OnCrouchDown;
    public static event Action OnCrouchHeld;
    public static event Action OnCrouchUp;

    public static event Action OnJumpDown;
    public static event Action OnJumpHeld;
    public static event Action OnJumpUp;

    public static event Action OnPickUpDown;
    public static event Action OnPickUpHeld;
    public static event Action OnPickUpUp;

    public static event Action OnLeftClickDown;
    public static event Action OnLeftClickHeld;
    public static event Action OnLeftClickUp;

    public static event Action OnRightClickDown;
    public static event Action OnRightClickHeld;
    public static event Action OnRightClickUp;

    [Header("Keybinds")]
    /// <summary> KeyCode for player jump. </summary>
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    /// <summary> KeyCode for player crouch. </summary>
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftShift;
    /// <summary> KeyCode for inspecting and picking up objects. </summary>
    [SerializeField] private KeyCode pickUpKey = KeyCode.E;

    // Called every frame.
    public void Update()
    {
        UpdateCrouch();
        UpdateJump();
        UpdatePickUp();
        UpdateLeftClick();
        UpdateRightClick();
    }

    private void UpdateJump()
    {
        if (Input.GetKeyDown(jumpKey))
        {
            OnJumpDown?.Invoke();
        }
        if (Input.GetKey(jumpKey))
        {
            OnJumpHeld?.Invoke();
        }
        if (Input.GetKeyUp(jumpKey))
        {
            OnJumpUp?.Invoke();
        }
    }

    private void UpdateCrouch()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            OnCrouchDown?.Invoke();
        }
        if (Input.GetKey(crouchKey))
        {
            OnCrouchHeld?.Invoke();
        }
        if (Input.GetKeyUp(crouchKey))
        {
            OnCrouchUp?.Invoke();
        }
    }

    private void UpdatePickUp()
    {
        if (Input.GetKeyDown(pickUpKey))
        {
            OnPickUpDown?.Invoke();
        }
        if (Input.GetKey(pickUpKey))
        {
            OnPickUpHeld?.Invoke();
        }
        if (Input.GetKeyUp(pickUpKey))
        {
            OnPickUpUp?.Invoke();
        }
    }

    private void UpdateLeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnLeftClickDown?.Invoke();
        }
        if (Input.GetMouseButton(0))
        {
            OnLeftClickHeld?.Invoke();
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnLeftClickUp?.Invoke();
        }
    }

    private void UpdateRightClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            OnRightClickDown?.Invoke();
        }
        if (Input.GetMouseButton(1))
        {
            OnRightClickHeld?.Invoke();
        }
        if (Input.GetMouseButtonUp(1))
        {
            OnRightClickUp?.Invoke();
        }
    }
}
