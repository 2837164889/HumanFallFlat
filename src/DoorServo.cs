using UnityEngine;

public class DoorServo : MonoBehaviour
{
	public SignalBase triggerSignal;

	public ServoSound servoSound1;

	public ServoSound servoSound2;

	public Rigidbody leftDoor;

	public Rigidbody rightDoor;

	public float minWidth;

	public float maxWidth;

	public float maxTensionWidth = 0.2f;

	public float maxVelocity = 1f;

	private ConfigurableJoint leftJoint;

	private ConfigurableJoint rightJoint;

	private Vector3 leftStart;

	private Vector3 rightStart;

	public float targetWidth;

	public float currentWidth;

	private void Start()
	{
		leftStart = leftDoor.position;
		rightStart = rightDoor.position;
		targetWidth = Mathf.Lerp(minWidth, maxWidth, triggerSignal.value);
		leftJoint = leftDoor.GetComponent<ConfigurableJoint>();
		rightJoint = rightDoor.GetComponent<ConfigurableJoint>();
		leftJoint.targetPosition = new Vector3((0f - targetWidth) / 2f, 0f, 0f);
		rightJoint.targetPosition = new Vector3((0f - targetWidth) / 2f, 0f, 0f);
	}

	private void FixedUpdate()
	{
		float num = targetWidth;
		float target = Mathf.Lerp(minWidth, maxWidth, triggerSignal.value);
		targetWidth = Mathf.MoveTowards(targetWidth, target, maxVelocity * Time.fixedDeltaTime);
		currentWidth = (leftDoor.position - rightDoor.position).magnitude;
		targetWidth = Mathf.MoveTowards(currentWidth, targetWidth, maxTensionWidth);
		float num2 = targetWidth;
		if (num != targetWidth)
		{
			Vector3 rhs = new Vector3((0f - num2) / 2f, 0f, 0f);
			if (leftJoint.targetPosition != rhs)
			{
				leftJoint.targetPosition = new Vector3((0f - num2) / 2f, 0f, 0f);
				leftDoor.WakeUp();
			}
			if (rightJoint.targetPosition != rhs)
			{
				rightJoint.targetPosition = new Vector3((0f - num2) / 2f, 0f, 0f);
				rightDoor.WakeUp();
			}
			if (servoSound1 != null && !servoSound1.isPlaying)
			{
				servoSound1.Play();
			}
			if (servoSound2 != null && !servoSound2.isPlaying)
			{
				servoSound2.Play();
			}
		}
		else
		{
			if (servoSound1 != null && servoSound1.isPlaying)
			{
				servoSound1.Stop();
			}
			if (servoSound2 != null && servoSound2.isPlaying)
			{
				servoSound2.Stop();
			}
		}
	}
}
