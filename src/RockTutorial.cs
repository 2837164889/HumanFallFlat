using UnityEngine;

public class RockTutorial : TutorialBlock
{
	public Rigidbody body;

	public float angleDistanceSuccess = 10f;

	private Quaternion bodyStartRot;

	protected override void OnEnable()
	{
		base.OnEnable();
		bodyStartRot = body.rotation;
	}

	public override bool IsPlayerActivityMakingSense()
	{
		return GroundManager.IsStandingAny(body.gameObject) && body.angularVelocity.magnitude > 0.1f;
	}

	public override bool CheckInstantSuccess(bool playerInside)
	{
		Vector3 eulerAngles = body.rotation.eulerAngles;
		float z = eulerAngles.z;
		Vector3 eulerAngles2 = bodyStartRot.eulerAngles;
		return Mathf.Abs(Math2d.NormalizeAngleDeg(z - eulerAngles2.z)) > angleDistanceSuccess;
	}
}
