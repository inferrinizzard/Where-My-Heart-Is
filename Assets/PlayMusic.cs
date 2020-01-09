using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            GetComponent<AudioSource>().Play();
            Invoke("Message", 3);

        }

    }

    private void Message()
    {
        FindObjectOfType<MessageWriter>().WriteMessage("Ok.../Now what?/Maybe if I.../.../ Yeah.../ That's done it...");
    }
}
