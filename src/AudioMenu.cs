public class AudioMenu : MenuTransition
{
	public MenuSlider masterSlider;

	public MenuSlider fxSlider;

	public MenuSlider musicSlider;

	public MenuSlider voiceSlider;

	public MenuSelector subtitlesSelector;

	public MenuSelector shuffleSelector;

	public override void ApplyMenuEffects()
	{
		MenuCameraEffects.FadeInPauseMenu();
	}

	public override void OnGotFocus()
	{
		base.OnGotFocus();
		masterSlider.value = Options.audioMaster;
		fxSlider.value = Options.audioFX;
		musicSlider.value = Options.audioMusic;
		voiceSlider.value = Options.audioVoice;
		subtitlesSelector.SelectIndex(Options.audioSubtitles);
		shuffleSelector.SelectIndex(Options.audioShuffle);
	}

	public void MasterChanged(float value)
	{
		Options.audioMaster = (int)value;
	}

	public void FXChanged(float value)
	{
		Options.audioFX = (int)value;
	}

	public void MusicChanged(float value)
	{
		Options.audioMusic = (int)value;
	}

	public void VoiceChanged(float value)
	{
		Options.audioVoice = (int)value;
	}

	public void SubtitlesChanged(int value)
	{
		Options.audioSubtitles = value;
	}

	public void ShuffleChanged(int value)
	{
		Options.audioShuffle = value;
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionBack<OptionsMenu>();
		}
	}

	public override void OnBack()
	{
		BackClick();
	}
}
