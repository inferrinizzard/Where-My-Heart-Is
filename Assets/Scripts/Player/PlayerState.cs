using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Abstract class for various player states </summary>
public abstract class PlayerState
{
	/// <summary> Reference to player. </summary>
	protected Player player;

	/// <summary> Constructor. </summary>
	/// <param name="_player"> Reference to player. </param>
	public PlayerState(Player _player)
	{
		player = _player;
	}

	/// <summary> What a state will do when starting it. </summary>
	public virtual void Start() { }

	/// <summary> What a state will do when ending it. </summary>
	public virtual void End() { }
}
