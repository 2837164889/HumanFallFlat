using InControl;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerBasicExample
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

		private void Start()
		{
			InputManager.OnDeviceDetached += OnDeviceDetached;
		}

		private void Update()
		{
			InputDevice activeDevice = InputManager.ActiveDevice;
			if (JoinButtonWasPressedOnDevice(activeDevice) && ThereIsNoPlayerUsingDevice(activeDevice))
			{
				CreatePlayer(activeDevice);
			}
		}

		private bool JoinButtonWasPressedOnDevice(InputDevice inputDevice)
		{
			return inputDevice.Action1.WasPressed || inputDevice.Action2.WasPressed || inputDevice.Action3.WasPressed || inputDevice.Action4.WasPressed;
		}

		private Player FindPlayerUsingDevice(InputDevice inputDevice)
		{
			int count = players.Count;
			for (int i = 0; i < count; i++)
			{
				Player player = players[i];
				if (player.Device == inputDevice)
				{
					return player;
				}
			}
			return null;
		}

		private bool ThereIsNoPlayerUsingDevice(InputDevice inputDevice)
		{
			return FindPlayerUsingDevice(inputDevice) == null;
		}

		private void OnDeviceDetached(InputDevice inputDevice)
		{
			Player player = FindPlayerUsingDevice(inputDevice);
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
				component.Device = inputDevice;
				players.Add(component);
				return component;
			}
			return null;
		}

		private void RemovePlayer(Player player)
		{
			playerPositions.Insert(0, player.transform.position);
			players.Remove(player);
			player.Device = null;
			Object.Destroy(player.gameObject);
		}

		private void OnGUI()
		{
			float num = 10f;
			GUI.Label(new Rect(10f, num, 300f, num + 22f), "Active players: " + players.Count + "/" + 4);
			num += 22f;
			if (players.Count < 4)
			{
				GUI.Label(new Rect(10f, num, 300f, num + 22f), "Press a button to join!");
				num += 22f;
			}
		}
	}
}
