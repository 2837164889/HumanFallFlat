using UnityEngine;

namespace HumanAPI
{
	[AddComponentMenu("Human/Lerp Emission", 10)]
	public class LerpEmission : LerpBase
	{
		[ColorUsage(false, true, 0f, 8f, 0.125f, 3f)]
		[Tooltip("Default Colour to Lerp from")]
		public Color from = Color.black;

		[ColorUsage(false, true, 0f, 8f, 0.125f, 3f)]
		[Tooltip("Colour to Lerp to")]
		public Color to = Color.white;

		[Tooltip("Element number to apply the lerp to")]
		public int elementNumber;

		[Tooltip("Whether or not to use the element number")]
		public bool useMaterialElementNumber;

		[Tooltip("Use this in order to show the prints coming from the script")]
		public bool showDebug;

		private MeshRenderer meshRenderer;

		protected override void Awake()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Awake ");
			}
			meshRenderer = GetComponent<MeshRenderer>();
			base.Awake();
		}

		protected override void ApplyValue(float value)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Applying value ");
			}
			Color color = Color.LerpUnclamped(from, to, value);
			if (useMaterialElementNumber)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Using the Element value " + elementNumber);
				}
				Material[] materials = meshRenderer.materials;
				Material material = materials[elementNumber];
				material.SetColor("_EmissionColor", color);
				material.EnableKeyword("_EMISSION");
				meshRenderer.materials = materials;
			}
			else
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Doing the normal stuff - not using element number ");
				}
				meshRenderer.material.SetColor("_EmissionColor", color);
				meshRenderer.material.EnableKeyword("_EMISSION");
			}
			DynamicGI.SetEmissive(meshRenderer, color);
		}
	}
}
