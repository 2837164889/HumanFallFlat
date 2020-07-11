using System.IO;
using UnityEngine;

namespace Multiplayer
{
	public static class StreamExtensions
	{
		public static void Write(this BinaryWriter writer, Vector3 vec3)
		{
			writer.Write(vec3.x);
			writer.Write(vec3.y);
			writer.Write(vec3.z);
		}

		public static Vector3 ReadVector3(this BinaryReader reader)
		{
			return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		public static void Write(this BinaryWriter writer, Quaternion quat)
		{
			writer.Write(quat.x);
			writer.Write(quat.y);
			writer.Write(quat.z);
			writer.Write(quat.w);
		}

		public static Quaternion ReadQuaternion(this BinaryReader reader)
		{
			return new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}
	}
}
