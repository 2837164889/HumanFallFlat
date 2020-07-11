using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecordingManager : MonoBehaviour
{
	private enum RecorderState
	{
		Idle,
		Recording,
		Playing
	}

	private const int transformFrameBytes = 28;

	public List<Transform> roots = new List<Transform>();

	private List<Transform> transforms;

	private RecorderState state;

	private RecordingStream stream;

	public int frame;

	public float time;

	private void Start()
	{
		roots.Add(Human.instance.transform);
		transforms = new List<Transform>();
		foreach (Transform root in roots)
		{
			Transform[] componentsInChildren = root.GetComponentsInChildren<Transform>();
			transforms.AddRange(componentsInChildren);
		}
	}

	private void LateUpdate()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			BeginRecording();
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			EndRecording();
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			BeginPlayback();
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			EndPlayback();
		}
		if (state == RecorderState.Playing)
		{
			ReadFrame();
		}
	}

	private void FixedUpdate()
	{
		if (frame++ % 2 == 0 && state == RecorderState.Recording)
		{
			RecordFrame();
		}
	}

	private void BeginRecording()
	{
		frame = 0;
		state = RecorderState.Recording;
		MemoryStream baseStream = new MemoryStream();
		stream = new RecordingStream(1, write: true, baseStream);
	}

	private void EndRecording()
	{
		File.WriteAllBytes("Recording.bytes", stream.stream.ToArray());
		state = RecorderState.Idle;
		stream = null;
	}

	private void BeginPlayback()
	{
		frame = 0;
		Human.instance.enabled = false;
		state = RecorderState.Playing;
		byte[] buffer = File.ReadAllBytes("Recording.bytes");
		MemoryStream baseStream = new MemoryStream(buffer);
		stream = new RecordingStream(0, write: false, baseStream);
		StripBehaviors();
	}

	public void BeginPlayback(byte[] data, float startingTime)
	{
		time = startingTime;
		Human.instance.enabled = false;
		state = RecorderState.Playing;
		MemoryStream baseStream = new MemoryStream(data);
		stream = new RecordingStream(0, write: false, baseStream);
		StripBehaviors();
		int num = Mathf.RoundToInt(startingTime * 60f);
		stream.stream.Position = 4 + 28 * num * transforms.Count;
	}

	private void StripBehaviors()
	{
		foreach (Transform transform in transforms)
		{
			Component[] components = transform.GetComponents<Component>();
			Component[] array = components;
			foreach (Component component in array)
			{
				if (!(component is Transform) && !(component is SkinnedMeshRenderer) && !(component is MeshRenderer) && !(component is MeshFilter))
				{
					if (component is Rigidbody)
					{
						(component as Rigidbody).isKinematic = true;
					}
					else if (component is Human)
					{
						(component as Human).enabled = false;
					}
					else
					{
						Object.Destroy(component);
					}
				}
			}
		}
	}

	private void EndPlayback()
	{
		Human.instance.enabled = true;
		state = RecorderState.Idle;
		stream = null;
	}

	private void RecordFrame()
	{
		Serialize(stream);
	}

	private void ReadFrame()
	{
		Serialize(stream);
		if (stream.stream.Position == stream.stream.Length)
		{
			EndPlayback();
		}
	}

	private void Serialize(RecordingStream rs)
	{
		foreach (Transform transform in transforms)
		{
			Vector3 data = transform.localPosition;
			Quaternion data2 = transform.localRotation;
			rs.Serialize(ref data);
			rs.Serialize(ref data2);
			if (rs.isReading)
			{
				transform.localPosition = data;
				transform.localRotation = data2;
			}
		}
	}
}
