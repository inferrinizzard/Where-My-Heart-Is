using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
	public ApplyMask mask;

	public void Awake()
	{
		mask = GetComponent<ApplyMask>();
	}
	public void ToggleMask(bool on) => mask.enabled = on;
}
