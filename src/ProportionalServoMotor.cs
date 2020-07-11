using HumanAPI;
using UnityEngine;

public abstract class ProportionalServoMotor : MonoBehaviour, IReset
{
	public SignalBase triggerSignal;

	public float deadzoneFrom = -0.1f;

	public float deadzoneTo = 0.1f;

	public bool resetSignalOnStop;

	public bool stopSoundOnStop;

	public float blockedDirection;

	public float resetSignalDelay = 1f;

	public SignalBase powerSignal;

	public float neededVoltage = 1f;

	public float minValue = -1f;

	public float maxValue = 1f;

	public float maxVelocity = 1f;

	public float timeToAccelerate;

	public Sound2 sound;

	public ServoSound servoSound;

	public float pitchPositive = 1f;

	public float pitchNegative = 1f;

	public float pitchLimit = 0.8f;

	public float velocity;

	public float targetValue;

	private float originalTargetValue;

	private float timeStopped;

	private float oldPower = 1f;

	protected virtual void OnEnable()
	{
		originalTargetValue = targetValue;
		Debug.LogError("ProportionalServoMotor", base.gameObject);
		Object.Destroy(this);
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		targetValue = originalTargetValue;
		velocity = 0f;
		blockedDirection = 0f;
		TargetValueChanged(targetValue);
		if (sound != null)
		{
			if (sound.isPlaying)
			{
				sound.StopSound();
			}
		}
		else if (servoSound != null && servoSound.isPlaying)
		{
			servoSound.Stop();
		}
	}

	private void FixedUpdate()
	{
		float value = triggerSignal.value;
		value = ((value < deadzoneFrom) ? ((value - deadzoneFrom) / (1f + deadzoneFrom)) : ((!(value > deadzoneTo)) ? 0f : ((value - deadzoneTo) / (1f + deadzoneTo))));
		float num = (!(powerSignal != null)) ? 1f : (powerSignal.value / neededVoltage);
		if (Mathf.Abs(num) < 0.5f)
		{
			num = 0f;
			value = 0f;
		}
		if (oldPower != num)
		{
			PowerChanged(Mathf.Abs(num));
		}
		oldPower = num;
		value *= num;
		float num2 = maxVelocity * value;
		if (timeToAccelerate == 0f)
		{
			velocity = num2;
		}
		else
		{
			if (velocity > 0f && num2 <= velocity)
			{
				velocity = Mathf.Max(0f, num2);
			}
			if (velocity < 0f && num2 >= velocity)
			{
				velocity = Mathf.Min(0f, num2);
			}
			velocity = Mathf.MoveTowards(velocity, num2, maxVelocity * Time.fixedDeltaTime / timeToAccelerate);
		}
		if (velocity != 0f)
		{
			if (stopSoundOnStop && Mathf.Sign(velocity) == blockedDirection)
			{
				return;
			}
			blockedDirection = 0f;
			if (sound != null)
			{
				if (!sound.isPlaying)
				{
					sound.Play();
				}
			}
			else if (servoSound != null && !servoSound.isPlaying)
			{
				servoSound.Play();
			}
			float num3 = targetValue;
			targetValue = Mathf.Clamp(targetValue + velocity * Time.fixedDeltaTime, minValue, maxValue);
			if (targetValue == num3)
			{
				timeStopped += Time.fixedDeltaTime;
				if (timeStopped > resetSignalDelay)
				{
					if (resetSignalOnStop)
					{
						triggerSignal.ResetSignal();
					}
					if (stopSoundOnStop)
					{
						if (sound != null)
						{
							sound.Stop();
						}
						else
						{
							servoSound.Stop();
						}
						blockedDirection = Mathf.Sign(velocity);
					}
				}
			}
			else
			{
				blockedDirection = 0f;
				timeStopped = 0f;
			}
			if (sound != null)
			{
				sound.SetPitch(((!(velocity > 0f)) ? pitchNegative : pitchPositive) * ((targetValue != num3) ? 1f : pitchLimit));
			}
			else if (servoSound != null)
			{
				servoSound.SetPitch(((!(velocity > 0f)) ? pitchNegative : pitchPositive) * ((targetValue != num3) ? 1f : pitchLimit));
			}
			TargetValueChanged(targetValue);
		}
		else if (sound != null)
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

	protected abstract void TargetValueChanged(float targetValue);

	protected virtual void PowerChanged(float powerPercent)
	{
	}
}
