using System;
using System.Collections.Generic;
using UnityEngine;

public static class SurfaceTypes
{
	private static Array s_TypeValues = Enum.GetValues(typeof(SurfaceType));

	private static string[] s_TypeNames = Enum.GetNames(typeof(SurfaceType));

	private static Dictionary<PhysicMaterial, SurfaceType> materialMap = new Dictionary<PhysicMaterial, SurfaceType>();

	public static SurfaceType Resolve(PhysicMaterial physics)
	{
		SurfaceType value = SurfaceType.Stone;
		if (physics == null)
		{
			return value;
		}
		if (materialMap.TryGetValue(physics, out value))
		{
			return value;
		}
		string[] array = s_TypeNames;
		int num = array.Length;
		try
		{
			string text = physics.name;
			if (text.EndsWith(" (Instance)"))
			{
				text = text.Substring(0, text.Length - 11);
			}
			for (int i = 0; i < num; i++)
			{
				if (string.Compare(array[i], text, StringComparison.OrdinalIgnoreCase) == 0)
				{
					value = (SurfaceType)s_TypeValues.GetValue(i);
					materialMap.Add(physics, value);
					return value;
				}
			}
		}
		catch
		{
		}
		Array array2 = s_TypeValues;
		for (int j = 0; j < num; j++)
		{
			if (physics.name.Contains(array[j]))
			{
				value = (SurfaceType)array2.GetValue(j);
				materialMap.Add(physics, value);
				return value;
			}
		}
		Debug.LogError("No suface for physics material " + physics.name);
		return SurfaceType.Unknown;
	}
}
