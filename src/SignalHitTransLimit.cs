using HumanAPI;
using UnityEngine;

public class SignalHitTransLimit : Node
{
	private bool startChecking;

	public NodeOutput limitReached;

	public NodeInput beginChecking;

	[Tooltip("The amount of movement to check for")]
	public float translationAmount;

	[Tooltip("Check the X Position")]
	public bool xPosition;

	[Tooltip("Check the Y Position")]
	public bool yPosition;

	[Tooltip("Check the Z Position")]
	public bool zPosition;

	private bool checkSettings;

	private float currentPosition;

	public float targetPosition;

	[Tooltip("Do we need to continually check the position")]
	public bool onceOnly = true;

	[Tooltip("Whether or not to see the output from this component")]
	public bool showDebug;

	private void Start()
	{
		calcPositionSetting();
		targetPosition = currentPosition + targetPosition;
		targetPosition = Round2Decilamls(targetPosition);
		if (showDebug)
		{
			Debug.Log(base.name + " current position = " + currentPosition);
		}
		if (showDebug)
		{
			Debug.Log(base.name + " Target Position = " + targetPosition);
		}
	}

	private void Update()
	{
		if (checkSettings)
		{
			calcPositionSetting();
			if (showDebug)
			{
				Debug.Log(base.name + " Position " + currentPosition);
			}
			if (currentPosition == targetPosition)
			{
				FireoffSignal();
			}
			else
			{
				limitReached.SetValue(0f);
			}
		}
	}

	private float Round2Decilamls(float roundingValue)
	{
		return Mathf.Round(roundingValue * 100f) / 100f;
	}

	public override void Process()
	{
		if (beginChecking.value > 0.5f)
		{
			checkSettings = true;
		}
	}

	private void FireoffSignal()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Limit Reached ");
		}
		limitReached.SetValue(1f);
		if (onceOnly)
		{
			checkSettings = false;
		}
	}

	private void calcPositionSetting()
	{
		if (xPosition)
		{
			Vector3 localPosition = base.transform.localPosition;
			currentPosition = localPosition.x;
		}
		if (yPosition)
		{
			Vector3 localPosition2 = base.transform.localPosition;
			currentPosition = localPosition2.y;
		}
		if (zPosition)
		{
			Vector3 localPosition3 = base.transform.localPosition;
			currentPosition = localPosition3.z;
		}
		currentPosition = Round2Decilamls(currentPosition);
	}
}
