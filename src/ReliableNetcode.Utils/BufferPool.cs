using System;
using System.Collections.Generic;

namespace ReliableNetcode.Utils
{
	public static class BufferPool
	{
		private static Dictionary<int, Queue<byte[]>> bufferPool = new Dictionary<int, Queue<byte[]>>();

		public static byte[] GetBuffer(int size)
		{
			lock (bufferPool)
			{
				if (bufferPool.ContainsKey(size) && bufferPool[size].Count > 0)
				{
					return bufferPool[size].Dequeue();
				}
			}
			return new byte[size];
		}

		public static void ReturnBuffer(byte[] buffer)
		{
			lock (bufferPool)
			{
				if (!bufferPool.ContainsKey(buffer.Length))
				{
					bufferPool.Add(buffer.Length, new Queue<byte[]>());
				}
				Array.Clear(buffer, 0, buffer.Length);
				bufferPool[buffer.Length].Enqueue(buffer);
			}
		}
	}
}
