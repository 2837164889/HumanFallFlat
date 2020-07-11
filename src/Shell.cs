using Multiplayer;
using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shell : MonoBehaviour
{
	public TMP_InputField input;

	public TextMeshProUGUI text;

	public GameObject ui;

	public static bool visible = false;

	public static Shell instance;

	private bool lostDevice;

	private bool reloadSkins;

	private static CommandRegistry commands = new CommandRegistry(Print);

	private static string contents = string.Empty;

	[CompilerGenerated]
	private static Action<string> _003C_003Ef__mg_0024cache0;

	private void Awake()
	{
		instance = this;
		Application.logMessageReceived += Application_logMessageReceived;
		RegisterCommand("?", commands.OnHelp);
		RegisterCommand("help", commands.OnHelp, "help <command>\r\nExplains command.");
	}

	private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
	{
		if (condition.Contains("device lost"))
		{
			lostDevice = true;
			return;
		}
		switch (type)
		{
		case LogType.Error:
		case LogType.Assert:
		case LogType.Exception:
			Print("<#FF0000>" + condition + "</color>");
			Print("<#FF7F7F>" + stackTrace + "</color>");
			break;
		case LogType.Warning:
			Print("<#FFFF00>" + condition + "</color>");
			break;
		default:
			Print(condition);
			break;
		}
	}

	private void Update()
	{
		if (lostDevice)
		{
			reloadSkins = true;
			lostDevice = false;
		}
		else if (reloadSkins)
		{
			reloadSkins = (lostDevice = false);
			for (int i = 0; i < NetGame.instance.players.Count; i++)
			{
				NetGame.instance.players[i].ReapplySkin();
			}
		}
		if (MenuSystem.keyboardState != 0 && MenuSystem.keyboardState != KeyboardState.Shell)
		{
			return;
		}
		if (Game.GetKeyDown(KeyCode.BackQuote) || Game.GetKeyDown(KeyCode.F1))
		{
			visible = !visible;
			ui.SetActive(visible);
			MenuSystem.keyboardState = (visible ? KeyboardState.Shell : KeyboardState.None);
			if (visible)
			{
				MenuSystem.instance.FocusOnMouseOver(enable: false);
				input.ActivateInputField();
			}
			else
			{
				input.DeactivateInputField();
				if (input.text.EndsWith("`"))
				{
					input.text = input.text.Substring(0, input.text.Length - 1);
				}
			}
		}
		if (!visible || !(EventSystem.current.currentSelectedGameObject == input.gameObject))
		{
			return;
		}
		if (Game.GetKeyDown(KeyCode.Escape))
		{
			if (!string.IsNullOrEmpty(input.text))
			{
				input.text = string.Empty;
			}
			else
			{
				visible = false;
				ui.SetActive(value: false);
				MenuSystem.keyboardState = KeyboardState.None;
				input.DeactivateInputField();
			}
		}
		if (Game.GetKeyDown(KeyCode.Return))
		{
			string text = input.text.Trim();
			Print("> " + text);
			commands.Execute(text.ToLowerInvariant());
			input.text = string.Empty;
			input.ActivateInputField();
		}
	}

	public static void RawInvoke(string cmd)
	{
		commands.Execute(cmd.ToLowerInvariant());
	}

	public static void RegisterCommand(string command, Action onCommand, string help = null)
	{
		commands.RegisterCommand(command, onCommand, help);
	}

	public static void RegisterCommand(string command, Action<string> onCommand, string help = null)
	{
		commands.RegisterCommand(command, onCommand, help);
	}

	public static void Print(string str)
	{
		contents = contents + "\r\n" + str;
		int num = 0;
		int num2 = -1;
		while ((num2 = contents.IndexOf('\n', num2 + 1)) >= 0)
		{
			num++;
		}
		num2 = -1;
		while (num >= 40)
		{
			num2 = contents.IndexOf('\n', num2 + 1);
			num--;
		}
		if (num2 > 0)
		{
			contents = contents.Substring(num2 + 1);
		}
		instance.text.text = contents;
	}
}
