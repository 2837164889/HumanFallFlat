using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RagdollExport : MonoBehaviour
{
	[Serializable]
	public class SerializedFrame
	{
		public int frame;

		public List<SerializedTransform> objects = new List<SerializedTransform>();

		public void Lerp(SerializedFrame f2, float t)
		{
			for (int i = 0; i < objects.Count; i++)
			{
				objects[i].Lerp(f2.objects[i], t);
			}
		}
	}

	[Serializable]
	public class SerializedTransform
	{
		public string name;

		public float[] pos;

		public float[] rot;

		public Vector3 position => new Vector3(pos[0], pos[1], pos[2]);

		public Quaternion rotation => new Quaternion(rot[0], rot[1], rot[2], rot[3]);

		public void Lerp(SerializedTransform f2, float t)
		{
			Vector3 vector = Vector3.Lerp(position, f2.position, t);
			Quaternion quaternion = Quaternion.Lerp(rotation, f2.rotation, t);
			pos = new float[3]
			{
				vector.x,
				vector.y,
				vector.z
			};
			rot = new float[4]
			{
				quaternion.x,
				quaternion.y,
				quaternion.z,
				quaternion.w
			};
		}
	}

	private Ragdoll ragdoll;

	private Dictionary<string, Quaternion> initialRot = new Dictionary<string, Quaternion>();

	private string filename;

	private bool writing;

	private List<SerializedFrame> frames = new List<SerializedFrame>();

	private void Awake()
	{
		ragdoll = GetComponent<Ragdoll>();
		SaveInitial(ragdoll.partHips.transform);
		SaveInitial(ragdoll.partWaist.transform);
		SaveInitial(ragdoll.partChest.transform);
		SaveInitial(ragdoll.partHead.transform);
		SaveInitial(ragdoll.partLeftArm.transform);
		SaveInitial(ragdoll.partLeftForearm.transform);
		SaveInitial(ragdoll.partLeftThigh.transform);
		SaveInitial(ragdoll.partLeftLeg.transform);
		SaveInitial(ragdoll.partRightArm.transform);
		SaveInitial(ragdoll.partRightForearm.transform);
		SaveInitial(ragdoll.partRightThigh.transform);
		SaveInitial(ragdoll.partRightLeg.transform);
	}

	private void SaveInitial(Transform t)
	{
		initialRot[t.name] = Quaternion.Inverse(t.localRotation);
		if (t.name == "Hips")
		{
			initialRot[t.name] = Quaternion.Inverse(t.rotation);
		}
	}

	private void Update()
	{
		if (Game.GetKeyDown(KeyCode.P))
		{
			writing = true;
			filename = string.Format("ragdoll-{0}.txt", DateTime.Now.ToString("HHmmss"));
			frames.Clear();
		}
	}

	private void FixedUpdate()
	{
		if (!writing)
		{
			return;
		}
		if (frames.Count > 1060)
		{
			writing = false;
			int num = 60;
			for (int i = 0; i < num; i++)
			{
				frames[i].Lerp(frames[i + 1000], 1f - 1f * (float)i / (float)num);
			}
			for (int j = 0; j < 1000; j++)
			{
				File.AppendAllText(filename, JsonUtility.ToJson(frames[j], prettyPrint: false));
				File.AppendAllText(filename, ",\r\n");
			}
		}
		SerializedFrame serializedFrame = new SerializedFrame();
		serializedFrame.objects.Add(StoreTransform(ragdoll.partHips.transform));
		serializedFrame.objects.Add(StoreTransform(ragdoll.partWaist.transform));
		serializedFrame.objects.Add(StoreTransform(ragdoll.partChest.transform));
		serializedFrame.objects.Add(StoreTransform(ragdoll.partHead.transform));
		serializedFrame.objects.Add(StoreTransform(ragdoll.partLeftArm.transform));
		serializedFrame.objects.Add(StoreTransform(ragdoll.partLeftForearm.transform));
		serializedFrame.objects.Add(StoreTransform(ragdoll.partLeftThigh.transform));
		serializedFrame.objects.Add(StoreTransform(ragdoll.partLeftLeg.transform));
		serializedFrame.objects.Add(StoreTransform(ragdoll.partRightArm.transform));
		serializedFrame.objects.Add(StoreTransform(ragdoll.partRightForearm.transform));
		serializedFrame.objects.Add(StoreTransform(ragdoll.partRightThigh.transform));
		serializedFrame.objects.Add(StoreTransform(ragdoll.partRightLeg.transform));
		frames.Add(serializedFrame);
	}

	private SerializedTransform StoreTransform(Transform t)
	{
		Vector3 position = t.position;
		Quaternion quaternion = initialRot[t.name] * t.localRotation;
		if (t.name == "Hips")
		{
			quaternion = initialRot[t.name] * t.rotation;
		}
		SerializedTransform serializedTransform = new SerializedTransform();
		serializedTransform.name = t.name;
		serializedTransform.pos = new float[3]
		{
			position.x,
			position.y,
			position.z
		};
		serializedTransform.rot = new float[4]
		{
			quaternion.x,
			quaternion.y,
			quaternion.z,
			quaternion.w
		};
		return serializedTransform;
	}
}
