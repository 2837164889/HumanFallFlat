using InControl;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerWithBindingsExample
{
	public class PlayerManager : MonoBehaviour
	{
		public GameObject playerPrefab;

		private const int maxPlayers = 4;

		private List<Vector3> playerPositions = new List<Vector3>
		{
			new Vector3(-1f, 1f, -10f),
			new Vector3(1f, 1f, -10f),
			new Vector3(-1f, -1f, -10f),
			new Vector3(1f, -1f, -10f)
		};

		private List<Player> players = new List<Player>(4);

		private PlayerActions keyboardListener;

		private PlayerActions joystickListener;

		private void OnEnable()
		{
			InputManager.OnDeviceDetached += OnDeviceDetached;
			keyboardListener = PlayerActions.CreateWithKeyboardBindings();
			joystickListener = PlayerActions.CreateWithJoystickBindings();
		}

		private void OnDisable()
		{
			InputManager.OnDeviceDetached -= OnDeviceDetached;
			joystickListener.Destroy();
			keyboardListener.Destroy();
		}

		private void Update()
		{
			if (JoinButtonWasPressedOnListener(joystickListener))
			{
				InputDevice activeDevice = InputManager.ActiveDevice;
				if (ThereIsNoPlayerUsingJoystick(activeDevice))
				{
					CreatePlayer(activeDevice);
				}
			}
			if (JoinButtonWasPressedOnListener(keyboardListener) && ThereIsNoPlayerUsingKeyboard())
			{
				CreatePlayer(null);
			}
		}

		private bool JoinButtonWasPressedOnListener(PlayerActions actions)
		{
			return actions.Green.WasPressed || actions.Red.WasPressed || actions.Blue.WasPressed || actions.Yellow.WasPressed;
		}

		private Player FindPlayerUsingJoystick(InputDevice inputDevice)
		{
			int count = players.Count;
			for (int i = 0; i < count; i++)
			{
				Player player = players[i];
				if (player.Actions.Device == inputDevice)
				{
					return player;
				}
			}
			return null;
		}

		private bool ThereIsNoPlayerUsingJoystick(InputDevice inputDevice)
		{
			return FindPlayerUsingJoystick(inputDevice) == null;
		}

		private Player FindPlayerUsingKeyboard()
		{
			int count = players.Count;
			for (int i = 0; i < count; i++)
			{
				Player player = players[i];
				if (player.Actions == keyboardListener)
				{
					return player;
				}
			}
			return null;
		}

		private bool ThereIsNoPlayerUsingKeyboard()
		{
			return FindPlayerUsingKeyboard() == null;
		}

		private void OnDeviceDetached(InputDevice inputDevice)
		{
			Player player = FindPlayerUsingJoystick(inputDevice);
			if (player != null)
			{
				RemovePlayer(player);
			}
		}

		private Player CreatePlayer(InputDevice inputDevice)
		{
			if (players.Count < 4)
			{
				Vector3 position = playerPositions[0];
				playerPositions.RemoveAt(0);
				GameObject gameObject = Object.Instantiate(playerPrefab, position, Quaternion.identity);
				Player component = gameObject.GetComponent<Player>();
				if (inputDevice == null)
				{
					component.Actions = keyboardListener;
				}
				else
				{
					PlayerActions playerActions = PlayerActions.CreateWithJoystickBindings();
					playerActions.Device = inputDevice;
					component.Actions = playerActions;
				}
				players.Add(component);
				return component;
			}
			return null;
		}

		private void RemovePlayer(Player player)
		{
			playerPositions.Insert(0, player.transform.position);
			players.Remove(player);
			player.Actions = null;
			Object.Destroy(player.gameObject);
		}

		private void OnGUI()
		{
			float num = 10f;
			GUI.Label(new Rect(10f, num, 300f, num + 22f), "Active players: " + players.Count + "/" + 4);
			num += 22f;
			if (players.Count < 4)
			{
				GUI.Label(new Rect(10f, num, 300f, num + 22f), "Press a button or a/s/d/f key to join!");
				num += 22f;
			}
		}
	}
}
