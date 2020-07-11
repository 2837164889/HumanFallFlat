using System.Collections;
using TMPro;
using UnityEngine;

public class SubtitleManager : MonoBehaviour
{
	public static SubtitleManager instance;

	public TextMeshProUGUI subtitleText;

	public TextMeshProUGUI instructionText;

	public TextMeshProUGUI progressText;

	public GameObject progressRing;

	public GameObject recordingLight;

	private float subtitleDismiss;

	private float instructionDismiss;

	private float progressDismiss;

	private float progressRingDismiss;

	private Coroutine dismissCoroutine;

	private bool subtitlesEnabled = true;

	private void OnEnable()
	{
		instance = this;
	}

	private void Start()
	{
		subtitleText.text = string.Empty;
		instructionText.text = string.Empty;
		progressText.text = string.Empty;
		progressRing.SetActive(value: false);
		recordingLight.SetActive(value: false);
		instructionText.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
	}

	public void SetSubtitle(string text)
	{
		subtitleText.text = text;
		subtitleDismiss = 2.1474836E+09f;
	}

	public void SetSubtitle(string text, float dismissTime)
	{
		subtitleText.text = text;
		subtitleDismiss = ((dismissTime == 0f) ? 2.1474836E+09f : dismissTime);
		if (dismissCoroutine == null)
		{
			dismissCoroutine = StartCoroutine(DismissCoroutine());
		}
	}

	public void ClearSubtitle()
	{
		SetSubtitle(string.Empty);
	}

	public void SetInstruction(string text)
	{
		instructionText.text = text;
	}

	public void SetInstruction(string text, float dismissTime)
	{
		instructionText.text = text;
		instructionDismiss = ((dismissTime == 0f) ? 2.1474836E+09f : dismissTime);
		if (dismissCoroutine == null)
		{
			dismissCoroutine = StartCoroutine(DismissCoroutine());
		}
	}

	public void ClearInstruction(float dismissTime)
	{
		instructionDismiss = dismissTime;
		if (dismissCoroutine == null)
		{
			dismissCoroutine = StartCoroutine(DismissCoroutine());
		}
	}

	public void ClearInstruction()
	{
		SetInstruction(string.Empty);
	}

	public void SetRecording()
	{
		recordingLight.SetActive(value: true);
	}

	public void ClearRecording()
	{
		recordingLight.SetActive(value: false);
	}

	public void SetProgress(string text)
	{
		progressText.text = text;
		progressDismiss = 2.1474836E+09f;
		progressRingDismiss = 2.1474836E+09f;
		progressRing.SetActive(value: true);
	}

	public void SetProgress(string text, float dismissTime, float ringTime)
	{
		progressText.text = text;
		progressDismiss = ((dismissTime == 0f) ? 2.1474836E+09f : dismissTime);
		progressRingDismiss = ((ringTime == 0f) ? 2.1474836E+09f : ringTime);
		progressRing.SetActive(value: true);
		if (dismissCoroutine == null)
		{
			dismissCoroutine = StartCoroutine(DismissCoroutine());
		}
	}

	public bool GetRingState()
	{
		return progressRing.activeSelf;
	}

	public bool CheckProgressText(string cmpString)
	{
		return progressText.text == cmpString;
	}

	public void ClearProgress()
	{
		SetProgress(string.Empty);
		progressRing.SetActive(value: false);
	}

	private IEnumerator DismissCoroutine()
	{
		while (instructionDismiss != 2.1474836E+09f || subtitleDismiss != 2.1474836E+09f || progressDismiss != 2.1474836E+09f)
		{
			if (subtitleDismiss != 2.1474836E+09f)
			{
				subtitleDismiss -= Time.deltaTime;
				if (subtitleDismiss <= 0f)
				{
					subtitleDismiss = 2.1474836E+09f;
					ClearSubtitle();
				}
			}
			if (instructionDismiss != 2.1474836E+09f)
			{
				instructionDismiss -= Time.deltaTime;
				if (instructionDismiss <= 0f)
				{
					instructionDismiss = 2.1474836E+09f;
					ClearInstruction();
				}
			}
			if (progressDismiss != 2.1474836E+09f)
			{
				progressRingDismiss -= Time.deltaTime;
				progressDismiss -= Time.deltaTime;
				if (progressRingDismiss <= 0f)
				{
					progressRing.SetActive(value: false);
				}
				if (progressDismiss <= 0f)
				{
					progressDismiss = 2.1474836E+09f;
					ClearProgress();
				}
			}
			yield return null;
		}
		dismissCoroutine = null;
	}

	public void Hide()
	{
		instructionText.gameObject.SetActive(value: false);
		subtitleText.gameObject.SetActive(value: false);
	}

	public void Show()
	{
		instructionText.gameObject.SetActive(value: true);
		subtitleText.gameObject.SetActive(subtitlesEnabled);
	}

	internal void EnableSubtitles(bool enable)
	{
		subtitlesEnabled = enable;
	}

	public void PlayNarrative(AudioClip clip)
	{
		GetComponent<AudioSource>().PlayOneShot(clip);
	}
}
