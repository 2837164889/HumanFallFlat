using HumanAPI;
using UnityEngine;
using UnityEngine.Audio;

public class GameAudio : MonoBehaviour
{
	public const string kMusicVolumeName = "MusicVolume";

	public const string kEffectsVolumeName = "EffectsVolume";

	public AudioMixer mainMixer;

	public static GameAudio instance;

	private float musicLevel;

	private float reverbLevel;

	private float ambienceLevel;

	private float effectsLevel;

	private float physicsLevel = -18f;

	private float characterLevel = -8f;

	private float ambienceFxLevel;

	private float transitionPhase = 1f;

	private float transitionSpeed;

	private float fromMainVerb;

	private float toMainVerb;

	private float currentMainVerb;

	private float fromMusic;

	private float toMusic;

	private float currentMusic;

	private float fromAmbience;

	private float toAmbience;

	private float currentAmbience;

	private float fromEffects;

	private float toEffects;

	private float currentEffects;

	private float fromPhysics;

	private float toPhysics;

	private float currentPhysics;

	private float fromCharacter;

	private float toCharacter;

	private float currentCharacter;

	private float fromAmbienceFx;

	private float toAmbienceFx;

	private float currentAmbienceFx;

	private void OnEnable()
	{
		instance = this;
	}

	public void SetMasterLevel(float level)
	{
		mainMixer.SetFloat("MasterVolume", Options.LogMap(level, 0f, 1f, -80f, -24f, 0f));
	}

	public void SetFXLevel(float level)
	{
		mainMixer.SetFloat("FXVolume", Options.LogMap(level, 0f, 0.8f, 1f, -80f, 0f, 6f));
	}

	public void SetMusicLevel(float level)
	{
		musicLevel = Options.LogMap(level, 0f, 0.8f, 1f, -80f, 0f, 6f);
		mainMixer.SetFloat("MusicVolume", musicLevel + currentMusic);
	}

	public void SetVoiceLevel(float level)
	{
		mainMixer.SetFloat("VoiceVolume", Options.LogMap(level, 0f, 0.8f, 1f, -80f, 0f, 6f));
	}

	public void SetReverbLevel(float level)
	{
		reverbLevel = level;
		mainMixer.SetFloat("ReverbVolume", reverbLevel + currentMainVerb);
	}

	public void SetAmbienceZoneMix(AmbienceZone zone, float transitionDuration)
	{
		transitionPhase = 0f;
		transitionSpeed = 1f / transitionDuration;
		fromMainVerb = currentMainVerb;
		fromMusic = currentMusic;
		fromAmbience = currentAmbience;
		fromEffects = currentEffects;
		fromPhysics = currentPhysics;
		fromCharacter = currentCharacter;
		fromAmbienceFx = currentAmbienceFx;
		toMainVerb = zone.mainVerbLevel;
		toMusic = zone.musicLevel;
		toAmbience = zone.ambienceLevel;
		toEffects = zone.effectsLevel;
		toPhysics = zone.physicsLevel;
		toCharacter = zone.characterLevel;
		toAmbienceFx = zone.ambienceFxLevel;
	}

	private void Update()
	{
		if (transitionPhase < 1f)
		{
			transitionPhase = Mathf.Clamp01(transitionPhase + Time.deltaTime * transitionSpeed);
			currentMainVerb = Mathf.Lerp(fromMainVerb, toMainVerb, transitionPhase);
			currentMusic = Mathf.Lerp(fromMusic, toMusic, transitionPhase);
			currentAmbience = Mathf.Lerp(fromAmbience, toAmbience, transitionPhase);
			currentEffects = Mathf.Lerp(fromEffects, toEffects, transitionPhase);
			currentPhysics = Mathf.Lerp(fromPhysics, toPhysics, transitionPhase);
			currentCharacter = Mathf.Lerp(fromCharacter, toCharacter, transitionPhase);
			currentAmbienceFx = Mathf.Lerp(fromAmbienceFx, toAmbienceFx, transitionPhase);
			mainMixer.SetFloat("MusicVolume", musicLevel + currentMusic);
			mainMixer.SetFloat("AmbienceVolume", ambienceLevel + currentAmbience);
			mainMixer.SetFloat("EffectsVolume", effectsLevel + currentEffects + 6f);
			mainMixer.SetFloat("PhysicsVolume", physicsLevel + currentPhysics + 5f);
			mainMixer.SetFloat("CharacterVolume", characterLevel + currentCharacter + 5f);
			mainMixer.SetFloat("AmbienceFxVolume", ambienceFxLevel + currentAmbienceFx);
			mainMixer.SetFloat("GrabVolume", 5f);
			mainMixer.SetFloat("BodyVolume", -5f);
			mainMixer.SetFloat("FootstepsVolume", -10f);
		}
	}
}
