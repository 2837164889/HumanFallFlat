using System.Collections.Generic;
using UnityEngine;

namespace Multiplayer
{
	public class LatencyCounter
	{
		private Queue<float> queue = new Queue<float>();

		private float _latency;

		private float _secLatency;

		public float latency;

		public void ReportLatency(float latency)
		{
			_latency = Mathf.Max(_latency, latency);
		}

		public void FrameComplete()
		{
			queue.Enqueue(_latency);
			_secLatency += _latency;
			if (queue.Count > 15)
			{
				_secLatency -= queue.Dequeue();
			}
			_latency = 0f;
			latency = _secLatency / 15f;
		}
	}
}
