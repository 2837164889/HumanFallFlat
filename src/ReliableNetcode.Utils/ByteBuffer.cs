using System;

namespace ReliableNetcode.Utils
{
	internal class ByteBuffer
	{
		protected byte[] _buffer;

		protected int size;

		public int Length => size;

		public byte[] InternalBuffer => _buffer;

		public byte this[int index]
		{
			get
			{
				if (index < 0 || index > size)
				{
					throw new IndexOutOfRangeException();
				}
				return _buffer[index];
			}
			set
			{
				if (index < 0 || index > size)
				{
					throw new IndexOutOfRangeException();
				}
				_buffer[index] = value;
			}
		}

		public ByteBuffer()
		{
			_buffer = null;
			size = 0;
		}

		public ByteBuffer(int size = 0)
		{
			_buffer = new byte[size];
			this.size = size;
		}

		public void SetSize(int newSize)
		{
			if (_buffer == null || _buffer.Length < newSize)
			{
				byte[] array = new byte[newSize];
				if (_buffer != null)
				{
					Buffer.BlockCopy(_buffer, 0, array, 0, _buffer.Length);
				}
				_buffer = array;
			}
			size = newSize;
		}

		public void BufferCopy(byte[] source, int src, int dest, int length)
		{
			Buffer.BlockCopy(source, src, _buffer, dest, length);
		}

		public void BufferCopy(ByteBuffer source, int src, int dest, int length)
		{
			Buffer.BlockCopy(source._buffer, src, _buffer, dest, length);
		}
	}
}
