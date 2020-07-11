using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CloudRender : MonoBehaviour
{
	private struct ParticleSort
	{
		public int index;

		public Vector3 worldPos;

		public float distance;

		public override string ToString()
		{
			return $"{index} {distance}";
		}
	}

	private class ThreadParams
	{
		public int start;

		public int end;

		public int vpCount;

		public bool running;

		public bool abort;

		public Vector3[] verts;

		public Color32[] cols;

		public Vector3 camX;

		public Vector3 camY;

		public ParticleSort[] psort;

		public AutoResetEvent startHandle;

		public AutoResetEvent completeHandle;
	}

	public static List<CloudRender> all = new List<CloudRender>();

	private int[] visibleClouds;

	public int visibleCloudsCount;

	private int[] visibleParticles;

	public int visibleParticlesCount;

	public int maxMeshParticles = 256;

	public Vector3[] vertices;

	public Color32[] colors;

	private int[] myThreads;

	private ParticleSort[] particleSort;

	private static ThreadParams[] helperParams;

	private static Thread[] helperThreads;

	private Vector3 camX;

	private Vector3 camY;

	private bool updateStarted;

	private Plane[] mPlanes = new Plane[6];

	private void OnEnable()
	{
		all.Add(this);
		if (CloudSystem.instance != null && CloudSystem.instance.cloudsData != null)
		{
			InitializeCloud();
		}
	}

	private void OnDisable()
	{
		KillThreads();
		all.Remove(this);
	}

	public void InitializeCloud()
	{
		visibleClouds = new int[CloudSystem.instance.maxClouds];
		visibleParticles = new int[CloudSystem.instance.maxParticles];
		maxMeshParticles = CloudSystem.instance.maxParticles / 4;
		vertices = new Vector3[maxMeshParticles * 4];
		colors = new Color32[maxMeshParticles * 4];
		particleSort = new ParticleSort[CloudSystem.instance.maxParticles];
		myThreads = new int[CloudSystem.instance.maxThreadCount];
		if (helperThreads == null)
		{
			helperThreads = new Thread[CloudSystem.instance.maxThreadCount];
			helperParams = new ThreadParams[CloudSystem.instance.maxThreadCount];
			for (int i = 0; i < CloudSystem.instance.maxThreadCount; i++)
			{
				helperThreads[i] = new Thread(Worker);
				helperParams[i] = new ThreadParams
				{
					start = maxMeshParticles / CloudSystem.instance.threadCount * (i % CloudSystem.instance.threadCount),
					end = maxMeshParticles / CloudSystem.instance.threadCount * (i % CloudSystem.instance.threadCount + 1),
					running = (i < CloudSystem.instance.threadCount),
					abort = false,
					verts = vertices,
					cols = colors,
					psort = particleSort,
					vpCount = visibleParticlesCount,
					startHandle = new AutoResetEvent(initialState: false),
					completeHandle = new AutoResetEvent(initialState: false)
				};
				myThreads[i] = ((i >= CloudSystem.instance.threadCount) ? (-1) : i);
				helperThreads[i].Start(helperParams[i]);
			}
			return;
		}
		for (int j = 0; j < CloudSystem.instance.threadCount; j++)
		{
			myThreads[j] = -1;
			for (int k = 0; k < CloudSystem.instance.maxThreadCount; k++)
			{
				if (!helperParams[k].running)
				{
					helperParams[k].start = maxMeshParticles / CloudSystem.instance.threadCount * j;
					helperParams[k].end = maxMeshParticles / CloudSystem.instance.threadCount * (j + 1);
					helperParams[k].abort = false;
					helperParams[k].verts = vertices;
					helperParams[k].cols = colors;
					helperParams[k].psort = particleSort;
					helperParams[k].vpCount = visibleParticlesCount;
					myThreads[j] = k;
					helperParams[k].running = true;
					break;
				}
			}
		}
	}

	private void OnDestroy()
	{
		KillThreads();
	}

	public void KillAllThreads()
	{
		if (helperThreads == null)
		{
			return;
		}
		for (int i = 0; i < CloudSystem.instance.maxThreadCount; i++)
		{
			helperParams[i].abort = true;
			helperParams[i].startHandle.Set();
		}
		for (int j = 0; j < CloudSystem.instance.maxThreadCount; j++)
		{
			if (helperParams[j].running)
			{
				helperParams[j].completeHandle.WaitOne(100);
			}
		}
	}

	public void KillThreads()
	{
		if (helperThreads == null)
		{
			return;
		}
		for (int i = 0; i < CloudSystem.instance.threadCount; i++)
		{
			for (int j = 0; j < CloudSystem.instance.maxThreadCount; j++)
			{
				if (myThreads[i] == j)
				{
					helperParams[j].abort = true;
					helperParams[j].startHandle.Set();
					myThreads[i] = -1;
				}
			}
		}
		for (int k = 0; k < CloudSystem.instance.maxThreadCount; k++)
		{
			if (helperParams[k].running)
			{
				helperParams[k].completeHandle.WaitOne(100);
			}
		}
	}

	private void Worker(object options)
	{
		ThreadParams threadParams = options as ThreadParams;
		while (true)
		{
			threadParams.startHandle.WaitOne();
			if (threadParams.abort)
			{
				threadParams.running = false;
				threadParams.cols = null;
				threadParams.verts = null;
			}
			if (threadParams.running)
			{
				BuildMesh(threadParams.start, threadParams.end, threadParams);
			}
			threadParams.completeHandle.Set();
		}
	}

	private void ShellSort(ParticleSort[] inputArray, int length)
	{
		for (int num = length / 2; num > 0; num = ((num / 2 == 0) ? ((num != 1) ? 1 : 0) : (num / 2)))
		{
			for (int i = 0; i < length; i++)
			{
				int num2 = i;
				ParticleSort particleSort = inputArray[i];
				while (num2 >= num && inputArray[num2 - num].distance > particleSort.distance)
				{
					inputArray[num2] = inputArray[num2 - num];
					num2 -= num;
				}
				inputArray[num2] = particleSort;
			}
		}
	}

	public void StartUpdate()
	{
		Camera component = GetComponent<Camera>();
		if (component == null)
		{
			return;
		}
		camX = component.transform.right;
		camY = component.transform.up;
		CullClouds(component);
		SortParticles(component);
		camX = component.transform.right;
		camY = component.transform.up;
		for (int i = 0; i < CloudSystem.instance.threadCount; i++)
		{
			if (myThreads[i] != -1)
			{
				helperParams[myThreads[i]].camX = camX;
				helperParams[myThreads[i]].camY = camY;
				helperParams[myThreads[i]].vpCount = visibleParticlesCount;
				helperParams[myThreads[i]].startHandle.Set();
			}
		}
		updateStarted = true;
	}

	private void OnPreRender()
	{
		if (!updateStarted)
		{
			return;
		}
		updateStarted = false;
		bool flag = true;
		for (int i = 0; i < CloudSystem.instance.threadCount; i++)
		{
			if (myThreads[i] != -1 && helperParams[myThreads[i]].running && !helperParams[myThreads[i]].completeHandle.WaitOne(1000))
			{
				flag = false;
			}
		}
		if (flag)
		{
			CloudSystem.instance.mesh.vertices = vertices;
			CloudSystem.instance.mesh.colors32 = colors;
			CloudSystem.instance.mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10000f);
		}
	}

	private void FastSinCos(float x, out float sin, out float cos)
	{
		if (x < 0f)
		{
			sin = 4f / (float)Math.PI * x + 0.40528473f * x * x;
		}
		else
		{
			sin = 4f / (float)Math.PI * x - 0.40528473f * x * x;
		}
		x += (float)Math.PI / 2f;
		if (x > (float)Math.PI)
		{
			x -= (float)Math.PI * 2f;
		}
		if (x < 0f)
		{
			cos = 4f / (float)Math.PI * x + 0.40528473f * x * x;
		}
		else
		{
			cos = 4f / (float)Math.PI * x - 0.40528473f * x * x;
		}
	}

	private void CullClouds(Camera camera)
	{
		Vector3 position = camera.transform.position;
		Vector3 b = MathUtils.WrapSigned(CloudSystem.instance.cloudSetOffset - position, CloudSystem.instance.cloudSetSize) + position;
		GeometryUtility.CalculateFrustumPlanes(camera, mPlanes);
		visibleCloudsCount = 0;
		if (Game.currentLevel != null && Game.currentLevel.noClouds)
		{
			return;
		}
		for (int i = 0; i < CloudSystem.instance.maxClouds; i++)
		{
			CloudSystem.CloudData cloudData = CloudSystem.instance.cloudsData[i];
			Vector3 a = cloudData.pos + b;
			a = MathUtils.WrapSigned(a - position, CloudSystem.instance.cloudSetSize) + position;
			float magnitude = (a - position).magnitude;
			if (!(magnitude - Mathf.Max(Mathf.Max(cloudData.size.x, cloudData.size.y), cloudData.size.z) > CloudSystem.instance.farClipEnd) && GeometryUtility.TestPlanesAABB(mPlanes, new Bounds(a, CloudSystem.instance.cloudsData[i].size)))
			{
				CloudSystem.instance.cloudsData[i].worldPos = a;
				visibleClouds[visibleCloudsCount++] = i;
			}
		}
	}

	private void SortParticles(Camera camera)
	{
		Vector3 position = camera.transform.position;
		CloudSystem.CloudParticleData[] particlesData = CloudSystem.instance.particlesData;
		visibleParticlesCount = 0;
		for (int i = 0; i < visibleCloudsCount; i++)
		{
			CloudSystem.CloudData cloudData = CloudSystem.instance.cloudsData[visibleClouds[i]];
			int startParticle = cloudData.startParticle;
			int endParticle = cloudData.endParticle;
			for (int j = startParticle; j < endParticle; j++)
			{
				Vector3 vector = particlesData[j].pos + cloudData.worldPos;
				float magnitude = (vector - position).magnitude;
				if (!(magnitude <= CloudSystem.instance.nearClipStart) && !(magnitude >= CloudSystem.instance.farClipEnd))
				{
					particleSort[visibleParticlesCount].index = j;
					particleSort[visibleParticlesCount].worldPos = vector;
					particleSort[visibleParticlesCount].distance = 0f - magnitude;
					visibleParticlesCount++;
				}
			}
		}
		if (visibleParticlesCount > CloudSystem.instance.maxVisibleParticlesCount)
		{
			CloudSystem.instance.maxVisibleParticlesCount = visibleParticlesCount;
		}
		ShellSort(particleSort, visibleParticlesCount);
	}

	private void BuildMesh(int start, int end, ThreadParams tp)
	{
		Vector3[] verts = tp.verts;
		Color32[] cols = tp.cols;
		List<CloudBox> list = new List<CloudBox>();
		lock (CloudBox.cloudLock)
		{
			for (int i = 0; i < CloudBox.all.Count; i++)
			{
				list.Add(CloudBox.all[i]);
			}
		}
		int num = Mathf.Clamp(tp.vpCount - maxMeshParticles, 0, tp.vpCount);
		for (int j = start; j < end; j++)
		{
			if (j >= tp.vpCount)
			{
				verts[j * 4] = (verts[j * 4 + 1] = (verts[j * 4 + 2] = (verts[j * 4 + 3] = Vector3.zero)));
				continue;
			}
			ParticleSort particleSort = tp.psort[j + num];
			int index = particleSort.index;
			CloudSystem.CloudParticleData cloudParticleData = CloudSystem.instance.particlesData[index];
			float size = cloudParticleData.size;
			float angle = cloudParticleData.angle;
			Color color = cloudParticleData.color;
			Vector3 worldPos = particleSort.worldPos;
			float distance = 0f - particleSort.distance;
			FastSinCos(angle, out float sin, out float cos);
			float num2 = (sin + cos) * size;
			float num3 = (cos - sin) * size;
			Vector3 vector = new Vector3(tp.camX.x * num3 + tp.camY.x * num2, tp.camX.y * num3 + tp.camY.y * num2, tp.camX.z * num3 + tp.camY.z * num2);
			Vector3 vector2 = new Vector3(tp.camX.x * num2 - tp.camY.x * num3, tp.camX.y * num2 - tp.camY.y * num3, tp.camX.z * num2 - tp.camY.z * num3);
			float a = color.a;
			a = CloudSystem.instance.DistanceClipAlpha(a, distance);
			int num4 = 0;
			while (a > 0.01f && num4 < list.Count)
			{
				a *= list[num4].GetAlpha(worldPos);
				num4++;
			}
			color.a = a;
			if (a > 0.01f)
			{
				verts[j * 4] = new Vector3(worldPos.x - vector.x, worldPos.y - vector.y, worldPos.z - vector.z);
				verts[j * 4 + 1] = new Vector3(worldPos.x - vector2.x, worldPos.y - vector2.y, worldPos.z - vector2.z);
				verts[j * 4 + 2] = new Vector3(worldPos.x + vector.x, worldPos.y + vector.y, worldPos.z + vector.z);
				verts[j * 4 + 3] = new Vector3(worldPos.x + vector2.x, worldPos.y + vector2.y, worldPos.z + vector2.z);
			}
			else
			{
				verts[j * 4] = worldPos;
				verts[j * 4 + 1] = worldPos;
				verts[j * 4 + 2] = worldPos;
				verts[j * 4 + 3] = worldPos;
			}
			cols[j * 4] = (cols[j * 4 + 1] = (cols[j * 4 + 2] = (cols[j * 4 + 3] = color)));
		}
	}
}
