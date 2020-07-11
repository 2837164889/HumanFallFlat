using HumanAPI;
using Multiplayer;
using Steamworks;
using UnityEngine.UI;

public class CustomizationSavedMenu : MenuTransition
{
	public static RagdollPresetMetadata preset;

	public UnityEngine.UI.Button PublishButton;

	public UnityEngine.UI.Button AgreeButton;

	private string tempFolder;

	public override void OnGotFocus()
	{
		base.OnGotFocus();
		CustomizationController.instance.cameraController.FocusCharacter();
	}

	protected override void Update()
	{
		base.Update();
		bool sSteamServersConnected = NetTransportSteam.sSteamServersConnected;
		if (PublishButton != null)
		{
			PublishButton.interactable = sSteamServersConnected;
		}
		if (AgreeButton != null)
		{
			AgreeButton.interactable = sSteamServersConnected;
		}
	}

	public override void OnBack()
	{
		BackClick();
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke && base.gameObject.activeSelf)
		{
			TransitionBack<CustomizationRootMenu>();
		}
	}

	public void WokshopTermsClick()
	{
		if (MenuSystem.CanInvoke)
		{
			WorkshopUpload.ShowWorkshopAgreement();
		}
	}

	public void WorkshopUploadClick()
	{
		if (MenuSystem.CanInvoke)
		{
			SteamProgressOverlay.instance.ShowSteamProgress(showProgress: true, null, null);
			base.gameObject.SetActive(value: false);
			tempFolder = FileTools.GetTempDirectory();
			string text = FileTools.Combine(tempFolder, "thumbnail.png");
			preset.Save(tempFolder);
			FileTools.WriteTexture(text, preset.thumbnailTexture);
			preset.ReleaseThumbnailReference();
			PresetRepository.CopySkinTextures(preset, tempFolder);
			WorkshopUpload.Upload(preset, tempFolder, text, string.Empty, OnPublishOver);
		}
	}

	private void OnPublishOver(WorkshopItemMetadata meta, bool needAgreement, EResult error)
	{
		FileTools.DeleteTempDirectory(tempFolder);
		switch (error)
		{
		case EResult.k_EResultOK:
			SteamProgressOverlay.instance.ShowSteamProgress(showProgress: false, null, null);
			base.gameObject.SetActive(value: true);
			preset.workshopId = meta.workshopId;
			preset.Save(preset.folder);
			TransitionForward<CustomizationRootMenu>();
			return;
		case EResult.k_EResultFileNotFound:
			preset.workshopId = 0uL;
			preset.Save(preset.folder);
			break;
		}
		SteamProgressOverlay.instance.ShowSteamProgress(showProgress: false, string.Empty, DismissSteamError);
	}

	public void DismissSteamError()
	{
		SteamProgressOverlay.instance.ShowSteamProgress(showProgress: false, null, null);
		base.gameObject.SetActive(value: true);
		TransitionForward<CustomizationRootMenu>();
	}
}
