using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary> Handles gameplay keys. </summary>
public class InputManager : MonoBehaviour
{
	[Header("Keybinds")]
	/// <summary> KeyCode for player jump. </summary>
	[SerializeField] private KeyCode jumpKey = KeyCode.Space;
	/// <summary> KeyCode for player crouch. </summary>
	[SerializeField] private KeyCode crouchKey = KeyCode.LeftShift;
	/// <summary> KeyCode for inspecting and picking up objects. </summary>
	[SerializeField] private KeyCode interactKey = KeyCode.E;
	/// <summary> KeyCode for inspecting and picking up objects. </summary>
	[SerializeField] private KeyCode altAimKey = KeyCode.LeftControl;
	/// <summary> KeyCode for pausing the game. </summary>
	[SerializeField] private KeyCode pauseKey = KeyCode.Escape;

	// Crouch Key Actions
	/// <summary> Crouch key is initially pressed down. </summary>
	public static event Action OnCrouchDown;
	/// <summary> Crouch key is held down. </summary>
	public static event Action OnCrouchHeld;
	/// <summary> Crouch key is let go. </summary>
	public static event Action OnCrouchUp;

	// Jump Key Actions
	/// <summary> Jump key is initially pressed down. </summary>
	public static event Action OnJumpDown;
	/// <summary> Jump key is held down. </summary>
	public static event Action OnJumpHeld;
	/// <summary> Jump key is let go. </summary>
	public static event Action OnJumpUp;

	// Pick Up Key Actions
	/// <summary> Pick Up key is initially pressed down. </summary>
	public static event Action OnInteractDown;
	/// <summary> Pick Up key is held down. </summary>
	public static event Action OnInteractHeld;
	/// <summary> Pick Up key is let go. </summary>
	public static event Action OnInteractUp;

	// Left Click Actions
	/// <summary> Left Click is initially pressed down. </summary>
	public static event Action OnLeftClickDown;
	/// <summary> Left Click is held down. </summary>
	public static event Action OnLeftClickHeld;
	/// <summary> Left Click is let go. </summary>
	public static event Action OnLeftClickUp;

	// Right Click Actions
	/// <summary> Right Click is initially pressed down. </summary>
	public static event Action OnRightClickDown;
	/// <summary> Right Click is held down. </summary>
	public static event Action OnRightClickHeld;
	/// <summary> Right Click is let go. </summary>
	public static event Action OnRightClickUp;

	// Alt Aim Key Actions
	/// <summary> Alt Aim Key is initially pressed down. </summary>
	public static event Action OnAltAimKeyDown;
	/// <summary> Alt Aim Key is held down. </summary>
	public static event Action OnAltAimKeyHeld;
	/// <summary> Alt Aim Key is let go. </summary>
	public static event Action OnAltAimKeyUp;


	//Pause Menu Key Actions
	/// <summary> Pause Key is initially pressed down. </summary>
	public static event Action OnPauseKeyDown;
	/// <summary> Pause Key is held down. </summary>
	public static event Action OnPauseKeyHeld;
	/// <summary> Pause Key is let go. </summary>
	public static event Action OnPauseKeyUp;

	public void Update()
	{
		UpdateCrouch();
		UpdateJump();
		UpdateInteract();
		UpdateLeftClick();
		UpdateRightClick();
		UpdateAltAimKey();
		UpdatePauseKey();
	}

	/// <summary> Update the actions for the Crouch Key. </summary>
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

	/// <summary> Update the actions for the Jump Key. </summary>
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

	/// <summary> Update the actions for the Pick Up Key. </summary>
	private void UpdateInteract()
	{
		if (Input.GetKeyDown(interactKey))
		{
			OnInteractDown?.Invoke();
		}
		if (Input.GetKey(interactKey))
		{
			OnInteractHeld?.Invoke();
		}
		if (Input.GetKeyUp(interactKey))
		{
			OnInteractUp?.Invoke();
		}
	}

	/// <summary> Update the actions for the Left Click. </summary>
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

	/// <summary> Update the actions for the Right Click. </summary>
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

	/// <summary> Update the actions for the Alt Aim Key. </summary>
	private void UpdateAltAimKey()
	{
		if (Input.GetKeyDown(altAimKey))
		{
			OnAltAimKeyDown?.Invoke();
		}
		if (Input.GetKey(altAimKey))
		{
			OnAltAimKeyHeld?.Invoke();
		}
		if (Input.GetKeyUp(altAimKey))
		{
			OnAltAimKeyUp?.Invoke();
		}
	}

	/// <summary> Update the actions for the Pause Key. </summary>
	private void UpdatePauseKey()
	{
		if (Input.GetKeyDown(pauseKey))
		{
			OnPauseKeyDown?.Invoke();
		}
		if (Input.GetKey(pauseKey))
		{
			OnPauseKeyHeld?.Invoke();
		}
		if (Input.GetKeyUp(pauseKey))
		{
			OnPauseKeyUp?.Invoke();
		}
	}
}
