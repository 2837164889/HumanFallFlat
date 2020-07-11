using HumanAPI;
using Multiplayer;
using UnityEngine;

public class SkinnedDoll : MonoBehaviour
{
	public Ragdoll ragdoll;

	public Transform skins;

	private RagdollCustomization customization;

	private Joint joint;

	public bool nailed => joint != null;

	private void Start()
	{
		CollisionSensor[] componentsInChildren = GetComponentsInChildren<CollisionSensor>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Object.Destroy(componentsInChildren[i]);
		}
	}

	public void ApplySkin(string skin)
	{
		if (string.IsNullOrEmpty(skin))
		{
			ragdoll.gameObject.SetActive(value: false);
			return;
		}
		Transform transform = skins.Find(skin.ToLowerInvariant());
		if (transform != null)
		{
			ragdoll.gameObject.SetActive(value: true);
			SkinnedDollPreset component = transform.GetComponent<SkinnedDollPreset>();
			RagdollPresetMetadata ragdollPresetMetadata = new RagdollPresetMetadata();
			ragdollPresetMetadata.folder = null;
			ragdollPresetMetadata.itemType = WorkshopItemType.RagdollPreset;
			ragdollPresetMetadata.main = new RagdollPresetPartMetadata
			{
				modelPath = ((!string.IsNullOrEmpty(component.full)) ? ("builtin:" + component.full) : "builtin:HumanDefaultBody")
			};
			ragdollPresetMetadata.head = new RagdollPresetPartMetadata
			{
				modelPath = ((!string.IsNullOrEmpty(component.head)) ? ("builtin:" + component.head) : "builtin:HumanHardHat")
			};
			ragdollPresetMetadata.upperBody = ((!string.IsNullOrEmpty(component.upper)) ? new RagdollPresetPartMetadata
			{
				modelPath = "builtin:" + component.upper
			} : null);
			ragdollPresetMetadata.lowerBody = ((!string.IsNullOrEmpty(component.lower)) ? new RagdollPresetPartMetadata
			{
				modelPath = "builtin:" + component.lower
			} : null);
			RagdollPresetMetadata ragdollPresetMetadata2 = ragdollPresetMetadata;
			if (!string.IsNullOrEmpty(component.full) && string.IsNullOrEmpty(component.head))
			{
				ragdollPresetMetadata2.head = null;
			}
			ApplyPreset(ragdollPresetMetadata2, bake: false);
		}
		else
		{
			ragdoll.gameObject.SetActive(value: false);
		}
	}

	private void ApplyPreset(RagdollPresetMetadata preset, bool bake = true)
	{
		if (customization == null)
		{
			customization = ragdoll.gameObject.AddComponent<RagdollCustomization>();
		}
		customization.ApplyPreset(preset, forceRebuild: true);
		customization.RebindColors(bake, compress: true);
		RigClipVolume[] componentsInChildren = GetComponentsInChildren<RigClipVolume>();
		if (customization.main != null)
		{
			RagdollModelSkinnedMesh[] componentsInChildren2 = customization.main.GetComponentsInChildren<RagdollModelSkinnedMesh>();
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				componentsInChildren2[i].Clip(componentsInChildren);
			}
		}
	}

	public void ReapplySkin()
	{
		ApplyPreset(customization.preset);
	}

	public void Nail(bool nail)
	{
		if (nail)
		{
			GetComponentInChildren<RespawnRoot>(includeInactive: true).Respawn(Vector3.zero);
			if (joint == null)
			{
				joint = ragdoll.partLeftHand.transform.gameObject.AddComponent<FixedJoint>();
			}
		}
		else if (joint != null)
		{
			Object.Destroy(joint);
		}
	}
}
