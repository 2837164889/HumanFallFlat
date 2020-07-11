using UnityEngine;

namespace HumanAPI
{
	public class SteamGauge : Node
	{
		[Tooltip("Input: Source signal")]
		public NodeInput input;

		[SerializeField]
		private GameObject pin;

		[Range(0f, 100f)]
		[SerializeField]
		private float pressure;

		[SerializeField]
		private float angleMin;

		[SerializeField]
		private float angleMax;

		private Vector3 startRotation;

		private float storedAngle;

		[SerializeField]
		private float speed;

		public float maxValue = 1f;

		private void Start()
		{
			startRotation = pin.transform.localEulerAngles;
			storedAngle = startRotation.x;
		}

		private void Update()
		{
			float target = Mathf.Lerp(angleMin, angleMax, input.value / maxValue);
			storedAngle = Mathf.MoveTowards(storedAngle, target, speed * Time.deltaTime);
			Quaternion localRotation = Quaternion.Euler(storedAngle, startRotation.y, startRotation.z);
			pin.transform.localRotation = localRotation;
		}
	}
}
