using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary> Handles player movement and player interaction </summary>
[System.Serializable]
public class Player : Singleton<Player>, IResetable, IStateMachine
{
	/// <summary> Reference to the current state. </summary>
	protected PlayerState State;

	/// <summary> Reference to the players last spawn. </summary>
	[HideInInspector] public Transform lastSpawn;
	/// <summary> Reference to player CharacterController. </summary>
	[HideInInspector] public CharacterController characterController;
	/// <summary> Reference to player Camera. </summary>
	[HideInInspector] public Camera cam;
	/// <summary> Reference to FX Controller. </summary>
	[HideInInspector] public Effects VFX;
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
	/// <summary> Whether the player in aiming the window or not. </summary>
	[HideInInspector] public bool aiming = false;
	/// <summary> Whether the player is still crouching after the crouch key has been let go. </summary>
	private bool stillCrouching = false;

	/// <summary> Vector3 to store and calculate move direction. </summary>
	private Vector3 moveDirection;

	// [Header("Game Object References")]
	/// <summary> Reference to heart window. </summary>
	[HideInInspector] public GameObject heartWindow;
	/// <summary> Reference to death plane. </summary>
	[HideInInspector] public Transform deathPlane;
	/// <summary> Get Window script from GameObject. </summary>
	[HideInInspector] public Window window;
	[HideInInspector] public PlayerAudio audioController;

	[Header("Parametres")]
	/// <summary> Player move speed. </summary>
	[SerializeField] float speed = 5f;
	/// <summary> Player gravity variable. </summary>
	[SerializeField] float gravity = 25f;
	/// <summary> Player jump force. </summary>
	public float jumpForce = 7f;
	/// <summary> Mouse sensitivity for camera rotation. </summary>
	[SerializeField] float mouseSensitivity = 2f;
	/// <summary> How far the player can reach to pick something up. </summary>
	public float playerReach = 4f;

    public bool windowEnabled = false;

	// [Header("Camera Variables")]
	/// <summary> Minimum angle the player can look upward. </summary>
	private float minX = -90f;
	/// <summary> Minimum angle the player can look downward. </summary>
	private float maxX = 90f;
	/// <summary> Stores the Y rotation of the player. </summary>
	[HideInInspector] public float rotationY = 0f;
	/// <summary> Stores the X rotation of the player. </summary>
	[HideInInspector] public float rotationX = 0f;

    public override void Awake()
    {
        //Player other = FindObjectsOfType<Player>().ToList().Find(p => p != this);
        //if(other != null) this.windowEnabled = other.windowEnabled;
        base.Awake();
    }

    void Start()
	{
		characterController = GetComponent<CharacterController>();
		cam = GetComponentInChildren<Camera>();
		VFX = cam.GetComponent<Effects>();
		window = GetComponentInChildren<Window>();
		heartWindow = window.gameObject;
		audioController = GetComponent<PlayerAudio>();

		// Get reference to the player height using the CharacterController's height.
		playerHeight = characterController.height;
		// Creates an empty game object at the position where a held object should be.
		heldObjectLocation = new GameObject("HeldObjectLocation").transform;
		heldObjectLocation.position = cam.transform.position + cam.transform.forward;
		heldObjectLocation.parent = cam.transform;

		Cursor.lockState = CursorLockMode.Locked; // turn off cursor
		Cursor.visible = false;

		Init();
	}

	public void Init()
	{
        heartWindow.SetActive(true);
        GetComponentInChildren<ApplyMask>().CreateMask();
        heartWindow.SetActive(false);
		deathPlane = GameObject.FindWithTag("Finish")?.transform;
		lastSpawn = GameObject.FindWithTag("Respawn")?.transform;
		if (lastSpawn)
		{
			transform.position = lastSpawn.position;
			rotationX = lastSpawn.eulerAngles.x;
			rotationY = lastSpawn.eulerAngles.y;
			//transform.rotation = lastSpawn.rotation;
			//cam.transform.eulerAngles = new Vector3(lastSpawn.eulerAngles.x, 0, 0);
		}
		playerCanMove = true;
		holding = false;
		looking = false;
		window.world = World.Instance;
		VFX.ToggleMask(false);
	}

	///	<summary> reset pos, rendundant </summary>
	public void Reset()
	{
		transform.position = Vector3.zero;
		transform.eulerAngles = Vector3.zero;
	}

	public void OnEnable()
	{
		// Subscribe input events to player behaviors
		InputManager.OnJumpDown += Jump;
		InputManager.OnCrouchDown += Crouch;
		InputManager.OnCrouchUp += UnCrouch;
		InputManager.OnPickUpDown += PickUp;
		InputManager.OnRightClickHeld += Aiming;
		InputManager.OnRightClickUp += StopAiming;
		InputManager.OnAltAimKeyHeld += Aiming;
		InputManager.OnAltAimKeyUp += StopAiming;
		InputManager.OnLeftClickDown += Cut;
	}

	public void OnDisable()
	{
		// Unsubscribe input events to player behaviors
		InputManager.OnJumpDown -= Jump;
		InputManager.OnCrouchDown -= Crouch;
		InputManager.OnCrouchUp -= UnCrouch;
		InputManager.OnPickUpDown -= PickUp;
		InputManager.OnRightClickHeld -= Aiming;
		InputManager.OnRightClickUp -= StopAiming;
		InputManager.OnAltAimKeyHeld -= Aiming;
		InputManager.OnAltAimKeyUp -= StopAiming;
		InputManager.OnLeftClickDown -= Cut;
	}

	public void SetState(PlayerState state)
	{
		State = state;
		State.Start();
	}

	void Update()
	{
		if (playerCanMove)
		{
			Move();
			ApplyGravity();
			Rotate();
			characterController.Move(moveDirection);
		}

		StuckCrouching();
		Die();
	}

	/// <summary> Player sudoku function. </summary>
	private void Die() { SetState(new Die(this)); }

	/// <summary> Moves and applies gravity to the player using Horizonal and Vertical Axes. </summary>
	private void Move()
	{
		moveDirection = Input.GetAxis("Vertical") * transform.forward + Input.GetAxis("Horizontal") * transform.right;
		Vector3 horizontal = characterController.velocity - characterController.velocity.y * Vector3.up;
		audioController.SetWalkingVelocity(Mathf.RoundToInt(horizontal.magnitude) / speed);
		moveDirection *= speed * Time.deltaTime;
	}

	/// <summary> Applies gravity to the player and includes jump. </summary>
	private void ApplyGravity()
	{
		if (!characterController.isGrounded)
		{
			verticalVelocity -= gravity * Time.deltaTime;
		}
		moveDirection.y = verticalVelocity * Time.deltaTime;
	}

	/// <summary> Player jump function. </summary>
	private void Jump()
	{
		if (characterController.isGrounded)SetState(new Jump(this));
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
		// Done exclusively on camera rotation so that movement is not hindered by looking up or down.
		cam.transform.localEulerAngles = new Vector3(-rotationX, 0, 0);

		// Allow the player to get out of the mouse lock.
		if (Input.GetKey(KeyCode.Escape))
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	/// <summary> Player crouch function. </summary>
	private void Crouch() => SetState(new Crouch(this));

	/// <summary> Player uncrouch function. </summary>
	/// <remarks> If the player is unable to uncrouch, it sets a bool to enable a check in update. </remarks>
	private void UnCrouch()
	{
		// Ray looking straight up from the player's position.
		Ray crouchRay = new Ray(transform.position, Vector3.up);
		if (!Physics.Raycast(crouchRay, out RaycastHit hit, playerHeight * 3 / 4) && crouching) { SetState(new UnCrouch(this)); }
		else { stillCrouching = true; } // The player did not uncrouch
	}

	/// <summary> If player was unable to uncrouch, perform this check until they can uncrouch. </summary>
	private void StuckCrouching()
	{
		if (stillCrouching)
		{
			// Ray looking straight up from the player's position.
			Ray crouchRay = new Ray(transform.position, Vector3.up);
			if (!Physics.Raycast(crouchRay, out RaycastHit hit, playerHeight * 3 / 4))
			{
				SetState(new UnCrouch(this));
				stillCrouching = false;
			}
		}
	}

	/// <summary> Handles player behavior when interacting with objects. </summary>
	private void PickUp()
	{
		if (!holding && !looking) { SetState(new PickUp(this)); }
		else if (looking) { SetState(new Inspect(this)); } //unused for now
		else if (holding) { SetState(new Drop(this)); }
	}

	/// <summary> Player aiming function. </summary>
	private void Aiming()
	{
		if (windowEnabled && !heartWindow.activeSelf && !holding)
		{
			SetState(new Aiming(this));
		}
		aiming = true;
	}

	/// <summary> Player stoped aiming function. </summary>
	private void StopAiming()
	{
		if (heartWindow.activeSelf)
		{
			heartWindow.SetActive(false);
			VFX.ToggleMask(false);
			audioController.CloseWindow();
		}
		aiming = false;
	}

	/// <summary> The player cut function. </summary>
	private void Cut()
	{
		if (aiming)SetState(new Cut(this));
	}

}
