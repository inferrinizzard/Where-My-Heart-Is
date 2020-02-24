using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineObject : MonoBehaviour
{
	// <summary> Material to be used for glow outline colour </summary>
	public Material glowMaterial;

	// <summary> If the outline cares about occlusion (probably only canvas is false) </summary>
	public bool depthCheck = true;
}
