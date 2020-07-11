using System;

namespace HumanAPI
{
	public class RagdollModelMetadata : WorkshopItemMetadata
	{
		public string model;

		[NonSerialized]
		public RagdollModel modelPrefab;
	}
}
