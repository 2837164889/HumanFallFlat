using UnityEngine;

public class StatMonitorHuman : MonoBehaviour
{
	private Human human;

	private Vector3 oldPos;

	private Vector3 climbStartPos;

	private Vector3 walkStartPos;

	private HumanState oldState;

	private bool oldOnGround;

	private void Awake()
	{
		human = GetComponent<Human>();
	}

	private void Update()
	{
		if (human == null)
		{
			return;
		}
		Vector3 position = human.transform.position;
		HumanState state = human.state;
		bool onGround = human.onGround;
		if (oldState != HumanState.Walk && state == HumanState.Walk)
		{
			walkStartPos = position;
		}
		if (oldState == HumanState.Walk)
		{
			float magnitude = (position - walkStartPos).ZeroY().magnitude;
			if (state != HumanState.Walk)
			{
				if (magnitude > 0.5f)
				{
					StatsAndAchievements.AddTravel(human, magnitude);
				}
			}
			else if (magnitude > 10f)
			{
				StatsAndAchievements.AddTravel(human, magnitude);
				walkStartPos = position;
			}
		}
		if (oldState == HumanState.Climb && (state == HumanState.Jump || state == HumanState.Idle || state == HumanState.Walk))
		{
			float num = position.y - climbStartPos.y;
			if (num > 0f)
			{
				StatsAndAchievements.AddClimb(human, num);
			}
		}
		if (state == HumanState.Climb && oldState != HumanState.Jump && oldState != HumanState.Climb)
		{
			climbStartPos = oldPos;
		}
		else if (state == HumanState.Jump && oldState != HumanState.Jump)
		{
			climbStartPos = oldPos;
		}
		else if (state == HumanState.Climb && position.y - climbStartPos.y < 0f)
		{
			climbStartPos = oldPos;
		}
		if (state == HumanState.Jump && oldState != HumanState.Jump)
		{
			StatsAndAchievements.IncreaseJumpCount(human);
		}
		oldState = state;
		oldPos = position;
	}
}
