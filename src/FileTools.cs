using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class FileTools
{
	public struct NativeTextureHeader
	{
		public int magicWord;

		public int width;

		public int height;

		public TextureFormat format;

		public int imageSize;

		public const int headerSize = 32;

		public static NativeTextureHeader Init()
		{
			NativeTextureHeader result = default(NativeTextureHeader);
			result.magicWord = 0;
			result.width = 0;
			result.height = 0;
			result.format = TextureFormat.R8;
			result.imageSize = 0;
			return result;
		}
	}

	private static char[] DirectorySeparators = new char[3]
	{
		'/',
		'\\',
		':'
	};

	private const int rawx_magicWord = 1482113362;

	public static string ChangeExtension(string path, string extension)
	{
		int num = path.LastIndexOf('.');
		if (num < 0)
		{
			return path + extension;
		}
		return path.Substring(0, num) + extension;
	}

	public static string ChangeFilename(string path, string filename)
	{
		int num = path.LastIndexOfAny(DirectorySeparators);
		if (num < 0)
		{
			return path + DirectorySeparators[0] + filename;
		}
		return path.Substring(0, num + 1) + filename;
	}

	private static string ChangeDirectory(string path, string directory)
	{
		int num = path.LastIndexOfAny(DirectorySeparators);
		if (num < 0)
		{
			return Combine(directory, path);
		}
		return Combine(directory, path.Substring(num + 1));
	}

	public static string GetTempDirectory()
	{
		try
		{
			for (int i = 0; i < 1000; i++)
			{
				string text = Path.Combine(Application.temporaryCachePath, i.ToString());
				if (!Directory.Exists(text) && !File.Exists(text))
				{
					Directory.CreateDirectory(text);
					return text;
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		throw new InvalidOperationException("Unable to find temp folder");
	}

	public static void DeleteTempDirectory(string path)
	{
		try
		{
			Directory.Delete(path, recursive: true);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public static string GetDirectory(string path)
	{
		int num = path.LastIndexOfAny(DirectorySeparators);
		if (num == path.Length - 1)
		{
			num = path.LastIndexOfAny(DirectorySeparators, num - 1);
		}
		return path.Substring(0, num + 1);
	}

	internal static void Copy(string sourcePath, string targetPath, bool deleteIfMissing, bool flush = true)
	{
		byte[] array = ReadAllBytes(sourcePath);
		if (array != null)
		{
			WriteAllBytes(targetPath, array, flush);
		}
		else if (deleteIfMissing)
		{
			DeleteFile(targetPath, flush);
		}
	}

	public static string Combine(string path, string filename)
	{
		if (path == null)
		{
			return null;
		}
		if (filename == null)
		{
			filename = string.Empty;
		}
		int num = path.LastIndexOfAny(DirectorySeparators);
		if (num == path.Length - 1)
		{
			return path + filename;
		}
		return path + DirectorySeparators[0] + filename;
	}

	public static byte[] ReadAllBytes(string path)
	{
		string[] array = path.Split(':');
		switch (array[0])
		{
		case "pr":
			return ReadPersistentBytes(array[1]);
		case "lvl":
			return ReadLocalBytes(ToLevelPath(array[1]));
		case "mdl":
			return ReadLocalBytes(ToModelPath(array[1]));
		case "ws":
			return ReadLocalBytes(ToWorkshopPath(array[1]));
		case "res":
			return ReadResourceBytes(array[1]);
		default:
			return ReadLocalBytes(path);
		}
	}

	public static string ReadAllText(string path)
	{
		string[] array = path.Split(':');
		switch (array[0])
		{
		case "pr":
			return ReadPersistentText(array[1]);
		case "lvl":
			return ReadLocalText(ToLevelPath(array[1]));
		case "mdl":
			return ReadLocalText(ToModelPath(array[1]));
		case "ws":
			return ReadLocalText(ToWorkshopPath(array[1]));
		case "res":
			return ReadResourceText(array[1]);
		default:
			return ReadLocalText(path);
		}
	}

	public static bool TestExists(string path)
	{
		string[] array = path.Split(':');
		switch (array[0])
		{
		case "pr":
			return TestExistsPersistent(array[1]);
		case "lvl":
			return TestExistsLocal(ToLevelPath(array[1]));
		case "mdl":
			return TestExistsLocal(ToModelPath(array[1]));
		case "ws":
			return TestExistsLocal(ToWorkshopPath(array[1]));
		case "res":
			return TestExistsResource(array[1]);
		default:
			return TestExistsLocal(path);
		}
	}

	private static bool TestExistsPersistent(string path)
	{
		if (!SteamManager.Initialized)
		{
			return false;
		}
		try
		{
			if (SteamRemoteStorage.FileExists(path))
			{
				return true;
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return false;
	}

	private static bool TestExistsLocal(string path)
	{
		try
		{
			if (File.Exists(path))
			{
				return true;
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return false;
	}

	private static bool TestExistsResource(string path)
	{
		TextAsset textAsset = Resources.Load<TextAsset>(path);
		return textAsset != null && (textAsset.bytes != null || textAsset.text != null);
	}

	public static void WriteAllBytes(string path, byte[] bytes, bool flush = true)
	{
		string[] array = path.Split(':');
		switch (array[0])
		{
		case "pr":
			WritePersistentBytes(array[1], bytes, flush);
			break;
		case "lvl":
			WriteLocalBytes(ToLevelPath(array[1]), bytes, flush);
			break;
		case "mdl":
			WriteLocalBytes(ToModelPath(array[1]), bytes, flush);
			break;
		case "ws":
			WriteLocalBytes(ToWorkshopPath(array[1]), bytes, flush);
			break;
		case "res":
			throw new InvalidOperationException("Can't write to resources " + path);
		default:
			WriteLocalBytes(path, bytes, flush);
			break;
		}
	}

	public static void WriteAllText(string path, string text, bool flush = true)
	{
		string[] array = path.Split(':');
		switch (array[0])
		{
		case "pr":
			WritePersistentText(array[1], text, flush);
			break;
		case "lvl":
			WriteLocalText(ToLevelPath(array[1]), text, flush);
			break;
		case "mdl":
			WriteLocalText(ToModelPath(array[1]), text, flush);
			break;
		case "ws":
			WriteLocalText(ToWorkshopPath(array[1]), text, flush);
			break;
		case "res":
			throw new InvalidOperationException("Can't write to resources " + path);
		default:
			WriteLocalText(path, text, flush);
			break;
		}
	}

	public static T ReadJson<T>(string path) where T : class
	{
		string text = ReadAllText(path);
		if (text == null)
		{
			return (T)null;
		}
		try
		{
			return JsonUtility.FromJson<T>(text);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return (T)null;
	}

	public static void WriteJson(string path, object data, bool flush = true)
	{
		WriteAllText(path, JsonUtility.ToJson(data), flush);
	}

	public static void DeleteFile(string path, bool flush = true)
	{
		string[] array = path.Split(':');
		switch (array[0])
		{
		case "pr":
			DeletePersistentFile(array[1], flush);
			break;
		case "lvl":
			DeleteLocalFile(ToLevelPath(array[1]), flush);
			break;
		case "mdl":
			DeleteLocalFile(ToModelPath(array[1]), flush);
			break;
		case "ws":
			DeleteLocalFile(ToWorkshopPath(array[1]), flush);
			break;
		case "res":
			throw new InvalidOperationException("Cant delete from resource " + path);
		default:
			DeleteLocalFile(path, flush);
			break;
		}
	}

	public static AssetBundle LoadBundle(string path)
	{
		string[] array = path.Split(':');
		switch (array[0])
		{
		case "pr":
			throw new InvalidOperationException("Cant load bundle from " + path);
		case "lvl":
			return AssetBundle.LoadFromFile(ToLevelPath(array[1]));
		case "mdl":
			return AssetBundle.LoadFromFile(ToModelPath(array[1]));
		case "ws":
			return AssetBundle.LoadFromFile(ToWorkshopPath(array[1]));
		case "res":
			throw new InvalidOperationException("Cant load bundle from " + path);
		default:
			return AssetBundle.LoadFromFile(path);
		}
	}

	public static Texture2D ReadTexture(string path, out bool isAsset)
	{
		string[] array = path.Split(':');
		switch (array[0])
		{
		case "pr":
		case "lvl":
		case "mdl":
		case "ws":
			isAsset = false;
			return TextureFromBytes(path, ReadAllBytes(path));
		case "res":
			isAsset = true;
			return ReadResourceTexture(array[1]);
		default:
			isAsset = false;
			return TextureFromBytes(path, ReadAllBytes(path));
		}
	}

	public static void WriteTexture(string path, Texture2D texture, bool flush = true)
	{
		byte[] bytes = (texture == null) ? new byte[0] : (path.ToLowerInvariant().EndsWith("rawx") ? NativeTextureEncode(texture) : ((!path.ToLowerInvariant().EndsWith("png")) ? texture.EncodeToJPG() : texture.EncodeToPNG()));
		WriteAllBytes(path, bytes, flush);
	}

	public static string[] ListDirectories(string path, bool ignoreMissingFile = false)
	{
		string[] array = path.Split(':');
		switch (array[0])
		{
		case "pr":
			return ListPersistentDirectories(array[1], path, ignoreMissingFile);
		case "lvl":
			return ListLocalDirectories(ToLevelPath(array[1]), path, ignoreMissingFile);
		case "mdl":
			return ListLocalDirectories(ToModelPath(array[1]), path, ignoreMissingFile);
		case "ws":
			return ListLocalDirectories(ToWorkshopPath(array[1]), path, ignoreMissingFile);
		case "res":
			return ListResourceDirectories(array[1], path, ignoreMissingFile);
		default:
			return ListLocalDirectories(path, path, ignoreMissingFile);
		}
	}

	public static string[] ListFiles(string path)
	{
		string[] array = path.Split(':');
		switch (array[0])
		{
		case "pr":
			return ListPersistentFiles(array[1], path);
		case "lvl":
			return ListLocalFiles(ToLevelPath(array[1]), path);
		case "mdl":
			return ListLocalFiles(ToModelPath(array[1]), path);
		case "ws":
			return ListLocalFiles(ToWorkshopPath(array[1]), path);
		case "res":
			return ListResourceFiles(array[1], path);
		default:
			return ListLocalFiles(path, path);
		}
	}

	public static string ToLocalPath(string path)
	{
		string[] array = path.Split(':');
		switch (array[0])
		{
		case "pr":
			throw new InvalidOperationException("Can't access persistent data");
		case "lvl":
			return ToLevelPath(array[1]);
		case "mdl":
			return ToModelPath(array[1]);
		case "ws":
			return ToWorkshopPath(array[1]);
		case "res":
			throw new InvalidOperationException("Can't access resource data");
		default:
			return path;
		}
	}

	private static string ToAppPersistentPath(string path)
	{
		string persistentDataPath = Application.persistentDataPath;
		return Combine(persistentDataPath, path);
	}

	private static string ToLevelPath(string path)
	{
		string path2 = Path.Combine(Path.Combine(Application.persistentDataPath, "Workshop"), "Levels");
		return Combine(path2, path);
	}

	private static string ToModelPath(string path)
	{
		string path2 = Path.Combine(PlayerPrefs.GetString("WorkshopRoot", Path.Combine(Application.dataPath, "Workshop")), "Models");
		return Combine(path2, path);
	}

	private static string ToWorkshopPath(string path)
	{
		int num = path.IndexOfAny(DirectorySeparators);
		ulong value;
		if (num < 0)
		{
			value = ulong.Parse(path);
		}
		else
		{
			value = ulong.Parse(path.Substring(0, num));
			path = path.Substring(num + 1);
		}
		SteamUGC.GetItemInstallInfo((PublishedFileId_t)value, out ulong _, out string pchFolder, 1024u, out uint _);
		if (!string.IsNullOrEmpty(pchFolder))
		{
			PlayerPrefs.SetString(path, pchFolder);
		}
		else
		{
			pchFolder = PlayerPrefs.GetString(path);
		}
		return Combine(pchFolder, path);
	}

	private static byte[] ReadPersistentBytes(string path)
	{
		if (!SteamManager.Initialized)
		{
			return null;
		}
		try
		{
			if (SteamRemoteStorage.FileExists(path))
			{
				byte[] array = new byte[SteamRemoteStorage.GetFileSize(path)];
				int num = SteamRemoteStorage.FileRead(path, array, array.Length);
				return array;
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return null;
	}

	private static string ReadPersistentText(string path)
	{
		byte[] array = ReadPersistentBytes(path);
		if (array == null)
		{
			return null;
		}
		return Encoding.UTF8.GetString(array, 0, array.Length);
	}

	private static void WritePersistentBytes(string path, byte[] bytes, bool flush = true)
	{
		if (SteamManager.Initialized)
		{
			try
			{
				SteamRemoteStorage.FileWrite(path, bytes, bytes.Length);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
	}

	private static void WritePersistentText(string path, string text, bool flush = true)
	{
		if (text != null)
		{
			byte[] bytes = new byte[Encoding.UTF8.GetByteCount(text)];
			Encoding.UTF8.GetBytes(text, 0, text.Length, bytes, 0);
			WritePersistentBytes(path, bytes, flush);
		}
	}

	private static void DeletePersistentFile(string path, bool flush)
	{
		if (SteamManager.Initialized)
		{
			try
			{
				if (SteamRemoteStorage.FileExists(path))
				{
					SteamRemoteStorage.FileDelete(path);
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
	}

	private static byte[] ReadLocalBytes(string path)
	{
		try
		{
			if (File.Exists(path))
			{
				return File.ReadAllBytes(path);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return null;
	}

	private static string ReadLocalText(string path)
	{
		try
		{
			if (File.Exists(path))
			{
				return File.ReadAllText(path);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return null;
	}

	private static void WriteLocalBytes(string path, byte[] bytes, bool flush = true)
	{
		try
		{
			if (!Directory.Exists(Path.GetDirectoryName(path)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			}
			File.WriteAllBytes(path, bytes);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	private static void WriteLocalText(string path, string text, bool flush = true)
	{
		try
		{
			if (!Directory.Exists(Path.GetDirectoryName(path)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			}
			File.WriteAllText(path, text);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	private static void DeleteLocalFile(string path, bool flush)
	{
		try
		{
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	private static byte[] ReadResourceBytes(string path)
	{
		return Resources.Load<TextAsset>(path).bytes;
	}

	private static string ReadResourceText(string path)
	{
		return Resources.Load<TextAsset>(path).text;
	}

	private static Texture2D ReadResourceTexture(string path)
	{
		return HFFResources.instance.FindTextureResource(path);
	}

	private static int Lazy_ToInt(byte[] buffer, int offset)
	{
		byte b = buffer[offset++];
		byte b2 = buffer[offset++];
		byte b3 = buffer[offset++];
		byte b4 = buffer[offset];
		return b | (b2 << 8) | (b3 << 16) | (b4 << 24);
	}

	private static void Lazy_FromInt(byte[] buffer, int offset, int data)
	{
		buffer[offset++] = (byte)(data & 0xFF);
		buffer[offset++] = (byte)((data >> 8) & 0xFF);
		buffer[offset++] = (byte)((data >> 16) & 0xFF);
		buffer[offset++] = (byte)((data >> 24) & 0xFF);
	}

	public static bool NativeTextureDecodeHeader(out NativeTextureHeader header, byte[] data)
	{
		header = NativeTextureHeader.Init();
		int num = data.Length - 32;
		if (num < 0)
		{
			return false;
		}
		header.magicWord = Lazy_ToInt(data, 0);
		header.width = Lazy_ToInt(data, 4);
		header.height = Lazy_ToInt(data, 8);
		header.format = (TextureFormat)Lazy_ToInt(data, 12);
		header.imageSize = Lazy_ToInt(data, 16);
		if (header.magicWord != 1482113362)
		{
			return false;
		}
		if (header.imageSize > num)
		{
			return false;
		}
		return true;
	}

	public static byte[] NativeTextureEncode(Texture2D tex)
	{
		if (tex == null)
		{
			return null;
		}
		byte[] rawTextureData = tex.GetRawTextureData();
		int num = rawTextureData.Length;
		int num2 = 32;
		byte[] array = new byte[num + num2];
		Lazy_FromInt(array, 0, 1482113362);
		Lazy_FromInt(array, 4, tex.width);
		Lazy_FromInt(array, 8, tex.height);
		Lazy_FromInt(array, 12, (int)tex.format);
		Lazy_FromInt(array, 16, num);
		Array.Copy(rawTextureData, 0, array, num2, num);
		return array;
	}

	public static Texture2D NativeTextureDecode(byte[] data, bool doApply)
	{
		if (!NativeTextureDecodeHeader(out NativeTextureHeader header, data))
		{
			return null;
		}
		Texture2D texture2D = new Texture2D(header.width, header.height, header.format, mipmap: false);
		byte[] array = new byte[header.imageSize];
		Array.Copy(data, 32, array, 0, header.imageSize);
		texture2D.LoadRawTextureData(array);
		if (doApply)
		{
			texture2D.Apply(updateMipmaps: false, makeNoLongerReadable: false);
		}
		return texture2D;
	}

	public static Texture2D TextureFromBytes(string name, byte[] bytes)
	{
		if (bytes == null)
		{
			return null;
		}
		try
		{
			Texture2D texture2D = NativeTextureDecode(bytes, doApply: true);
			if (texture2D != null)
			{
				texture2D.name = name;
				return texture2D;
			}
			texture2D = new Texture2D(1, 1);
			texture2D.name = name;
			texture2D.LoadImage(bytes);
			return texture2D;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return null;
	}

	private static string[] ListResourceDirectories(string path, string replacement, bool ignoreMissingFile)
	{
		throw new NotImplementedException();
	}

	private static string[] ListResourceFiles(string path, string replacement)
	{
		throw new NotImplementedException();
	}

	private static string[] ListLocalDirectories(string path, string replacement, bool ignoreMissingFile)
	{
		try
		{
			string[] directories = Directory.GetDirectories(path);
			for (int i = 0; i < directories.Length; i++)
			{
				directories[i] = ChangeDirectory(directories[i], replacement);
			}
			return directories;
		}
		catch (Exception exception)
		{
			if (!ignoreMissingFile)
			{
				Debug.LogException(exception);
			}
		}
		return new string[0];
	}

	private static string[] ListLocalFiles(string path, string replacement)
	{
		try
		{
			string[] files = Directory.GetFiles(path);
			for (int i = 0; i < files.Length; i++)
			{
				files[i] = ChangeDirectory(files[i], replacement);
			}
			return files;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return new string[0];
	}

	private static string[] ListPersistentDirectories(string path, string replacement, bool ignoreMissingFile)
	{
		if (!SteamManager.Initialized)
		{
			return new string[0];
		}
		try
		{
			List<string> list = new List<string>();
			int fileCount = SteamRemoteStorage.GetFileCount();
			for (int i = 0; i < fileCount; i++)
			{
				string fileNameAndSize = SteamRemoteStorage.GetFileNameAndSize(i, out int _);
				if (fileNameAndSize.StartsWith(path) && fileNameAndSize.IndexOfAny(DirectorySeparators, path.Length) == path.Length)
				{
					fileNameAndSize = fileNameAndSize.Substring(path.Length + 1);
					int num = fileNameAndSize.IndexOfAny(DirectorySeparators);
					if (num >= 0)
					{
						list.Add(Combine("pr:" + path, fileNameAndSize.Substring(0, num)));
					}
				}
			}
			return list.ToArray();
		}
		catch (Exception exception)
		{
			if (!ignoreMissingFile)
			{
				Debug.LogException(exception);
			}
		}
		return new string[0];
	}

	private static string[] ListPersistentFiles(string path, string replacement)
	{
		if (!SteamManager.Initialized)
		{
			return null;
		}
		try
		{
			List<string> list = new List<string>();
			int fileCount = SteamRemoteStorage.GetFileCount();
			for (int i = 0; i < fileCount; i++)
			{
				int pnFileSizeInBytes;
				string fileNameAndSize = SteamRemoteStorage.GetFileNameAndSize(i, out pnFileSizeInBytes);
				if (!string.IsNullOrEmpty(fileNameAndSize) && Path.Combine(path, Path.GetFileName(fileNameAndSize)).Equals(fileNameAndSize, StringComparison.InvariantCultureIgnoreCase))
				{
					list.Add(ChangeDirectory(fileNameAndSize, replacement));
				}
			}
			return list.ToArray();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return new string[0];
	}
}
