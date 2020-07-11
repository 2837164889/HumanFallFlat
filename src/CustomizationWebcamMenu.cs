using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CustomizationWebcamMenu : MenuTransition
{
	public GameObject prepareUI;

	public MenuButton mirrorButton;

	public GameObject mirrorMarker;

	public TextMeshProUGUI timer;

	[NonSerialized]
	public WebCamTexture webCam;

	public Texture2D fakeTexture;

	private CustomizationPaintMenu paintMenu;

	private int mirror = 1;

	private int desiredCameraIndex;

	private string desiredCameraName;

	public bool isStreaming;

	public override void OnGotFocus()
	{
		paintMenu = MenuSystem.instance.GetMenu<CustomizationPaintMenu>();
		base.OnGotFocus();
		MenuButton component = lastFocusedElement.GetComponent<MenuButton>();
		if (component != null)
		{
			component.OnSelect(null);
		}
		MenuCameraEffects.FadeInWebcamMenu();
		prepareUI.SetActive(value: true);
		desiredCameraIndex = PlayerPrefs.GetInt("WebCamIdx", 0);
		desiredCameraName = PlayerPrefs.GetString("WebCamName", null);
		mirror = PlayerPrefs.GetInt("WebCamMirror", 1);
		if (mirror == 0)
		{
			mirror = 1;
		}
		StartWebCam();
	}

	public override void OnBack()
	{
		BackClick();
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			paintMenu.paint.CancelStroke();
			Teardown();
			CustomizationPaintMenu.dontReload = true;
			TransitionBack<CustomizationPaintMenu>();
		}
	}

	public void Teardown()
	{
		timer.gameObject.SetActive(value: false);
		MenuCameraEffects.FadeInMainMenu();
		ReleaseWebCam();
		paintMenu.paint.StopStream();
	}

	public void TakePictureClick()
	{
		if (MenuSystem.CanInvoke)
		{
			StartCoroutine(TakePicture());
		}
	}

	public void SwitchCameraClick()
	{
		if (MenuSystem.CanInvoke && WebCamTexture.devices != null && WebCamTexture.devices.Length > 0)
		{
			ReleaseWebCam();
			desiredCameraIndex = (desiredCameraIndex + 1) % WebCamTexture.devices.Length;
			desiredCameraName = null;
			StartWebCam();
		}
	}

	public void MirrorToggleClick()
	{
		if (MenuSystem.CanInvoke)
		{
			mirror *= -1;
			mirrorButton.isOn = (mirror < 0);
			mirrorButton.transform.localScale = ((mirror >= 0) ? Vector3.one : (Vector3.one * 1.05f));
			mirrorMarker.SetActive(mirror < 0);
			PlayerPrefs.SetInt("WebCamMirror", mirror);
		}
	}

	private IEnumerator TakePicture()
	{
		prepareUI.SetActive(value: false);
		timer.gameObject.SetActive(value: true);
		timer.text = "5";
		yield return new WaitForSeconds(1f);
		timer.text = "4";
		yield return new WaitForSeconds(1f);
		timer.text = "3";
		yield return new WaitForSeconds(1f);
		timer.text = "2";
		yield return new WaitForSeconds(1f);
		timer.text = "1";
		yield return new WaitForSeconds(1f);
		timer.text = "0";
		StopWebCam();
		yield return new WaitForSeconds(1f);
		timer.gameObject.SetActive(value: false);
		TransitionForward<CustomizationWebcamDoneMenu>();
	}

	protected override void Update()
	{
		base.Update();
		if (isStreaming)
		{
			paintMenu.paint.Project(webCam, mirror);
		}
	}

	private void StartWebCam()
	{
		if (webCam != null)
		{
			webCam.Play();
		}
		else
		{
			if (WebCamTexture.devices == null || WebCamTexture.devices.Length == 0)
			{
				CustomizeNoDeviceMenu.reason = CustomizeNoDeviceMenuReason.NoWebcam;
				TransitionForward<CustomizeNoDeviceMenu>();
				return;
			}
			if (!string.IsNullOrEmpty(desiredCameraName))
			{
				for (int i = 0; i < WebCamTexture.devices.Length; i++)
				{
					if (WebCamTexture.devices[i].name == desiredCameraName)
					{
						desiredCameraIndex = i;
					}
				}
			}
			for (int j = 0; j < WebCamTexture.devices.Length; j++)
			{
				desiredCameraName = WebCamTexture.devices[desiredCameraIndex].name;
				webCam = new WebCamTexture(desiredCameraName);
				if (webCam != null)
				{
					webCam.Play();
					if (webCam.isPlaying)
					{
						PlayerPrefs.SetInt("WebCamIdx", desiredCameraIndex);
						PlayerPrefs.SetString("WebCamName", desiredCameraName);
						break;
					}
				}
				desiredCameraIndex++;
			}
		}
		if (webCam == null)
		{
			CustomizeNoDeviceMenu.reason = CustomizeNoDeviceMenuReason.NoWebcam;
			TransitionForward<CustomizeNoDeviceMenu>();
		}
		else
		{
			paintMenu.paint.BeginStream();
			isStreaming = true;
		}
	}

	private void StopWebCam()
	{
		isStreaming = false;
		if (webCam != null)
		{
			webCam.Pause();
		}
	}

	private void ReleaseWebCam()
	{
		isStreaming = false;
		if (webCam != null)
		{
			webCam.Stop();
			webCam = null;
		}
	}
}
