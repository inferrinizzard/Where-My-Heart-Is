using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Handles the sizing of the key button outlines in the Controls Menu. </summary>
public class KeyBindButtonHandler : MonoBehaviour
{
    /// <summary> Text of the button. </summary>
    public Text text;
    /// <summary> The frame around the button. </summary>
    public RectTransform keyFrame;
    /// <summary> The smudge asset (also the button). </summary>
    public RectTransform smudge;

    /// <summary> Default width of the key frame. </summary>
    private float keyFrameWidthDefault;
    /// <summary> Default height of the key frame. </summary>
    private float keyFrameHeightDefault;
    /// <summary> Default width of the smudge. </summary>
    private float smudgeWidthDefault;
    /// <summary> Default height of the smudge. </summary>
    private float smudgeHeightDefault;

    private void Start()
    {
        // Set all the defaults
        keyFrameWidthDefault = keyFrame.sizeDelta.x;
        keyFrameHeightDefault = keyFrame.sizeDelta.y;
        smudgeWidthDefault = smudge.sizeDelta.x;
        smudgeHeightDefault = smudge.sizeDelta.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the size needed for the text is bigger than the default size.
        if (text.preferredWidth > keyFrameWidthDefault)
        {
            // Make the frame and smudge bigger.
            keyFrame.sizeDelta = new Vector2(text.preferredWidth + 25, keyFrameHeightDefault);
            smudge.sizeDelta = new Vector2(text.preferredWidth + 25 + (smudgeWidthDefault - keyFrameWidthDefault), smudgeHeightDefault);
        }
        else
        {
            // Otherwise reset them.
            keyFrame.sizeDelta = new Vector2(keyFrameWidthDefault, keyFrameHeightDefault);
            smudge.sizeDelta = new Vector2(smudgeWidthDefault, smudgeHeightDefault);
        }
    }
}
