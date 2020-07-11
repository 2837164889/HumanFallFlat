using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ProGrids
{
	public static class pg_Util
	{
		private abstract class SnapEnabledOverride
		{
			public abstract bool IsEnabled();
		}

		private class SnapIsEnabledOverride : SnapEnabledOverride
		{
			private bool m_SnapIsEnabled;

			public SnapIsEnabledOverride(bool snapIsEnabled)
			{
				m_SnapIsEnabled = snapIsEnabled;
			}

			public override bool IsEnabled()
			{
				return m_SnapIsEnabled;
			}
		}

		private class ConditionalSnapOverride : SnapEnabledOverride
		{
			public Func<bool> m_IsEnabledDelegate;

			public ConditionalSnapOverride(Func<bool> d)
			{
				m_IsEnabledDelegate = d;
			}

			public override bool IsEnabled()
			{
				return m_IsEnabledDelegate();
			}
		}

		private const float EPSILON = 0.0001f;

		private static Dictionary<Transform, SnapEnabledOverride> m_SnapOverrideCache = new Dictionary<Transform, SnapEnabledOverride>();

		private static Dictionary<Type, bool> m_NoSnapAttributeTypeCache = new Dictionary<Type, bool>();

		private static Dictionary<Type, MethodInfo> m_ConditionalSnapAttributeCache = new Dictionary<Type, MethodInfo>();

		public static Color ColorWithString(string value)
		{
			string valid = "01234567890.,";
			value = new string(value.Where((char c) => valid.Contains(c)).ToArray());
			string[] array = value.Split(',');
			if (array.Length < 4)
			{
				return new Color(1f, 0f, 1f, 1f);
			}
			return new Color(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
		}

		private static Vector3 VectorToMask(Vector3 vec)
		{
			return new Vector3((!(Mathf.Abs(vec.x) > Mathf.Epsilon)) ? 0f : 1f, (!(Mathf.Abs(vec.y) > Mathf.Epsilon)) ? 0f : 1f, (!(Mathf.Abs(vec.z) > Mathf.Epsilon)) ? 0f : 1f);
		}

		private static Axis MaskToAxis(Vector3 vec)
		{
			Axis axis = Axis.None;
			if (Mathf.Abs(vec.x) > 0f)
			{
				axis |= Axis.X;
			}
			if (Mathf.Abs(vec.y) > 0f)
			{
				axis |= Axis.Y;
			}
			if (Mathf.Abs(vec.z) > 0f)
			{
				axis |= Axis.Z;
			}
			return axis;
		}

		private static Axis BestAxis(Vector3 vec)
		{
			float num = Mathf.Abs(vec.x);
			float num2 = Mathf.Abs(vec.y);
			float num3 = Mathf.Abs(vec.z);
			return (num > num2 && num > num3) ? Axis.X : ((!(num2 > num) || !(num2 > num3)) ? Axis.Z : Axis.Y);
		}

		public static Axis CalcDragAxis(Vector3 movement, Camera cam)
		{
			Vector3 vector = VectorToMask(movement);
			if (vector.x + vector.y + vector.z == 2f)
			{
				return MaskToAxis(Vector3.one - vector);
			}
			switch (MaskToAxis(vector))
			{
			case Axis.X:
				if (Mathf.Abs(Vector3.Dot(cam.transform.forward, Vector3.up)) < Mathf.Abs(Vector3.Dot(cam.transform.forward, Vector3.forward)))
				{
					return Axis.Z;
				}
				return Axis.Y;
			case Axis.Y:
				if (Mathf.Abs(Vector3.Dot(cam.transform.forward, Vector3.right)) < Mathf.Abs(Vector3.Dot(cam.transform.forward, Vector3.forward)))
				{
					return Axis.Z;
				}
				return Axis.X;
			case Axis.Z:
				if (Mathf.Abs(Vector3.Dot(cam.transform.forward, Vector3.right)) < Mathf.Abs(Vector3.Dot(cam.transform.forward, Vector3.up)))
				{
					return Axis.Y;
				}
				return Axis.X;
			default:
				return Axis.None;
			}
		}

		public static float ValueFromMask(Vector3 val, Vector3 mask)
		{
			if (Mathf.Abs(mask.x) > 0.0001f)
			{
				return val.x;
			}
			if (Mathf.Abs(mask.y) > 0.0001f)
			{
				return val.y;
			}
			return val.z;
		}

		public static Vector3 SnapValue(Vector3 val, float snapValue)
		{
			float x = val.x;
			float y = val.y;
			float z = val.z;
			return new Vector3(Snap(x, snapValue), Snap(y, snapValue), Snap(z, snapValue));
		}

		private static Type GetType(string type, string assembly = null)
		{
			Type type2 = Type.GetType(type);
			if (type2 == null)
			{
				IEnumerable<Assembly> enumerable = AppDomain.CurrentDomain.GetAssemblies();
				if (assembly != null)
				{
					enumerable = enumerable.Where((Assembly x) => x.FullName.Contains(assembly));
				}
				{
					foreach (Assembly item in enumerable)
					{
						type2 = item.GetType(type);
						if (type2 != null)
						{
							return type2;
						}
					}
					return type2;
				}
			}
			return type2;
		}

		public static void SetUnityGridEnabled(bool isEnabled)
		{
			try
			{
				Type type = GetType("UnityEditor.AnnotationUtility");
				PropertyInfo property = type.GetProperty("showGrid", BindingFlags.Static | BindingFlags.NonPublic);
				property.SetValue(null, isEnabled, BindingFlags.Static | BindingFlags.NonPublic, null, null, null);
			}
			catch
			{
			}
		}

		public static bool GetUnityGridEnabled()
		{
			try
			{
				Type type = GetType("UnityEditor.AnnotationUtility");
				PropertyInfo property = type.GetProperty("showGrid", BindingFlags.Static | BindingFlags.NonPublic);
				return (bool)property.GetValue(null, null);
			}
			catch
			{
			}
			return false;
		}

		public static Vector3 SnapValue(Vector3 val, Vector3 mask, float snapValue)
		{
			float x = val.x;
			float y = val.y;
			float z = val.z;
			return new Vector3((!(Mathf.Abs(mask.x) < 0.0001f)) ? Snap(x, snapValue) : x, (!(Mathf.Abs(mask.y) < 0.0001f)) ? Snap(y, snapValue) : y, (!(Mathf.Abs(mask.z) < 0.0001f)) ? Snap(z, snapValue) : z);
		}

		public static Vector3 SnapToCeil(Vector3 val, Vector3 mask, float snapValue)
		{
			float x = val.x;
			float y = val.y;
			float z = val.z;
			return new Vector3((!(Mathf.Abs(mask.x) < 0.0001f)) ? SnapToCeil(x, snapValue) : x, (!(Mathf.Abs(mask.y) < 0.0001f)) ? SnapToCeil(y, snapValue) : y, (!(Mathf.Abs(mask.z) < 0.0001f)) ? SnapToCeil(z, snapValue) : z);
		}

		public static Vector3 SnapToFloor(Vector3 val, float snapValue)
		{
			float x = val.x;
			float y = val.y;
			float z = val.z;
			return new Vector3(SnapToFloor(x, snapValue), SnapToFloor(y, snapValue), SnapToFloor(z, snapValue));
		}

		public static Vector3 SnapToFloor(Vector3 val, Vector3 mask, float snapValue)
		{
			float x = val.x;
			float y = val.y;
			float z = val.z;
			return new Vector3((!(Mathf.Abs(mask.x) < 0.0001f)) ? SnapToFloor(x, snapValue) : x, (!(Mathf.Abs(mask.y) < 0.0001f)) ? SnapToFloor(y, snapValue) : y, (!(Mathf.Abs(mask.z) < 0.0001f)) ? SnapToFloor(z, snapValue) : z);
		}

		public static float Snap(float val, float round)
		{
			return round * Mathf.Round(val / round);
		}

		public static float SnapToFloor(float val, float snapValue)
		{
			return snapValue * Mathf.Floor(val / snapValue);
		}

		public static float SnapToCeil(float val, float snapValue)
		{
			return snapValue * Mathf.Ceil(val / snapValue);
		}

		public static Vector3 CeilFloor(Vector3 v)
		{
			v.x = ((!(v.x < 0f)) ? 1 : (-1));
			v.y = ((!(v.y < 0f)) ? 1 : (-1));
			v.z = ((!(v.z < 0f)) ? 1 : (-1));
			return v;
		}

		public static void ClearSnapEnabledCache()
		{
			m_SnapOverrideCache.Clear();
		}

		public static bool SnapIsEnabled(Transform t)
		{
			if (m_SnapOverrideCache.TryGetValue(t, out SnapEnabledOverride value))
			{
				return value.IsEnabled();
			}
			object[] source = null;
			MonoBehaviour[] components = t.GetComponents<MonoBehaviour>();
			foreach (Component c in components)
			{
				if (c == null)
				{
					continue;
				}
				Type type = c.GetType();
				if (m_NoSnapAttributeTypeCache.TryGetValue(type, out bool value2))
				{
					if (value2)
					{
						m_SnapOverrideCache.Add(t, new SnapIsEnabledOverride(!value2));
						return true;
					}
				}
				else
				{
					source = type.GetCustomAttributes(inherit: true);
					value2 = source.Any((object x) => x?.ToString().Contains("ProGridsNoSnap") ?? false);
					m_NoSnapAttributeTypeCache.Add(type, value2);
					if (value2)
					{
						m_SnapOverrideCache.Add(t, new SnapIsEnabledOverride(!value2));
						return true;
					}
				}
				if (m_ConditionalSnapAttributeCache.TryGetValue(type, out MethodInfo mi))
				{
					if (mi != null)
					{
						m_SnapOverrideCache.Add(t, new ConditionalSnapOverride(() => (bool)mi.Invoke(c, null)));
						return (bool)mi.Invoke(c, null);
					}
				}
				else if (source.Any((object x) => x?.ToString().Contains("ProGridsConditionalSnap") ?? false))
				{
					mi = type.GetMethod("IsSnapEnabled", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
					m_ConditionalSnapAttributeCache.Add(type, mi);
					if (mi != null)
					{
						m_SnapOverrideCache.Add(t, new ConditionalSnapOverride(() => (bool)mi.Invoke(c, null)));
						return (bool)mi.Invoke(c, null);
					}
				}
				else
				{
					m_ConditionalSnapAttributeCache.Add(type, null);
				}
			}
			m_SnapOverrideCache.Add(t, new SnapIsEnabledOverride(snapIsEnabled: true));
			return true;
		}
	}
}
