using UnityEngine;

public class CustomizeNoDeviceMenu : MenuTransition
{
	public static CustomizeNoDeviceMenuReason reason;

	public GameObject webcamUI;

	public override void OnGotFocus()
	{
		base.OnGotFocus();
		webcamUI.SetActive(reason == CustomizeNoDeviceMenuReason.NoWebcam);
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			CustomizationPaintMenu.dontReload = true;
			TransitionBack<CustomizationPaintMenu>();
		}
	}

	public override void OnBack()
	{
		BackClick();
	}
}
