using Multiplayer;
using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace HumanAPI
{
	public class RagdollPresetMetadata : WorkshopItemMetadata
	{
		public RagdollPresetPartMetadata main;

		public RagdollPresetPartMetadata head;

		public RagdollPresetPartMetadata upperBody;

		public RagdollPresetPartMetadata lowerBody;

		private byte[] md5;

		private byte[] serialized;

		public RagdollPresetPartMetadata GetPart(WorkshopItemType part)
		{
			switch (part)
			{
			case WorkshopItemType.ModelFull:
				return main;
			case WorkshopItemType.ModelHead:
				return head;
			case WorkshopItemType.ModelUpperBody:
				return upperBody;
			case WorkshopItemType.ModelLowerBody:
				return lowerBody;
			default:
				throw new Exception("Invalid part");
			}
		}

		public void SetPart(WorkshopItemType part, RagdollPresetPartMetadata meta)
		{
			switch (part)
			{
			case WorkshopItemType.ModelFull:
				main = meta;
				break;
			case WorkshopItemType.ModelHead:
				head = meta;
				break;
			case WorkshopItemType.ModelUpperBody:
				upperBody = meta;
				break;
			case WorkshopItemType.ModelLowerBody:
				lowerBody = meta;
				break;
			default:
				throw new Exception("Invalid part");
			}
		}

		public void SetColor(WorkshopItemType part, int channel, Color color)
		{
			RagdollPresetPartMetadata part2 = GetPart(part);
			switch (channel)
			{
			case 1:
				part2.color1 = HexConverter.ColorToHex(color);
				break;
			case 2:
				part2.color2 = HexConverter.ColorToHex(color);
				break;
			case 3:
				part2.color3 = HexConverter.ColorToHex(color);
				break;
			}
		}

		public Color GetColor(WorkshopItemType part, int channel)
		{
			RagdollPresetPartMetadata part2 = GetPart(part);
			switch (channel)
			{
			case 1:
				return HexConverter.HexToColor(part2.color1, default(Color));
			case 2:
				return HexConverter.HexToColor(part2.color2, default(Color));
			case 3:
				return HexConverter.HexToColor(part2.color3, default(Color));
			default:
				return default(Color);
			}
		}

		public void ClearColors()
		{
			if (main != null)
			{
				main.color1 = (main.color2 = (main.color3 = null));
			}
			if (head != null)
			{
				head.color1 = (head.color2 = (head.color3 = null));
			}
			if (upperBody != null)
			{
				upperBody.color1 = (upperBody.color2 = (upperBody.color3 = null));
			}
			if (lowerBody != null)
			{
				lowerBody.color1 = (lowerBody.color2 = (lowerBody.color3 = null));
			}
		}

		public byte[] GetSerialized()
		{
			if (serialized == null)
			{
				serialized = Serialize();
			}
			return serialized;
		}

		public byte[] GetCRC()
		{
			if (md5 == null)
			{
				using (MD5 mD = MD5.Create())
				{
					byte[] buffer = GetSerialized();
					md5 = mD.ComputeHash(buffer);
				}
			}
			return md5;
		}

		public static string FormatCRC(byte[] crc)
		{
			if (crc == null)
			{
				return "(null)";
			}
			string text = string.Empty;
			for (int i = 0; i < crc.Length; i++)
			{
				text += crc[i].ToString("X2");
			}
			return text;
		}

		private void SerializePart(WorkshopItemType part, NetStream stream)
		{
			byte[] array = null;
			RagdollPresetPartMetadata part2 = GetPart(part);
			if (part2 != null)
			{
				array = part2.bytes;
				if (array == null && !string.IsNullOrEmpty(folder))
				{
					string path = FileTools.Combine(folder, part.ToString() + ".png");
					array = FileTools.ReadAllBytes(path);
				}
			}
			if (array != null)
			{
				stream.Write(v: true);
				stream.WriteArray(array, 32);
			}
			else
			{
				stream.Write(v: false);
			}
		}

		public byte[] Serialize()
		{
			NetStream netStream = NetStream.AllocStream();
			try
			{
				string text = JsonUtility.ToJson(this);
				netStream.Write(text);
				SerializePart(WorkshopItemType.ModelFull, netStream);
				SerializePart(WorkshopItemType.ModelHead, netStream);
				SerializePart(WorkshopItemType.ModelUpperBody, netStream);
				SerializePart(WorkshopItemType.ModelLowerBody, netStream);
				return netStream.ToArray();
			}
			finally
			{
				if (netStream != null)
				{
					netStream = netStream.Release();
				}
			}
		}

		private void DeserializePart(WorkshopItemType part, NetStream stream)
		{
			if (stream.ReadBool())
			{
				RagdollPresetPartMetadata part2 = GetPart(part);
				byte[] array = part2.bytes = stream.ReadArray(32);
			}
		}

		public static RagdollPresetMetadata Deserialize(byte[] data)
		{
			NetStream netStream = NetStream.AllocStream(data);
			try
			{
				string text = netStream.ReadString();
				RagdollPresetMetadata ragdollPresetMetadata = JsonUtility.FromJson<RagdollPresetMetadata>(text);
				if (ragdollPresetMetadata == null)
				{
					Debug.LogErrorFormat("Unable to deserialize metadata {0}", text);
					return null;
				}
				ragdollPresetMetadata.folder = null;
				ragdollPresetMetadata.serialized = data;
				ragdollPresetMetadata.DeserializePart(WorkshopItemType.ModelFull, netStream);
				ragdollPresetMetadata.DeserializePart(WorkshopItemType.ModelHead, netStream);
				ragdollPresetMetadata.DeserializePart(WorkshopItemType.ModelUpperBody, netStream);
				ragdollPresetMetadata.DeserializePart(WorkshopItemType.ModelLowerBody, netStream);
				return ragdollPresetMetadata;
			}
			finally
			{
				if (netStream != null)
				{
					netStream = netStream.Release();
				}
			}
		}

		public void SaveNetSkin(uint localCoopIndex, string id)
		{
			string path = Path.Combine(Application.persistentDataPath, "net/" + id.ToLower() + localCoopIndex.ToString());
			FileTools.WriteAllBytes(path, GetSerialized());
		}

		public static RagdollPresetMetadata LoadNetSkin(uint localCoopIndex, string id)
		{
			string path = Path.Combine(Application.persistentDataPath, "net/" + id.ToLower() + localCoopIndex.ToString());
			byte[] array = FileTools.ReadAllBytes(path);
			if (array == null)
			{
				return null;
			}
			return Deserialize(array);
		}

		public bool CheckCRC(byte[] skinCRC)
		{
			byte[] cRC = GetCRC();
			if (cRC.Length != skinCRC.Length)
			{
				return false;
			}
			for (int i = 0; i < cRC.Length; i++)
			{
				if (cRC[i] != skinCRC[i])
				{
					return false;
				}
			}
			return true;
		}
	}
}
