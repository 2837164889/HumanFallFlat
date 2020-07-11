using HumanAPI;
using Multiplayer;
using System;
using UnityEngine;

[Serializable]
public class CollisionAudioHitConfig
{
	[NonSerialized]
	public ushort netId;

	public SampleLibrary sampleLib;

	public float levelDB;

	public float compTresholdDB;

	public float compRatio = 4f;

	public float velocityComp = 1f;

	public float impulseComp = 10f;

	private float minImpact = 1f;

	public float pitch0 = 1f;

	public float pitch1 = 1f;

	public float pitch0velocity;

	public float pitch1velocity = 100f;

	public bool Play(CollisionAudioSensor sensor, AudioChannel channel, CollisionAudioHitMonitor monitor, Vector3 pos, float impulse, float velocity, float volume, float pitch)
	{
		float num = impulse / CollisionAudioEngine.instance.unitImpulse;
		float num2 = velocity / CollisionAudioEngine.instance.unitVelocity;
		if (num > 1f)
		{
			num = (num - 1f) / impulseComp + 1f;
		}
		if (num2 > 1f)
		{
			num2 = (num2 - 1f) / velocityComp + 1f;
		}
		float t = Mathf.InverseLerp(pitch0velocity, pitch1velocity, velocity);
		pitch *= Mathf.Lerp(pitch0, pitch1, t);
		float num3 = num * num2 * volume;
		if (NetGame.isServer || ReplayRecorder.isRecording)
		{
			sensor.BroadcastCollisionAudio(this, channel, pos, num3, pitch);
		}
		return PlayWithKnownEmit(channel, monitor, pos, num3, pitch);
	}

	public bool PlayWithKnownEmit(AudioChannel channel, CollisionAudioHitMonitor monitor, Vector3 pos, float emit, float pitch)
	{
		if (Listener.instance == null)
		{
			return false;
		}
		float magnitude = (pos - Listener.instance.transform.position).magnitude;
		float num = (!(magnitude < 2f)) ? (2f / magnitude) : 1f;
		float num2 = AudioUtils.DBToValue(compTresholdDB) / num;
		float num3 = 1f;
		if (emit > num2)
		{
			float num4 = AudioUtils.ValueToDB(emit);
			float num5 = AudioUtils.ValueToDB(num2);
			float decibel = (num5 - num4) * (1f - 1f / compRatio);
			num3 = AudioUtils.DBToValue(decibel);
		}
		float num6 = emit * num3;
		float rms = num6 * AudioUtils.DBToValue(levelDB);
		return sampleLib.PlayRMS(channel, pos, rms, pitch);
	}
}
