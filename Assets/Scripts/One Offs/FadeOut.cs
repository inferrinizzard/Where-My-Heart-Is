using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    public Texture2D fadeOutTexture;
    public float fadeSpeed = 0.8f;

    private int drawDepth = -1000; // the texture's order in the draw hierarchy: a low number means it renders on top
    private float alpha = -1.0f; // 1 fade in, -1.0 fade out
    private int fadeDir = 1; //-1 fade in, 1 fade out

    private void Start()
    {
        fadeOutTexture = Resources.Load<Texture2D>("black");

    }

    private void OnGUI()
    {
        alpha += fadeDir * fadeSpeed * Time.deltaTime;
        alpha = Mathf.Clamp01(alpha);

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = drawDepth;  // render texture on top
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
    }

    public float BeginFade (int direction)
    {
        fadeDir = direction;
        return (fadeSpeed);
    }

    void OnLevelWasLoaded()
    {
        BeginFade(-1);  // call the fade in function
    }
}