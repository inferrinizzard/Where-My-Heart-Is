using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

/// <summary> Handles setting of controls and ui associated with the controls page. </summary>
public class ControlsMenu : MonoBehaviour
{
	/// <summary> Input key from unity event system. </summary>
	KeyCode inputKey;
	/// <summary> Whether we are waiting for a user input to bind to a control. </summary>
	bool lookingForKey;
	/// <summary> Enum for which control is currently being set. </summary>
	int changingControl;
	/// <summary> If we were just looking for a key or not. </summary>
	/// <remarks> Used to make sure that escape will not close the menu and only return from the key setting action. </remarks>
	[HideInInspector] public bool wasLookingForKey = false;

	/// <summary> Text element for the jump button UI element. </summary>
	public Text jumpButtonText;
	/// <summary> Text element for the interact button UI element. </summary>
	public Text interactButtonText;
	/// <summary> Text element for the alternate aim button UI element. </summary>
	public Text altAimButtonText;
	/// <summary> Sensitivity slider element for mouse sensitivity. </summary>
	public Slider sensitivitySlider;
	/// <summary> Sensitivity input field for mouse sensitivity. </summary>
	public InputField sensitivityInputField;

	/// <summary> Enum to specify controls for changingControl. </summary>
	enum Controls
	{
		Jump = 0,
		Interact = 1,
		AltAim = 2
	}

	private void Start()
	{
		// Set all text fields to appropriate defaults.
		jumpButtonText.text = ParseKey(InputManager.jumpKey.ToString());
		interactButtonText.text = ParseKey(InputManager.interactKey.ToString());
		altAimButtonText.text = ParseKey(InputManager.altAimKey.ToString());
		sensitivityInputField.text = sensitivitySlider.value.ToString();
	}

	private void Update()
	{
		if (isActiveAndEnabled)
		{
			if (lookingForKey)
			{
				// Change the prompt for the control we are changing.
				switch (changingControl)
				{
					case (int) Controls.Jump:
						jumpButtonText.text = "press any key";
						break;
					case (int) Controls.Interact:
						interactButtonText.text = "press any key";
						break;
					case (int)Controls.AltAim:
						altAimButtonText.text = "press any key";
						break;
					default:
						Debug.Log("error assigning key");
						break;
				}
			}
		}
	}

	// We use OnGUI so we only advance once we have been given a key input from Unity.
	public void OnGUI()
	{
		// Check that we are looking for a key, and that the event we get is a key input.
		if (Event.current.isKey && Event.current.type == EventType.KeyDown && lookingForKey)
		{
			inputKey = Event.current.keyCode;

			// Set the key based on the changingControl enum.
			switch (changingControl)
			{
				case (int) Controls.Jump:
					if (inputKey != KeyCode.Escape) InputManager.jumpKey = inputKey; else wasLookingForKey = true;
					jumpButtonText.text = ParseKey(InputManager.jumpKey.ToString());
					break;
				case (int) Controls.Interact:
					if (inputKey != KeyCode.Escape) InputManager.interactKey = inputKey; else wasLookingForKey = true;
					interactButtonText.text = ParseKey(InputManager.interactKey.ToString());
					break;
				case (int)Controls.AltAim:
					if (inputKey != KeyCode.Escape) InputManager.altAimKey = inputKey; else wasLookingForKey = true;
					altAimButtonText.text = ParseKey(InputManager.altAimKey.ToString());
					break;
				default:
					Debug.Log("error assigning key");
					break;
			}

			lookingForKey = false;
		}
	}

	/// <summary> Method to start changing the jump key. </summary>
	public void SetJumpKey()
	{
		if (!lookingForKey)
		{
			lookingForKey = true;
			changingControl = (int) Controls.Jump;
		}
	}

	/// <summary> Method to start changing the interact key. </summary>
	public void SetInteractKey()
	{
		if (!lookingForKey)
		{
			lookingForKey = true;
			changingControl = (int) Controls.Interact;
		}
	}

	/// <summary> Method to start changing the alternate aim key. </summary>
	public void SetAltAimKey()
	{
		if (!lookingForKey)
		{
			lookingForKey = true;
			changingControl = (int) Controls.AltAim;
		}
	}

	/// <summary> Sets the mouse sensitivity and the input field based on the input from the slider. </summary>
	public void SetSensitivitySlider()
	{
		sensitivityInputField.text = sensitivitySlider.value.ToString();
		Player.mouseSensitivity = sensitivitySlider.value;
	}

	/// <summary> Sets the mouse sensitivity and the slider based on the input from the input field. </summary>
	public void SetSensitivityInputField()
	{
		float input;
		if (sensitivityInputField.text == "")
		{
			input = sensitivitySlider.minValue;
			sensitivityInputField.text = sensitivitySlider.minValue.ToString();
		}
		else
			input = float.Parse(sensitivityInputField.text);
		input = Mathf.Clamp(input, sensitivitySlider.minValue, sensitivitySlider.maxValue);
		sensitivitySlider.value = input;
		Player.mouseSensitivity = sensitivitySlider.value;
	}

	/// <summary> Changes longer keycodes into shorter ones for UI aesthetic purposes.</summary>
	/// <param name="input"> KeyCode as string. </param>
	/// <returns> KeyCode as string to be put in menus and prompts. </returns>
	public string ParseKey(string input)
	{
		if(input == "LeftControl")
		{
			return "L Ctrl";
		}
		if(input == "RightControl")
		{
			return "R Ctrl";
		}
		if (input == "LeftAlt")
		{
			return "L Alt";
		}
		if (input == "RightAlt")
		{
			return "R Alt";
		}
		return input;
	}

	public void RefreshSettings()
	{
		jumpButtonText.text = ParseKey(InputManager.jumpKey.ToString());
		interactButtonText.text = ParseKey(InputManager.interactKey.ToString());
		altAimButtonText.text = ParseKey(InputManager.altAimKey.ToString());

		sensitivitySlider.value = Player.mouseSensitivity;
		sensitivityInputField.text = Player.mouseSensitivity.ToString();
	}
}
