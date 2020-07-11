using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReplayUI : MonoBehaviour
{
	public static ReplayUI instance;

	public Image progress;

	public RectTransform[] cameraIndicators;

	public int totalFrames;

	private CanvasGroup group;

	public float lastActivity;

	private void Awake()
	{
		instance = this;
		base.gameObject.SetActive(value: false);
		HideCameras();
		group = GetComponent<CanvasGroup>();
	}

	public void Show(int totalFrames)
	{
		if (this.totalFrames != totalFrames)
		{
			lastActivity = Time.time;
		}
		this.totalFrames = totalFrames;
		base.gameObject.SetActive(value: true);
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Update()
	{
		float fillAmount = progress.fillAmount;
		progress.fillAmount = 1f * (float)ReplayRecorder.instance.currentFrame / (float)totalFrames;
		if (fillAmount != progress.fillAmount)
		{
			lastActivity = Time.time;
		}
		float value = Time.time - lastActivity;
		group.alpha = Mathf.InverseLerp(1f, 0.5f, value);
	}

	public void SyncCameras(FreeRoamCam.CameraKeyFrame[] keyframes)
	{
		lastActivity = Time.time;
		for (int i = 0; i < 9; i++)
		{
			FreeRoamCam.CameraKeyFrame cameraKeyFrame = keyframes[i];
			if (totalFrames > 0 && cameraKeyFrame.frame >= 0f)
			{
				cameraIndicators[i].gameObject.SetActive(value: true);
				RectTransform obj = cameraIndicators[i];
				Vector2 vector = new Vector2(1f * cameraKeyFrame.frame / (float)totalFrames, 1f);
				cameraIndicators[i].anchorMax = vector;
				obj.anchorMin = vector;
				TextMeshProUGUI componentInChildren = cameraIndicators[i].GetComponentInChildren<TextMeshProUGUI>();
				if (cameraKeyFrame.targetFocus)
				{
					componentInChildren.text = (i + 1).ToString() + ((!cameraKeyFrame.humanRelative) ? "t" : "h");
				}
				else
				{
					componentInChildren.text = (i + 1).ToString();
				}
			}
			else
			{
				cameraIndicators[i].gameObject.SetActive(value: false);
			}
		}
	}

	public void HideCameras()
	{
		lastActivity = Time.time;
		for (int i = 0; i < 9; i++)
		{
			cameraIndicators[i].gameObject.SetActive(value: false);
		}
	}
}
