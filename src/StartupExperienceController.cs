using InControl;
using Multiplayer;
using System.Collections;
using UnityEngine;

public class StartupExperienceController : MonoBehaviour
{
	private enum HumanState
	{
		HoldBeforeFall,
		Fall,
		ReleaseLatch
	}

	public IntroDrones drones;

	public static StartupExperienceController instance;

	private MenuCameraEffects cameraEffects;

	private CameraController3 cameraController;

	private Human human;

	private Ragdoll ragdoll;

	private HumanState humanState;

	public GameObject gamePrefab;

	private float offsetTime;

	private Vector3 offset;

	private bool syncCamera;

	public void Awake()
	{
		instance = this;
		if (Game.instance == null)
		{
			Object.Instantiate(gamePrefab);
		}
	}

	private IEnumerator Start()
	{
		yield return null;
		yield return null;
		yield return null;
		Dependencies.Initialize<App>();
		App.instance.BeginStartup();
	}

	public void PlayStartupExperience()
	{
		StartCoroutine(StartupExperienceRoutine());
	}

	public void SkipStartupExperience(object multiplayerServer)
	{
		cameraEffects = Object.FindObjectOfType<MenuCameraEffects>();
		cameraController = Object.FindObjectOfType<CameraController3>();
		human = Object.FindObjectOfType<Human>();
		ragdoll = human.ragdoll;
		if (StartupExperienceUI.instance.gameObject != null)
		{
			StartupExperienceUI.instance.gameObject.SetActive(value: false);
		}
		Object.Destroy(StartupExperienceUI.instance.gameObject);
		LeaveGameStartupXP();
		DestroyStartupStuff();
		MenuSystem.instance.ShowMainMenu(hideLogo: true);
		if (multiplayerServer != null)
		{
			App.instance.AcceptInvite(multiplayerServer);
		}
	}

	private IEnumerator StartupExperienceRoutine()
	{
		cameraEffects = Object.FindObjectOfType<MenuCameraEffects>();
		cameraController = Object.FindObjectOfType<CameraController3>();
		human = Human.all[0];
		ragdoll = human.ragdoll;
		StartupExperienceUI startupUI = StartupExperienceUI.instance;
		MenuSystem.instance.EnterMenuInputMode();
		EnterGameStartupXP();
		Begin();
		drones.Play();
		while (drones.dronesTime < 1f)
		{
			yield return null;
		}
		FadeOutDim();
		while (drones.dronesTime < offsetTime)
		{
			yield return null;
		}
		startupUI.curveLogo.Play("CurveIn");
		while (drones.dronesTime < 4f + offsetTime)
		{
			yield return null;
		}
		startupUI.curveLogo.Play("CurveOut");
		while ((double)drones.dronesTime < 6.01 + (double)offsetTime)
		{
			yield return null;
		}
		startupUI.noBrakesLogo.Play("NbgLogoIn");
		while ((double)drones.dronesTime < 9.12 + (double)offsetTime)
		{
			yield return null;
		}
		startupUI.noBrakesLogo.Play("NbgLogoOut");
		while ((double)drones.dronesTime < 10.5 + (double)offsetTime)
		{
			yield return null;
		}
		startupUI.humanLogo.gameObject.SetActive(value: true);
		startupUI.humanLogo.Play("HumanLogoIn");
		startupUI.gameByLine1.Play("GameByInOut");
		startupUI.gameByLine2.Play("GameByInOut");
		yield return new WaitForSeconds(1f);
		DropHuman();
		bool pressAnythingVisible = false;
		while ((double)drones.dronesTime < 13.2 + (double)offsetTime && !IsAnythingPressed())
		{
			yield return null;
		}
		if (!IsAnythingPressed())
		{
			pressAnythingVisible = true;
			startupUI.pressAnything.Play("PressAnythingIn");
			while (!IsAnythingPressed())
			{
				yield return null;
			}
		}
		ReleaseLatch();
		startupUI.humanLogo.Play("HumanLogoOut");
		if (pressAnythingVisible)
		{
			startupUI.pressAnything.Play("PressAnythingOut");
		}
	}

	private bool IsAnythingPressed()
	{
		InputDevice activeDevice = InputManager.ActiveDevice;
		return activeDevice.AnyButton.IsPressed || InputManager.AnyKeyIsPressed || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2);
	}

	private void EnterGameStartupXP()
	{
		MenuSystem.instance.ExitMenuInputMode();
		if (human != null)
		{
			human.disableInput = true;
			human.GetComponent<HumanMotion2>().enabled = false;
			ragdoll.AllowHandBallRotation(allow: false);
			cameraController.enabled = false;
			cameraController.transform.position = new Vector3(-0.5f, 4f, 0f);
		}
	}

	public void LeaveGameStartupXP()
	{
		if (human != null)
		{
			human.disableInput = false;
			human.GetComponent<HumanMotion2>().enabled = true;
			ragdoll.AllowHandBallRotation(allow: true);
			cameraController.enabled = true;
			cameraController.TransitionFromCurrent(3f);
		}
	}

	public void DestroyStartupStuff()
	{
		Object.Destroy(base.gameObject);
	}

	public void Begin()
	{
		cameraEffects.BlackOut();
	}

	public void FadeOutDim()
	{
		cameraEffects.FadeOutBlackOut();
	}

	public void DropHuman()
	{
		humanState = HumanState.Fall;
	}

	public void ReleaseLatch()
	{
		humanState = HumanState.ReleaseLatch;
		GetComponent<StartupExperienceGeometry>().ReleaseDoor();
		syncCamera = true;
		offset = cameraController.transform.position - human.transform.position;
	}

	private void Update()
	{
		if (human == null)
		{
			return;
		}
		if (humanState == HumanState.HoldBeforeFall)
		{
			human.transform.position = base.transform.position.SetY(20f);
			human.ControlVelocity(15f, killHorizontal: true);
			ragdoll.StretchHandsLegs(Vector3.forward, Vector3.right, 50);
		}
		else if (humanState == HumanState.Fall)
		{
			Vector3 position = human.transform.position;
			if (position.y > 2f)
			{
				Transform transform = human.transform;
				Vector3 position2 = base.transform.position;
				Vector3 position3 = human.transform.position;
				transform.position = position2.SetY(position3.y);
				human.ControlVelocity(15f, killHorizontal: true);
				ragdoll.StretchHandsLegs(2f * Vector3.forward + Vector3.right, 2f * Vector3.right - Vector3.forward, 10);
			}
		}
	}

	private void LateUpdate()
	{
		if (syncCamera)
		{
			Vector3 position = cameraController.transform.position;
			if (position.y > -2f)
			{
				cameraController.transform.position = human.transform.position + offset;
				return;
			}
			syncCamera = false;
			LeaveGameStartupXP();
			DestroyStartupStuff();
			App.instance.StartupFinished();
		}
	}
}
