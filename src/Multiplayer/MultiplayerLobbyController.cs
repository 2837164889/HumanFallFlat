using InControl;
using UnityEngine;

namespace Multiplayer
{
	public class MultiplayerLobbyController : MonoBehaviour
	{
		public Transform playerSpawn;

		public GameCamera gameCamera;

		public static MultiplayerLobbyController instance;

		private bool uiVisible;

		private void Awake()
		{
			Game.instance.BeforeLoad();
		}

		private void Start()
		{
			Game.instance.AfterLoad(0, 0);
		}

		private void OnEnable()
		{
			if (instance != null)
			{
				Teardown();
			}
			instance = this;
			uiVisible = true;
			instance.gameCamera.gameObject.SetActive(value: true);
			MenuCameraEffects.instance.OverrideCamera(gameCamera.transform, applyEffects: true);
			Dialogs.HideProgress();
		}

		public static void Teardown()
		{
			if (instance != null && instance.uiVisible)
			{
				MenuCameraEffects.instance.RemoveOverride();
				instance.gameCamera.gameObject.SetActive(value: false);
				instance.uiVisible = false;
			}
			instance = null;
			Game.instance.AfterUnload();
		}

		protected void Update()
		{
			if (instance != null && !uiVisible && Game.GetKeyDown(KeyCode.Escape) && MenuSystem.CanInvokeFromGame)
			{
				ShowUI();
			}
			if (!NetGame.isServer)
			{
				return;
			}
			for (int i = 0; i < Human.all.Count; i++)
			{
				Human human = Human.all[i];
				Vector3 position = human.transform.position;
				if (position.y < -50f)
				{
					human.transform.position = Vector3.zero;
					human.KillHorizontalVelocity();
					human.MakeUnconscious(1f);
				}
			}
		}

		private void FixedUpdate()
		{
			InputDevice activeDevice = InputManager.ActiveDevice;
			if (instance != null && !uiVisible && activeDevice.CommandWasPressed && MenuSystem.CanInvokeFromGame)
			{
				ShowUI();
			}
		}

		public void HideUI()
		{
			if (NetGame.instance.local.players.Count >= 1)
			{
				uiVisible = false;
				MenuCameraEffects.instance.RemoveOverride();
				gameCamera.gameObject.SetActive(value: false);
				MenuSystem.instance.HideMenus();
				NetPlayer netPlayer = NetGame.instance.local.players[0];
				netPlayer.cameraController.TransitionFrom(gameCamera, 10f, 1f);
			}
		}

		public void ShowUI()
		{
			uiVisible = true;
			gameCamera.gameObject.SetActive(value: true);
			MenuCameraEffects.instance.OverrideCamera(gameCamera.transform, applyEffects: true);
			MenuSystem.instance.FadeInForward(MenuSystem.instance.GetMenu<MultiplayerLobbyMenu>());
			MenuSystem.instance.EnterMenuInputMode();
			MenuSystem.instance.state = MenuSystemState.MainMenu;
			SubtitleManager.instance.Hide();
		}
	}
}
