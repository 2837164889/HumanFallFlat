using Multiplayer;
using UnityEngine;

public class TutorialRemote : MonoBehaviour, IGrabbable, INetBehavior
{
	public TutorialScreen screen;

	public TutorialRemote[] sameTutorials;

	private NetBody netBody;

	public void OnGrab()
	{
		if (!NetGame.isClient)
		{
			screen.Show();
		}
		if (sameTutorials != null && !NetGame.isClient)
		{
			for (int i = 0; i < sameTutorials.Length; i++)
			{
				sameTutorials[i].netBody.SetVisible(visible: false);
			}
		}
	}

	public void OnRelease()
	{
		if (!NetGame.isClient)
		{
			screen.Hide();
		}
		GrabManager.Release(screen.gameObject, 0.3f);
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		base.gameObject.SetActive(value: true);
	}

	public void StartNetwork(NetIdentity identity)
	{
		netBody = GetComponent<NetBody>();
	}

	public void SetMaster(bool isMaster)
	{
	}

	public void CollectState(NetStream stream)
	{
		NetBoolEncoder.CollectState(stream, netBody.isVisible);
		NetBoolEncoder.CollectState(stream, screen.isVisible);
	}

	private void ApplyRemote(bool show)
	{
		if (show != netBody.isVisible)
		{
			netBody.SetVisible(show);
		}
	}

	private void ApplyScreen(bool show)
	{
		if (show != screen.isVisible)
		{
			if (show)
			{
				screen.Show();
			}
			else
			{
				screen.Hide();
			}
		}
	}

	public void ApplyState(NetStream state)
	{
		ApplyRemote(NetBoolEncoder.ApplyState(state));
		ApplyScreen(NetBoolEncoder.ApplyState(state));
	}

	public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
	{
		ApplyRemote(NetBoolEncoder.ApplyLerpedState(state0, state1, mix));
		ApplyScreen(NetBoolEncoder.ApplyLerpedState(state0, state1, mix));
	}

	public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
	{
		NetBoolEncoder.CalculateDelta(state0, state1, delta);
		NetBoolEncoder.CalculateDelta(state0, state1, delta);
	}

	public void AddDelta(NetStream state0, NetStream delta, NetStream result)
	{
		NetBoolEncoder.AddDelta(state0, delta, result);
		NetBoolEncoder.AddDelta(state0, delta, result);
	}

	public int CalculateMaxDeltaSizeInBits()
	{
		return 2 * NetBoolEncoder.CalculateMaxDeltaSizeInBits();
	}
}
