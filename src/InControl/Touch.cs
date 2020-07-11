using UnityEngine;

namespace InControl
{
	public class Touch
	{
		public static readonly int FingerID_None = -1;

		public static readonly int FingerID_Mouse = -2;

		public int fingerId;

		public TouchPhase phase;

		public int tapCount;

		public Vector2 position;

		public Vector2 deltaPosition;

		public Vector2 lastPosition;

		public float deltaTime;

		public ulong updateTick;

		public TouchType type;

		public float altitudeAngle;

		public float azimuthAngle;

		public float maximumPossiblePressure;

		public float pressure;

		public float radius;

		public float radiusVariance;

		public float normalizedPressure => Mathf.Clamp(pressure / maximumPossiblePressure, 0.001f, 1f);

		internal Touch()
		{
			fingerId = FingerID_None;
			phase = TouchPhase.Ended;
		}

		internal void Reset()
		{
			fingerId = FingerID_None;
			phase = TouchPhase.Ended;
			tapCount = 0;
			position = Vector2.zero;
			deltaPosition = Vector2.zero;
			lastPosition = Vector2.zero;
			deltaTime = 0f;
			updateTick = 0uL;
			type = TouchType.Direct;
			altitudeAngle = 0f;
			azimuthAngle = 0f;
			maximumPossiblePressure = 1f;
			pressure = 0f;
			radius = 0f;
			radiusVariance = 0f;
		}

		internal void SetWithTouchData(UnityEngine.Touch touch, ulong updateTick, float deltaTime)
		{
			phase = touch.phase;
			tapCount = touch.tapCount;
			altitudeAngle = touch.altitudeAngle;
			azimuthAngle = touch.azimuthAngle;
			maximumPossiblePressure = touch.maximumPossiblePressure;
			pressure = touch.pressure;
			radius = touch.radius;
			radiusVariance = touch.radiusVariance;
			Vector2 a = touch.position;
			if (a.x < 0f)
			{
				a.x = (float)Screen.width + a.x;
			}
			if (phase == TouchPhase.Began)
			{
				deltaPosition = Vector2.zero;
				lastPosition = a;
				position = a;
			}
			else
			{
				if (phase == TouchPhase.Stationary)
				{
					phase = TouchPhase.Moved;
				}
				deltaPosition = a - lastPosition;
				lastPosition = position;
				position = a;
			}
			this.deltaTime = deltaTime;
			this.updateTick = updateTick;
		}

		internal bool SetWithMouseData(ulong updateTick, float deltaTime)
		{
			if (Input.touchCount > 0)
			{
				return false;
			}
			Vector3 mousePosition = Input.mousePosition;
			float x = Mathf.Round(mousePosition.x);
			Vector3 mousePosition2 = Input.mousePosition;
			Vector2 a = new Vector2(x, Mathf.Round(mousePosition2.y));
			if (Input.GetMouseButtonDown(0))
			{
				phase = TouchPhase.Began;
				pressure = 1f;
				maximumPossiblePressure = 1f;
				tapCount = 1;
				type = TouchType.Mouse;
				deltaPosition = Vector2.zero;
				lastPosition = a;
				position = a;
				this.deltaTime = deltaTime;
				this.updateTick = updateTick;
				return true;
			}
			if (Input.GetMouseButtonUp(0))
			{
				phase = TouchPhase.Ended;
				pressure = 0f;
				maximumPossiblePressure = 1f;
				tapCount = 1;
				type = TouchType.Mouse;
				deltaPosition = a - lastPosition;
				lastPosition = position;
				position = a;
				this.deltaTime = deltaTime;
				this.updateTick = updateTick;
				return true;
			}
			if (Input.GetMouseButton(0))
			{
				phase = TouchPhase.Moved;
				pressure = 1f;
				maximumPossiblePressure = 1f;
				tapCount = 1;
				type = TouchType.Mouse;
				deltaPosition = a - lastPosition;
				lastPosition = position;
				position = a;
				this.deltaTime = deltaTime;
				this.updateTick = updateTick;
				return true;
			}
			return false;
		}
	}
}
