using System;
using System.Collections.Generic;
using System.IO;

namespace InControl
{
	public struct KeyCombo
	{
		private int includeSize;

		private ulong includeData;

		private int excludeSize;

		private ulong excludeData;

		private static Dictionary<ulong, string> cachedStrings = new Dictionary<ulong, string>();

		[Obsolete("Use KeyCombo.IncludeCount instead.")]
		public int Count => includeSize;

		public int IncludeCount => includeSize;

		public int ExcludeCount => excludeSize;

		public bool IsPressed
		{
			get
			{
				if (includeSize == 0)
				{
					return false;
				}
				bool flag = true;
				for (int i = 0; i < includeSize; i++)
				{
					int includeInt = GetIncludeInt(i);
					flag = (flag && KeyInfo.KeyList[includeInt].IsPressed);
				}
				for (int j = 0; j < excludeSize; j++)
				{
					int excludeInt = GetExcludeInt(j);
					if (KeyInfo.KeyList[excludeInt].IsPressed)
					{
						return false;
					}
				}
				return flag;
			}
		}

		public KeyCombo(params Key[] keys)
		{
			includeData = 0uL;
			includeSize = 0;
			excludeData = 0uL;
			excludeSize = 0;
			for (int i = 0; i < keys.Length; i++)
			{
				AddInclude(keys[i]);
			}
		}

		private void AddIncludeInt(int key)
		{
			if (includeSize != 8)
			{
				includeData |= (ulong)(((long)key & 255L) << includeSize * 8);
				includeSize++;
			}
		}

		private int GetIncludeInt(int index)
		{
			return (int)((includeData >> index * 8) & 0xFF);
		}

		[Obsolete("Use KeyCombo.AddInclude instead.")]
		public void Add(Key key)
		{
			AddInclude(key);
		}

		[Obsolete("Use KeyCombo.GetInclude instead.")]
		public Key Get(int index)
		{
			return GetInclude(index);
		}

		public void AddInclude(Key key)
		{
			AddIncludeInt((int)key);
		}

		public Key GetInclude(int index)
		{
			if (index < 0 || index >= includeSize)
			{
				throw new IndexOutOfRangeException("Index " + index + " is out of the range 0.." + includeSize);
			}
			return (Key)GetIncludeInt(index);
		}

		private void AddExcludeInt(int key)
		{
			if (excludeSize != 8)
			{
				excludeData |= (ulong)(((long)key & 255L) << excludeSize * 8);
				excludeSize++;
			}
		}

		private int GetExcludeInt(int index)
		{
			return (int)((excludeData >> index * 8) & 0xFF);
		}

		public void AddExclude(Key key)
		{
			AddExcludeInt((int)key);
		}

		public Key GetExclude(int index)
		{
			if (index < 0 || index >= excludeSize)
			{
				throw new IndexOutOfRangeException("Index " + index + " is out of the range 0.." + excludeSize);
			}
			return (Key)GetExcludeInt(index);
		}

		public static KeyCombo With(params Key[] keys)
		{
			return new KeyCombo(keys);
		}

		public KeyCombo AndNot(params Key[] keys)
		{
			for (int i = 0; i < keys.Length; i++)
			{
				AddExclude(keys[i]);
			}
			return this;
		}

		public void Clear()
		{
			includeData = 0uL;
			includeSize = 0;
			excludeData = 0uL;
			excludeSize = 0;
		}

		public static KeyCombo Detect(bool modifiersAsKeys)
		{
			KeyCombo result = default(KeyCombo);
			if (modifiersAsKeys)
			{
				for (int i = 5; i < 13; i++)
				{
					if (KeyInfo.KeyList[i].IsPressed)
					{
						result.AddIncludeInt(i);
						return result;
					}
				}
			}
			else
			{
				for (int j = 1; j < 5; j++)
				{
					if (KeyInfo.KeyList[j].IsPressed)
					{
						result.AddIncludeInt(j);
					}
				}
			}
			for (int k = 13; k < KeyInfo.KeyList.Length; k++)
			{
				if (KeyInfo.KeyList[k].IsPressed)
				{
					result.AddIncludeInt(k);
					return result;
				}
			}
			result.Clear();
			return result;
		}

		public override string ToString()
		{
			if (!cachedStrings.TryGetValue(includeData, out string value))
			{
				value = string.Empty;
				for (int i = 0; i < includeSize; i++)
				{
					if (i != 0)
					{
						value += " ";
					}
					int includeInt = GetIncludeInt(i);
					value += KeyInfo.KeyList[includeInt].Name;
				}
			}
			return value;
		}

		public static bool operator ==(KeyCombo a, KeyCombo b)
		{
			return a.includeData == b.includeData && a.excludeData == b.excludeData;
		}

		public static bool operator !=(KeyCombo a, KeyCombo b)
		{
			return a.includeData != b.includeData || a.excludeData != b.excludeData;
		}

		public override bool Equals(object other)
		{
			if (other is KeyCombo)
			{
				KeyCombo keyCombo = (KeyCombo)other;
				return includeData == keyCombo.includeData && excludeData == keyCombo.excludeData;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int num = 17;
			num = num * 31 + includeData.GetHashCode();
			return num * 31 + excludeData.GetHashCode();
		}

		internal void Load(BinaryReader reader, ushort dataFormatVersion)
		{
			switch (dataFormatVersion)
			{
			case 1:
				includeSize = reader.ReadInt32();
				includeData = reader.ReadUInt64();
				break;
			case 2:
				includeSize = reader.ReadInt32();
				includeData = reader.ReadUInt64();
				excludeSize = reader.ReadInt32();
				excludeData = reader.ReadUInt64();
				break;
			default:
				throw new InControlException("Unknown data format version: " + dataFormatVersion);
			}
		}

		internal void Save(BinaryWriter writer)
		{
			writer.Write(includeSize);
			writer.Write(includeData);
			writer.Write(excludeSize);
			writer.Write(excludeData);
		}
	}
}
