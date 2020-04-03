using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeySetter : MonoBehaviour
{
    KeyCode inputKey;
    bool lookingForKey;
    int changingControl;

    public GameObject jumpButton;

    enum Controls
    {
        Jump = 0,
        Crouch = 1,
        Interact = 2
    }

    private void Start()
    {
        //set all the keys for control ui
        jumpButton.GetComponent<Text>().text = InputManager.jumpKey.ToString();
    }

    private void Update()
    {
        if(isActiveAndEnabled)
        {
            foreach (KeyCode _key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(_key))
                {
                    //inputKey = _key;
                }
            }
        }
    }

    public void OnGUI()
    {
        if(Event.current.isKey && Event.current.type == EventType.KeyDown && lookingForKey)
        {
            Debug.Log(Event.current.keyCode);
            inputKey = Event.current.keyCode;

            switch (changingControl)
            {
                case (int)Controls.Jump:
                    InputManager.jumpKey = inputKey;
                    jumpButton.GetComponent<Text>().text = inputKey.ToString();
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
        lookingForKey = true;
        changingControl = (int)Controls.Jump;
    }

    public void SetCrouchKey(KeyCode key)
    {
        InputManager.crouchKey = key;
    }

    public void SetInteractKey(KeyCode key)
    {
        InputManager.interactKey = key;
    }
}
