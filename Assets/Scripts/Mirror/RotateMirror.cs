using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotates mirror by 90 degrees when player interacts
/// </summary>
public class RotateMirror : InteractableObject
{
    [SerializeField] GameObject mirror;
    bool rotated = false;

    //public float Rotation_Speed;
  //  public float Rotation_Friction; //The smaller the value, the more Friction there is. [Keep this at 1 unless you know what you are doing].
    //public float Rotation_Smoothness; //Believe it or not, adjusting this before anything else is the best way to go.

   // private float Resulting_Value_from_Input;
   // private Quaternion Quaternion_Rotate_From;
   // private Quaternion Quaternion_Rotate_To;

    //[SerializeField] public float rotate = 90;

    public override void Interact()
    {
        if (rotated == false)
        {
          //  rotate = 90;
            mirror.transform.Rotate(0, 0, 90);
            rotated = true;
        }
        else
        {
          //  rotate = -90;
            mirror.transform.Rotate(0, 0, -90);
            rotated = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
       // Quaternion_Rotate_From = mirror.transform.rotation;
     //   Quaternion_Rotate_To = Quaternion.Euler(90,0, rotate);

        //mirror.transform.rotation = Quaternion.Lerp(Quaternion_Rotate_From, Quaternion_Rotate_To, Time.deltaTime * Rotation_Smoothness);
    }
}
