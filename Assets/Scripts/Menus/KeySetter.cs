using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

/// <summary> Handles setting of controls and ui associated. </summary>
public class KeySetter : MonoBehaviour
{
	KeyCode inputKey;
	bool lookingForKey;
	int changingControl;

	public bool wasLookingForKey = false;

	public GameObject jumpButton;
	public GameObject interactButton;
	public Slider sensitivitySlider;
	public InputField sensitivityInputField;

	enum Controls
	{
		Jump = 0,
		Interact = 1
	}

	private void Start()
	{
		//set all the keys for control ui
		jumpButton.GetComponent<Text>().text = InputManager.jumpKey.ToString();
		interactButton.GetComponent<Text>().text = InputManager.interactKey.ToString();
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
						jumpButton.GetComponent<Text>().text = "press any key";
						break;
					case (int) Controls.Interact:
						interactButton.GetComponent<Text>().text = "press any key";
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
					jumpButton.GetComponent<Text>().text = InputManager.jumpKey.ToString();
					break;
				case (int) Controls.Interact:
					if (inputKey != KeyCode.Escape) InputManager.interactKey = inputKey; else wasLookingForKey = true;
					interactButton.GetComponent<Text>().text = InputManager.interactKey.ToString();
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

	public void SetSensitivitySlider()
	{
		sensitivityInputField.text = sensitivitySlider.value.ToString();
		Player.mouseSensitivity = sensitivitySlider.value;
	}

	public void SetSensitivityInputField()
	{
		float input = float.Parse(sensitivityInputField.text);
		input = Mathf.Clamp(input, sensitivitySlider.minValue, sensitivitySlider.maxValue);
		Debug.Log(input);
		sensitivitySlider.value = input; 
		Player.mouseSensitivity = sensitivitySlider.value;
	}
}
