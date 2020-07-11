using UnityEngine;

namespace InControl
{
	public class UnityGyroAxisSource : InputControlSource
	{
		public enum GyroAxis
		{
			X,
			Y
		}

		private static Quaternion zeroAttitude;

		public int Axis;

		public UnityGyroAxisSource()
		{
			Calibrate();
		}

		public UnityGyroAxisSource(GyroAxis axis)
		{
			Axis = (int)axis;
			Calibrate();
		}

		public float GetValue(InputDevice inputDevice)
		{
			return GetAxis()[Axis];
		}

		public bool GetState(InputDevice inputDevice)
		{
			return Utility.IsNotZero(GetValue(inputDevice));
		}

		private static Quaternion GetAttitude()
		{
			return Quaternion.Inverse(zeroAttitude) * Input.gyro.attitude;
		}

		private static Vector3 GetAxis()
		{
			Vector3 vector = GetAttitude() * Vector3.forward;
			float x = ApplyDeadZone(Mathf.Clamp(vector.x, -1f, 1f));
			float y = ApplyDeadZone(Mathf.Clamp(vector.y, -1f, 1f));
			return new Vector3(x, y);
		}

		private static float ApplyDeadZone(float value)
		{
			return Mathf.InverseLerp(0.05f, 1f, Utility.Abs(value)) * Mathf.Sign(value);
		}

		public static void Calibrate()
		{
			zeroAttitude = Input.gyro.attitude;
		}
	}
}
