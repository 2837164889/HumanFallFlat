using HumanAPI;
using UnityEngine;

public class SignalTweenBase : MonoBehaviour
{
	public SignalBase triggerSignal;

	public bool isDiscrete = true;

	public float tweenTime = 0.2f;

	public float value;

	public bool invert;

	public Sound2 sound;

	public ServoSound servoSound;

	public bool restartServoOnSwitch;

	private float phase;

	private float from;

	private float to;

	private float GetValue()
	{
		if (isDiscrete)
		{
			return triggerSignal.boolValue ? 1 : 0;
		}
		return triggerSignal.value;
	}

	protected virtual void OnEnable()
	{
		if (!(triggerSignal == null))
		{
			triggerSignal.onValueChanged += SignalChanged;
			value = GetValue();
			from = (to = value);
			phase = 1f;
			OnValueChanged((!invert) ? value : (1f - value));
		}
	}

	protected virtual void OnDisable()
	{
		if (!(triggerSignal == null))
		{
			triggerSignal.onValueChanged -= SignalChanged;
		}
	}

	private void SignalChanged(float val)
	{
		float num = GetValue();
		if (num == to)
		{
			return;
		}
		to = num;
		from = value;
		phase = 0f;
		if (sound != null)
		{
			if (restartServoOnSwitch || !sound.isPlaying)
			{
				sound.Play();
			}
		}
		else if (servoSound != null && (restartServoOnSwitch || !servoSound.isPlaying))
		{
			servoSound.Play();
		}
	}

	public virtual void Update()
	{
		if (triggerSignal == null || phase == 1f)
		{
			return;
		}
		if (tweenTime == 0f)
		{
			phase = 1f;
		}
		else
		{
			phase = Mathf.Clamp01(phase + Time.deltaTime / tweenTime);
		}
		if (phase == 1f)
		{
			if (sound != null)
			{
				if (sound.isPlaying)
				{
					sound.Stop();
				}
			}
			else if (servoSound != null && servoSound.isPlaying)
			{
				servoSound.Stop();
			}
		}
		value = Mathf.Lerp(from, to, Ease.easeInOutQuad(0f, 1f, phase));
		OnValueChanged((!invert) ? value : (1f - value));
	}

	public virtual void OnValueChanged(float value)
	{
	}
}
