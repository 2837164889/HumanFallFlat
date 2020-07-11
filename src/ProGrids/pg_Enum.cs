using UnityEngine;

namespace ProGrids
{
	public static class pg_Enum
	{
		public static Vector3 InverseAxisMask(Vector3 v, Axis axis)
		{
			switch (axis)
			{
			case Axis.X:
			case Axis.NegX:
				return Vector3.Scale(v, new Vector3(0f, 1f, 1f));
			case Axis.Y:
			case Axis.NegY:
				return Vector3.Scale(v, new Vector3(1f, 0f, 1f));
			case Axis.Z:
			case Axis.NegZ:
				return Vector3.Scale(v, new Vector3(1f, 1f, 0f));
			default:
				return v;
			}
		}

		public static Vector3 AxisMask(Vector3 v, Axis axis)
		{
			switch (axis)
			{
			case Axis.X:
			case Axis.NegX:
				return Vector3.Scale(v, new Vector3(1f, 0f, 0f));
			case Axis.Y:
			case Axis.NegY:
				return Vector3.Scale(v, new Vector3(0f, 1f, 0f));
			case Axis.Z:
			case Axis.NegZ:
				return Vector3.Scale(v, new Vector3(0f, 0f, 1f));
			default:
				return v;
			}
		}

		public static float SnapUnitValue(SnapUnit su)
		{
			switch (su)
			{
			case SnapUnit.Meter:
				return 1f;
			case SnapUnit.Centimeter:
				return 0.01f;
			case SnapUnit.Millimeter:
				return 0.001f;
			case SnapUnit.Inch:
				return 0.025399987f;
			case SnapUnit.Foot:
				return 0.3048f;
			case SnapUnit.Yard:
				return 1.09361f;
			case SnapUnit.Parsec:
				return 5f;
			default:
				return 1f;
			}
		}
	}
}
