using HumanAPI;
using Multiplayer;
using System;
using UnityEngine;

public class JointScriptRatchet1 : AngularJoint
{
	[Tooltip("A Reference to the sound to play when ratchetting")]
	public AudioSource audioSource1;

	[Tooltip("A Reference to the sound to play when ratchetting")]
	public AudioSource audioSource2;

	[Tooltip("A reference to the amount of degrees between teeth")]
	public float toothDegrees = 30f;

	[NonSerialized]
	[Tooltip("Whether or not the ratchet joint has been released")]
	public bool release;

	private int currentTooth;

	private bool oldRelease;

	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	public override void ResetState(int checkpoint, int subObjectives)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Reset the State ");
		}
		base.ResetState(checkpoint, subObjectives);
		currentTooth = 0;
		oldRelease = (release = false);
	}

	protected override void UpdateLimitJoint()
	{
		if (release != oldRelease)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Release does not match oldrelease");
			}
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
			if (audioSource1 != null)
			{
				audioSource1.Play();
			}
			if (audioSource2 != null)
			{
				audioSource2.Play();
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
