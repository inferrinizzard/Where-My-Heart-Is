using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Handles player movement and player interaction </summary>
[System.Serializable]
// public class Player : Singleton<Player>
public class Player : StateMachine
{
	/// <summary> Reference to the players last spawn. </summary>
	[SerializeField] public Transform lastSpawn = default;

	/// <summary> Reference to player CharacterController. </summary>
	[HideInInspector] public CharacterController characterController;
	/// <summary> Reference to player Camera. </summary>
	[HideInInspector] public Camera cam;
	/// <summary> Empty GameObject for where to put a Pickupable object. </summary>
	[HideInInspector] public Transform heldObjectLocation;
	/// <summary> Reference to a Pickupable object that has been picked up. </summary>
	[HideInInspector] public InteractableObject heldObject;
	/// <summary> Vector3 to store and calculate vertical velocity. </summary>
	[HideInInspector] public float verticalVelocity;
	/// <summary> Player's height. </summary>
	[HideInInspector] public float playerHeight;
	/// <summary> Whether the player can move or not. </summary>
	[HideInInspector] public bool playerCanMove = true;
	/// <summary> Whether the player is jumping or not. </summary>
	[HideInInspector] public bool jumping = false;
	/// <summary> Whether the player is crouching or not. </summary>
	[HideInInspector] public bool crouching = false;
	/// <summary> Whether the player is holding something or not. </summary>
	[HideInInspector] public bool holding = false;
	/// <summary> Whether the player is inspecting a Pickupable object or not. </summary>
	[HideInInspector] public bool looking = false;
	/// <summary> Vector3 to store and calculate move direction. </summary>
	private Vector3 moveDirection;

	[Header("Game Object References")]
	/// <summary> Reference to heart window. </summary>
	public GameObject heartWindow;
	/// <summary> Reference to death plane. </summary>
	public GameObject deathPlane;

	[Header("Parametres")]
	/// <summary> Player move speed. </summary>
	[SerializeField] private float speed = 5f;
	/// <summary> Player gravity variable. </summary>
	[SerializeField] private float gravity = 25f;
	/// <summary> Player jump force. </summary>
	[SerializeField] public float jumpForce = 7f;
	/// <summary> Mouse sensitivity for camera rotation. </summary>
	[SerializeField] private float mouseSensitivity = 2f;
	/// <summary> How far the player can reach to pick something up. </summary>
	[SerializeField] public float playerReach = 4f;

	[Header("Keybinds")]
	/// <summary> KeyCode for player jump. </summary>
	[SerializeField] private KeyCode jumpKey = KeyCode.Space;
	/// <summary> KeyCode for player crouch. </summary>
	[SerializeField] private KeyCode crouchKey = KeyCode.LeftShift;
	/// <summary> KeyCode for inspecting and picking up objects. </summary>
	[SerializeField] private KeyCode pickUpKey = KeyCode.E;

	[Header("Camera Variables")]
	/// <summary> Minimum angle the player can look upward. </summary>
	private float minX = -90f;
	/// <summary> Minimum angle the player can look downward. </summary>
	private float maxX = 90f;

	/// <summary> Stores the Y rotation of the player. </summary>
	[HideInInspector] public float rotationY = 0f;
	/// <summary> Stores the X rotation of the player. </summary>
	[HideInInspector] public float rotationX = 0f;

	/// <summary> Initializes variables before the game starts. </summary>
	private void Awake()
	{
		// base.Awake();
		characterController = GetComponent<CharacterController>();
		cam = GetComponentInChildren<Camera>();
		heartWindow = GetComponentInChildren<Window>().gameObject;

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
		Cursor.visible = false;
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
			ApplyGravity();
			Crouch(); // Crouch.
			Rotate(); // Mouse based rotation for camera and player.
			Cut(); // Player cut powers.

			characterController.Move(moveDirection);
		}

		PickUp(); // Ability to pick up is independent from player movement.
		Die();
	}

	private void Die() { SetState(new Die(this)); }

	/// <summary> Moves and applies gravity to the player using Horizonal and Vertical Axes. </summary>
	private void Move()
	{
		// Get the Vertical and Horizontal Axes and scale them by movement speed.
		moveDirection = Input.GetAxis("Vertical") * transform.forward + Input.GetAxis("Horizontal") * transform.right;
		//moveDirection.Normalize();
		Vector3 horizontal = characterController.velocity - characterController.velocity.y * Vector3.up;
		GetComponent<PlayerAudio>().SetWalkingVelocity(Mathf.RoundToInt(horizontal.magnitude) / speed);
		// Scale the moveDirection to account for different runtimes.
		moveDirection *= speed * Time.deltaTime;
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
		if (characterController.isGrounded && Input.GetKeyDown(jumpKey)) SetState(new Jump(this));
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
		if (Input.GetKeyDown(crouchKey)) { SetState(new Crouch(this)); }
		// Check if there is anything above the player before uncrouching.
		else if (!Input.GetKey(crouchKey) && !Physics.Raycast(crouchRay, out RaycastHit hit, playerHeight * 3 / 4) && crouching) { SetState(new UnCrouch(this)); }
	}

	/// <summary> Handles player behavior with picking up and inspecting objects. </summary>
	private void PickUp()
	{
		// Check if the player is pressing the pick up key.
		if (Input.GetKeyDown(pickUpKey))
		{
			if (!holding && !looking) { SetState(new PickUp(this)); }
			else if (looking) { SetState(new Inspect(this)); } // If the player is holding something and inspecting it, when the player presses the pick up key... 
			else if (holding) { SetState(new Drop(this)); }
		}
	}

	/// <summary> Function to aim and apply player cut power. </summary>
	private void Cut()
	{
		if ((Input.GetMouseButton(1) || Input.GetKey(KeyCode.LeftControl)) && !holding)
		{
			// Aiming...
			if (!heartWindow.activeSelf) { SetState(new Aiming(this)); }
			if (Input.GetMouseButtonDown(0)) { SetState(new Cut(this)); }
		}
		else
		{
			// Not Aiming...
			if (heartWindow.activeSelf)
			{
				heartWindow.SetActive(false);
				GetComponent<PlayerAudio>().CloseWindow();
			}
		}
	}

	/// <summary> Function to get transform of where the held object should be. </summary>
	/// <returns> Returns a reference to the player's heldObjectLocation transform. </returns>
	public Transform GetHeldObjectLocation()
	{
		return heldObjectLocation.transform;
	}
}
