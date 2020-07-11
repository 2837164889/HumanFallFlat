using UnityEngine;

namespace HumanAPI
{
	public class RailSnapParameters : MonoBehaviour
	{
		public float maxSpeed = 3f;

		public float accelerationTime = 1f;

		public float decelerationTime = 1f;

		public float posSpringX = 500000f;

		public float posDampX = 10000f;

		public float posSpringY = 50000f;

		public float posDampY = 10000f;
	}
}
