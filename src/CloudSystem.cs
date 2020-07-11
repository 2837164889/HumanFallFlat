using System;
using UnityEngine;

public class CloudSystem : MonoBehaviour
{
	public struct CloudData
	{
		public Vector3 worldPos;

		public Vector3 pos;

		public Vector3 size;

		public int startParticle;

		public int endParticle;
	}

	public struct CloudParticleData
	{
		public Vector3 pos;

		public Color color;

		public float size;

		public float angle;
	}

	public static CloudSystem instance;

	public int maxThreadCount = 4;

	public int threadCount = 4;

	public int maxClouds = 32;

	public int segmentsPerCloud = 4;

	public int particlesPerSegment = 8;

	public Vector3 cloudSetSize = new Vector3(400f, 400f, 400f);

	public float nearClipStart = 5f;

	public float nearClipEnd = 10f;

	public float farClipStart = 150f;

	public float farClipEnd = 200f;

	public Vector3 cloudSize = new Vector3(20f, 5f, 20f);

	public Vector3 cloudSegmentSize = new Vector3(15f, 10f, 15f);

	public float particleScale = 15f;

	public float rotateSpeed = 0.05f;

	public Vector3 moveSpeed = new Vector3(2f, 0f, 0f);

	public int maxParticles = 1024;

	public Color mainColor = new Color32(158, 158, 158, 77);

	public Color tintColor = new Color32(79, 79, 79, byte.MaxValue);

	public float tintCenter = 0.7f;

	public float tintScale = 1f;

	public float tintOffset;

	public Vector3 cloudSetOffset;

	private MeshFilter meshFilter;

	public Mesh mesh;

	private int curQuality = -1;

	public CloudData[] cloudsData;

	public CloudParticleData[] particlesData;

	public int maxVisibleParticlesCount;

	private void OnEnable()
	{
		instance = this;
		meshFilter = GetComponent<MeshFilter>();
		mesh = new Mesh();
		mesh.name = "Clouds";
	}

	public void SetQuality(int quality)
	{
		if (quality != curQuality)
		{
			curQuality = quality;
			Debug.Log("CloudSystem quality=" + curQuality);
			switch (quality)
			{
			case 0:
				threadCount = 1;
				maxClouds = 0;
				segmentsPerCloud = 2;
				particlesPerSegment = 2;
				break;
			case 1:
				threadCount = 1;
				maxClouds = 48;
				segmentsPerCloud = 3;
				particlesPerSegment = 3;
				break;
			case 2:
				threadCount = 1;
				maxClouds = 64;
				segmentsPerCloud = 4;
				particlesPerSegment = 4;
				break;
			case 3:
				threadCount = 1;
				maxClouds = 96;
				segmentsPerCloud = 4;
				particlesPerSegment = 6;
				break;
			case 4:
				threadCount = 1;
				maxClouds = 128;
				segmentsPerCloud = 6;
				particlesPerSegment = 8;
				break;
			}
			InitializeCloud();
		}
	}

	private void InitializeCloud()
	{
		CloudRender.all[0].KillAllThreads();
		maxParticles = maxClouds * segmentsPerCloud * particlesPerSegment;
		Debug.Log("CloudSystem maxParticles=" + maxParticles + " " + CloudRender.all.Count);
		GenerateCloudSet(new Vector3(400f, 500f, 400f));
		for (int i = 0; i < CloudRender.all.Count; i++)
		{
			CloudRender.all[i].InitializeCloud();
		}
		if (CloudRender.all.Count > 0)
		{
			mesh.Clear();
			Vector2[] array = new Vector2[CloudRender.all[0].maxMeshParticles * 4];
			int[] array2 = new int[CloudRender.all[0].maxMeshParticles * 6];
			for (int j = 0; j < CloudRender.all[0].maxMeshParticles; j++)
			{
				array2[j * 6] = j * 4;
				array2[j * 6 + 1] = j * 4 + 1;
				array2[j * 6 + 2] = j * 4 + 2;
				array2[j * 6 + 3] = j * 4 + 2;
				array2[j * 6 + 4] = j * 4 + 3;
				array2[j * 6 + 5] = j * 4;
				array[j * 4] = new Vector2(0f, 0f);
				array[j * 4 + 1] = new Vector2(0f, 1f);
				array[j * 4 + 2] = new Vector2(1f, 1f);
				array[j * 4 + 3] = new Vector2(1f, 0f);
			}
			mesh.vertices = CloudRender.all[0].vertices;
			mesh.colors32 = CloudRender.all[0].colors;
			mesh.uv = array;
			mesh.triangles = array2;
			meshFilter.mesh = mesh;
		}
	}

	private void GenerateCloudSet(Vector3 size)
	{
		cloudsData = new CloudData[maxClouds];
		particlesData = new CloudParticleData[maxParticles];
		for (int i = 0; i < maxClouds; i++)
		{
			Vector3 center = new Vector3(size.x * UnityEngine.Random.value, size.y * UnityEngine.Random.value, size.z * UnityEngine.Random.value);
			GenerateCloud(i, center);
		}
	}

	private void GenerateCloud(int index, Vector3 center)
	{
		int num = index * segmentsPerCloud * particlesPerSegment;
		int num2 = (index + 1) * segmentsPerCloud * particlesPerSegment;
		cloudsData[index].startParticle = num;
		cloudsData[index].endParticle = num2;
		for (int i = num; i < num2; i += particlesPerSegment)
		{
			BuildCloudPart(Vector3.Scale(UnityEngine.Random.insideUnitSphere, cloudSize), i, i + particlesPerSegment);
		}
		Bounds bounds = new Bounds(particlesData[num].pos, Vector3.zero);
		for (int j = num; j < num2; j++)
		{
			Vector3 b = new Vector3(particlesData[j].size / 2f, particlesData[j].size / 2f, particlesData[j].size / 2f);
			bounds.Encapsulate(particlesData[j].pos - b);
			bounds.Encapsulate(particlesData[j].pos + b);
		}
		cloudsData[index].pos = bounds.center + center;
		cloudsData[index].size = bounds.size;
		for (int k = num; k < num2; k++)
		{
			particlesData[k].pos -= bounds.center;
		}
		ColorCloud(num, num2);
	}

	private void BuildCloudPart(Vector3 pos, int start, int end)
	{
		for (int i = start; i < end; i++)
		{
			particlesData[i].pos = pos + Vector3.Scale(UnityEngine.Random.insideUnitSphere, cloudSegmentSize);
			particlesData[i].size = particleScale * UnityEngine.Random.Range(0.6f, 1f);
			particlesData[i].angle = UnityEngine.Random.value * (float)Math.PI * 2f - (float)Math.PI;
		}
	}

	private void ColorCloud(int start, int end)
	{
		float num = float.MaxValue;
		float num2 = float.MinValue;
		for (int i = start; i < end; i++)
		{
			float y = particlesData[i].pos.y;
			num = Mathf.Min(num, y);
			num2 = Mathf.Max(num2, y);
		}
		for (int j = start; j < end; j++)
		{
			float y2 = particlesData[j].pos.y;
			float num3 = (y2 - num) / (num2 - num);
			float t = Mathf.Abs(num3 - tintCenter) * tintScale + tintOffset;
			particlesData[j].color = Color.Lerp(mainColor, tintColor, t);
		}
	}

	private void LateUpdate()
	{
		float deltaTime = Time.deltaTime;
		cloudSetOffset += moveSpeed * deltaTime;
		for (int i = 0; i < CloudBox.all.Count; i++)
		{
			CloudBox.all[i].ReadPos();
		}
		for (int j = 0; j < CloudRender.all.Count; j++)
		{
			CloudRender.all[j].StartUpdate();
		}
	}

	public void Scroll(Vector3 offset)
	{
		cloudSetOffset += offset;
	}

	public float DistanceClipAlpha(float alpha, float distance)
	{
		if (distance < nearClipEnd)
		{
			alpha *= Mathf.Clamp01((distance - nearClipStart) / (nearClipEnd - nearClipStart));
		}
		if (distance > farClipStart)
		{
			alpha *= Mathf.Clamp01((distance - farClipEnd) / (farClipStart - farClipEnd));
		}
		return alpha;
	}
}
