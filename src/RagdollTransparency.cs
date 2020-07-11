using System.Collections.Generic;
using UnityEngine;

public class RagdollTransparency : MonoBehaviour
{
	private CameraController3 cameraController;

	public List<Material> materials = new List<Material>();

	private float cutoutTimer;

	private bool replaced;

	public void Initialize(CameraController3 cameraController, Ragdoll ragdoll)
	{
		this.cameraController = cameraController;
		ClearMaterials();
		Dictionary<Material, Material> dictionary = new Dictionary<Material, Material>();
		Renderer[] componentsInChildren = ragdoll.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Material[] sharedMaterials = componentsInChildren[i].sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				if ("Standard".Equals(sharedMaterials[j].shader.name) && sharedMaterials[j].GetFloat("_Mode") != 3f)
				{
					Material value = null;
					if (!dictionary.TryGetValue(sharedMaterials[j], out value))
					{
						value = new Material(sharedMaterials[j]);
						dictionary[sharedMaterials[j]] = value;
						materials.Add(value);
					}
					sharedMaterials[j] = value;
				}
			}
			componentsInChildren[i].sharedMaterials = sharedMaterials;
		}
	}

	private void ClearMaterials()
	{
		for (int i = 0; i < materials.Count; i++)
		{
			Object.Destroy(materials[i]);
		}
		materials.Clear();
	}

	private void OnDestroy()
	{
		ClearMaterials();
	}

	public void OnPreRender()
	{
		if (cameraController != null && (cameraController.offset < 0.8f || cutoutTimer > 0f))
		{
			replaced = true;
			if (cameraController.offset < 0.8f)
			{
				cutoutTimer = Mathf.Clamp01(cutoutTimer + Time.deltaTime);
			}
			else
			{
				cutoutTimer = Mathf.Clamp01(cutoutTimer - Time.deltaTime);
			}
			float value = (cameraController.mode != CameraMode.FirstPerson) ? (cutoutTimer * 1.25f - 0.25f) : 1.2f;
			for (int i = 0; i < materials.Count; i++)
			{
				if (materials[i].GetTexture("_MetallicGlossMap") != null)
				{
					materials[i].shader = Shaders.instance.transparentHumanShaderMetal;
				}
				else
				{
					materials[i].shader = Shaders.instance.transparentHumanShader;
				}
				materials[i].SetFloat("_ClipDist", value);
			}
		}
		else
		{
			replaced = false;
		}
	}

	public void OnPostRender()
	{
		if (replaced)
		{
			for (int i = 0; i < materials.Count; i++)
			{
				materials[i].shader = Shaders.instance.opaqueHumanShader;
			}
		}
	}

	private void SetTransparentBlendMode(Material material)
	{
		material.SetOverrideTag("RenderType", "Transparent");
		material.SetInt("_SrcBlend", 1);
		material.SetInt("_DstBlend", 10);
		material.SetInt("_ZWrite", 0);
		material.DisableKeyword("_ALPHATEST_ON");
		material.DisableKeyword("_ALPHABLEND_ON");
		material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
		material.renderQueue = 3000;
	}
}
