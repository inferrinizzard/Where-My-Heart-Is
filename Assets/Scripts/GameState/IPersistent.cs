interface IPersistent
{
	void OnBeginTransition();
	void TransitionUpdate();
	void OnCompleteTransition();
}
