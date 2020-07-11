using System;
using UnityEngine;

namespace HumanAPI
{
	public class WorkshopItemMetadata
	{
		public const string kMetaDataFilename = "metadata.json";

		[NonSerialized]
		public string folder;

		public string type;

		public ulong workshopId;

		public string title = string.Empty;

		public string description = string.Empty;

		public int typeTags = 1;

		public int playerTags = 7;

		public string themeTags;

		public int flags;

		public ulong hash = GenerateHash();

		private bool _isParsed;

		private WorkshopItemType _type;

		public WorkshopItemSource levelType;

		[NonSerialized]
		public byte[] cachedThumbnailBytes;

		[NonSerialized]
		public Texture2D cachedThumbnail;

		[NonSerialized]
		private int thumbnailRefCount = 1;

		public string metaPath => FileTools.Combine(folder, "metadata.json");

		public virtual string thumbPath => FileTools.Combine(folder, "thumbnail.png");

		public bool isLevel => (itemType == WorkshopItemType.Levels) | (itemType == WorkshopItemType.Level);

		public bool isPreset => itemType == WorkshopItemType.RagdollPreset;

		public bool isModel => !isLevel && !isPreset;

		public WorkshopItemType itemType
		{
			get
			{
				Parse();
				return _type;
			}
			set
			{
				_type = value;
				type = value.ToString();
			}
		}

		public Texture2D thumbnailTexture
		{
			get
			{
				if (string.IsNullOrEmpty(folder))
				{
					return null;
				}
				if (cachedThumbnail == null)
				{
					bool isAsset;
					if (cachedThumbnailBytes != null)
					{
						cachedThumbnail = FileTools.TextureFromBytes("thumb", cachedThumbnailBytes);
						isAsset = false;
					}
					else
					{
						cachedThumbnail = FileTools.ReadTexture(thumbPath, out isAsset);
					}
					thumbnailRefCount = (isAsset ? 1 : 0);
				}
				thumbnailRefCount++;
				return cachedThumbnail;
			}
		}

		private static ulong GenerateHash()
		{
			ulong num = 0uL;
			byte[] array = Guid.NewGuid().ToByteArray();
			for (int i = 0; i < array.Length; i += 2)
			{
				num <<= 8;
				num |= array[i];
			}
			return num;
		}

		public void Save(string folder)
		{
			string path = FileTools.Combine(folder, "metadata.json");
			FileTools.WriteJson(path, this);
		}

		public static WorkshopItemMetadata Load(string folder)
		{
			string path = FileTools.Combine(folder, "metadata.json");
			string text = FileTools.ReadAllText(path);
			if (text == null)
			{
				return null;
			}
			try
			{
				WorkshopItemMetadata workshopItemMetadata = JsonUtility.FromJson<WorkshopItemMetadata>(text);
				if (workshopItemMetadata != null)
				{
					switch (workshopItemMetadata.itemType)
					{
					case WorkshopItemType.Levels:
					case WorkshopItemType.Lobbies:
						workshopItemMetadata = JsonUtility.FromJson<WorkshopLevelMetadata>(text);
						if (workshopItemMetadata.typeTags == 2)
						{
							workshopItemMetadata.itemType = WorkshopItemType.Lobbies;
						}
						break;
					case WorkshopItemType.ModelFull:
					case WorkshopItemType.ModelHead:
					case WorkshopItemType.ModelUpperBody:
					case WorkshopItemType.ModelLowerBody:
						workshopItemMetadata = JsonUtility.FromJson<RagdollModelMetadata>(text);
						break;
					case WorkshopItemType.RagdollPreset:
						workshopItemMetadata = JsonUtility.FromJson<RagdollPresetMetadata>(text);
						break;
					}
					if (workshopItemMetadata != null)
					{
						workshopItemMetadata.folder = folder;
						return workshopItemMetadata;
					}
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return null;
		}

		public static T Load<T>(string folder) where T : WorkshopItemMetadata
		{
			return Load(folder) as T;
		}

		private void Parse()
		{
			if (!_isParsed)
			{
				try
				{
					if (string.IsNullOrEmpty(type))
					{
						_type = WorkshopItemType.Level;
					}
					else if (type.Equals("LevelSumo"))
					{
						_type = WorkshopItemType.Levels;
					}
					else
					{
						_type = (WorkshopItemType)Enum.Parse(typeof(WorkshopItemType), type);
					}
					_isParsed = true;
				}
				catch
				{
				}
			}
		}

		public void ReleaseThumbnailReference()
		{
			thumbnailRefCount--;
			if (thumbnailRefCount == 0 && cachedThumbnail != null)
			{
				UnityEngine.Object.Destroy(cachedThumbnail);
				cachedThumbnail = null;
			}
		}

		public void Reload()
		{
			if (cachedThumbnail != null)
			{
				UnityEngine.Object.Destroy(cachedThumbnail);
			}
			cachedThumbnail = null;
			string text = FileTools.ReadAllText(metaPath);
			if (text != null)
			{
				JsonUtility.FromJsonOverwrite(text, this);
			}
		}
	}
}
