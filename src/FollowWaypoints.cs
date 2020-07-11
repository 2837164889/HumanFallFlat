using HumanAPI;
using Multiplayer;
using UnityEngine;

public class FollowWaypoints : Node
{
	[Tooltip("Array of the locations to move through")]
	public Transform[] waypoints;

	[Tooltip("Whether this thing is powered or not")]
	public NodeInput power;

	[Tooltip("Whether this thing is getting and input or not")]
	public NodeInput input;

	[Tooltip("Whether this thing has finished a loop through the waypoints")]
	public NodeOutput finished;

	[Tooltip("Whether or not to start again once complete")]
	public bool loop = true;

	[Tooltip("The speed at which the thing moves through the waypoints")]
	public float speed = 1f;

	[Tooltip("The current location for this thing in terms of waypoints")]
	public int curPoint = -1;

	private void Awake()
	{
		if (curPoint != -1)
		{
			return;
		}
		float num = (waypoints[0].position - base.transform.position).magnitude;
		for (int i = 0; i < waypoints.Length; i++)
		{
			float magnitude = (waypoints[i].position - base.transform.position).magnitude;
			if (magnitude < num)
			{
				num = magnitude;
				curPoint = i;
			}
		}
	}

	private void FixedUpdate()
	{
		if (!ReplayRecorder.isPlaying && !NetGame.isClient && power.value != 0f && input.value != 0f)
		{
			Advance(input.value * speed * Time.fixedDeltaTime);
		}
	}

	private void Advance(float delta)
	{
		int num = curPoint + 1;
		if (num == waypoints.Length && loop)
		{
			num = 0;
			finished.SetValue(0f);
		}
		base.transform.position = Vector3.MoveTowards(base.transform.position, waypoints[num].position, delta);
		base.transform.forward = Vector3.MoveTowards(base.transform.forward, waypoints[num].forward, delta);
		if ((base.transform.position - waypoints[num].position).magnitude < 0.1f)
		{
			curPoint = num;
		}
	}
}
