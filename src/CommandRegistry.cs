using I2.Loc;
using System;
using System.Collections.Generic;

public class CommandRegistry
{
	public string helpColor = "<#7F7F7F>";

	public Action<string> print;

	public bool helpExpand;

	private Dictionary<string, string> description = new Dictionary<string, string>();

	private Dictionary<string, Action> commands = new Dictionary<string, Action>();

	private Dictionary<string, Action<string>> commandsStr = new Dictionary<string, Action<string>>();

	private static CommandRegistry current;

	private static string currentCommand;

	public CommandRegistry(Action<string> print, bool expand = false)
	{
		this.print = print;
		helpExpand = expand;
	}

	public void RegisterCommand(string command, Action onCommand, string help = null)
	{
		commands[command] = onCommand;
		if (help != null)
		{
			description[command] = help;
		}
	}

	public void RegisterCommand(string command, Action<string> onCommand, string help = null)
	{
		commandsStr[command] = onCommand;
		if (help != null)
		{
			description[command] = help;
		}
	}

	public void UnRegisterCommand(string command, Action onCommand, string help = null)
	{
		commands.Remove(command);
	}

	public void UnRegisterCommand(string command, Action<string> onCommand, string help = null)
	{
		commandsStr.Remove(command);
	}

	internal void Execute(string txt)
	{
		current = this;
		currentCommand = txt;
		if (commands.TryGetValue(txt, out Action value))
		{
			value();
			return;
		}
		if (commandsStr.TryGetValue(txt, out Action<string> value2))
		{
			value2(null);
			return;
		}
		int num = txt.IndexOf(' ');
		if (num > 0 && commandsStr.TryGetValue(txt.Substring(0, num).ToLowerInvariant(), out value2))
		{
			currentCommand = txt.Substring(0, num).ToLowerInvariant();
			value2(txt.Substring(num + 1).Trim());
		}
	}

	private static string Translate(string text)
	{
		if (text.StartsWith("#"))
		{
			return ScriptLocalization.Get(text.Substring(1));
		}
		return text;
	}

	public void OnHelp(string param)
	{
		if (string.IsNullOrEmpty(param))
		{
			foreach (string key in commands.Keys)
			{
				if (description.ContainsKey(key))
				{
					print(helpColor + ((!helpExpand) ? key : Translate(description[key])) + "</color>");
				}
			}
			foreach (string key2 in commandsStr.Keys)
			{
				if (description.ContainsKey(key2))
				{
					print(helpColor + ((!helpExpand) ? key2 : Translate(description[key2])) + "</color>");
				}
			}
			return;
		}
		string[] array = param.ToLowerInvariant().Split(new char[1]
		{
			' '
		}, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length == 1 && description.ContainsKey(array[0]))
		{
			print(helpColor + Translate(description[array[0]]) + "</color>");
		}
	}

	public static void ShowCurrentHelp()
	{
		current.OnHelp(currentCommand);
	}

	public static void Print(string txt)
	{
		current.print(txt);
	}
}
