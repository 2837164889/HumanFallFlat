using UnityEngine;

public class FeetPushTutorial : TutorialBlock
{
	public Rigidbody platform;

	public float platformDistanceSuccess = 1f;

	private Vector3 platformStartPos;

	protected override void OnEnable()
	{
		base.OnEnable();
		platformStartPos = platform.position;
	}

	public override bool IsPlayerActivityMakingSense()
	{
		for (int i = 0; i < Human.all.Count; i++)
		{
			Human human = Human.all[i];
			if (human.hasGrabbed && human.groundManager.IsStanding(platform.gameObject) && platform.velocity.magnitude > 0.5f)
			{
				return true;
			}
		}
		return false;
	}

	public override bool CheckInstantSuccess(bool playerInside)
	{
		return GroundManager.IsStandingAny(platform.gameObject) && (platform.position - platformStartPos).magnitude > platformDistanceSuccess;
	}
}
