using System.Collections.Generic;

namespace Multiplayer
{
	public class BitRateCounter
	{
		public int bitsLastFrame;

		public int bps;

		private int _framebits;

		private int _secbits;

		private Queue<int> queue = new Queue<int>();

		public float kbps => (float)(bps * 10 / 1024) * 0.1f;

		public void ReportBits(int bits)
		{
			_framebits += bits;
		}

		public void FrameComplete()
		{
			queue.Enqueue(_framebits);
			_secbits += _framebits * 4;
			if (queue.Count > 15)
			{
				_secbits -= queue.Dequeue() * 4;
			}
			bitsLastFrame = _framebits;
			_framebits = 0;
			bps = _secbits;
		}
	}
}
