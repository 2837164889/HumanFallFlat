using ReliableNetcode.Utils;

namespace ReliableNetcode
{
	internal class SequenceBuffer<T> where T : class, new()
	{
		private const uint NULL_SEQUENCE = uint.MaxValue;

		public ushort sequence;

		private int numEntries;

		private uint[] entrySequence;

		private T[] entryData;

		public int Size => numEntries;

		public SequenceBuffer(int bufferSize)
		{
			sequence = 0;
			numEntries = bufferSize;
			entrySequence = new uint[bufferSize];
			for (int i = 0; i < bufferSize; i++)
			{
				entrySequence[i] = uint.MaxValue;
			}
			entryData = new T[bufferSize];
			for (int j = 0; j < bufferSize; j++)
			{
				entryData[j] = new T();
			}
		}

		public void Reset()
		{
			sequence = 0;
			for (int i = 0; i < numEntries; i++)
			{
				entrySequence[i] = uint.MaxValue;
			}
		}

		public void RemoveEntries(int startSequence, int finishSequence)
		{
			if (finishSequence < startSequence)
			{
				finishSequence += 65536;
			}
			if (finishSequence - startSequence < numEntries)
			{
				for (int i = startSequence; i <= finishSequence; i++)
				{
					entrySequence[i % numEntries] = uint.MaxValue;
				}
			}
			else
			{
				for (int j = 0; j < numEntries; j++)
				{
					entrySequence[j] = uint.MaxValue;
				}
			}
		}

		public bool TestInsert(ushort sequence)
		{
			return !PacketIO.SequenceLessThan(sequence, (ushort)(this.sequence - numEntries));
		}

		public T Insert(ushort sequence)
		{
			if (PacketIO.SequenceLessThan(sequence, (ushort)(this.sequence - numEntries)))
			{
				return (T)null;
			}
			if (PacketIO.SequenceGreaterThan((ushort)(sequence + 1), this.sequence))
			{
				RemoveEntries(this.sequence, sequence);
				this.sequence = (ushort)(sequence + 1);
			}
			int num = (int)sequence % numEntries;
			entrySequence[num] = sequence;
			return entryData[num];
		}

		public void Remove(ushort sequence)
		{
			entrySequence[(int)sequence % numEntries] = uint.MaxValue;
		}

		public bool Available(ushort sequence)
		{
			return entrySequence[(int)sequence % numEntries] == uint.MaxValue;
		}

		public bool Exists(ushort sequence)
		{
			return entrySequence[(int)sequence % numEntries] == sequence;
		}

		public T Find(ushort sequence)
		{
			int num = (int)sequence % numEntries;
			uint num2 = entrySequence[num];
			if (num2 == sequence)
			{
				return entryData[num];
			}
			return (T)null;
		}

		public T AtIndex(int index)
		{
			uint num = entrySequence[index];
			if (num == uint.MaxValue)
			{
				return (T)null;
			}
			return entryData[index];
		}

		public void GenerateAckBits(out ushort ack, out uint ackBits)
		{
			ack = (ushort)(sequence - 1);
			ackBits = 0u;
			uint num = 1u;
			for (int i = 0; i < 32; i++)
			{
				ushort num2 = (ushort)(ack - i);
				if (Exists(num2))
				{
					ackBits |= num;
				}
				num <<= 1;
			}
		}
	}
}
