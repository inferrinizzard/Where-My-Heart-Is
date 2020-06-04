using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyBindButtonHandler : MonoBehaviour
{
    public Text text;
    public RectTransform keyFrame;
    public RectTransform smudge;

    private float keyFrameWidthDefault;
    private float keyFrameHeightDefault;
    private float smudgeWidthDefault;
    private float smudgeHeightDefault;

    private void Start()
    {
        keyFrameWidthDefault = keyFrame.sizeDelta.x;
        keyFrameHeightDefault = keyFrame.sizeDelta.y;
        smudgeWidthDefault = smudge.sizeDelta.x;
        smudgeHeightDefault = smudge.sizeDelta.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (text.preferredWidth > keyFrameWidthDefault)
        {
            keyFrame.sizeDelta = new Vector2(text.preferredWidth + 25, keyFrameHeightDefault);
            smudge.sizeDelta = new Vector2(text.preferredWidth + 25 + (smudgeWidthDefault - keyFrameWidthDefault), smudgeHeightDefault);
        }
        else
        {
            keyFrame.sizeDelta = new Vector2(keyFrameWidthDefault, keyFrameHeightDefault);
            smudge.sizeDelta = new Vector2(smudgeWidthDefault, smudgeHeightDefault);
        }
    }
}
