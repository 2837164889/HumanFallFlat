using ProBuilder2.Common;
using ProBuilder2.MeshOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProBuilder2.Examples
{
	[RequireComponent(typeof(AudioSource))]
	public class IcoBumpin : MonoBehaviour
	{
		private struct FaceRef
		{
			public pb_Face face;

			public Vector3 nrm;

			public int[] indices;

			public FaceRef(pb_Face f, Vector3 n, int[] i)
			{
				face = f;
				nrm = n;
				indices = i;
			}
		}

		private pb_Object ico;

		private Mesh icoMesh;

		private Transform icoTransform;

		private AudioSource audioSource;

		private FaceRef[] outsides;

		private Vector3[] original_vertices;

		private Vector3[] displaced_vertices;

		[Range(1f, 10f)]
		public float icoRadius = 2f;

		[Range(0f, 3f)]
		public int icoSubdivisions = 2;

		[Range(0f, 1f)]
		public float startingExtrusion = 0.1f;

		public Material material;

		[Range(1f, 50f)]
		public float extrusion = 30f;

		[Range(8f, 128f)]
		public int fftBounds = 32;

		[Range(0f, 10f)]
		public float verticalBounce = 4f;

		public AnimationCurve frequencyCurve;

		public LineRenderer waveform;

		public float waveformHeight = 2f;

		public float waveformRadius = 20f;

		public float waveformSpeed = 0.1f;

		public bool rotateWaveformRing;

		public bool bounceWaveform;

		public GameObject missingClipWarning;

		private Vector3 icoPosition = Vector3.zero;

		private float faces_length;

		private const float TWOPI = 6.283185f;

		private const int WAVEFORM_SAMPLES = 1024;

		private const int FFT_SAMPLES = 4096;

		private float[] fft = new float[4096];

		private float[] fft_history = new float[4096];

		private float[] data = new float[1024];

		private float[] data_history = new float[1024];

		private float rms;

		private float rms_history;

		private void Start()
		{
			audioSource = GetComponent<AudioSource>();
			if (audioSource.clip == null)
			{
				missingClipWarning.SetActive(value: true);
			}
			ico = pb_ShapeGenerator.IcosahedronGenerator(icoRadius, icoSubdivisions);
			pb_Face[] faces = ico.faces;
			pb_Face[] array = faces;
			foreach (pb_Face pb_Face in array)
			{
				pb_Face.material = material;
			}
			ico.Extrude(faces, ExtrudeMethod.IndividualFaces, startingExtrusion);
			ico.ToMesh();
			ico.Refresh();
			outsides = new FaceRef[faces.Length];
			Dictionary<int, int> lookup = ico.sharedIndices.ToDictionary();
			for (int j = 0; j < faces.Length; j++)
			{
				outsides[j] = new FaceRef(faces[j], pb_Math.Normal(ico, faces[j]), ico.sharedIndices.AllIndicesWithValues(lookup, faces[j].distinctIndices).ToArray());
			}
			original_vertices = new Vector3[ico.vertices.Length];
			Array.Copy(ico.vertices, original_vertices, ico.vertices.Length);
			displaced_vertices = ico.vertices;
			icoMesh = ico.msh;
			icoTransform = ico.transform;
			faces_length = outsides.Length;
			icoPosition = icoTransform.position;
			waveform.positionCount = 1024;
			if (bounceWaveform)
			{
				waveform.transform.parent = icoTransform;
			}
			audioSource.Play();
		}

		private void Update()
		{
			audioSource.GetSpectrumData(fft, 0, FFTWindow.BlackmanHarris);
			audioSource.GetOutputData(data, 0);
			rms = RMS(data);
			for (int i = 0; i < outsides.Length; i++)
			{
				float num = (float)i / faces_length;
				int num2 = (int)(num * (float)fftBounds);
				Vector3 b = outsides[i].nrm * ((fft[num2] + fft_history[num2]) * 0.5f * (frequencyCurve.Evaluate(num) * 0.5f + 0.5f)) * extrusion;
				int[] indices = outsides[i].indices;
				foreach (int num3 in indices)
				{
					displaced_vertices[num3] = original_vertices[num3] + b;
				}
			}
			Vector3 zero = Vector3.zero;
			for (int k = 0; k < 1024; k++)
			{
				int num4 = (k < 1023) ? k : 0;
				zero.x = Mathf.Cos((float)num4 / 1024f * 6.283185f) * (waveformRadius + (data[num4] + data_history[num4]) * 0.5f * waveformHeight);
				zero.z = Mathf.Sin((float)num4 / 1024f * 6.283185f) * (waveformRadius + (data[num4] + data_history[num4]) * 0.5f * waveformHeight);
				zero.y = 0f;
				waveform.SetPosition(k, zero);
			}
			if (rotateWaveformRing)
			{
				Vector3 eulerAngles = waveform.transform.localRotation.eulerAngles;
				eulerAngles.x = Mathf.PerlinNoise(Time.time * waveformSpeed, 0f) * 360f;
				eulerAngles.y = Mathf.PerlinNoise(0f, Time.time * waveformSpeed) * 360f;
				waveform.transform.localRotation = Quaternion.Euler(eulerAngles);
			}
			icoPosition.y = 0f - verticalBounce + (rms + rms_history) * verticalBounce;
			icoTransform.position = icoPosition;
			Array.Copy(fft, fft_history, 4096);
			Array.Copy(data, data_history, 1024);
			rms_history = rms;
			icoMesh.vertices = displaced_vertices;
		}

		private float RMS(float[] arr)
		{
			float num = 0f;
			float num2 = arr.Length;
			for (int i = 0; (float)i < num2; i++)
			{
				num += Mathf.Abs(arr[i]);
			}
			return Mathf.Sqrt(num / num2);
		}
	}
}
