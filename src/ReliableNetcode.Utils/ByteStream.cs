using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ReliableNetcode.Utils
{
	public class ByteStream : Stream
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct unionVal
		{
			[FieldOffset(0)]
			public uint intVal;

			[FieldOffset(0)]
			public float floatVal;

			[FieldOffset(0)]
			public ulong longVal;

			[FieldOffset(0)]
			public double doubleVal;
		}

		protected byte[] srcByteArray;

		public override long Position
		{
			get;
			set;
		}

		public override long Length => srcByteArray.Length;

		public override bool CanRead => true;

		public override bool CanWrite => true;

		public override bool CanSeek => true;

		public void SetStreamSource(byte[] sourceBuffer)
		{
			srcByteArray = sourceBuffer;
			Position = 0L;
		}

		public byte[] ReadBytes(int length)
		{
			byte[] array = new byte[length];
			Read(array, 0, length);
			return array;
		}

		public char ReadChar()
		{
			int num = 0;
			for (int i = 0; i < 2; i++)
			{
				num |= ReadByte() << (i << 3);
			}
			return (char)num;
		}

		public char[] ReadChars(int length)
		{
			char[] array = new char[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = ReadChar();
			}
			return array;
		}

		public string ReadString()
		{
			uint length = ReadUInt32();
			char[] value = ReadChars((int)length);
			return new string(value);
		}

		public short ReadInt16()
		{
			int num = 0;
			for (int i = 0; i < 2; i++)
			{
				num |= ReadByte() << (i << 3);
			}
			return (short)num;
		}

		public int ReadInt32()
		{
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				num |= ReadByte() << (i << 3);
			}
			return num;
		}

		public long ReadInt64()
		{
			long num = 0L;
			for (int i = 0; i < 8; i++)
			{
				num |= (long)(int)ReadByte() << (i << 3);
			}
			return num;
		}

		public ushort ReadUInt16()
		{
			ushort num = 0;
			for (int i = 0; i < 2; i++)
			{
				num = (ushort)(num | (ushort)(ReadByte() << (i << 3)));
			}
			return num;
		}

		public uint ReadUInt32()
		{
			uint num = 0u;
			for (int i = 0; i < 4; i++)
			{
				num = (uint)((int)num | (ReadByte() << (i << 3)));
			}
			return num;
		}

		public ulong ReadUInt64()
		{
			ulong num = 0uL;
			for (int i = 0; i < 8; i++)
			{
				num |= (ulong)ReadByte() << (i << 3);
			}
			return num;
		}

		public float ReadSingle()
		{
			uint intVal = ReadUInt32();
			unionVal unionVal = default(unionVal);
			unionVal.intVal = intVal;
			return unionVal.floatVal;
		}

		public double ReadDouble()
		{
			ulong longVal = ReadUInt64();
			unionVal unionVal = default(unionVal);
			unionVal.longVal = longVal;
			return unionVal.doubleVal;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = (int)Math.Min(count, Length - Position);
			Array.Copy(srcByteArray, Position, buffer, offset, num);
			Position += num;
			return num;
		}

		public new byte ReadByte()
		{
			long position = Position;
			byte result = srcByteArray[position++];
			Position = position;
			return result;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			int num = (int)Math.Min(count, Length - Position);
			Array.Copy(buffer, offset, srcByteArray, Position, num);
			Position += num;
		}

		public override void WriteByte(byte value)
		{
			long position = Position;
			srcByteArray[position++] = value;
			Position = position;
		}

		public void Write(byte val)
		{
			WriteByte(val);
		}

		public void Write(byte[] val)
		{
			Write(val, 0, val.Length);
		}

		public void Write(char val)
		{
			uint num = val;
			for (int i = 0; i < 2; i++)
			{
				WriteByte((byte)(num & 0xFF));
				num >>= 8;
			}
		}

		public void Write(char[] val)
		{
			for (int i = 0; i < val.Length; i++)
			{
				Write(val[i]);
			}
		}

		public void Write(string val)
		{
			Write((uint)val.Length);
			for (int i = 0; i < val.Length; i++)
			{
				Write(val[i]);
			}
		}

		public void Write(short val)
		{
			for (int i = 0; i < 2; i++)
			{
				WriteByte((byte)(val & 0xFF));
				val = (short)(val >> 8);
			}
		}

		public void Write(int val)
		{
			for (int i = 0; i < 4; i++)
			{
				WriteByte((byte)(val & 0xFF));
				val >>= 8;
			}
		}

		public void Write(long val)
		{
			for (int i = 0; i < 8; i++)
			{
				WriteByte((byte)(val & 0xFF));
				val >>= 8;
			}
		}

		public void Write(ushort val)
		{
			for (int i = 0; i < 2; i++)
			{
				WriteByte((byte)(val & 0xFF));
				val = (ushort)(val >> 8);
			}
		}

		public void Write(uint val)
		{
			for (int i = 0; i < 4; i++)
			{
				WriteByte((byte)(val & 0xFF));
				val >>= 8;
			}
		}

		public void Write(ulong val)
		{
			for (int i = 0; i < 8; i++)
			{
				WriteByte((byte)(val & 0xFF));
				val >>= 8;
			}
		}

		public void Write(float val)
		{
			unionVal unionVal = default(unionVal);
			unionVal.floatVal = val;
			Write(unionVal.intVal);
		}

		public void Write(double val)
		{
			unionVal unionVal = default(unionVal);
			unionVal.doubleVal = val;
			Write(unionVal.longVal);
		}

		public override void Flush()
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
			case SeekOrigin.Begin:
				Position = offset;
				break;
			case SeekOrigin.Current:
				Position += offset;
				break;
			default:
				Position = Length - offset - 1;
				break;
			}
			return Position;
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}
	}
}
