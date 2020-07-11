using UnityEngine;

namespace HumanAPI
{
	public class CircuitConnector : MonoBehaviour
	{
		public ICircuitComponent parent;

		public bool isForward;

		public CircuitConnector connected;
	}
}
