using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public sealed class GameProgress : MonoBehaviour
{
	public sealed class ProgressData
	{
		private Dictionary<ulong, uint> store = new Dictionary<ulong, uint>();

		public void Add(ulong hash, uint checkpoint)
		{
			if (store.ContainsKey(hash))
			{
				store[hash] = checkpoint;
			}
			else
			{
				store.Add(hash, checkpoint);
			}
		}

		public bool UpdateValue(ulong hash, uint checkpoint)
		{
			if (store.TryGetValue(hash, out uint value) && checkpoint <= value)
			{
				return false;
			}
			store[hash] = checkpoint;
			return true;
		}

		public void ClearValue(ulong hash)
		{
			store.Remove(hash);
		}

		public int Get(ulong hash)
		{
			if (store.TryGetValue(hash, out uint value))
			{
				return (int)value;
			}
			return -1;
		}

		public byte[] ToBytes()
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(65536);
			int count = store.Count;
			binaryWriter.Write(count);
			foreach (KeyValuePair<ulong, uint> item in store)
			{
				binaryWriter.Write(item.Key);
				binaryWriter.Write(item.Value);
			}
			byte[] buffer = memoryStream.GetBuffer();
			binaryWriter.Close();
			memoryStream.Close();
			return buffer;
		}
	}

	public const int kNoProgress = -1;

	private const string kWorkshopSaveFileName = "Progress.bin";

	private const int kSaveVersion = 65536;

	private string saveFilename;

	private ProgressData customLevelsProgressData;

	public GameProgress()
	{
		ReadProgress();
		GameSave.GetWorkshopCheckpoint(out ulong workshopLevel, out int checkpoint);
		if (workshopLevel != 0)
		{
			GameSave.ClearWorkshopCheckpoint();
			CustomLevelProgress(workshopLevel, checkpoint);
		}
	}

	private void ReadProgress()
	{
		customLevelsProgressData = new ProgressData();
		ReadProgressFile("Progress.bin", customLevelsProgressData);
	}

	private void ReadProgressFile(string fileName, ProgressData progressData)
	{
		if (SteamRemoteStorage.FileExists(fileName))
		{
			try
			{
				byte[] array = new byte[SteamRemoteStorage.GetFileSize(fileName)];
				int num = SteamRemoteStorage.FileRead(fileName, array, array.Length);
				MemoryStream memoryStream = new MemoryStream(array);
				BinaryReader binaryReader = new BinaryReader(memoryStream);
				int num2 = binaryReader.ReadInt32();
				int num3 = binaryReader.ReadInt32();
				List<ulong> list = new List<ulong>();
				List<uint> list2 = new List<uint>();
				for (int i = 0; i < num3; i++)
				{
					list.Add(binaryReader.ReadUInt64());
					list2.Add(binaryReader.ReadUInt32());
				}
				for (int j = 0; j < list.Count; j++)
				{
					progressData.Add(list[j], list2[j]);
				}
				binaryReader.Close();
				memoryStream.Close();
			}
			catch (Exception ex)
			{
				Debug.LogError("Error loading " + fileName + ": " + ex);
			}
		}
	}

	private void WriteProgress()
	{
		byte[] array = customLevelsProgressData.ToBytes();
		SteamRemoteStorage.FileWrite("Progress.bin", array, array.Length);
	}

	public void CustomLevelProgress(ulong hash, int checkpoint)
	{
		if (checkpoint == -1)
		{
			customLevelsProgressData.ClearValue(hash);
			WriteProgress();
		}
		else if (customLevelsProgressData.UpdateValue(hash, (uint)checkpoint))
		{
			WriteProgress();
		}
	}

	public int GetProgressWorkshop(ulong hash)
	{
		return customLevelsProgressData.Get(hash);
	}
}
