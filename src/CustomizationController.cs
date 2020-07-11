using HumanAPI;
using Multiplayer;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationController : MonoBehaviour
{
	public static CustomizationController instance;

	public static Action onInitialized;

	public Camera thumbnailCamera;

	public CustomizationCameraController cameraController;

	public Transform turntable;

	public Transform mainCharacter;

	public Transform coopCharacter;

	public const int kStandardIconSize = 512;

	public const int kSwitchBrokenIconSize = 512;

	public const int kSwitchPresetIconSize = 128;

	public const int kSAPresetIconSize = 256;

	public const int kPresetIconSize = 166;

	private RagdollCustomization[] customizations = new RagdollCustomization[2];

	public bool showBoth = true;

	public int activePlayer;

	public RagdollCustomization activeCustomization;

	public float transitionDuration = 0.5f;

	public float transitionPhase = 1f;

	public float angleFrom;

	public float angleTo;

	private const bool kScaledIconEnable = true;

	private const int kScaledIconSize = 256;

	public Camera mainCamera => cameraController.cameraHolder.GetComponentInChildren<Camera>();

	private void OnEnable()
	{
		instance = this;
		cameraController = GetComponent<CustomizationCameraController>();
		if (onInitialized != null)
		{
			onInitialized();
		}
	}

	public void Initialize()
	{
		MenuCameraEffects.EnterCustomization();
		NetGame.instance.StopLocalGame();
		Listener.instance.OverrideTransform(cameraController.cameraHolder);
		customizations[0] = InitializeRagdoll(mainCharacter);
		customizations[1] = InitializeRagdoll(coopCharacter);
		ReloadSkin(0);
		ReloadSkin(1);
		activePlayer = PlayerPrefs.GetInt("MainSkinIndex", 0);
		activeCustomization = customizations[activePlayer];
		turntable.rotation = Quaternion.Euler(0f, (activePlayer > 0) ? 180 : 0, 0f);
	}

	private RagdollCustomization InitializeRagdoll(Transform parent)
	{
		Ragdoll component = UnityEngine.Object.Instantiate(Game.instance.ragdollPrefab.gameObject, parent, worldPositionStays: false).GetComponent<Ragdoll>();
		component.Lock();
		RagdollCustomization ragdollCustomization = component.gameObject.AddComponent<RagdollCustomization>();
		return component.GetComponent<RagdollCustomization>();
	}

	public void Teardown()
	{
		MenuCameraEffects.LeaveCustomization();
		instance = null;
		Listener.instance.EndTransfromOverride();
		NetGame.instance.StartLocalGame();
	}

	public void SetActivePlayer(int index)
	{
		PlayerPrefs.SetInt("MainSkinIndex", index);
		activePlayer = index;
		Vector3 eulerAngles = turntable.rotation.eulerAngles;
		angleFrom = eulerAngles.y;
		angleTo = ((index > 0) ? 180 : 0);
		if (angleFrom > angleTo)
		{
			angleFrom -= 360f;
		}
		transitionPhase = 0f;
		if (!showBoth)
		{
			ShowActive();
		}
		activeCustomization = customizations[index];
	}

	public void ShowBoth()
	{
		showBoth = true;
		mainCharacter.gameObject.SetActive(value: true);
		coopCharacter.gameObject.SetActive(value: true);
	}

	public void ShowActive()
	{
		showBoth = false;
		mainCharacter.gameObject.SetActive(activePlayer == 0);
		coopCharacter.gameObject.SetActive(activePlayer == 1);
	}

	private void Update()
	{
		if (transitionPhase < 1f)
		{
			transitionPhase = Math.Min(transitionPhase + Time.deltaTime / transitionDuration, 1f);
			float t = Ease.easeInOutQuad(0f, 1f, transitionPhase);
			turntable.rotation = Quaternion.Euler(0f, Mathf.Lerp(angleFrom, angleTo, t), 0f);
		}
	}

	private void ReloadSkin(int playerIdx)
	{
		customizations[playerIdx].ApplyPreset(PresetRepository.ClonePreset(WorkshopRepository.instance.presetRepo.GetPlayerSkin(playerIdx)));
		customizations[playerIdx].RebindColors(bake: false);
	}

	public void SetColor(WorkshopItemType part, int channel, Color color)
	{
		activeCustomization.preset.SetColor(part, channel, color);
		activeCustomization.ApplyPreset(activeCustomization.preset);
		activeCustomization.RebindColors(bake: false);
	}

	public Color GetColor(WorkshopItemType part, int channel)
	{
		return activeCustomization.preset.GetColor(part, channel);
	}

	public void ClearColors()
	{
		activeCustomization.preset.ClearColors();
	}

	public RagdollPresetPartMetadata GetPart(WorkshopItemType part)
	{
		return activeCustomization.preset.GetPart(part);
	}

	public void SetPart(WorkshopItemType part, RagdollPresetPartMetadata data)
	{
		if (data != null)
		{
			data.suppressCustomTexture = true;
		}
		activeCustomization.preset.SetPart(part, data);
		activeCustomization.ApplyPreset(activeCustomization.preset);
		RagdollModel model = activeCustomization.GetModel(part);
		if (model != null)
		{
			data.color1 = HexConverter.ColorToHex(model.color1);
			data.color2 = HexConverter.ColorToHex(model.color2);
			data.color3 = HexConverter.ColorToHex(model.color3);
		}
		activeCustomization.RebindColors(bake: false);
	}

	public void CommitParts()
	{
		WorkshopRepository.instance.presetRepo.SaveSkin(activePlayer, activeCustomization.preset);
		ReloadSkin(activePlayer);
	}

	public void Rollback()
	{
		ReloadSkin(activePlayer);
	}

	public string GetSkinPresetReference()
	{
		return WorkshopRepository.instance.presetRepo.GetSkinPresetReference(activePlayer);
	}

	public void LoadPreset(RagdollPresetMetadata preset)
	{
		activeCustomization.ApplyPreset(preset);
		activeCustomization.RebindColors(bake: false);
	}

	public void ApplyLoadedPreset()
	{
		WorkshopRepository.instance.presetRepo.SelectSkinPreset(activePlayer, activeCustomization.preset);
		ReloadSkin(activePlayer);
	}

	public RagdollPresetMetadata GetSkin()
	{
		return WorkshopRepository.instance.presetRepo.GetPlayerSkin(activePlayer);
	}

	public RagdollPresetMetadata SavePreset(RagdollPresetMetadata saveOver, string title, string description)
	{
		RagdollPresetMetadata ragdollPresetMetadata = WorkshopRepository.instance.presetRepo.SaveSkinAsPreset(activePlayer, activeCustomization.preset, saveOver, title, description);
		CaptureThumbnail(thumbnailCamera, ragdollPresetMetadata.thumbPath);
		ReloadSkin(activePlayer);
		return ragdollPresetMetadata;
	}

	public void EnsureThumbnails(List<RagdollPresetMetadata> items)
	{
		bool flag = false;
		bool flag2 = showBoth;
		foreach (RagdollPresetMetadata item in items)
		{
			string thumbPath = item.thumbPath;
			if (!FileTools.TestExists(thumbPath))
			{
				Debug.LogFormat("[THUMB] Rebuilding thumbnail {0} from preset...", thumbPath);
				if (!flag)
				{
					if (transitionPhase < 1f)
					{
						turntable.rotation = Quaternion.Euler(0f, angleTo, 0f);
					}
					if (showBoth)
					{
						ShowActive();
					}
				}
				instance.LoadPreset(item);
				CaptureThumbnail(thumbnailCamera, item.thumbPath);
				flag = true;
			}
		}
		if (flag)
		{
			ReloadSkin(activePlayer);
			if (transitionPhase < 1f)
			{
				float t = Ease.easeInOutQuad(0f, 1f, transitionPhase);
				turntable.rotation = Quaternion.Euler(0f, Mathf.Lerp(angleFrom, angleTo, t), 0f);
			}
			if (flag2)
			{
				ShowBoth();
			}
		}
	}

	public void DeletePreset(RagdollPresetMetadata preset)
	{
		WorkshopRepository.instance.presetRepo.DeletePreset(preset);
	}

	private void CaptureThumbnail(Camera camera, string thumbPath)
	{
		RenderTexture renderTexture = null;
		renderTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
		RenderTexture renderTexture2 = null;
		RenderTexture renderTexture3 = null;
		renderTexture2 = new RenderTexture(512, 512, 16, RenderTextureFormat.ARGB32);
		renderTexture3 = (camera.targetTexture = renderTexture2);
		camera.Render();
		Graphics.Blit(renderTexture2, renderTexture);
		RenderTexture.active = null;
		camera.targetTexture = null;
		UnityEngine.Object.DestroyImmediate(renderTexture2);
		RenderTexture.active = renderTexture;
		Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, mipmap: false);
		texture2D.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
		RenderTexture.active = null;
		camera.targetTexture = null;
		UnityEngine.Object.DestroyImmediate(renderTexture);
		FileTools.WriteTexture(thumbPath, texture2D);
	}
}
