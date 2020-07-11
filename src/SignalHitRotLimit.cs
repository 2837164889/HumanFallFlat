using HumanAPI;
using System.Collections;
using UnityEngine;

public class SignalHitRotLimit : Node
{
	private bool checkSettings;

	public NodeOutput limitReached;

	public NodeInput beginChecking;

	[Tooltip("The amount of rotation to check for")]
	public float rotationTarget;

	[Tooltip("How accuarate the limit should be")]
	public float degreeOfAccuracy = 2f;

	[Tooltip("Check the X Rotation")]
	public bool xRotation;

	[Tooltip("Check the Y Rotation")]
	public bool yRotation;

	[Tooltip("Check the Z Rotation")]
	public bool zRotation;

	[Tooltip("Whether or we need an input to start checking the level of rotation")]
	public bool alwaysCheck;

	private float currentRotation;

	private float repeatedRotationTarget;

	private float currentTarget;

	[Tooltip("Do we need to continually check the roation position")]
	public bool onceOnly = true;

	[Tooltip("Whether to repeat the check")]
	public bool repeatedCheck;

	[Tooltip("Whether or not to see the output from this component")]
	public bool showDebug;

	private bool waiting;

	private void Start()
	{
		CalcEulerSetting();
		if (showDebug)
		{
			Debug.Log(base.name + " Initial current Rotation = " + currentRotation);
			Debug.Log(base.name + " Initial Target Rotation = " + rotationTarget);
		}
		repeatedRotationTarget = rotationTarget;
		if (!repeatedCheck)
		{
			return;
		}
		if (currentTarget >= 0f)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Change Rotation A ");
			}
			rotationTarget = currentRotation + repeatedRotationTarget;
		}
		else if (rotationTarget < 0f)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Change Rotatation B ");
			}
			rotationTarget = currentTarget - repeatedRotationTarget;
		}
	}

	private void Update()
	{
		if (!checkSettings && !alwaysCheck)
		{
			return;
		}
		CalcEulerSetting();
		if (repeatedCheck)
		{
			currentTarget = currentRotation - rotationTarget;
			if (currentTarget < 0f)
			{
				currentTarget *= -1f;
			}
		}
		if (currentRotation >= rotationTarget - degreeOfAccuracy && currentRotation <= rotationTarget + degreeOfAccuracy)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Sending the signal now ");
			}
			FireoffSignal();
			if (repeatedCheck)
			{
				rotationTarget = currentRotation + repeatedRotationTarget;
			}
			if (rotationTarget > 360f)
			{
				rotationTarget -= 360f;
			}
		}
	}

	public override void Process()
	{
		if (beginChecking.value > 0.5f || alwaysCheck)
		{
			checkSettings = true;
		}
		else
		{
			checkSettings = false;
		}
	}

	private void FireoffSignal()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Limit Reached ");
		}
		limitReached.SetValue(1f);
		waiting = true;
		StartCoroutine(ResetLimiReached());
	}

	private void CalcEulerSetting()
	{
		if (xRotation)
		{
			Vector3 eulerAngles = base.transform.localRotation.eulerAngles;
			currentRotation = eulerAngles.x;
		}
		if (yRotation)
		{
			Vector3 eulerAngles2 = base.transform.localRotation.eulerAngles;
			currentRotation = eulerAngles2.y;
		}
		if (zRotation)
		{
			Vector3 eulerAngles3 = base.transform.localRotation.eulerAngles;
			currentRotation = eulerAngles3.z;
		}
		if (currentRotation < 0f)
		{
			currentRotation *= -1f;
		}
		if (showDebug)
		{
			Debug.Log(base.name + " Current Rotation = " + currentRotation);
			Debug.Log(base.name + " Current Rotation Target = " + rotationTarget);
		}
	}

	public IEnumerator ResetLimiReached()
	{
		yield return new WaitForSeconds(0.1f);
		limitReached.SetValue(0f);
		waiting = false;
		if (showDebug)
		{
			Debug.Log(base.name + " Just changed the value back ");
		}
	}
}
