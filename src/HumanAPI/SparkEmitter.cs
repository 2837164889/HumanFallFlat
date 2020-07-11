using Multiplayer;
using UnityEngine;

namespace HumanAPI
{
	public class SparkEmitter : Node
	{
		[Tooltip("Min number of particles per second")]
		public float maxRate;

		[Tooltip("The Max number of particles per second")]
		public float minRate;

		public NodeInput input;

		[Tooltip("Nummber of particles to spawn in a burst on the on signal")]
		public int burstOn;

		[Tooltip("number of particles to spawn in a burst on the off signal")]
		public int burstOff;

		public SignalBase burstSignal;

		private bool isOn;

		private float delay;

		private void Awake()
		{
			delay = Random.Range(1f / maxRate, 1f / minRate);
			if (burstSignal != null)
			{
				burstSignal.onValueChanged += BurstSignal_onValueChanged;
				isOn = burstSignal.boolValue;
			}
		}

		public override void Process()
		{
			base.Process();
			bool flag = Mathf.Abs(input.value) >= 0.5f;
			if (isOn != flag)
			{
				isOn = flag;
				SparkPool.instance.Emit((!isOn) ? burstOff : burstOn, base.transform.position);
			}
		}

		private void BurstSignal_onValueChanged(float obj)
		{
			if (isOn != burstSignal.boolValue)
			{
				isOn = burstSignal.boolValue;
				SparkPool.instance.Emit((!isOn) ? burstOff : burstOn, base.transform.position);
			}
		}

		private void Update()
		{
			if ((burstSignal != null || input != null) && !isOn)
			{
				return;
			}
			delay -= Time.deltaTime;
			if (delay <= 0f)
			{
				float num = float.MaxValue;
				for (int i = 0; i < NetGame.instance.local.players.Count; i++)
				{
					Human human = NetGame.instance.local.players[i].human;
					num = Mathf.Min((human.ragdoll.transform.position - base.transform.position).magnitude, num);
				}
				if (num < 30f)
				{
					SparkPool.instance.Emit(1, base.transform.position);
				}
				delay = Random.Range(1f / maxRate, 1f / minRate);
			}
		}
	}
}
