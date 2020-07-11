using I2.Loc;
using Multiplayer;
using UnityEngine.UI;

public class MultiplayerSelectLobbyMenuItem : ListViewItem
{
	public Text label;

	public Text labelPlayers;

	private const string kClosedTextID = "MULTIPLAYER/JOIN.Closed";

	private static string closedText;

	public NetTransport.ILobbyEntry boundData;

	private void Start()
	{
		if (closedText == null)
		{
			Localise();
			LocalizationManager.OnLocalisation += Localise;
		}
	}

	private void Localise()
	{
		closedText = ScriptLocalization.Get("MULTIPLAYER/JOIN.Closed");
	}

	public override void Bind(int index, object data)
	{
		base.Bind(index, data);
		DataRefreshed(data);
	}

	public bool IsActive()
	{
		MenuButton component = GetComponent<MenuButton>();
		if ((bool)component)
		{
			return GetComponent<MenuButton>().isOn;
		}
		return false;
	}

	public void SetActive(bool active)
	{
		GetComponent<MenuButton>().isOn = active;
	}

	public void DataInvalid()
	{
		labelPlayers.text = closedText;
	}

	public void DataRefreshed(object data)
	{
		boundData = (NetTransport.ILobbyEntry)data;
		label.text = boundData.name();
		boundData.getDisplayInfo(out NetTransport.LobbyDisplayInfo info);
		if (info.ShouldDisplayAllAttr(3221225472u) && info.MaxPlayers != 0)
		{
			labelPlayers.text = $"{info.NumPlayersForDisplay}/{info.MaxPlayers}";
		}
		else
		{
			labelPlayers.text = string.Empty;
		}
	}
}
