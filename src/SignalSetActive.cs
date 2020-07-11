using UnityEngine;

public class SignalSetActive : MonoBehaviour
{
	public SignalBase triggerSignal;

	public GameObject target;

	public bool invert;

	private void OnEnable()
	{
		triggerSignal.onValueChanged += SignalChanged;
		SignalChanged(triggerSignal.value);
	}

	protected void OnDisable()
	{
		triggerSignal.onValueChanged -= SignalChanged;
	}

	private void SignalChanged(float val)
	{
		target.SetActive(triggerSignal.boolValue ^ invert);
	}
}
