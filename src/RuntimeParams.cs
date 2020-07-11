using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class RuntimeParams
{
	private static Dictionary<string, string> m_Params;

	public const string ResourceName = "Curve/AssetHashes";

	public const string EditorFilename = "Assets/Resources/Curve/AssetHashes.bytes";

	private static void InternalInit()
	{
		m_Params = new Dictionary<string, string>();
		Load();
	}

	public static void Init()
	{
		if (m_Params == null)
		{
			InternalInit();
		}
	}

	public static bool CheckForKey(string key)
	{
		if (m_Params == null)
		{
			InternalInit();
		}
		return m_Params.ContainsKey(key);
	}

	public static string GetString(string key, string defaultValue = null)
	{
		if (m_Params == null)
		{
			InternalInit();
		}
		if (!m_Params.TryGetValue(key, out string value))
		{
			return defaultValue;
		}
		return value;
	}

	public static int GetInt(string key, int defaultValue = 0)
	{
		string @string = GetString(key);
		if (@string == null)
		{
			return defaultValue;
		}
		if (!int.TryParse(@string, out int result))
		{
			return defaultValue;
		}
		return result;
	}

	public static uint GetUInt(string key, uint defaultValue = 0u)
	{
		string @string = GetString(key);
		if (@string == null)
		{
			return defaultValue;
		}
		if (!uint.TryParse(@string, out uint result))
		{
			return defaultValue;
		}
		return result;
	}

	public static bool GetBool(string key, bool defaultValue = false)
	{
		return GetInt(key, defaultValue ? 1 : 0) != 0;
	}

	public static void Load()
	{
		try
		{
			TextAsset textAsset = Resources.Load<TextAsset>("Curve/AssetHashes");
			if (textAsset != null)
			{
				byte[] bytes = textAsset.bytes;
				byte[] array = new byte[bytes.Length];
				int i = 0;
				for (int num = array.Length; i < num; i++)
				{
					array[i] = (byte)((bytes[i] ^ (205 + i * 47)) & 0xFF);
				}
				using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(array)))
				{
					int num2 = binaryReader.ReadInt32();
					while (num2-- > 0)
					{
						string key = binaryReader.ReadString();
						string value = binaryReader.ReadString();
						m_Params[key] = value;
					}
				}
				Resources.UnloadAsset(textAsset);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}
}
