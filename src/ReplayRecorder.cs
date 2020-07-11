using Multiplayer;
using System.Collections.Generic;
using UnityEngine;

public class ReplayRecorder : MonoBehaviour
{
	public enum ReplayState
	{
		None,
		Record,
		Stop,
		PlayForward,
		PlayBackward
	}

	public static ReplayRecorder instance;

	public ReplayState state;

	public int currentFrame;

	public float recordingStartTime;

	private int lastFrame;

	private Dictionary<uint, NetFrames> capture = new Dictionary<uint, NetFrames>();

	public static bool isRecording => instance.state == ReplayState.Record;

	public static bool isPlaying => instance.state == ReplayState.PlayForward || instance.state == ReplayState.PlayBackward || instance.state == ReplayState.Stop;

	public static float time
	{
		get
		{
			if (instance.state == ReplayState.None)
			{
				return Time.time;
			}
			return instance.recordingStartTime + (float)instance.currentFrame * Time.fixedDeltaTime;
		}
	}

	private void Awake()
	{
		instance = this;
	}

	public void PostSimulate()
	{
		if (Game.instance == null)
		{
			return;
		}
		if (Game.instance.state != GameState.PlayingLevel)
		{
			state = ReplayState.None;
			SubtitleManager.instance.ClearRecording();
		}
		if (state == ReplayState.None)
		{
			return;
		}
		if (state == ReplayState.Record)
		{
			CaptureFrame();
			return;
		}
		PlaybackFrame(currentFrame);
		if (state == ReplayState.PlayForward)
		{
			currentFrame++;
		}
		else if (state == ReplayState.PlayBackward)
		{
			currentFrame--;
		}
		currentFrame = Mathf.Clamp(currentFrame, 0, lastFrame);
	}

	public void PreLateUpdate()
	{
	}

	private void Update()
	{
		if (Game.instance == null)
		{
			return;
		}
		if (Game.instance.state != GameState.PlayingLevel || NetGame.isClient)
		{
			Abort();
			return;
		}
		if (state == ReplayState.Stop)
		{
			ReplayUI.instance.Show(lastFrame);
		}
		else
		{
			ReplayUI.instance.Hide();
		}
		if (Input.GetKeyDown(KeyCode.KeypadMultiply))
		{
			if (state == ReplayState.None)
			{
				ReplayUI.instance.Hide();
				BeginRecording();
				CheatCodes.NotifyCheat("REC");
				SubtitleManager.instance.SetRecording();
			}
			else if (state == ReplayState.Record)
			{
				StopRecording();
			}
			else
			{
				state = ReplayState.None;
			}
		}
		if (state == ReplayState.None || state == ReplayState.Record || state == ReplayState.None)
		{
			return;
		}
		bool key = Input.GetKey(KeyCode.LeftShift);
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			currentFrame -= ((!key) ? 1 : 10);
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			currentFrame += ((!key) ? 1 : 10);
		}
		if (Input.GetKeyDown(KeyCode.Less))
		{
			currentFrame--;
		}
		if (Input.GetKeyDown(KeyCode.Greater))
		{
			currentFrame++;
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (state == ReplayState.Stop)
			{
				state = ((!key) ? ReplayState.PlayForward : ReplayState.PlayBackward);
			}
			else
			{
				state = ReplayState.Stop;
			}
		}
		currentFrame = Mathf.Clamp(currentFrame, 0, lastFrame);
	}

	public static void Stop()
	{
		if (isRecording)
		{
			instance.StopRecording();
		}
	}

	public static void Abort()
	{
		if (isRecording)
		{
			instance.StopRecording();
		}
		if (isPlaying)
		{
			instance.state = ReplayState.None;
		}
	}

	private void StopRecording()
	{
		SubtitleManager.instance.ClearRecording();
		state = ReplayState.Stop;
		currentFrame = lastFrame;
	}

	private void BeginRecording()
	{
		state = ReplayState.Record;
		recordingStartTime = Time.time;
		lastFrame = 0;
		currentFrame = 0;
		capture.Clear();
	}

	private void CaptureFrame()
	{
		lastFrame = currentFrame;
		currentFrame++;
	}

	public void SubmitFrame(NetScope scope, NetStream full, NetStream events)
	{
		if (!capture.TryGetValue(scope.netId, out NetFrames value))
		{
			value = new NetFrames();
			capture[scope.netId] = value;
		}
		if (full != null)
		{
			value.PushState(currentFrame, full.AddRef());
		}
		if (events != null)
		{
			value.PushEvents(currentFrame, events.AddRef());
		}
	}

	public void Play(NetScope scope)
	{
		if (!scope.exitingLevel && capture.TryGetValue(scope.netId, out NetFrames value))
		{
			scope.RenderState(value, currentFrame, 0f);
			if (state == ReplayState.PlayForward)
			{
				scope.PlaybackEvents(value, currentFrame - 1, currentFrame);
			}
		}
	}

	private void PlaybackFrame(int frame)
	{
	}
}
