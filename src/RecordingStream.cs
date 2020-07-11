using System.IO;
using UnityEngine;

public class RecordingStream
{
	public const int CURRENT_VERSION = 1;

	public int version;

	private bool write;

	public MemoryStream stream;

	private BinaryReader reader;

	private BinaryWriter writer;

	public bool isWriting => write;

	public bool isReading => !write;

	public RecordingStream(int version, bool write, MemoryStream baseStream)
	{
		this.version = version;
		stream = baseStream;
		this.write = write;
		if (write)
		{
			writer = new BinaryWriter(stream);
		}
		else
		{
			reader = new BinaryReader(stream);
		}
		Serialize(ref this.version);
	}

	public void Serialize(ref int data)
	{
		if (write)
		{
			WriteInt(data);
		}
		else
		{
			data = ReadInt();
		}
	}

	public void WriteInt(int data)
	{
		writer.Write(data);
	}

	public int ReadInt()
	{
		return reader.ReadInt32();
	}

	public void Serialize(ref float data)
	{
		if (write)
		{
			WriteFloat(data);
		}
		else
		{
			data = ReadFloat();
		}
	}

	public void WriteFloat(float data)
	{
		writer.Write(data);
	}

	public float ReadFloat()
	{
		return reader.ReadSingle();
	}

	public void Serialize(ref Vector3 data)
	{
		if (write)
		{
			WriteVector3(data);
		}
		else
		{
			data = ReadVector3();
		}
	}

	public void WriteVector3(Vector3 data)
	{
		WriteFloat(data.x);
		WriteFloat(data.y);
		WriteFloat(data.z);
	}

	public Vector3 ReadVector3()
	{
		return new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
	}

	public void Serialize(ref Quaternion data)
	{
		if (write)
		{
			WriteQuaternion(data);
		}
		else
		{
			data = ReadQuaternion();
		}
	}

	public void WriteQuaternion(Quaternion data)
	{
		WriteFloat(data.x);
		WriteFloat(data.y);
		WriteFloat(data.z);
		WriteFloat(data.w);
	}

	public Quaternion ReadQuaternion()
	{
		return new Quaternion(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
	}
}
