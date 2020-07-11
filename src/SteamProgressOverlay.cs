using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SteamProgressOverlay : MonoBehaviour
{
	public static SteamProgressOverlay instance;

	public GameObject steamProgress;

	public GameObject steamError;

	public TextMeshProUGUI steamErrorCode;

	private Action onDismiss;

	public bool DialogShowing()
	{
		return steamProgress.activeSelf;
	}

	public bool DialogErrorShowing()
	{
		return steamError.activeSelf;
	}

	private void Awake()
	{
		instance = this;
	}

	private void Update()
	{
		WorkshopUpload.UploadMonitor();
	}

	public void ShowSteamProgress(bool showProgress, string error, Action onDismiss)
	{
		this.onDismiss = onDismiss;
		steamProgress.SetActive(showProgress);
		steamError.SetActive(error != null);
		steamErrorCode.text = error;
		if (error != null)
		{
			GameObject gameObject = GetComponentInChildren<Selectable>().gameObject;
			EventSystem.current.SetSelectedGameObject(gameObject);
		}
	}

	public void Dismiss()
	{
		if (onDismiss != null)
		{
			onDismiss();
			onDismiss = null;
		}
	}
}
