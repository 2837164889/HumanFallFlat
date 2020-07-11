using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	public sealed class PropertySheetFactory
	{
		private readonly Dictionary<Shader, PropertySheet> m_Sheets;

		public PropertySheetFactory()
		{
			m_Sheets = new Dictionary<Shader, PropertySheet>();
		}

		public PropertySheet Get(string shaderName)
		{
			Shader shader = Shader.Find(shaderName);
			if (shader == null)
			{
				throw new ArgumentException($"Invalid shader ({shaderName})");
			}
			return Get(shader);
		}

		public PropertySheet Get(Shader shader)
		{
			if (shader == null)
			{
				throw new ArgumentException($"Invalid shader ({shader})");
			}
			if (m_Sheets.TryGetValue(shader, out PropertySheet value))
			{
				return value;
			}
			string name = shader.name;
			Material material = new Material(shader);
			material.name = $"PostProcess - {name.Substring(name.LastIndexOf('/') + 1)}";
			material.hideFlags = HideFlags.DontSave;
			Material material2 = material;
			value = new PropertySheet(material2);
			m_Sheets.Add(shader, value);
			return value;
		}

		public void Release()
		{
			foreach (PropertySheet value in m_Sheets.Values)
			{
				value.Release();
			}
			m_Sheets.Clear();
		}
	}
}
