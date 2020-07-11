using InControl;
using UnityEngine;

public static class Options
{
	public static PlayerActions keyboardBindings;

	public static PlayerActions controllerBindings = new PlayerActions();

	public const int defaultCloudQuality = 3;

	public static int videoBrightness
	{
		get
		{
			return PlayerPrefs.GetInt("videoBrightness", 10);
		}
		set
		{
			PlayerPrefs.SetInt("videoBrightness", value);
			ApplyVideoBrightness();
		}
	}

	public static int advancedVideoPreset
	{
		get
		{
			return PlayerPrefs.GetInt("advancedVideoPreset", 4);
		}
		set
		{
			PlayerPrefs.SetInt("advancedVideoPreset", value);
		}
	}

	public static int advancedVideoClouds
	{
		get
		{
			return PlayerPrefs.GetInt("advancedVideoClouds", 3);
		}
		set
		{
			PlayerPrefs.SetInt("advancedVideoClouds", value);
		}
	}

	public static int advancedVideoShadows
	{
		get
		{
			return PlayerPrefs.GetInt("advancedVideoShadows", 4);
		}
		set
		{
			PlayerPrefs.SetInt("advancedVideoShadows", value);
		}
	}

	public static int advancedVideoTexture
	{
		get
		{
			return PlayerPrefs.GetInt("advancedVideoTexture", 2);
		}
		set
		{
			PlayerPrefs.SetInt("advancedVideoTexture", value);
		}
	}

	public static int advancedVideoAO
	{
		get
		{
			return PlayerPrefs.GetInt("advancedVideoAO", 1);
		}
		set
		{
			PlayerPrefs.SetInt("advancedVideoAO", value);
		}
	}

	public static int advancedVideoAA
	{
		get
		{
			return PlayerPrefs.GetInt("advancedVideoAA", 2);
		}
		set
		{
			PlayerPrefs.SetInt("advancedVideoAA", value);
		}
	}

	public static int advancedVideoVsync
	{
		get
		{
			return PlayerPrefs.GetInt("advancedVideoVsync", 1);
		}
		set
		{
			PlayerPrefs.SetInt("advancedVideoVsync", value);
		}
	}

	public static int advancedVideoHDR
	{
		get
		{
			return PlayerPrefs.GetInt("advancedVideoHdr", 1);
		}
		set
		{
			PlayerPrefs.SetInt("advancedVideoHdr", value);
		}
	}

	public static int advancedVideoExposure
	{
		get
		{
			return PlayerPrefs.GetInt("advancedVideoExposure", 1);
		}
		set
		{
			PlayerPrefs.SetInt("advancedVideoExposure", value);
		}
	}

	public static int advancedVideoBloom
	{
		get
		{
			return PlayerPrefs.GetInt("advancedVideoBloom", 1);
		}
		set
		{
			PlayerPrefs.SetInt("advancedVideoBloom", value);
		}
	}

	public static int advancedVideoDOF
	{
		get
		{
			return PlayerPrefs.GetInt("advancedVideoDOF", 1);
		}
		set
		{
			PlayerPrefs.SetInt("advancedVideoDOF", value);
		}
	}

	public static int advancedVideoChromatic
	{
		get
		{
			return PlayerPrefs.GetInt("advancedVideoChromatic", 1);
		}
		set
		{
			PlayerPrefs.SetInt("advancedVideoChromatic", value);
		}
	}

	public static int audioMaster
	{
		get
		{
			return PlayerPrefs.GetInt("audioMaster", 20);
		}
		set
		{
			PlayerPrefs.SetInt("audioMaster", value);
			ApplyAudioMaster();
		}
	}

	public static int audioFX
	{
		get
		{
			return PlayerPrefs.GetInt("audioFX", 16);
		}
		set
		{
			PlayerPrefs.SetInt("audioFX", value);
			ApplyAudioFX();
		}
	}

	public static int audioMusic
	{
		get
		{
			return PlayerPrefs.GetInt("audioMusic", 16);
		}
		set
		{
			PlayerPrefs.SetInt("audioMusic", value);
			ApplyAudioMusic();
		}
	}

	public static int audioVoice
	{
		get
		{
			return PlayerPrefs.GetInt("audioVoice", 16);
		}
		set
		{
			PlayerPrefs.SetInt("audioVoice", value);
			ApplyAudioVoice();
		}
	}

	public static int audioSubtitles
	{
		get
		{
			return PlayerPrefs.GetInt("audioSubtitles", 1);
		}
		set
		{
			PlayerPrefs.SetInt("audioSubtitles", value);
			ApplyAudioSubtitles();
		}
	}

	public static int audioShuffle
	{
		get
		{
			return PlayerPrefs.GetInt("audioShuffle", 0);
		}
		set
		{
			PlayerPrefs.SetInt("audioShuffle", value);
			ApplyAudioShuffle();
		}
	}

	public static int cameraFov
	{
		get
		{
			return PlayerPrefs.GetInt("cameraFov", 5);
		}
		set
		{
			PlayerPrefs.SetInt("cameraFov", value);
			ApplyCameraSettings();
		}
	}

	public static int cameraSmoothing
	{
		get
		{
			return PlayerPrefs.GetInt("cameraSmoothing", 10);
		}
		set
		{
			PlayerPrefs.SetInt("cameraSmoothing", value);
			ApplyCameraSettings();
		}
	}

	public static int controllerLayout
	{
		get
		{
			return PlayerPrefs.GetInt("controllerLayout", 0);
		}
		set
		{
			PlayerPrefs.SetInt("controllerLayout", value);
			ApplyControllerSettings();
		}
	}

	public static int controllerLookMode
	{
		get
		{
			return PlayerPrefs.GetInt("controllerLookMode", 0);
		}
		set
		{
			PlayerPrefs.SetInt("controllerLookMode", value);
			ApplyControllerSettings();
		}
	}

	public static int controllerInvert
	{
		get
		{
			return PlayerPrefs.GetInt("controllerInvert", 0);
		}
		set
		{
			PlayerPrefs.SetInt("controllerInvert", value);
			ApplyControllerSettings();
		}
	}

	public static int controllerInvertH
	{
		get
		{
			return PlayerPrefs.GetInt("controllerInvertH", 0);
		}
		set
		{
			PlayerPrefs.SetInt("controllerInvertH", value);
			ApplyControllerSettings();
		}
	}

	public static float controllerHSensitivity
	{
		get
		{
			return PlayerPrefs.GetFloat("controllerHSensitivity", 0.3f);
		}
		set
		{
			PlayerPrefs.SetFloat("controllerHSensitivity", value);
			ApplyControllerSettings();
		}
	}

	public static float controllerVSensitivity
	{
		get
		{
			return PlayerPrefs.GetFloat("controllerVSensitivity", 0.3f);
		}
		set
		{
			PlayerPrefs.SetFloat("controllerVSensitivity", value);
			ApplyControllerSettings();
		}
	}

	public static int mouseInvert
	{
		get
		{
			return PlayerPrefs.GetInt("mouseInvert", 0);
		}
		set
		{
			PlayerPrefs.SetInt("mouseInvert", value);
			ApplyMouseSettings();
		}
	}

	public static int mouseLookMode
	{
		get
		{
			return PlayerPrefs.GetInt("mouseLookMode", 0);
		}
		set
		{
			PlayerPrefs.SetInt("mouseLookMode", value);
			ApplyMouseSettings();
		}
	}

	public static float mouseSensitivity
	{
		get
		{
			return PlayerPrefs.GetFloat("mouseSensitivity", 0.3f);
		}
		set
		{
			PlayerPrefs.SetFloat("mouseSensitivity", value);
			ApplyMouseSettings();
		}
	}

	public static int lobbyLevel
	{
		get
		{
			return PlayerPrefs.GetInt("lobbyLevel", 0);
		}
		set
		{
			PlayerPrefs.SetInt("lobbyLevel", value);
		}
	}

	public static ulong multiplayerLobbyLevelStore
	{
		get
		{
			ulong num = (ulong)((long)PlayerPrefs.GetInt("multiplayerLobbyLevelH", 0) << 32);
			ulong num2 = (ulong)PlayerPrefs.GetInt("multiplayerLobbyLevelL", 0);
			return num | num2;
		}
		set
		{
			PlayerPrefs.SetInt("multiplayerLobbyLevelH", (int)(value >> 32));
			PlayerPrefs.SetInt("multiplayerLobbyLevelL", (int)(value & uint.MaxValue));
		}
	}

	public static int lobbyMaxPlayers
	{
		get
		{
			return PlayerPrefs.GetInt("lobbyMaxPlayers", 8);
		}
		set
		{
			PlayerPrefs.SetInt("lobbyMaxPlayers", value);
		}
	}

	public static int lobbyInviteOnly
	{
		get
		{
			return PlayerPrefs.GetInt("lobbyInviteOnly", 0);
		}
		set
		{
			PlayerPrefs.SetInt("lobbyInviteOnly", value);
		}
	}

	public static int lobbyJoinInProgress
	{
		get
		{
			return PlayerPrefs.GetInt("lobbyJoinInProgress", 1);
		}
		set
		{
			PlayerPrefs.SetInt("lobbyJoinInProgress", value);
		}
	}

	public static int lobbyLockLevel
	{
		get
		{
			return PlayerPrefs.GetInt("lobbyLockLevel", 0);
		}
		set
		{
			PlayerPrefs.SetInt("lobbyLockLevel", value);
		}
	}

	public static void Load()
	{
		if (controllerLookMode > 1)
		{
			controllerLookMode = 0;
		}
		ApplyVideoBrightness();
		ApplyAdvancedVideo();
		ApplyAudioMaster();
		ApplyAudioMusic();
		ApplyAudioFX();
		ApplyAudioVoice();
		ApplyAudioSubtitles();
		ApplyAudioShuffle();
		keyboardBindings = PlayerActions.CreateWithDefaultBindings();
		LoadKeyboardBindings();
		ApplyMouseSettings();
		ApplyControllerSettings();
		ApplyCameraSettings();
	}

	private static void ApplyVideoBrightness()
	{
		MenuCameraEffects.SetGamma((float)videoBrightness / 20f);
	}

	public static void ApplyAdvancedVideo()
	{
		QualitySettings.SetQualityLevel(advancedVideoShadows);
		QualitySettings.vSyncCount = advancedVideoVsync;
		QualitySettings.masterTextureLimit = ((advancedVideoTexture == 0) ? 1 : 0);
		QualitySettings.anisotropicFiltering = ((advancedVideoTexture > 1) ? AnisotropicFiltering.Enable : AnisotropicFiltering.Disable);
		if (CloudSystem.instance != null)
		{
			CloudSystem.instance.SetQuality(advancedVideoClouds);
		}
		ApplyCameraEffects();
	}

	public static void ApplyCameraEffects()
	{
		MenuCameraEffects.EnableEffects(advancedVideoAA, advancedVideoAO, advancedVideoHDR > 0, advancedVideoExposure > 0, advancedVideoBloom > 0, advancedVideoDOF > 0, advancedVideoChromatic > 0);
	}

	private static void ApplyAudioMaster()
	{
		if (GameAudio.instance != null)
		{
			GameAudio.instance.SetMasterLevel((float)audioMaster / 20f);
		}
	}

	private static void ApplyAudioFX()
	{
		if (GameAudio.instance != null)
		{
			GameAudio.instance.SetFXLevel((float)audioFX / 20f);
		}
	}

	private static void ApplyAudioMusic()
	{
		if (GameAudio.instance != null)
		{
			GameAudio.instance.SetMusicLevel((float)audioMusic / 20f);
		}
	}

	private static void ApplyAudioVoice()
	{
		if (GameAudio.instance != null)
		{
			GameAudio.instance.SetVoiceLevel((float)audioVoice / 20f);
		}
	}

	private static void ApplyAudioSubtitles()
	{
		if (SubtitleManager.instance != null)
		{
			SubtitleManager.instance.EnableSubtitles(audioSubtitles > 0);
		}
	}

	private static void ApplyAudioShuffle()
	{
		if ((bool)MusicManager.instance)
		{
			MusicManager.instance.shuffle = (audioShuffle > 0);
		}
	}

	public static float LogMap(float value, float sourceLow, float sourceHigh, float targetFrom, float targetCenter, float targetTo)
	{
		float p = Mathf.Log((targetCenter - targetFrom) / (targetTo - targetFrom), 0.5f);
		return Mathf.Lerp(targetFrom, targetTo, Mathf.Pow(Mathf.InverseLerp(sourceLow, sourceHigh, value), p));
	}

	public static float LogMap(float value, float sourceLow, float sourceMid, float sourceHigh, float targetFrom, float targetCenter, float targetTo)
	{
		float p = (sourceMid - sourceLow) / (sourceHigh - sourceLow);
		float p2 = Mathf.Log((targetCenter - targetFrom) / (targetTo - targetFrom), p);
		return Mathf.Lerp(targetFrom, targetTo, Mathf.Pow(Mathf.InverseLerp(sourceLow, sourceHigh, value), p2));
	}

	public static float ThreePointMap(float value, float sourceLow, float sourceMid, float sourceHigh, float targetFrom, float targetCenter, float targetTo)
	{
		if (value < sourceMid)
		{
			return Mathf.Lerp(targetFrom, targetCenter, Mathf.InverseLerp(sourceLow, sourceMid, value));
		}
		return Mathf.Lerp(targetCenter, targetTo, Mathf.InverseLerp(sourceMid, sourceHigh, value));
	}

	public static float FourPointMap(float value, float sourceLow, float sourceMidLow, float sourceMid, float sourceHigh, float targetFrom, float targetLowCenter, float targetCenter, float targetTo)
	{
		if (value < sourceMidLow)
		{
			return Mathf.Lerp(targetFrom, targetLowCenter, Mathf.InverseLerp(sourceLow, sourceMidLow, value));
		}
		if (value < sourceMid)
		{
			return Mathf.Lerp(targetLowCenter, targetCenter, Mathf.InverseLerp(sourceMidLow, sourceMid, value));
		}
		return Mathf.Lerp(targetCenter, targetTo, Mathf.InverseLerp(sourceMid, sourceHigh, value));
	}

	private static void ApplyMouseSettings()
	{
		keyboardBindings.ApplyMouseSensitivity(mouseInvert > 0, mouseSensitivity);
		PlayerManager.instance.ApplyControls();
	}

	public static PlayerActions CreateInput(InputDevice inputDevice)
	{
		PlayerActions playerActions = PlayerActions.CreateWithControllerBindings(controllerLayout, controllerInvert > 0, controllerInvertH > 0, controllerHSensitivity, controllerVSensitivity);
		playerActions.Device = inputDevice;
		return playerActions;
	}

	private static void ApplyCameraSettings()
	{
		CameraController3.SetFov((float)cameraFov / 20f);
		CameraController3.SetSmoothing((float)cameraSmoothing / 20f);
	}

	private static void ApplyControllerSettings()
	{
		if ((bool)PlayerManager.instance)
		{
			controllerBindings = PlayerActions.CreateWithControllerBindings(controllerLayout, controllerInvert > 0, controllerInvertH > 0, controllerHSensitivity, controllerVSensitivity);
			PlayerManager.instance.ApplyControls();
		}
	}

	public static void LoadKeyboardBindings()
	{
		if (PlayerPrefs.HasKey("Bindings"))
		{
			string @string = PlayerPrefs.GetString("Bindings");
			keyboardBindings.Load(@string);
		}
	}

	public static void SaveKeyboardBindings()
	{
		string value = keyboardBindings.Save();
		PlayerPrefs.SetString("Bindings", value);
	}
}
