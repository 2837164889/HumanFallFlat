using UnityEngine;

namespace HumanAPI
{
	public class CircuitSounds : MonoBehaviour
	{
		public static CircuitSounds instance;

		public Sound2 plugWireNoCurrent;

		public Sound2 plugWireCurrent;

		public Sound2 unplugWireNoCurrent;

		public Sound2 uplugWireCurrent;

		public Sound2 plugWireShortCircuit;

		private void OnEnable()
		{
			instance = this;
		}

		public static void PlugWireNoCurrent(Vector3 pos)
		{
			instance.plugWireNoCurrent.PlayOneShot(pos);
		}

		public static void PlugWireCurrent(Vector3 pos)
		{
			instance.plugWireCurrent.PlayOneShot(pos);
		}

		public static void UnplugWireNoCurrent(Vector3 pos)
		{
			instance.unplugWireNoCurrent.PlayOneShot(pos);
		}

		public static void UplugWireCurrent(Vector3 pos)
		{
			instance.uplugWireCurrent.PlayOneShot(pos);
		}

		public static void PlugWireShortCircuit(Vector3 pos)
		{
			instance.plugWireShortCircuit.PlayOneShot(pos);
		}
	}
}
