using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

/// <summary> Handles setting of controls and ui associated. </summary>
public class ControlsMenu : MonoBehaviour
{
	KeyCode inputKey;
	bool lookingForKey;
	int changingControl;

	[HideInInspector] public bool wasLookingForKey = false;

	public Text jumpButtonText;
	public Text interactButtonText;
	public Text altAimButtonText;
	public Slider sensitivitySlider;
	public InputField sensitivityInputField;

	enum Controls
	{
		Jump = 0,
		Interact = 1,
		AltAim = 2
	}

	private void Start()
	{
		//set all the keys for control ui
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

	public void OnGUI()
	{
		if (Event.current.isKey && Event.current.type == EventType.KeyDown && lookingForKey)
		{
			inputKey = Event.current.keyCode;

			switch (changingControl)
			{
				case (int) Controls.Jump:
					if (inputKey != KeyCode.Escape) InputManager.jumpKey = inputKey; else wasLookingForKey = true;
					jumpButtonText.text = ParseKey(InputManager.jumpKey.ToString()); ;
					break;
				case (int) Controls.Interact:
					if (inputKey != KeyCode.Escape) InputManager.interactKey = inputKey; else wasLookingForKey = true;
					interactButtonText.text = ParseKey(InputManager.interactKey.ToString()); ;
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

	public void SetJumpKey()
	{
		if (!lookingForKey)
		{
			lookingForKey = true;
			changingControl = (int) Controls.Jump;
		}
	}

	public void SetInteractKey()
	{
		if (!lookingForKey)
		{
			lookingForKey = true;
			changingControl = (int) Controls.Interact;
		}
	}

	public void SetAltAimKey()
	{
		if (!lookingForKey)
		{
			lookingForKey = true;
			changingControl = (int) Controls.AltAim;
		}
	}

	public void SetSensitivitySlider()
	{
		sensitivityInputField.text = sensitivitySlider.value.ToString();
		Player.mouseSensitivity = sensitivitySlider.value;
	}

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
}
