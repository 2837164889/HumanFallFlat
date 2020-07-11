using Multiplayer;
using UnityEngine;

namespace HumanAPI
{
	public class HeatToSignal : Node
	{
		public NodeOutput heat;

		protected override void OnEnable()
		{
			base.OnEnable();
			NetBody componentInParent = GetComponentInParent<NetBody>();
			if ((bool)componentInParent)
			{
				componentInParent.m_respawnEvent.AddListener(Reset);
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			NetBody componentInParent = GetComponentInParent<NetBody>();
			if ((bool)componentInParent)
			{
				componentInParent.m_respawnEvent.RemoveListener(Reset);
			}
		}

		public void InsideObjectChangedState(GameObject other)
		{
			OnEnter(other);
		}

		private void OnEnter(GameObject other)
		{
			Flame component = other.GetComponent<Flame>();
			if (component != null)
			{
				heat.SetValue(heat.value + component.isHot.value);
			}
			Flammable component2 = other.GetComponent<Flammable>();
			if ((bool)component2)
			{
				heat.SetValue(heat.value + component2.output.value);
			}
		}

		public void OnTriggerEnter(Collider other)
		{
			OnEnter(other.gameObject);
		}

		private void Reset()
		{
			heat.SetValue(0f);
		}

		public void OnTriggerExit(Collider other)
		{
			Flame component = other.GetComponent<Flame>();
			if (component != null)
			{
				float num = heat.value - component.isHot.value;
				if (num >= 0f)
				{
					heat.SetValue(num);
				}
			}
			Flammable component2 = other.GetComponent<Flammable>();
			if ((bool)component2)
			{
				float num2 = heat.value - component2.output.value;
				if (num2 >= 0f)
				{
					heat.SetValue(num2);
				}
			}
		}
	}
}
