using HumanAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintTool : MonoBehaviour
{
	public Color color = Color.white;

	public float cursorKernel = 10f;

	public float cursorFalloff = 10f;

	public const int layer = 31;

	public const int layerMask = int.MinValue;

	public bool paintBackface;

	public bool hasChanges;

	private Dictionary<WorkshopItemType, int> mask;

	private List<RagdollModel> parts = new List<RagdollModel>();

	private Camera unprojectCamera;

	public Material unprojectMaterial;

	private Camera myCam;

	private RagdollTexture texture;

	private RagdollCustomization customization;

	public bool inStroke;

	public void Initialize(RagdollCustomization customization)
	{
		this.customization = customization;
		unprojectMaterial = new Material(Shaders.instance.paintShader);
		unprojectCamera = CustomizationController.instance.mainCamera;
		GameObject gameObject = new GameObject();
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
		myCam = gameObject.AddComponent<Camera>();
		myCam.enabled = false;
		myCam.clearFlags = CameraClearFlags.Nothing;
		myCam.cullingMask = int.MinValue;
		myCam.renderingPath = RenderingPath.Forward;
		mask = new Dictionary<WorkshopItemType, int>
		{
			{
				WorkshopItemType.ModelFull,
				7
			},
			{
				WorkshopItemType.ModelHead,
				7
			},
			{
				WorkshopItemType.ModelUpperBody,
				7
			},
			{
				WorkshopItemType.ModelLowerBody,
				7
			}
		};
		parts.Clear();
		if (customization.main != null && customization.main.texture.paintingEnabled)
		{
			parts.Add(customization.main);
		}
		if (customization.head != null && customization.head.texture.paintingEnabled)
		{
			parts.Add(customization.head);
		}
		if (customization.upper != null && customization.upper.texture.paintingEnabled)
		{
			parts.Add(customization.upper);
		}
		if (customization.lower != null && customization.lower.texture.paintingEnabled)
		{
			parts.Add(customization.lower);
		}
		for (int i = 0; i < parts.Count; i++)
		{
			parts[i].texture.EnterPaint();
		}
		StartCoroutine(EnsurePaddedMeshes());
	}

	public void SetMask(WorkshopItemType part, int number, bool on)
	{
		int num = 1 << number - 1;
		mask[part] = ((mask[part] & ~num) | (on ? num : 0));
		RagdollModel model = customization.GetModel(part);
		if (model != null)
		{
			model.SetMask(mask[part]);
		}
	}

	public int GetMask(WorkshopItemType part)
	{
		return mask[part];
	}

	public bool GetMask(WorkshopItemType part, int number)
	{
		int num = 1 << number - 1;
		return (mask[part] & num) != 0;
	}

	private IEnumerator EnsurePaddedMeshes()
	{
		yield return new WaitForSeconds(0.3f);
		for (int i = 0; i < parts.Count; i++)
		{
			yield return null;
			RagdollPaddedModel pad = parts[i].padded;
		}
	}

	public void Commit()
	{
		for (int i = 0; i < parts.Count; i++)
		{
			parts[i].texture.SaveToPreset();
		}
		hasChanges = false;
	}

	public void Teardown()
	{
		Object.Destroy(myCam.gameObject);
		for (int i = 0; i < parts.Count; i++)
		{
			parts[i].texture.LeavePaint();
		}
		hasChanges = false;
	}

	public void Paint()
	{
		unprojectMaterial.color = color;
		Material material = unprojectMaterial;
		Vector3 mousePosition = Input.mousePosition;
		float x = mousePosition.x;
		Vector3 mousePosition2 = Input.mousePosition;
		material.SetVector("_PointPos", new Vector4(x, mousePosition2.y, 0f, 0f));
		unprojectMaterial.SetFloat("_PointSize", cursorKernel);
		unprojectMaterial.SetFloat("_FalloffSize", cursorFalloff);
		unprojectMaterial.SetVector("_ScreenSize", new Vector4(Screen.width, Screen.height, 0f, 0f));
		unprojectMaterial.SetFloat("_PaintBackface", paintBackface ? 1 : 0);
		Render();
	}

	public void Project(Texture texture, float mirror)
	{
		unprojectMaterial.SetTexture("_ProjectTex", texture);
		unprojectMaterial.SetFloat("_MirrorMultiplier", mirror * (float)Screen.width / (float)Screen.height * (float)texture.height / (float)texture.width);
		unprojectMaterial.SetFloat("_PaintBackface", paintBackface ? 1 : 0);
		Render();
	}

	private void Render()
	{
		hasChanges = true;
		if (!inStroke)
		{
			for (int i = 0; i < parts.Count; i++)
			{
				parts[i].texture.BeginStroke();
			}
			inStroke = true;
		}
		myCam.transform.position = unprojectCamera.transform.position;
		myCam.transform.rotation = unprojectCamera.transform.rotation;
		myCam.nearClipPlane = unprojectCamera.nearClipPlane;
		myCam.farClipPlane = unprojectCamera.farClipPlane;
		myCam.fieldOfView = unprojectCamera.fieldOfView;
		myCam.aspect = unprojectCamera.aspect;
		Matrix4x4 projectionMatrix = unprojectCamera.projectionMatrix;
		projectionMatrix[0, 2] = MenuCameraEffects.instance.cameraCenter.x;
		projectionMatrix[1, 2] = MenuCameraEffects.instance.cameraCenter.y;
		myCam.projectionMatrix = projectionMatrix;
		for (int j = 0; j < parts.Count; j++)
		{
			RagdollModel ragdollModel = parts[j];
			ragdollModel.texture.PaintStep(myCam, unprojectMaterial, mask[ragdollModel.ragdollPart]);
			ragdollModel.padded.Enable(enable: true);
			ragdollModel.padded.SetMaterial(unprojectMaterial);
			myCam.Render();
			myCam.targetTexture = null;
			ragdollModel.padded.Enable(enable: false);
		}
	}

	public void CancelStroke()
	{
		if (inStroke)
		{
			inStroke = false;
			for (int i = 0; i < parts.Count; i++)
			{
				parts[i].texture.CancelStroke();
			}
		}
	}

	public void EndStroke()
	{
		if (inStroke)
		{
			inStroke = false;
			for (int i = 0; i < parts.Count; i++)
			{
				parts[i].texture.EndStroke();
			}
		}
	}

	public void Undo()
	{
		for (int i = 0; i < parts.Count; i++)
		{
			parts[i].texture.Undo();
		}
		hasChanges = true;
	}

	public void Redo()
	{
		for (int i = 0; i < parts.Count; i++)
		{
			parts[i].texture.Redo();
		}
		hasChanges = true;
	}

	public void BeginStream()
	{
		unprojectMaterial.shader = Shaders.instance.webcamShader;
	}

	public void StopStream()
	{
		unprojectMaterial.shader = Shaders.instance.paintShader;
	}
}
