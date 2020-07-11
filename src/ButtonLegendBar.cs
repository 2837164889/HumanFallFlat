using I2.Loc;
using Multiplayer;
using System;
using System.Text;
using TMPro;
using UnityEngine;

public class ButtonLegendBar : MonoBehaviour
{
	public GameObject LegendBar;

	public SplitScreenNotification splitScreenNotification;

	public MultiplayerMenuOptions multiplayerMenuOptions;

	public LobbySelectMenuOptions lobbySelectMenuOptions;

	private static bool showSplitScreenNotification;

	private static bool showPlayerActions;

	public static ButtonLegendBar instance;

	public GameObject restartObject;

	public TextMeshProUGUI PlayText;

	public TextMeshProUGUI playGlyph;

	public TextMeshProUGUI restartGlyph;

	public TextMeshProUGUI backGlyph;

	public GameObject CarrouselOptionsContainer;

	private float playGlyphInitialFontSize;

	private const float kPlayGlyphFontSizeMultiplyerText = 0.6f;

	private StringBuilder glyphStringbuilder;

	private void Awake()
	{
		instance = this;
		glyphStringbuilder = new StringBuilder();
	}

	private void Start()
	{
		playGlyphInitialFontSize = playGlyph.fontSize;
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	private void OnEnable()
	{
		MenuSystem menuSystem = MenuSystem.instance;
		menuSystem.InputDeviceChange = (Action<MenuSystem.eInputDeviceType>)Delegate.Combine(menuSystem.InputDeviceChange, new Action<MenuSystem.eInputDeviceType>(OnInputDeviceChanged));
	}

	private void OnDisable()
	{
		MenuSystem menuSystem = MenuSystem.instance;
		menuSystem.InputDeviceChange = (Action<MenuSystem.eInputDeviceType>)Delegate.Remove(menuSystem.InputDeviceChange, new Action<MenuSystem.eInputDeviceType>(OnInputDeviceChanged));
	}

	public static void RefreshStatus()
	{
		if (instance != null)
		{
			instance.Update();
		}
	}

	public static void SetShowSplitScreenNotification(bool value)
	{
		showSplitScreenNotification = true;
	}

	public static void SetShowPlayerActions(bool value)
	{
		showPlayerActions = true;
		Debug.Log("Setting player actions - " + value);
	}

	private void Update()
	{
		bool active = PlayerManager.instance.SecondPlayerStatus != PlayerManager.PlayerStatus.Online && !DLCMenu.InDLCMenu;
		splitScreenNotification.gameObject.SetActive(active);
		splitScreenNotification.SplitRefreshEnable(LevelSelectMenu2.InLevelSelectMenu || CustomizationPresetMenu.InCustomizationPresetMenu);
		splitScreenNotification.DLCAvailableEnable(MainMenu.InMainMenu && DLC.instance.SupportsDLC());
		lobbySelectMenuOptions.gameObject.SetActive(MultiplayerSelectLobbyMenu.InLobbySelectMenu);
		multiplayerMenuOptions.gameObject.SetActive(multiplayerMenuOptions.ShouldShow);
		bool flag = false;
		MenuTransition activeMenu = MenuSystem.instance.activeMenu;
		if (activeMenu != null && !DialogOverlay.IsOnIncludingDelay() && App.state != AppSate.LoadLevel)
		{
			flag = true;
			if (activeMenu is MultiplayerErrorMenu || activeMenu is ConfirmMenu)
			{
				flag = false;
			}
		}
		if (SteamProgressOverlay.instance.DialogShowing())
		{
			flag = false;
		}
		if (LegendBar.activeSelf != flag)
		{
			LegendBar.SetActive(flag);
		}
	}

	private void OnInputDeviceChanged(MenuSystem.eInputDeviceType deviceType)
	{
		UpdateLegendGlyphs(deviceType);
	}

	public void ToogleCarouselLegend(bool state)
	{
		UpdateLegendGlyphs(MenuSystem.instance.GetCurrentInputDevice());
		CarrouselOptionsContainer.SetActive(state);
	}

	private void UpdateLegendGlyphs(MenuSystem.eInputDeviceType deviceType)
	{
		switch (deviceType)
		{
		case MenuSystem.eInputDeviceType.Controller:
		{
			TextMeshProUGUI textMeshProUGUI5 = playGlyph;
			float num = playGlyphInitialFontSize;
			restartGlyph.fontSize = num;
			num = num;
			backGlyph.fontSize = num;
			textMeshProUGUI5.fontSize = num;
			TextMeshProUGUI textMeshProUGUI6 = playGlyph;
			Color blue = Color.black;
			restartGlyph.color = blue;
			blue = blue;
			backGlyph.color = blue;
			textMeshProUGUI6.color = blue;
			playGlyph.text = "\u20fd\n";
			backGlyph.text = "\u20fe\n";
			restartGlyph.text = "℈\n";
			break;
		}
		case MenuSystem.eInputDeviceType.Keyboard:
		{
			TextMeshProUGUI textMeshProUGUI3 = playGlyph;
			float num = playGlyphInitialFontSize * 0.6f;
			restartGlyph.fontSize = num;
			num = num;
			backGlyph.fontSize = num;
			textMeshProUGUI3.fontSize = num;
			TextMeshProUGUI textMeshProUGUI4 = playGlyph;
			Color blue = Color.blue;
			restartGlyph.color = blue;
			blue = blue;
			backGlyph.color = blue;
			textMeshProUGUI4.color = blue;
			playGlyph.text = CreateBracketedKeyGlyph("F");
			backGlyph.text = CreateBracketedKeyGlyph("ESC");
			restartGlyph.text = CreateBracketedKeyGlyph("R");
			break;
		}
		case MenuSystem.eInputDeviceType.Mouse:
		{
			TextMeshProUGUI textMeshProUGUI = playGlyph;
			float num = playGlyphInitialFontSize * 0.6f;
			restartGlyph.fontSize = num;
			num = num;
			backGlyph.fontSize = num;
			textMeshProUGUI.fontSize = num;
			TextMeshProUGUI textMeshProUGUI2 = playGlyph;
			Color blue = Color.blue;
			restartGlyph.color = blue;
			blue = blue;
			backGlyph.color = blue;
			textMeshProUGUI2.color = blue;
			playGlyph.text = CreateBracketedKeyGlyph(ScriptLocalization.Get("MENU.LEVELSELECT/LMB"));
			backGlyph.text = CreateBracketedKeyGlyph("ESC");
			restartGlyph.text = CreateBracketedKeyGlyph(ScriptLocalization.Get("MENU.LEVELSELECT/RMB"));
			break;
		}
		}
	}

	private string CreateBracketedKeyGlyph(string glyph)
	{
		glyphStringbuilder.Length = 0;
		glyphStringbuilder.Append("[");
		glyphStringbuilder.Append(glyph);
		glyphStringbuilder.Append("]");
		glyphStringbuilder.Append("  \n");
		return glyphStringbuilder.ToString();
	}

	public void SetContinueMode(bool inContinueMode)
	{
		restartObject.SetActive(inContinueMode);
		PlayText.text = ((!inContinueMode) ? ScriptLocalization.Get("MENU.LEVELSELECT/PLAY") : ScriptLocalization.Get("MENU/PLAY/CONTINUE"));
	}
}
