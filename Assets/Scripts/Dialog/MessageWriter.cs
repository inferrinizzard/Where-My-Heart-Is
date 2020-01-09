using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MessageWriter : MonoBehaviour
{
    public Text textBox;
    public GameObject textBoxParent;
    public float textSpeed;
    public List<AudioClip> voiceSounds;

    private string currentMessage;
    private string[] messageQueue;
    private int currentMessageIndex;

    private int currentCharacter;
    private bool writingMessage;

    private void Start()
    {
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            WriteMessage("It's lonely here.../ I'd feel better with something to listen to.../ I think my headphones are next to my lamp...");

        }
    }

    public void WriteMessage(string message)
    {
        messageQueue = message.Split('/');
        currentMessage = messageQueue[0];
        currentCharacter = 0;
        currentMessageIndex = 0;

        writingMessage = true;
        ActivateTextBox();
        InvokeRepeating("AdvanceTypewriter", 0f, textSpeed);
    }

    public void Interrupt()
    {
        if(writingMessage)
        {
            currentCharacter = currentMessage.Length - 1;
        }
        else
        {
            currentMessageIndex++;
            if(currentMessageIndex >= messageQueue.Length)
            {
                DeactivateTextBox();
            }
            else
            {
                currentMessage = messageQueue[currentMessageIndex];
                currentCharacter = 0;
                writingMessage = true;
                ActivateTextBox();
                InvokeRepeating("AdvanceTypewriter", 0f, textSpeed);
            }
        }
    }

    private void AdvanceTypewriter()
    {
        textBox.text = currentMessage.Substring(0, currentCharacter);

        if (currentCharacter >= currentMessage.Length)
        {
            writingMessage = false;
            CancelInvoke("AdvanceTypewriter");
            return;
        }


        AudioClip sound = SelectSound(currentMessage[currentCharacter]);
        if (sound != null)
        {
            GetComponent<AudioSource>().clip = sound;
            GetComponent<AudioSource>().Play();
        }

        currentCharacter++;
    }

    private void ActivateTextBox()
    {
        textBoxParent.gameObject.SetActive(true);
        textBox.text = "";
    }

    private void DeactivateTextBox()
    {
        textBoxParent.gameObject.SetActive(false);
    }

    private AudioClip SelectSound(char character)
    {
        if(character != ' ')
        {
            return voiceSounds[Mathf.RoundToInt(Random.Range(0, voiceSounds.Count - 1))];
        }
        else
        {
            return null;
        }
    }
}
