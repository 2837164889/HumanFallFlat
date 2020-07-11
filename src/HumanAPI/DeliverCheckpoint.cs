using UnityEngine;

namespace HumanAPI
{
	public class DeliverCheckpoint : Checkpoint, IReset
	{
		public Rigidbody[] requiredBodies;

		public int requiredCount;

		private bool[] delivered;

		private void Awake()
		{
			delivered = new bool[requiredBodies.Length];
		}

		public override void OnTriggerEnter(Collider other)
		{
			bool flag = false;
			Rigidbody componentInParent = other.GetComponentInParent<Rigidbody>();
			for (int i = 0; i < requiredBodies.Length; i++)
			{
				if (componentInParent == requiredBodies[i] && !delivered[i])
				{
					delivered[i] = true;
					flag = true;
				}
			}
			if (!flag)
			{
				return;
			}
			int num = 0;
			for (int j = 0; j < requiredBodies.Length; j++)
			{
				if (delivered[j])
				{
					num++;
				}
			}
			if ((requiredCount > 0 && num >= requiredCount) || (requiredCount == 0 && num >= 1))
			{
				Pass();
			}
		}

		public void OnTriggerLeave(Collider other)
		{
			Rigidbody componentInParent = other.GetComponentInParent<Rigidbody>();
			for (int i = 0; i < requiredBodies.Length; i++)
			{
				if (componentInParent == requiredBodies[i])
				{
					delivered[i] = false;
				}
			}
		}

		void IReset.ResetState(int checkpoint, int subObjectives)
		{
			if (checkpoint < number)
			{
				for (int i = 0; i < delivered.Length; i++)
				{
					delivered[i] = false;
				}
			}
		}
	}
}
