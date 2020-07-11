using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Image Effects/Other/Antialiasing")]
	public class Antialiasing : MonoBehaviour
	{
		public Shader shaderFXAAIII;

		public AAMode mode = AAMode.FXAA3Console;

		public float edgeThresholdMin = 0.05f;

		public float edgeThreshold = 0.2f;

		public float edgeSharpness = 4f;

		private Material materialFXAAIII;

		public Material CurrentAAMaterial()
		{
			Material material = null;
			AAMode aAMode = mode;
			if (aAMode == AAMode.FXAA3Console)
			{
				return materialFXAAIII;
			}
			return null;
		}

		public bool CheckResources()
		{
			if (materialFXAAIII == null)
			{
				materialFXAAIII = new Material(shaderFXAAIII);
				materialFXAAIII.hideFlags = HideFlags.DontSave;
			}
			return true;
		}

		public void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (CheckResources() && mode == AAMode.FXAA3Console && materialFXAAIII != null)
			{
				materialFXAAIII.SetFloat("_EdgeThresholdMin", edgeThresholdMin);
				materialFXAAIII.SetFloat("_EdgeThreshold", edgeThreshold);
				materialFXAAIII.SetFloat("_EdgeSharpness", edgeSharpness);
				Graphics.Blit(source, destination, materialFXAAIII);
			}
		}
	}
}
