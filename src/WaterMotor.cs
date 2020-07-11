using HumanAPI;
using UnityEngine;

public class WaterMotor : Node, IReset
{
	public NodeInput input;

	public SignalBase triggerSignal;

	public float force;

	public float forceLinear;

	public float forceSquare;

	public Transform forcePoint;

	public ServoSound servoSound;

	public Sound2 motorSound;

	public float pitchHigh = 1f;

	public float pitchLow = 1f;

	private FloatingMesh floatingmesh;

	private Rigidbody rigidbody;

	private void Awake()
	{
		if (floatingmesh == null)
		{
			floatingmesh = GetComponentInParent<FloatingMesh>();
		}
		if (rigidbody == null)
		{
			rigidbody = GetComponentInParent<Rigidbody>();
		}
	}

	private void FixedUpdate()
	{
		float num = 0f;
		num = ((!(triggerSignal != null)) ? input.value : triggerSignal.value);
		if (motorSound != null)
		{
			if (num != 0f && !motorSound.isPlaying)
			{
				motorSound.Play();
				char choice = (!(floatingmesh.sensor.waterBody == null)) ? 'A' : 'B';
				motorSound.Switch(choice);
			}
			else if (num == 0f && motorSound.isPlaying)
			{
				motorSound.Stop();
			}
		}
		else if (servoSound != null)
		{
			if (num != 0f && !servoSound.isPlaying)
			{
				servoSound.Play();
				bool flag = floatingmesh.sensor.waterBody == null;
				servoSound.secondMedium = (flag && servoSound.loopClips2.Length > 0);
			}
			else if (num == 0f && servoSound.isPlaying)
			{
				servoSound.Stop();
			}
		}
		Rigidbody rigidbody = this.rigidbody;
		float magnitude = rigidbody.velocity.magnitude;
		if (num != 0f)
		{
			rigidbody.SafeAddForceAtPosition(forcePoint.forward * (force + forceLinear * magnitude + forceSquare * magnitude * magnitude) * num, forcePoint.position);
		}
		if (motorSound != null)
		{
			char choice2 = (!(floatingmesh.sensor.waterBody == null)) ? 'A' : 'B';
			motorSound.Switch(choice2);
			motorSound.SetPitch(Mathf.Lerp(pitchLow, pitchHigh, Mathf.Abs(num)));
		}
		else
		{
			if (!(servoSound != null))
			{
				return;
			}
			bool flag2 = floatingmesh.sensor.waterBody == null && servoSound.loopClips2.Length > 0;
			if (servoSound.secondMedium != flag2)
			{
				servoSound.secondMedium = flag2;
				if (num != 0f)
				{
					servoSound.CrossfadeLoop();
				}
			}
			servoSound.SetPitch(Mathf.Lerp(pitchLow, pitchHigh, Mathf.Abs(num)));
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		if (servoSound != null && servoSound.isPlaying)
		{
			servoSound.Stop();
		}
	}
}
