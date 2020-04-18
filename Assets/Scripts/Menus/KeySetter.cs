using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Handles setting of controls and ui associated. </summary>
public class KeySetter : MonoBehaviour
{
    /// <summary> Input key. </summary>
    KeyCode inputKey;
    /// <summary> Whether the script is looking for a key to assign. </summary>
    bool lookingForKey;
    /// <summary> Which control is currently being changed. </summary>
    int changingControl;

    /// <summary> Text to display current jump key. </summary>
    public GameObject jumpButton;
    /// <summary> Text to display current interact key. </summary>
    public GameObject interactButton;
    /// <summary> Slider for the mouse sensitivity. </summary>
    public Slider sensitivitySlider;
    /// <summary> Input Field for the mouse sensitivity. </summary>
    public InputField sensitivityInputField;

    /// <summary> Enum which holds different controls to be set. </summary>
    enum Controls
    {
        Jump = 0,
        Interact = 1
    }

    private void Start()
    {
        //set all the keys for ui
        jumpButton.GetComponent<Text>().text = InputManager.jumpKey.ToString();
        interactButton.GetComponent<Text>().text = InputManager.interactKey.ToString();
    }

    private void Update()
    {
        if(isActiveAndEnabled)
        {
            if(lookingForKey)
            {
                switch (changingControl)
                {
                    case (int)Controls.Jump:
                        InputManager.jumpKey = inputKey;
                        jumpButton.GetComponent<Text>().text = "press any key";
                        break;
                    case (int)Controls.Interact:
                        InputManager.interactKey = inputKey;
                        interactButton.GetComponent<Text>().text = "press any key";
                        break;
                    default:
                        Debug.Log("error assigning key");
                        break;
                }
            }
        }
    }

    // Checks for keyboard presses and assigns accordingly.
    public void OnGUI()
    {
        if(Event.current.isKey && Event.current.type == EventType.KeyDown && lookingForKey)
        {
            inputKey = Event.current.keyCode;

            switch (changingControl)
            {
                case (int)Controls.Jump:
                    InputManager.jumpKey = inputKey;
                    jumpButton.GetComponent<Text>().text = inputKey.ToString();
                    break;
                case (int)Controls.Interact:
                    InputManager.interactKey = inputKey;
                    interactButton.GetComponent<Text>().text = inputKey.ToString();
                    break;
                default:
                    Debug.Log("error assigning key");
                    break;
            }

            lookingForKey = false;
        }
    }

    /// <summary> Configures script to look for the jump key. </summary>
    public void SetJumpKey()
    {
        if (!lookingForKey)
        {
            lookingForKey = true;
            changingControl = (int)Controls.Jump;
        }
    }

    /// <summary> Configures script to look for the interact key. </summary>
    public void SetInteractKey()
    {
        if(!lookingForKey)
        {
            lookingForKey = true;
            changingControl = (int)Controls.Interact;
        }
    }

    /// <summary> Sets the sensitivity based on the slider's value. </summary>
    public void SetSensitivitySlider()
    {
        sensitivityInputField.text = sensitivitySlider.value.ToString();
        Player.mouseSensitivity = sensitivitySlider.value;
    }

    /// <summary> Gets the input from the Input Field and sets it to the sensitivity slider. </summary>
    /// <remarks> This parses the input and constrains it to the slider values, then uses the slider function to set it. </remarks>
    public void SetSensitivityInputField()
    {
        float input = float.Parse(sensitivityInputField.text);
        input = Mathf.Clamp(input, sensitivitySlider.minValue, sensitivitySlider.maxValue);
        sensitivitySlider.value = input;
        SetSensitivitySlider();
    }
}
