using HumanAPI;
using UnityEngine;

public class BreakOnSignal : MonoBehaviour, IPostReset
{
	public Sound2 sound;

	public float treshold;

	public Joint jointToBreak;

	public SignalBase source;

	public GameObject jointHolder;

	private void Awake()
	{
		if (jointHolder == null)
		{
			jointHolder = jointToBreak.gameObject;
		}
		if (jointHolder.GetComponents<Joint>().Length > 1)
		{
			Debug.LogError("BreakOnSignal has multiple jooints", this);
		}
	}

	private void OnEnable()
	{
		source.onValueChanged += SignalChanged;
	}

	private void OnDisable()
	{
		source.onValueChanged -= SignalChanged;
	}

	private void SignalChanged(float value)
	{
		if (base.enabled && value > treshold)
		{
			CGShift component = jointToBreak.GetComponent<CGShift>();
			if (component != null)
			{
				component.ResetCG();
			}
			Object.Destroy(jointToBreak);
			base.enabled = false;
			if (sound != null)
			{
				sound.PlayOneShot();
			}
		}
	}

	public void PostResetState(int checkpoint)
	{
		base.enabled = true;
		jointToBreak = jointHolder.GetComponent<Joint>();
	}
}
