using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameCamera : MonoBehaviour
{
	[NonSerialized]
	public Camera gameCam;

	private PostProcessLayer pp_layer;

	private PostProcessLayer.Antialiasing previousSetting;

	private void Awake()
	{
		gameCam = GetComponent<Camera>();
		pp_layer = gameCam.GetComponent<PostProcessLayer>();
		int num = 24;
		float num2 = 60f;
		float[] array = new float[32];
		array[num] = num2;
		gameCam.layerCullDistances = array;
	}

	private void NameTagCallback(bool active)
	{
		if (!(pp_layer == null))
		{
			if (active)
			{
				previousSetting = pp_layer.antialiasingMode;
				pp_layer.antialiasingMode = PostProcessLayer.Antialiasing.None;
			}
			else
			{
				pp_layer.antialiasingMode = previousSetting;
			}
		}
	}

	private void OnDisable()
	{
		NameTag.UnRegisterForNameTagCallbacks(NameTagCallback);
	}

	private void OnEnable()
	{
		NameTag.RegisterForNameTagCallbacks(NameTagCallback);
		if (pp_layer != null && pp_layer.haveBundlesBeenInited)
		{
			pp_layer.ResetHistory();
		}
	}

	private void OnPreCull()
	{
		ApplyCameraOffset();
	}

	public void ApplyCameraOffset()
	{
		if (pp_layer != null)
		{
			pp_layer.projOffset = MenuCameraEffects.instance.cameraCenter;
		}
		if (MenuCameraEffects.instance.cameraCenter != Vector2.zero)
		{
			OffsetCenter(MenuCameraEffects.instance.cameraCenter);
		}
		else
		{
			ResetCenterOffset();
		}
	}

	private void OffsetCenter(Vector2 center)
	{
		gameCam.ResetProjectionMatrix();
		Matrix4x4 projectionMatrix = gameCam.projectionMatrix;
		projectionMatrix[0, 2] = center.x;
		projectionMatrix[1, 2] = center.y;
		gameCam.projectionMatrix = projectionMatrix;
		gameCam.nonJitteredProjectionMatrix = projectionMatrix;
	}

	public void ResetCenterOffset()
	{
		gameCam.ResetProjectionMatrix();
	}
}
