using Multiplayer;
using UnityEngine;

public class PlanksNoThanksAchievement : MonoBehaviour, IReset
{
	public static PlanksNoThanksAchievement instance;

	public float plankMoveThreshold;

	public float plankAngleThreshold;

	public NetBody[] plankNetBodies;

	private bool isCancelled;

	private bool hasCheckedCheckpointStart;

	public static bool AchievementValid => instance != null && !instance.isCancelled;

	private void Awake()
	{
		instance = this;
	}

	public void OnDestroy()
	{
		instance = null;
	}

	public void Update()
	{
		if (NetGame.isClient || isCancelled)
		{
			return;
		}
		NetBody[] array = plankNetBodies;
		int num = 0;
		while (true)
		{
			if (num >= array.Length)
			{
				return;
			}
			NetBody netBody = array[num];
			if (netBody != null && netBody.RigidBody != null)
			{
				NetBody netBody2 = netBody;
				Rigidbody rigidBody = netBody.RigidBody;
				if (Vector3.Distance(rigidBody.position, netBody2.startPos) >= plankMoveThreshold || Quaternion.Angle(rigidBody.rotation, netBody2.startRot) >= plankAngleThreshold)
				{
					break;
				}
			}
			num++;
		}
		isCancelled = true;
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		if (!hasCheckedCheckpointStart || checkpoint == 0)
		{
			hasCheckedCheckpointStart = true;
			isCancelled = (checkpoint != 0);
		}
	}
}
