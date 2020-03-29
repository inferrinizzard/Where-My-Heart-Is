interface IResetable
{
	/// <summary> called when next level loaded </summary>
	void Init();
	/// <summary> called when current level unloaded </summary>
	void Reset();
}
