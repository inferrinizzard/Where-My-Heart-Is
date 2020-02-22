using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Handles state behavior. </summary>
public interface IStateMachine
{
	/// <summary> Reference to the current state. </summary>
	// protected PlayerState State;

	/// <summary> Sets current state. </summary>
	/// <param name="state"> State to be set to. </param>
	void SetState(PlayerState state);

	void OnEnable();

	void OnDisable();
}
