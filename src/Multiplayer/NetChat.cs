using I2.Loc;
using InControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer
{
	public class NetChat : MonoBehaviour
	{
		public Color[] colors;

		public InputField input;

		public Text text;

		public GameObject receivedUI;

		public GameObject typeUI;

		public static bool visible = false;

		public static bool typing = false;

		public static NetChat instance;

		public static CommandRegistry serverCommands = new CommandRegistry(Print, expand: true);

		public static CommandRegistry clientCommands = new CommandRegistry(Print, expand: true);

		private InControlInputModule inputModule;

		public static bool allowChat = true;

		private float phase;

		private float speed;

		private float dismissIn;

		private static string contents = string.Empty;

		private const float kDismissChatWindowTime = 3f;

		[CompilerGenerated]
		private static Action<string> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Action<string> _003C_003Ef__mg_0024cache1;

		public static void RegisterCommand(bool server, bool client, string command, Action onCommand, string help = null)
		{
			if (server)
			{
				serverCommands.RegisterCommand(command, onCommand, help);
			}
			if (client)
			{
				clientCommands.RegisterCommand(command, onCommand, help);
			}
		}

		public static void RegisterCommand(bool server, bool client, string command, Action<string> onCommand, string help = null)
		{
			if (server)
			{
				serverCommands.RegisterCommand(command, onCommand, help);
			}
			if (client)
			{
				clientCommands.RegisterCommand(command, onCommand, help);
			}
		}

		public static void UnRegisterCommand(bool server, bool client, string command, Action onCommand, string help = null)
		{
			if (server)
			{
				serverCommands.UnRegisterCommand(command, onCommand, help);
			}
			if (client)
			{
				clientCommands.UnRegisterCommand(command, onCommand, help);
			}
		}

		public static void UnRegisterCommand(bool server, bool client, string command, Action<string> onCommand, string help = null)
		{
			if (server)
			{
				serverCommands.UnRegisterCommand(command, onCommand, help);
			}
			if (client)
			{
				clientCommands.UnRegisterCommand(command, onCommand, help);
			}
		}

		private void OnEnable()
		{
			instance = this;
			inputModule = UnityEngine.Object.FindObjectOfType<InControlInputModule>();
			Show(showMessages: false, showType: false);
			serverCommands.helpColor = (clientCommands.helpColor = "<#FFFF7F>");
			serverCommands.RegisterCommand("?", serverCommands.OnHelp);
			serverCommands.RegisterCommand("help", serverCommands.OnHelp);
			clientCommands.RegisterCommand("?", clientCommands.OnHelp);
			clientCommands.RegisterCommand("help", clientCommands.OnHelp);
			RegisterCommand(server: true, client: true, "list", OnList, "#MULTIPLAYER/CHAT.HelpList");
			RegisterCommand(server: true, client: true, "mute", OnMute, "#MULTIPLAYER/CHAT.HelpMute");
			RegisterCommand(server: true, client: false, "kick", OnKick, "#MULTIPLAYER/CHAT.HelpKick");
		}

		public static void PrintHelp()
		{
			if (NetGame.isServer)
			{
				serverCommands.OnHelp(string.Empty);
			}
			else
			{
				clientCommands.OnHelp(string.Empty);
			}
		}

		private NetHost GetClient(string txt)
		{
			if (string.IsNullOrEmpty(txt))
			{
				return null;
			}
			string[] array = txt.Split(new char[1]
			{
				' '
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length != 1)
			{
				return null;
			}
			int result = 0;
			if (int.TryParse(array[0], out result))
			{
				if (result == 1)
				{
					return NetGame.instance.server;
				}
				if (result - 2 < NetGame.instance.readyclients.Count && result > 1)
				{
					return NetGame.instance.readyclients[result - 2];
				}
			}
			string arg = ScriptLocalization.Get("XTRA/NetChat_NoPlayer");
			Print($"{arg} {result}");
			OnList();
			return null;
		}

		private void OnList()
		{
			if (!NetGame.isNetStarted)
			{
				string str = ScriptLocalization.Get("XTRA/NetChat_OnlyMP");
				Print(str);
			}
			else
			{
				int num = 1;
				Print(string.Format("{0} {1} {2}", num++, NetGame.instance.server.name, (!NetGame.instance.server.mute) ? string.Empty : ScriptLocalization.Get("XTRA/NetChat_Muted")));
				foreach (NetHost readyclient in NetGame.instance.readyclients)
				{
					Print(string.Format("{0} {1} {2}", num++, readyclient.name, (!readyclient.mute) ? string.Empty : ScriptLocalization.Get("XTRA/NetChat_Muted")));
				}
			}
		}

		private void OnMute(string txt)
		{
			if (string.IsNullOrEmpty(txt))
			{
				CommandRegistry.ShowCurrentHelp();
				return;
			}
			if (!NetGame.isNetStarted)
			{
				string str = ScriptLocalization.Get("XTRA/NetChat_OnlyMP");
				Print(str);
				return;
			}
			NetHost client = GetClient(txt);
			if (client == null)
			{
				return;
			}
			if (client == NetGame.instance.local)
			{
				string str2 = ScriptLocalization.Get("XTRA/NetChat_NoMuteMe");
				Print(str2);
				return;
			}
			client.mute = !client.mute;
			if (client.mute)
			{
				string text = ScriptLocalization.Get("XTRA/NetChat_Muted");
				Print(text);
				Print($"{text} {txt} {client.name}");
			}
			else
			{
				string text2 = ScriptLocalization.Get("XTRA/NetChat_UnMuted");
				Print(text2);
				Print($"{text2} {txt} {client.name}");
			}
		}

		private void OnKick(string txt)
		{
			if (string.IsNullOrEmpty(txt))
			{
				CommandRegistry.ShowCurrentHelp();
				return;
			}
			if (!NetGame.isServer)
			{
				string str = ScriptLocalization.Get("XTRA/NetChat_OnlyHost");
				Print(str);
				return;
			}
			NetHost client = GetClient(txt);
			if (client != null)
			{
				if (client == NetGame.instance.local)
				{
					string str2 = ScriptLocalization.Get("XTRA/NetChat_NoKickMe");
					Print(str2);
				}
				else
				{
					NetGame.instance.Kick(client);
				}
			}
		}

		private void Show(bool showMessages, bool showType)
		{
			if (showMessages || showType)
			{
				speed = 10f;
			}
			else
			{
				speed = -2f;
			}
			if (showType)
			{
				showMessages = true;
			}
			visible = showMessages;
			typing = showType;
			typeUI.SetActive(typing);
			if (typing)
			{
				input.ActivateInputField();
			}
			if (typing)
			{
				MenuSystem.keyboardState = KeyboardState.NetChat;
				if (MenuSystem.instance != null)
				{
					MenuSystem.instance.FocusOnMouseOver(enable: false);
				}
			}
			else
			{
				if (MenuSystem.instance != null)
				{
					MenuSystem.instance.FocusOnMouseOver(enable: true);
				}
				StartCoroutine(UnblockMenu());
			}
		}

		private IEnumerator UnblockMenu()
		{
			yield return new WaitForSeconds(0.1f);
			MenuSystem.keyboardState = KeyboardState.None;
		}

		private void Update()
		{
			phase = Mathf.Clamp01(phase + speed * Time.deltaTime);
			if (GetComponent<CanvasGroup>().alpha != phase)
			{
				GetComponent<CanvasGroup>().alpha = phase;
				if (phase > 0f != receivedUI.activeSelf)
				{
					receivedUI.SetActive(phase > 0f);
				}
			}
			allowChat = (MenuSystem.instance.activeMenu == null || MenuSystem.instance.activeMenu is MultiplayerLobbyMenu);
			if (!NetGame.isNetStarted && !string.IsNullOrEmpty(contents))
			{
				contents = string.Empty;
				CropContents();
			}
			if (!NetGame.isNetStarted || !allowChat)
			{
				if (visible)
				{
					Show(showMessages: false, showType: false);
				}
				return;
			}
			if (!typing && visible && dismissIn > 0f)
			{
				dismissIn -= Time.deltaTime;
				if (dismissIn <= 0f)
				{
					Show(showMessages: false, showType: false);
				}
			}
			if (MenuSystem.keyboardState != 0 && MenuSystem.keyboardState != KeyboardState.NetChat)
			{
				return;
			}
			if (Input.GetKeyDown(KeyCode.T) && !typing)
			{
				Show(showMessages: true, showType: true);
			}
			else if (Input.GetKeyDown(KeyCode.Return) && typing)
			{
				string text = input.text.Trim();
				if (string.IsNullOrEmpty(text))
				{
					input.text = string.Empty;
					input.DeactivateInputField();
					Show(showMessages: false, showType: false);
					return;
				}
				if (text.StartsWith("/"))
				{
					if (NetGame.isServer)
					{
						serverCommands.Execute(text.Substring(1));
					}
					else
					{
						clientCommands.Execute(text.Substring(1));
					}
				}
				else
				{
					Send(text);
				}
				input.text = string.Empty;
				Show(showMessages: true, showType: false);
			}
			else if (Input.GetKeyDown(KeyCode.Escape) && typing)
			{
				input.text = string.Empty;
				input.DeactivateInputField();
				Show(showMessages: false, showType: false);
			}
		}

		public static void Print(string str)
		{
			if (!visible)
			{
				instance.Show(showMessages: true, typing);
			}
			instance.dismissIn = 3f;
			str = str.Replace("<#", "<color=#");
			if (string.IsNullOrEmpty(contents))
			{
				contents = str;
			}
			else
			{
				contents = contents + "\r\n" + str;
			}
			CropContents();
		}

		public static void CropContents()
		{
			instance.text.text = contents;
			List<UILineInfo> list = new List<UILineInfo>();
			for (float preferredHeight = instance.text.preferredHeight; preferredHeight > instance.text.rectTransform.rect.height; preferredHeight = instance.text.preferredHeight)
			{
				instance.text.cachedTextGeneratorForLayout.GetLines(list);
				if (list.Count == 0)
				{
					break;
				}
				if (list.Count == 1)
				{
					contents = string.Empty;
				}
				else
				{
					UILineInfo uILineInfo = list[1];
					if (uILineInfo.startCharIdx > 0)
					{
						string obj = contents;
						UILineInfo uILineInfo2 = list[1];
						contents = obj.Substring(uILineInfo2.startCharIdx);
					}
					else
					{
						int num = contents.IndexOf('\n', 0);
						if (num >= 0)
						{
							contents = contents.Substring(num + 1);
						}
						else
						{
							contents = string.Empty;
						}
					}
				}
				instance.text.text = contents;
			}
		}

		public static void OnReceive(uint clientId, string nick, string msg)
		{
			NetHost netHost = NetGame.instance.FindReadyHost(clientId);
			if (netHost != null && !netHost.mute)
			{
				msg = msg.Replace('<', '〈');
				msg = msg.Replace('>', '〉');
				Print($"<#{HexConverter.ColorToHex(instance.colors[(long)clientId % (long)instance.colors.Length])}>{nick}</color> {msg}");
			}
		}

		public void Send(string msg)
		{
			if (NetGame.isNetStarted)
			{
				NetGame.instance.SendChatMessage(msg);
			}
		}
	}
}
