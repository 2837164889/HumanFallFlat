using I2.Loc;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoLogMenu : MenuTransition
{
	public RawImage image;

	public AudioSource audioSource;

	private VideoLogButton selectedItem;

	private VideoPlayer video;

	private SrtSubtitles subtitles;

	public TextMeshProUGUI subtitleText;

	private Texture thumbnailTexture;

	private bool videoStarted;

	private SrtSubtitles.SrtLine currentLine;

	private float audiosourceStart;

	public VideoLogButton SelectedItem => selectedItem;

	private float audiosourceTime
	{
		get
		{
			float num = audioSource.time;
			if (float.IsInfinity(num) || float.IsNaN(num))
			{
				num = 1f * (float)audioSource.timeSamples / 48000f;
			}
			if (num == 0f)
			{
				return Time.unscaledTime - audiosourceStart;
			}
			return num;
		}
	}

	public override void OnGotFocus()
	{
		base.OnGotFocus();
		VideoLogButton[] componentsInChildren = GetComponentsInChildren<VideoLogButton>(includeInactive: true);
		List<VideoRepositoryItem> list = VideoRepository.instance.ListAvailableItems();
		videoStarted = false;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			VideoLogButton videoLogButton = componentsInChildren[i];
			videoLogButton.justGotFocus = false;
			if (i < list.Count)
			{
				videoLogButton.gameObject.SetActive(value: true);
				videoLogButton.Bind(list[i]);
				Link(videoLogButton.GetComponent<Selectable>(), componentsInChildren[(i + 1) % list.Count].GetComponent<Selectable>());
			}
			else
			{
				videoLogButton.gameObject.SetActive(value: false);
			}
		}
		selectedItem = null;
		SelectItem(componentsInChildren[0]);
		subtitleText.text = string.Empty;
		Transform transform = base.transform.Find("MenuPanel/Buttons/BottomRow/PlayButton");
		if (transform != null)
		{
			transform.gameObject.SetActive(value: false);
		}
		Transform transform2 = base.transform.Find("MenuPanel/Buttons/BottomRow/BackButton");
		if (transform2 != null && list.Count > 0)
		{
			Selectable component = transform2.GetComponent<Selectable>();
			Link(componentsInChildren[list.Count - 1].GetComponent<Selectable>(), component, makeExplicit: true);
			Link(component, componentsInChildren[0].GetComponent<Selectable>(), makeExplicit: true);
		}
	}

	public override void OnLostFocus()
	{
		base.OnLostFocus();
		StopVideo();
	}

	public override void OnTansitionedIn()
	{
		base.OnTansitionedIn();
		MenuSystem.instance.FocusOnMouseOver(enable: false);
	}

	private void OnDisable()
	{
		image.texture = null;
		image.material = null;
	}

	public override void ApplyMenuEffects()
	{
		MenuCameraEffects.FadeInPauseMenu();
	}

	private void PrepareCompleted(VideoPlayer source)
	{
		image.texture = video.texture;
		image.material = HFFResources.instance.PCLinearMovieFixMenu;
		if (video != null)
		{
			video.Play();
			MusicManager.instance.Pause();
			audioSource.Play();
			audiosourceStart = Time.unscaledTime;
		}
		videoStarted = true;
	}

	private void LoadMovie()
	{
		if (video == null)
		{
			video = base.gameObject.GetComponent<VideoPlayer>();
			if (video == null)
			{
				video = base.gameObject.AddComponent<VideoPlayer>();
			}
		}
		video.playOnAwake = false;
		audioSource.playOnAwake = false;
		video.source = VideoSource.Url;
		string url = Application.streamingAssetsPath + "/Videos/" + selectedItem.videoRepositoryItem.name + "Video.mp4";
		video.url = url;
		video.audioOutputMode = VideoAudioOutputMode.AudioSource;
		video.controlledAudioTrackCount = 1;
		video.EnableAudioTrack(0, enabled: true);
		video.SetTargetAudioSource(0, audioSource);
		video.prepareCompleted += PrepareCompleted;
		video.Prepare();
	}

	public void PlayClick()
	{
		if (MenuSystem.CanInvoke)
		{
			string text = ScriptLocalization.Get("SUBTITLES/" + selectedItem.videoRepositoryItem.name);
			TextAsset videoSrt = HFFResources.instance.GetVideoSrt(selectedItem.videoRepositoryItem.name + "Srt");
			subtitles = new SrtSubtitles();
			subtitles.Load(selectedItem.videoRepositoryItem.name, videoSrt.text);
			videoSrt = null;
			LoadMovie();
		}
	}

	public void SelectItem(VideoLogButton item)
	{
		if (!(item.videoRepositoryItem == null) && !(selectedItem == item))
		{
			StopVideo();
			selectedItem = item;
			thumbnailTexture = HFFResources.instance.GetVideoThumb(item.videoRepositoryItem.name + "Thumb");
			image.texture = thumbnailTexture;
			image.material = null;
		}
	}

	public void PlayVideo(VideoLogButton item)
	{
		SelectItem(item);
		PlayClick();
	}

	public void StopVideo()
	{
		if (video != null)
		{
			video.Stop();
		}
		video = null;
		if (audioSource != null)
		{
			audioSource.Stop();
			audioSource.clip = null;
		}
		subtitleText.text = string.Empty;
		MusicManager.instance.Resume();
		image.texture = thumbnailTexture;
		image.material = null;
		videoStarted = false;
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionBack<ExtrasMenu>();
		}
	}

	public override void OnBack()
	{
		if (video != null)
		{
			VideoLogButton item = selectedItem;
			selectedItem = null;
			SelectItem(item);
		}
		else
		{
			BackClick();
		}
	}

	protected override void Update()
	{
		base.Update();
		if (video != null && videoStarted && video.isPlaying && subtitles != null)
		{
			float audiosourceTime = this.audiosourceTime;
			if (currentLine != null)
			{
				if (!currentLine.ShouldShow(audiosourceTime))
				{
					subtitleText.text = null;
				}
				currentLine = null;
			}
			if (currentLine == null)
			{
				currentLine = subtitles.GetLineToDisplay(audiosourceTime);
				if (currentLine != null && Options.audioSubtitles > 0)
				{
					subtitleText.text = ScriptLocalization.Get("SUBTITLES/" + currentLine.key);
				}
			}
		}
		if (video != null && videoStarted && !video.isPlaying)
		{
			VideoLogButton item = selectedItem;
			selectedItem = null;
			SelectItem(item);
		}
	}
}
