using HumanAPI;
using Multiplayer;
using System;
using UnityEngine;

public class RatchetJoint : AngularJoint
{
	public Sound2 sound;

	public float toothDegrees = 30f;

	[NonSerialized]
	public bool release;

	private int currentTooth;

	private bool oldRelease;

	public override void ResetState(int checkpoint, int subObjectives)
	{
		base.ResetState(checkpoint, subObjectives);
		currentTooth = 0;
		oldRelease = (release = false);
	}

	protected override void UpdateLimitJoint()
	{
		if (release != oldRelease)
		{
			oldRelease = release;
			if (limitJoint != null)
			{
				UnityEngine.Object.Destroy(limitJoint);
			}
			if (!release)
			{
				currentTooth = Mathf.FloorToInt(Mathf.Clamp(GetValue(), minValue, maxValue) / toothDegrees);
			}
			oldRelease = release;
		}
		if (release)
		{
			base.UpdateLimitJoint();
			return;
		}
		float value = GetValue();
		int num = Mathf.FloorToInt(Mathf.Clamp(value, minValue, maxValue) / toothDegrees);
		if (num > currentTooth)
		{
			currentTooth = num;
			if (sound != null)
			{
				sound.PlayOneShot();
			}
			if (limitJoint != null)
			{
				UnityEngine.Object.Destroy(limitJoint);
			}
		}
		else if ((ReplayRecorder.isPlaying || NetGame.isClient) && num != currentTooth)
		{
			currentTooth = num;
			if (limitJoint != null)
			{
				UnityEngine.Object.Destroy(limitJoint);
			}
		}
		if (limitJoint == null)
		{
			float num2 = (float)num * toothDegrees;
			float num3 = num2 + 2f * toothDegrees;
			if (num3 > maxValue)
			{
				num3 = maxValue;
			}
			limitJoint = CreateLimitJoint(num2 - value, num3 - value);
		}
		limitUpdateValue = value;
	}
}
