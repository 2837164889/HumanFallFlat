using UnityEngine;

public class ElevatorServo : MonoBehaviour
{
	public ServoSound servoSound;

	public SignalBase triggerSignal;

	public float maxVelocity = 1f;

	public Vector3 offPos;

	public Vector3 onPos;

	private ConfigurableJoint joint;

	private Rigidbody body;

	private Vector3 startPos;

	private Vector3 targetPos;

	private void OnEnable()
	{
		targetPos = ((!triggerSignal.boolValue) ? offPos : onPos);
		body = GetComponent<Rigidbody>();
		if (!body.isKinematic)
		{
			joint = GetComponent<ConfigurableJoint>();
			joint.targetPosition = targetPos;
		}
		startPos = body.position;
	}

	private void FixedUpdate()
	{
		Vector3 lhs = targetPos;
		Vector3 target = (!triggerSignal.boolValue) ? offPos : onPos;
		targetPos = Vector3.MoveTowards(targetPos, target, maxVelocity * Time.fixedDeltaTime);
		if (lhs != targetPos)
		{
			if (!body.isKinematic)
			{
				joint.targetPosition = targetPos;
			}
			else
			{
				body.MovePosition(startPos + base.transform.parent.TransformVector(targetPos));
			}
			body.WakeUp();
			if (servoSound != null && !servoSound.isPlaying)
			{
				servoSound.Play();
			}
		}
		else if (servoSound != null && servoSound.isPlaying)
		{
			servoSound.Stop();
		}
	}
}
