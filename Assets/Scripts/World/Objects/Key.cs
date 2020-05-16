using UnityEngine;

public class Key : Pickupable
{
	void Awake() => dissolves = true;
	public void Destroy() => Destroy(gameObject);
}
