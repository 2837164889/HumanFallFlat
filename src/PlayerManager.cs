using InControl;
using Multiplayer;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	public enum PlayerStatus
	{
		CanJoin,
		Joining,
		Joined,
		CanLeave,
		Leaving,
		Left,
		PostLeft,
		Online
	}

	private List<InputDevice> activeDevices = new List<InputDevice>();

	private List<InputDevice> devicesHoldingJoin = new List<InputDevice>();

	private InputDevice activeDevice;

	public static PlayerManager instance;

	public float maxSplitScreenDelay = 2f;

	public float buttonPressWaitTime = 1f;

	private float currentSplitScreenDelay;

	private InputDevice previousDevice;

	private InputDevice currentDevice;

	private float addCoopBlock;

	private const float kSecondPlayerRequestedTimeDelay = 3f;

	private float secondPlayerRequestedTime;

	private bool _secondPlayerRequested;

	public PlayerStatus SecondPlayerStatus
	{
		get;
		set;
	}

	public bool p1Locked => devicesHoldingJoin.Count > 0;

	public bool p2Locked => activeDevices.Count > 1;

	public InputDevice p1Device => (activeDevices.Count <= 0) ? devicesHoldingJoin[0] : activeDevices[0];

	public InputDevice p2Device => activeDevices[1];

	private bool secondPlayerRequested
	{
		get
		{
			return _secondPlayerRequested;
		}
		set
		{
			_secondPlayerRequested = value;
			secondPlayerRequestedTime = Time.unscaledTime + 3f;
		}
	}

	private void ActiveDeviceChanged(InputDevice device)
	{
		if (device != currentDevice)
		{
			previousDevice = currentDevice;
			currentDevice = device;
		}
	}

	private void OnEnable()
	{
		instance = this;
		InputManager.OnDeviceDetached += OnDeviceDetached;
		InputManager.OnActiveDeviceChanged += ActiveDeviceChanged;
	}

	private bool GameStateValidForCoop(bool checkLoadLevel = false)
	{
		if (App.state != AppSate.Menu && App.state != AppSate.PlayLevel && App.state != AppSate.Customize && App.state != AppSate.ClientLobby && App.state != AppSate.ServerLobby && App.state != AppSate.ClientPlayLevel && App.state != AppSate.ServerPlayLevel && App.state != AppSate.ClientJoin && App.state != AppSate.ClientLoadLobby && App.state != AppSate.ServerHost && App.state != AppSate.ServerLoadLobby && App.state != AppSate.ClientWaitServerLoad)
		{
			if (!checkLoadLevel)
			{
				return false;
			}
			if (App.state != AppSate.LoadLevel && App.state != AppSate.ClientLoadLevel && App.state != AppSate.ServerLoadLevel)
			{
				return false;
			}
		}
		return true;
	}

	private void UpdateStandaloneCoop()
	{
		switch (SecondPlayerStatus)
		{
		case PlayerStatus.CanJoin:
		{
			if (!GameStateValidForCoop())
			{
				SecondPlayerStatus = PlayerStatus.Online;
				break;
			}
			List<NetPlayer> players = NetGame.instance.local.players;
			activeDevices.Clear();
			InputDevice inputDevice = (players.Count <= 0) ? NetPlayer.Player0Device : players[0].controls.device;
			int num = 0;
			foreach (InputDevice device in InputManager.Devices)
			{
				if (device.DeviceClass != InputDeviceClass.Keyboard && device.DeviceClass != InputDeviceClass.Mouse)
				{
					num++;
				}
			}
			foreach (InputDevice device2 in InputManager.Devices)
			{
				if (device2 != inputDevice && device2.DeviceClass != InputDeviceClass.Keyboard && device2.DeviceClass != InputDeviceClass.Mouse && device2.Command.IsPressed && device2.Command.HasChanged && (num <= 1 || device2 != previousDevice))
				{
					activeDevice = device2;
					activeDevices.Add(previousDevice);
					Debug.Log("InputManager.ActiveDevice: " + ((previousDevice == null) ? "null" : previousDevice.GUID.ToString()));
					Debug.Log("device: " + device2.GUID);
					activeDevices.Add(device2);
					currentSplitScreenDelay = 0f;
					SecondPlayerStatus = PlayerStatus.Joining;
				}
			}
			break;
		}
		case PlayerStatus.Joining:
			if (!GameStateValidForCoop())
			{
				SecondPlayerStatus = PlayerStatus.Online;
			}
			else if (activeDevice.Command.IsPressed)
			{
				currentSplitScreenDelay += Time.unscaledDeltaTime;
				currentSplitScreenDelay += Time.unscaledDeltaTime;
				if (currentSplitScreenDelay >= maxSplitScreenDelay)
				{
					currentSplitScreenDelay = 0f;
					SecondPlayerStatus = PlayerStatus.Joined;
				}
			}
			else
			{
				SecondPlayerStatus = PlayerStatus.CanJoin;
			}
			break;
		case PlayerStatus.Joined:
			if (!GameStateValidForCoop(checkLoadLevel: true))
			{
				SecondPlayerStatus = PlayerStatus.Left;
				break;
			}
			currentSplitScreenDelay += Time.unscaledDeltaTime;
			if (currentSplitScreenDelay >= maxSplitScreenDelay)
			{
				currentSplitScreenDelay = 0f;
				SecondPlayerStatus = PlayerStatus.CanLeave;
			}
			break;
		case PlayerStatus.CanLeave:
			if (!GameStateValidForCoop(checkLoadLevel: true))
			{
				SecondPlayerStatus = PlayerStatus.Left;
			}
			else if (MenuSystem.instance.activeMenu != null && activeDevice.Command.IsPressed && activeDevice.Command.HasChanged)
			{
				currentSplitScreenDelay = 0f;
				SecondPlayerStatus = PlayerStatus.Leaving;
			}
			break;
		case PlayerStatus.Leaving:
			if (!GameStateValidForCoop(checkLoadLevel: true))
			{
				SecondPlayerStatus = PlayerStatus.Left;
			}
			else if (activeDevice.Command.IsPressed)
			{
				currentSplitScreenDelay += Time.unscaledDeltaTime;
				if (currentSplitScreenDelay >= maxSplitScreenDelay)
				{
					SecondPlayerStatus = PlayerStatus.Left;
				}
			}
			else
			{
				currentSplitScreenDelay = 0f;
				SecondPlayerStatus = PlayerStatus.CanLeave;
			}
			break;
		case PlayerStatus.Left:
			if (NetGame.instance.local.players.Count > 1)
			{
				SetSingle();
			}
			currentSplitScreenDelay = 0f;
			SecondPlayerStatus = PlayerStatus.PostLeft;
			break;
		case PlayerStatus.PostLeft:
			if (!DialogOverlay.IsShowing)
			{
				currentSplitScreenDelay += Time.unscaledDeltaTime;
			}
			if (currentSplitScreenDelay >= maxSplitScreenDelay)
			{
				currentSplitScreenDelay = 0f;
				if (!GameStateValidForCoop())
				{
					SecondPlayerStatus = PlayerStatus.Online;
				}
				else
				{
					SecondPlayerStatus = PlayerStatus.CanJoin;
				}
			}
			break;
		case PlayerStatus.Online:
			if (!GameStateValidForCoop())
			{
				return;
			}
			SecondPlayerStatus = PlayerStatus.CanJoin;
			break;
		}
		if (!secondPlayerRequested && (SecondPlayerStatus == PlayerStatus.Joined || SecondPlayerStatus == PlayerStatus.CanLeave) && Time.timeScale != 0f && (App.state == AppSate.PlayLevel || App.state == AppSate.ClientLobby || App.state == AppSate.ServerLobby || App.state == AppSate.ClientPlayLevel || App.state == AppSate.ServerPlayLevel) && NetGame.instance.local.players.Count < 2)
		{
			NetGame.instance.AddLocalPlayer();
			if (NetGame.instance.local.players.Count > 1)
			{
				addCoopBlock = 1f;
			}
			secondPlayerRequested = true;
		}
		else if (App.state != AppSate.PlayLevel && App.state != AppSate.LoadLevel && App.state != AppSate.ClientLobby && App.state != AppSate.ServerLobby && App.state != AppSate.ClientLoadLevel && App.state != AppSate.ServerLoadLevel && App.state != AppSate.ClientPlayLevel && App.state != AppSate.ServerPlayLevel && NetGame.instance.local.players.Count > 1)
		{
			SetSingle();
		}
		if (secondPlayerRequested)
		{
			if (NetGame.instance.local.players.Count > 1)
			{
				secondPlayerRequested = false;
			}
			else if (Time.unscaledTime > secondPlayerRequestedTime)
			{
				secondPlayerRequested = false;
				SecondPlayerStatus = PlayerStatus.CanJoin;
			}
		}
	}

	private void Update()
	{
		if (addCoopBlock > 0f)
		{
			addCoopBlock -= Time.deltaTime;
		}
		else if (!(Game.instance == null) && !(NetGame.instance == null) && NetGame.instance.local != null)
		{
			UpdateStandaloneCoop();
		}
	}

	private void OnDeviceDetached(InputDevice inputDevice)
	{
		if (activeDevices.Count > 1)
		{
			if (activeDevices[1] == null || activeDevices[0] == inputDevice)
			{
				RemovePlayer(0);
			}
			else if (activeDevices[0] == null || activeDevices[1] == inputDevice)
			{
				SetSingle();
				SecondPlayerStatus = PlayerStatus.Online;
			}
		}
	}

	public static void SetSingle()
	{
		if (instance.activeDevices.Count == 2)
		{
			instance.RemovePlayer(1);
		}
		if (Human.all.Count > 0)
		{
			Human.all[0].state = HumanState.Idle;
		}
	}

	private void RemovePlayer(int index)
	{
		if (NetGame.instance.local.players.Count > 1)
		{
			NetGame.instance.RemoveLocalPlayer(NetGame.instance.local.players[index]);
		}
	}

	public void ApplyControls()
	{
		List<NetPlayer> players = NetGame.instance.local.players;
		if (activeDevices.Count < 2 || players.Count < 2)
		{
			for (int i = 0; i < players.Count; i++)
			{
				players[i].controls.SetAny();
			}
			return;
		}
		bool flag = false;
		for (int j = 0; j < activeDevices.Count; j++)
		{
			flag |= (activeDevices[j] == null);
		}
		for (int k = 0; k < players.Count; k++)
		{
			if (flag)
			{
				if (activeDevices[k] == null)
				{
					players[k].controls.SetMouse();
				}
				else
				{
					players[k].controls.SetController();
				}
			}
			else
			{
				players[k].controls.SetDevice(activeDevices[k]);
			}
		}
	}

	public void OnLocalPlayerAdded(NetPlayer player)
	{
		ApplyControls();
		if (NetGame.instance.local.players.Count > 1 && activeDevices.Count < 2)
		{
			NetGame.instance.RemoveLocalPlayer(player);
		}
	}

	public void OnLocalPlayerRemoved(NetPlayer player)
	{
		ApplyControls();
	}

	private void IncrementSecondPlayerStatus()
	{
		if (SecondPlayerStatus != PlayerStatus.Left)
		{
			SecondPlayerStatus++;
		}
		else
		{
			SecondPlayerStatus = PlayerStatus.CanJoin;
		}
	}
}
