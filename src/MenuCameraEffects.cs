using Multiplayer;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.ImageEffects;

public class MenuCameraEffects : MonoBehaviour, IDependency
{
	public enum AntialiasType
	{
		None,
		FastApproximateAntialiasing,
		SubpixelMorphologicalAntialiasing,
		TemporalAntialiasing,
		MSAA
	}

	public enum AmbientOcclusionMode
	{
		None,
		ScalableAmbientObscurance,
		MultiScaleVolumetricObscurance
	}

	public class GameCamera
	{
		public Camera camera;

		public Transform holder;

		public BlurOptimized blur;

		public PostProcessLayer layer;

		public DepthOfField depthOfField;

		internal int resetCounter;

		public void SetViewport(int number, int total)
		{
			if (number >= 4 || number >= total)
			{
				camera.rect = new Rect(1.5f, 0f, 0.1f, 0.1f);
				return;
			}
			float num = 0f;
			float num2 = 1f;
			float num3 = 0f;
			float num4 = 1f;
			if (total > 2)
			{
				num3 = 0.5f;
				if (number >= 2)
				{
					num4 -= 0.5f;
					num3 -= 0.5f;
				}
			}
			if (total > 1)
			{
				num2 = 0.5f;
				if ((number & 1) != 0)
				{
					num += 0.5f;
					num2 += 0.5f;
				}
			}
			num = Math.Max(num, sonyScreenClip.xMin);
			num2 = Math.Min(num2, sonyScreenClip.xMax);
			num3 = Math.Max(num3, sonyScreenClip.yMin);
			num4 = Math.Min(num4, sonyScreenClip.yMax);
			camera.rect = new Rect(num, num3, num2 - num, num4 - num3);
			camera.enabled = true;
		}
	}

	public class FloatTransition
	{
		public float from;

		public float to;

		public float current;

		public float duration;

		public float time;

		public FloatTransition(float value)
		{
			from = (to = (current = value));
			time = (duration = 0f);
		}

		public void Transition(float to, float duration)
		{
			from = current;
			this.to = to;
			this.duration = duration;
			time = 0f;
		}

		public bool Step(float dt)
		{
			float num = current;
			time += dt;
			if (duration == 0f || time >= duration)
			{
				current = to;
			}
			else
			{
				current = Ease.easeInOutQuad(from, to, time / duration);
			}
			return num != current;
		}
	}

	public PostProcessProfile postProfile;

	public PostProcessProfile[] profiles;

	public PostProcessVolume creditsPostProcessVolume;

	public static MenuCameraEffects instance;

	public static Rect sonyScreenClip = new Rect(0f, 0f, 1f, 1f);

	private List<GameCamera> activeCameras = new List<GameCamera>();

	private List<GameCamera> gameCameras = new List<GameCamera>();

	private GameCamera tempCamera;

	private bool destroyTempOnExit;

	private AntialiasType antialiasing = AntialiasType.TemporalAntialiasing;

	private AmbientOcclusionMode occlusion = AmbientOcclusionMode.MultiScaleVolumetricObscurance;

	private bool allowHDR = true;

	private bool allowBloom = true;

	private bool allowDepthOfField = true;

	private bool allowChromaticAberration = true;

	private bool allowExposure = true;

	private bool blockBlur;

	public float cameraZoom = 1f;

	public Vector2 cameraCenter = Vector2.zero;

	public float creditsAdjust;

	[NonSerialized]
	public FloatTransition zoom = new FloatTransition(1f);

	private float transitionDuration = 0.5f;

	private FloatTransition blur = new FloatTransition(0f);

	private FloatTransition offsetX = new FloatTransition(0f);

	private FloatTransition offsetY = new FloatTransition(0f);

	private FloatTransition vignette = new FloatTransition(0f);

	private FloatTransition dim = new FloatTransition(0f);

	private FloatTransition credits = new FloatTransition(0f);

	private FloatTransition superMasterFade = new FloatTransition(0f);

	private int bonusViewports;

	private Material defaultSkyboxMaterial;

	private Material skyboxMaterial;

	private Color defaultSkyboxColor;

	private Color creditsSkyboxColor = new Color(121f / 255f, 25f / 51f, 127f / 255f);

	public bool CreditsAdjustInTransition => credits.current != credits.to;

	public static void EnterCustomization()
	{
		instance.blockBlur = true;
		ApplyEffects();
	}

	public static void LeaveCustomization()
	{
		instance.blockBlur = false;
		ApplyEffects();
	}

	public void Initialize()
	{
		instance = this;
		defaultSkyboxMaterial = RenderSettings.skybox;
		skyboxMaterial = new Material(defaultSkyboxMaterial);
		RenderSettings.skybox = skyboxMaterial;
		defaultSkyboxColor = new Color(0.585f, 0.585f, 0.585f);
		Dependencies.OnInitialized(this);
	}

	private static GameCamera WrapCamera(Camera camera)
	{
		PostProcessLayer postProcessLayer = camera.GetComponent<PostProcessLayer>();
		CameraController3 componentInParent = camera.GetComponentInParent<CameraController3>();
		if (postProcessLayer == null)
		{
			postProcessLayer = new PostProcessLayer();
		}
		GameCamera gameCamera = new GameCamera();
		gameCamera.camera = camera;
		gameCamera.blur = camera.GetComponent<BlurOptimized>();
		gameCamera.holder = camera.transform.parent;
		gameCamera.resetCounter = 3;
		gameCamera.layer = postProcessLayer;
		GameCamera gameCamera2 = gameCamera;
		if (componentInParent != null)
		{
			postProcessLayer.volumeTrigger = componentInParent.human.transform;
			GameObject gameObject = new GameObject("Quick Volume (override focus)");
			gameObject.transform.SetParent(postProcessLayer.volumeTrigger, worldPositionStays: false);
			gameObject.layer = 30;
			SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
			sphereCollider.isTrigger = true;
			sphereCollider.radius = 0.1f;
			PostProcessVolume postProcessVolume = gameObject.AddComponent<PostProcessVolume>();
			postProcessVolume.priority = 2f;
			PostProcessProfile postProcessProfile = postProcessVolume.sharedProfile = postProcessVolume.profile;
			gameCamera2.depthOfField = ScriptableObject.CreateInstance<DepthOfField>();
			gameCamera2.depthOfField.enabled.Override(instance.allowDepthOfField && !instance.blockBlur);
			postProcessProfile.AddSettings(gameCamera2.depthOfField);
		}
		else
		{
			postProcessLayer.volumeTrigger = camera.transform;
		}
		return gameCamera2;
	}

	public void SetNetJoinLocalSpinner(bool on)
	{
		bonusViewports = (on ? 1 : 0);
		SetupViewports();
	}

	public void AddHuman(NetPlayer player)
	{
		player.cameraController.gameCam = AddCamera(player.cameraController.transform);
		RagdollTransparency ragdollTransparency = player.cameraController.gameCam.gameObject.AddComponent<RagdollTransparency>();
		ragdollTransparency.Initialize(player.cameraController, player.human.ragdoll);
		SetupViewports();
	}

	public void RemoveHuman(NetPlayer player)
	{
		RemoveCamera(player.cameraController.transform);
		SetupViewports();
	}

	public void SetupViewports()
	{
		for (int i = 0; i < gameCameras.Count; i++)
		{
			gameCameras[i].SetViewport(i, gameCameras.Count + bonusViewports);
		}
	}

	public void SetCOOPFullScreenViewport(bool both)
	{
		if (both)
		{
			gameCameras[0].SetViewport(0, 2);
			gameCameras[1].SetViewport(1, 2);
		}
		else
		{
			gameCameras[0].SetViewport(0, 1);
			gameCameras[1].camera.enabled = false;
		}
	}

	private GameCamera FindCamera(Camera cam)
	{
		for (int i = 0; i < activeCameras.Count; i++)
		{
			if (activeCameras[i].camera == cam)
			{
				return activeCameras[i];
			}
		}
		return null;
	}

	private Camera AddCamera(Transform parent)
	{
		Camera component = UnityEngine.Object.Instantiate(Game.instance.cameraPrefab.gameObject, parent, worldPositionStays: false).GetComponent<Camera>();
		GameCamera item = WrapCamera(component);
		gameCameras.Add(item);
		if (tempCamera == null)
		{
			activeCameras.Add(item);
			ApplyEffects();
		}
		else
		{
			component.gameObject.SetActive(value: false);
		}
		return component;
	}

	private void RemoveCamera(Transform parent)
	{
		for (int num = gameCameras.Count - 1; num >= 0; num--)
		{
			if (gameCameras[num].holder == parent)
			{
				gameCameras.RemoveAt(num);
			}
		}
		for (int num2 = activeCameras.Count - 1; num2 >= 0; num2--)
		{
			if (activeCameras[num2].holder == parent)
			{
				activeCameras.RemoveAt(num2);
			}
		}
	}

	public Camera OverrideCamera(Transform parent, bool applyEffects)
	{
		Camera component = parent.GetComponent<Camera>();
		if (component == null)
		{
			component = UnityEngine.Object.Instantiate(Game.instance.cameraPrefab.gameObject, parent, worldPositionStays: false).GetComponent<Camera>();
			destroyTempOnExit = true;
		}
		else
		{
			destroyTempOnExit = false;
		}
		tempCamera = WrapCamera(component);
		for (int i = 0; i < gameCameras.Count; i++)
		{
			gameCameras[i].camera.gameObject.SetActive(value: false);
		}
		activeCameras.Clear();
		if (applyEffects)
		{
			activeCameras.Add(tempCamera);
			ApplyEffects();
		}
		Listener.instance.OverrideTransform(component.transform);
		return component;
	}

	public void RemoveOverride()
	{
		if ((bool)Listener.instance)
		{
			Listener.instance.EndTransfromOverride();
		}
		if (destroyTempOnExit)
		{
			UnityEngine.Object.Destroy(tempCamera.camera.gameObject);
		}
		destroyTempOnExit = false;
		for (int i = 0; i < gameCameras.Count; i++)
		{
			gameCameras[i].camera.gameObject.SetActive(value: true);
		}
		activeCameras.Clear();
		activeCameras.AddRange(gameCameras);
		tempCamera = null;
		ApplyEffects();
	}

	private void Transition(float cameraZoom, Vector2 cameraOffset, float blur, float vignette, float dim, float credits = 0f)
	{
		zoom.Transition(cameraZoom, transitionDuration);
		offsetX.Transition(cameraOffset.x, transitionDuration);
		offsetY.Transition(cameraOffset.y, transitionDuration);
		this.blur.Transition(blur, transitionDuration);
		this.vignette.Transition(vignette, transitionDuration);
		this.dim.Transition(dim, transitionDuration);
		this.credits.Transition(credits, transitionDuration);
	}

	private void ApplyTransition()
	{
		Step(transitionDuration * 2f);
	}

	public void BlackOut()
	{
		Transition(1f, Vector2.zero, 0f, 1f, 1f);
		ApplyTransition();
	}

	public void FadeOutBlackOut()
	{
		transitionDuration = 5f;
		Transition(1f, Vector2.zero, 0f, 0.7f, 0f);
	}

	public static void FadeInMainMenu()
	{
		instance.transitionDuration = 0.5f;
		instance.Transition(1f, Vector2.one, 0f, 0.7f, 0f);
	}

	public static void SuperMasterFade(float target, float duration)
	{
		instance.superMasterFade.Transition(target, duration);
	}

	public static void FadeToBlack(float duration)
	{
		instance.transitionDuration = duration;
		instance.Transition(instance.zoom.to, new Vector2(instance.offsetX.to, instance.offsetY.to), 1f, 1f, 1f);
	}

	public static void FadeFromBlack(float duration)
	{
		instance.transitionDuration = duration;
		instance.Transition(instance.zoom.to, new Vector2(instance.offsetX.to, instance.offsetY.to), 0f, 0.7f, 0f);
	}

	public static void FadeInWebcamMenu()
	{
		instance.transitionDuration = 0.5f;
		instance.Transition(1.5f, new Vector2(0f, 1f), 0f, 1f, 0f);
	}

	public static void FadeInCredits()
	{
		instance.transitionDuration = 1.5f;
		instance.Transition(1f, Vector2.zero, 0f, 0f, 0f, 1f);
	}

	public static void FadeInPauseMenu()
	{
		CaveRender.fogDensityMultiplier = 1f;
		instance.Transition(1.5f, Vector2.one, 1f, 1f, 0.5f);
	}

	public static void FadeInLobby()
	{
		instance.Transition(1.5f, Vector2.one, 0f, 1f, 0.75f);
	}

	public static void FadeInCoopMenu()
	{
		instance.Transition(1.5f, Vector2.zero, 1f, 1f, 0.5f);
	}

	public static void FadeOut(float duration = 0f)
	{
		if (duration == 0f)
		{
			duration = 0.5f;
		}
		instance.transitionDuration = duration;
		instance.Transition(1f, Vector2.zero, 0f, 0f, 0f);
	}

	private void LateUpdate()
	{
		Step(Time.unscaledDeltaTime);
	}

	private void Step(float time)
	{
		bool flag = false;
		if (DialogOverlay.IsOn())
		{
			if (superMasterFade.to != superMasterFade.from)
			{
				if (superMasterFade.from > superMasterFade.to)
				{
					superMasterFade.time = 0f;
				}
				else
				{
					superMasterFade.time = superMasterFade.duration;
				}
			}
			flag = superMasterFade.Step(0f);
		}
		else
		{
			flag = superMasterFade.Step(time);
		}
		offsetX.Step(time);
		offsetY.Step(time);
		cameraCenter = new Vector2(-0.23f * offsetX.current, 0.23f * offsetY.current);
		zoom.Step(time);
		cameraZoom = zoom.current;
		if (blur.Step(time) || flag)
		{
			ApplyBlur();
		}
		bool flag2 = vignette.Step(time);
		if ((flag2 | dim.Step(time)) || flag)
		{
			ApplyVignette();
		}
		if (RenderSettings.skybox == defaultSkyboxMaterial)
		{
			RenderSettings.skybox = skyboxMaterial;
		}
		if (credits.Step(time))
		{
			creditsAdjust = credits.current;
			skyboxMaterial.SetColor("_Tint", Color.Lerp(defaultSkyboxColor, creditsSkyboxColor, creditsAdjust));
			creditsPostProcessVolume.weight = instance.creditsAdjust;
			creditsPostProcessVolume.gameObject.SetActive(creditsAdjust != 0f);
		}
	}

	public static void EnableEffects(int antialiasing, int occlusion, bool enableHDR, bool enableExposure, bool enableBloom, bool enableDepthOfField, bool enableChromaticAberration)
	{
		if (!(instance == null))
		{
			instance.antialiasing = (AntialiasType)antialiasing;
			instance.occlusion = (AmbientOcclusionMode)occlusion;
			instance.allowHDR = enableHDR;
			instance.allowExposure = enableExposure;
			instance.allowBloom = enableBloom;
			instance.allowDepthOfField = enableDepthOfField;
			instance.allowChromaticAberration = enableChromaticAberration;
			ApplyEffects();
		}
	}

	private void ApplyEffect<T>(bool enable) where T : PostProcessEffectSettings
	{
		for (int i = 0; i < profiles.Length; i++)
		{
			if (profiles[i].TryGetSettings(out T outSetting))
			{
				outSetting.enabled.Override(enable);
			}
		}
	}

	public void ForceDisableOcclusion(bool forceDisableOcclusion)
	{
		ApplyOcclusion((!forceDisableOcclusion) ? occlusion : AmbientOcclusionMode.None);
	}

	private void ApplyOcclusion(AmbientOcclusionMode occlusion)
	{
		for (int i = 0; i < profiles.Length; i++)
		{
			if (profiles[i].TryGetSettings(out AmbientOcclusion outSetting))
			{
				outSetting.enabled.Override(occlusion != AmbientOcclusionMode.None);
				outSetting.mode.Override((occlusion == AmbientOcclusionMode.MultiScaleVolumetricObscurance) ? UnityEngine.Rendering.PostProcessing.AmbientOcclusionMode.MultiScaleVolumetricObscurance : UnityEngine.Rendering.PostProcessing.AmbientOcclusionMode.ScalableAmbientObscurance);
			}
		}
	}

	public static void SuspendEffects(bool suspend)
	{
		instance.ApplyOcclusion((!suspend) ? instance.occlusion : AmbientOcclusionMode.None);
		instance.ApplyEffect<AutoExposure>(instance.allowExposure && !suspend);
		instance.ApplyEffect<DepthOfField>(instance.allowDepthOfField && !instance.blockBlur && !suspend);
		instance.ApplyEffect<Bloom>(instance.allowBloom && !suspend);
		instance.ApplyEffect<ChromaticAberration>(instance.allowChromaticAberration && !suspend);
		instance.ApplyEffect<ColorGrading>(!suspend);
		instance.ApplyEffect<Vignette>(!suspend);
		for (int i = 0; i < instance.activeCameras.Count; i++)
		{
			if (instance.activeCameras[i].depthOfField != null)
			{
				instance.activeCameras[i].depthOfField.enabled.Override(instance.allowDepthOfField && !instance.blockBlur && !suspend);
			}
		}
		if (instance.postProfile.TryGetSettings(out ColorGrading outSetting))
		{
			outSetting.enabled.Override(!suspend);
		}
		for (int j = 0; j < instance.activeCameras.Count; j++)
		{
			instance.activeCameras[j].camera.depthTextureMode = DepthTextureMode.None;
		}
	}

	private static void ApplyEffects()
	{
		SuspendEffects(suspend: false);
		instance.ApplyBlur();
		instance.ApplyVignette();
		for (int i = 0; i < instance.activeCameras.Count; i++)
		{
			GameCamera gameCamera = instance.activeCameras[i];
			gameCamera.camera.allowHDR = instance.allowHDR;
			gameCamera.camera.allowMSAA = (instance.antialiasing == AntialiasType.MSAA);
			gameCamera.layer.antialiasingMode = (PostProcessLayer.Antialiasing)((instance.antialiasing != AntialiasType.MSAA) ? instance.antialiasing : AntialiasType.None);
		}
	}

	private void ApplyVignette()
	{
		for (int i = 0; i < profiles.Length; i++)
		{
			if (profiles[i].TryGetSettings(out Vignette outSetting))
			{
				float x = 1f - (1f - dim.current) * (1f - superMasterFade.current);
				outSetting.opacity.Override(x);
				x = 1f - (1f - vignette.current) * (1f - superMasterFade.current);
				outSetting.intensity.Override(x / 3f);
			}
		}
	}

	private void ApplyBlur()
	{
		for (int i = 0; i < activeCameras.Count; i++)
		{
			if (activeCameras[i] != null && !(activeCameras[i].blur == null))
			{
				if (blur.current == 0f)
				{
					activeCameras[i].blur.enabled = false;
					continue;
				}
				activeCameras[i].blur.enabled = true;
				float current = blur.current;
				current = 1f - (1f - current) * (1f - superMasterFade.current);
				activeCameras[i].blur.blurSize = current * current * 10f;
				activeCameras[i].blur.downsample = ((!(current > 0.5f)) ? 1 : 2);
			}
		}
	}

	public static void SetGamma(float gamma)
	{
		if (!(instance == null) && instance.postProfile.TryGetSettings(out ColorGrading outSetting))
		{
			outSetting.postExposure.Override(Mathf.Lerp(-1f, 1f, gamma));
			gamma = Mathf.Lerp(-0.5f, 0.5f, gamma);
			outSetting.gamma.Override(new Vector4(0f, 0f, 0f, gamma));
		}
	}

	public void OverrideDepthOfField(Camera cam, float value)
	{
		instance.FindCamera(cam)?.depthOfField.focusDistance.Override(value);
	}
}
