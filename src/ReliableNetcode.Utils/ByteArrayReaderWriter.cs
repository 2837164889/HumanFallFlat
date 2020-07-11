using System;
using System.Collections.Generic;
using System.IO;

namespace ReliableNetcode.Utils
{
	public class ByteArrayReaderWriter : IDisposable
	{
		protected static Queue<ByteArrayReaderWriter> readerPool = new Queue<ByteArrayReaderWriter>();

		protected ByteStream readStream;

		protected ByteStream writeStream;

		public long ReadPosition => readStream.Position;

		public bool IsDoneReading => readStream.Position >= readStream.Length;

		public long WritePosition => writeStream.Position;

		public ByteArrayReaderWriter()
		{
			readStream = new ByteStream();
			writeStream = new ByteStream();
		}

		public static ByteArrayReaderWriter Get(byte[] byteArray)
		{
			ByteArrayReaderWriter byteArrayReaderWriter = null;
			lock (readerPool)
			{
				byteArrayReaderWriter = ((readerPool.Count <= 0) ? new ByteArrayReaderWriter() : readerPool.Dequeue());
			}
			byteArrayReaderWriter.SetStream(byteArray);
			return byteArrayReaderWriter;
		}

		public static void Release(ByteArrayReaderWriter reader)
		{
			lock (readerPool)
			{
				readerPool.Enqueue(reader);
			}
		}

		public void SetStream(byte[] byteArray)
		{
			readStream.SetStreamSource(byteArray);
			writeStream.SetStreamSource(byteArray);
		}

		public void SeekRead(long pos)
		{
			readStream.Seek(pos, SeekOrigin.Begin);
		}

		public void SeekWrite(long pos)
		{
			writeStream.Seek(pos, SeekOrigin.Begin);
		}

		public void Write(byte val)
		{
			writeStream.Write(val);
		}

		public void Write(byte[] val)
		{
			writeStream.Write(val);
		}

		public void Write(char val)
		{
			writeStream.Write(val);
		}

		public void Write(char[] val)
		{
			writeStream.Write(val);
		}

		public void Write(string val)
		{
			writeStream.Write(val);
		}

		public void Write(short val)
		{
			writeStream.Write(val);
		}

		public void Write(int val)
		{
			writeStream.Write(val);
		}

		public void Write(long val)
		{
			writeStream.Write(val);
		}

		public void Write(ushort val)
		{
			writeStream.Write(val);
		}

		public void Write(uint val)
		{
			writeStream.Write(val);
		}

		public void Write(ulong val)
		{
			writeStream.Write(val);
		}

		public void Write(float val)
		{
			writeStream.Write(val);
		}

		public void Write(double val)
		{
			writeStream.Write(val);
		}

		public void WriteASCII(char[] chars)
		{
			for (int i = 0; i < chars.Length; i++)
			{
				byte val = (byte)(chars[i] & 0xFF);
				Write(val);
			}
		}

		public void WriteASCII(string str)
		{
			for (int i = 0; i < str.Length; i++)
			{
				byte val = (byte)(str[i] & 0xFF);
				Write(val);
			}
		}

		public void WriteBuffer(byte[] buffer, int length)
		{
			for (int i = 0; i < length; i++)
			{
				Write(buffer[i]);
			}
		}

		public byte ReadByte()
		{
			return readStream.ReadByte();
		}

		public byte[] ReadBytes(int length)
		{
			return readStream.ReadBytes(length);
		}

		public char ReadChar()
		{
			return readStream.ReadChar();
		}

		public char[] ReadChars(int length)
		{
			return readStream.ReadChars(length);
		}

		public string ReadString()
		{
			return readStream.ReadString();
		}

		public short ReadInt16()
		{
			return readStream.ReadInt16();
		}

		public int ReadInt32()
		{
			return readStream.ReadInt32();
		}

		public long ReadInt64()
		{
			return readStream.ReadInt64();
		}

		public ushort ReadUInt16()
		{
			return readStream.ReadUInt16();
		}

		public uint ReadUInt32()
		{
			return readStream.ReadUInt32();
		}

		public ulong ReadUInt64()
		{
			return readStream.ReadUInt64();
		}

		public float ReadSingle()
		{
			return readStream.ReadSingle();
		}

		public double ReadDouble()
		{
			return readStream.ReadDouble();
		}

		public void ReadASCIICharsIntoBuffer(char[] buffer, int length)
		{
			for (int i = 0; i < length; i++)
			{
				buffer[i] = (char)ReadByte();
			}
		}

		public void ReadBytesIntoBuffer(byte[] buffer, int length)
		{
			for (int i = 0; i < length; i++)
			{
				buffer[i] = ReadByte();
			}
		}

		public void Dispose()
		{
			Release(this);
		}
	}
}
