using System;

namespace HumanAPI
{
	[Serializable]
	public class RagdollPresetPartMetadata
	{
		public string modelPath;

		public string color1;

		public string color2;

		public string color3;

		[NonSerialized]
		public bool suppressCustomTexture;

		[NonSerialized]
		public byte[] bytes;

		public static RagdollPresetPartMetadata Clone(RagdollPresetPartMetadata source)
		{
			if (source == null)
			{
				return null;
			}
			RagdollPresetPartMetadata ragdollPresetPartMetadata = new RagdollPresetPartMetadata();
			ragdollPresetPartMetadata.modelPath = source.modelPath;
			ragdollPresetPartMetadata.color1 = source.color1;
			ragdollPresetPartMetadata.color2 = source.color2;
			ragdollPresetPartMetadata.color3 = source.color3;
			return ragdollPresetPartMetadata;
		}

		public static bool IsEmpty(string modelPath)
		{
			return string.IsNullOrEmpty(modelPath) || "builtin:HumanNoHat".Equals(modelPath) || "builtin:HumanNoLower".Equals(modelPath) || "builtin:HumanNoUpper".Equals(modelPath);
		}

		public static bool IsEmpty(RagdollPresetPartMetadata part)
		{
			return part == null || IsEmpty(part.modelPath);
		}
	}
}
