using HumanAPI;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioRouting : MonoBehaviour
{
	public AudioMixerGroup footsteps;

	public AudioMixerGroup body;

	public AudioMixerGroup grab;

	public AudioMixerGroup dialogue;

	public AudioMixerGroup tutorials;

	public AudioMixerGroup music;

	public AudioMixerGroup ambience;

	public AudioMixerGroup effects;

	public AudioMixerGroup physics;

	public AudioMixerGroup ambienceFX;

	private static AudioRouting instance;

	private void Awake()
	{
		instance = this;
	}

	public static AudioMixerGroup GetChannel(AudioChannel channel)
	{
		switch (channel)
		{
		case AudioChannel.Footsteps:
			return instance.footsteps;
		case AudioChannel.Body:
			return instance.body;
		case AudioChannel.Grab:
			return instance.grab;
		case AudioChannel.Dialogue:
			return instance.dialogue;
		case AudioChannel.Tutorials:
			return instance.tutorials;
		case AudioChannel.Music:
			return instance.music;
		case AudioChannel.Ambience:
			return instance.ambience;
		case AudioChannel.Effects:
			return instance.effects;
		case AudioChannel.Physics:
			return instance.physics;
		case AudioChannel.AmbienceFX:
			return instance.ambienceFX;
		default:
			throw new InvalidOperationException();
		}
	}
}
