using System;
using UnityEngine;

namespace HumanAPI
{
	[Serializable]
	[CreateAssetMenu(fileName = "RagdollModel", menuName = "RagdollModel")]
	public class WorkshopRagdollModel : ScriptableObject
	{
		public ulong workshopId;

		public RagdollModel model;

		public Texture2D thumbnail;

		public string title;

		[Multiline]
		public string description;

		[Multiline]
		public string updateNotes;

		public WorkshopItemMetadata meta
		{
			get
			{
				RagdollModelMetadata ragdollModelMetadata = new RagdollModelMetadata();
				ragdollModelMetadata.folder = "builtin:" + base.name;
				ragdollModelMetadata.itemType = model.ragdollPart;
				ragdollModelMetadata.title = title;
				ragdollModelMetadata.description = description;
				ragdollModelMetadata.cachedThumbnail = thumbnail;
				ragdollModelMetadata.modelPrefab = model;
				return ragdollModelMetadata;
			}
		}
	}
}
