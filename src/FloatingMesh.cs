using Multiplayer;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FloatingMesh : MonoBehaviour
{
	private struct FloatingVertex
	{
		public Vector3 pos;

		public Vector3 velocity;

		public float depth;
	}

	[Tooltip("Index for each vert we will pass through when applying any movement")]
	public static int counter;

	private int index;

	[Tooltip("The collision surfaces that connect to the water surface")]
	public Collider hull;

	[Tooltip("The mesh connected to the hull , mesh that needs to float ")]
	public Mesh mesh;

	[Tooltip("A particular water sensor for this floating mesh")]
	public WaterSensor sensor;

	[Tooltip("The density of the object , effects the reaction away from the water surface")]
	public float density = 1000f;

	[Tooltip("Vector for the direction to ignore when appplying force to the hull ")]
	public Vector3 ignoreHydrodynamicForce;

	[Tooltip("The pressure in a particular direction")]
	public float pressureLinear = 20f;

	[Tooltip("The square of the pressure")]
	public float pressureSquare = 10f;

	[Tooltip("The amount of suction in a particular direction ")]
	public float suctionLinear = 20f;

	[Tooltip("The square of the suction")]
	public float suctionSquare = 10f;

	[Tooltip("How fast the power should fall off over the hull")]
	public float falloffPower = 1f;

	[Tooltip("How much to be effected by the wind")]
	public float bendWind = 0.1f;

	private Rigidbody body;

	private Vector3[] meshVertices;

	private int[] triangles;

	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	private FloatingVertex[] vertices;

	private Vector3 center;

	private Vector3 resultantMoment;

	private Vector3 resultantForce;

	private Vector3 resultantStaticForce;

	public float horizontalHydrostaticTreshold;

	private void Start()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Started ");
		}
		index = counter++;
		body = GetComponent<Rigidbody>();
		if (body == null)
		{
			throw new InvalidOperationException("Floater needs a Rigidbody");
		}
		if (hull == null)
		{
			hull = GetComponentInChildren<Collider>();
		}
		if (hull == null)
		{
			throw new InvalidOperationException("MeshCollider");
		}
		sensor = hull.GetComponent<WaterSensor>();
		if (sensor == null)
		{
			sensor = hull.gameObject.AddComponent<WaterSensor>();
		}
		if (mesh == null && hull is MeshCollider)
		{
			mesh = (hull as MeshCollider).sharedMesh;
		}
		triangles = mesh.triangles;
		meshVertices = mesh.vertices;
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < triangles.Length; i++)
		{
			Vector3 item = meshVertices[triangles[i]];
			int num = list.IndexOf(item);
			if (num < 0)
			{
				num = list.Count;
				list.Add(item);
			}
			triangles[i] = num;
		}
		meshVertices = list.ToArray();
		for (int j = 0; j < meshVertices.Length; j++)
		{
			meshVertices[j] = base.transform.InverseTransformPoint(hull.transform.TransformPoint(meshVertices[j]));
		}
		vertices = new FloatingVertex[meshVertices.Length];
	}

	private void ApplyForces()
	{
		if (triangles == null || triangles.Length == 0)
		{
			Start();
		}
		if (body.IsSleeping())
		{
			return;
		}
		TransformVertices(base.transform.localToWorldMatrix);
		center = body.worldCenterOfMass;
		resultantMoment = (resultantForce = (resultantStaticForce = Vector3.zero));
		for (int i = 0; i < triangles.Length; i += 3)
		{
			ApplyTriangleForces(vertices[triangles[i]], vertices[triangles[i + 1]], vertices[triangles[i + 2]]);
		}
		if (horizontalHydrostaticTreshold > 0f)
		{
			Vector3 vector = new Vector3(resultantStaticForce.x, 0f, resultantStaticForce.z);
			if (vector.magnitude < horizontalHydrostaticTreshold)
			{
				resultantForce -= vector;
			}
		}
		if (ignoreHydrodynamicForce != Vector3.zero)
		{
			Vector3 a = base.transform.TransformVector(ignoreHydrodynamicForce);
			float num = Vector3.Dot(resultantForce, a.normalized);
			if (num < 0f)
			{
				resultantForce -= num * a;
			}
		}
		resultantForce = Vector3.ClampMagnitude(resultantForce, body.mass * 100f);
		resultantMoment = Vector3.ClampMagnitude(resultantMoment, body.mass * 100f);
		if (float.IsNaN(resultantForce.x) || float.IsNaN(resultantForce.y) || float.IsNaN(resultantForce.z))
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Force NaN ");
			}
		}
		else
		{
			body.AddForce(resultantForce * FloatingMeshSync.skipFrames);
		}
		if (float.IsNaN(resultantForce.x) || float.IsNaN(resultantForce.y) || float.IsNaN(resultantForce.z))
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Torque NaN ");
			}
		}
		else
		{
			body.AddTorque(resultantMoment * FloatingMeshSync.skipFrames);
		}
	}

	private void TransformVertices(Matrix4x4 localToWorldMatrix)
	{
		float weight = 0f;
		Vector3 b = Current.Sample(body.worldCenterOfMass, out weight);
		for (int i = 0; i < meshVertices.Length; i++)
		{
			Vector3 vector = localToWorldMatrix.MultiplyPoint3x4(meshVertices[i]);
			Vector3 a = Vector3.zero;
			if (body != null)
			{
				a = body.GetPointVelocity(vector);
			}
			Vector3 velocity = Vector3.zero;
			float depth = -1f;
			if (sensor != null && sensor.waterBody != null)
			{
				depth = sensor.waterBody.SampleDepth(vector, out velocity);
				velocity = Vector3.Lerp(velocity, b, weight);
			}
			vertices[i] = new FloatingVertex
			{
				pos = vector,
				velocity = a - velocity,
				depth = depth
			};
		}
	}

	private void ApplyTriangleForces(FloatingVertex a, FloatingVertex b, FloatingVertex c)
	{
		if (a.depth <= 0f && b.depth <= 0f && c.depth <= 0f)
		{
			return;
		}
		Vector3 areaNormal = -Vector3.Cross(a.pos - b.pos, c.pos - b.pos) * 0.5f;
		FloatingVertex t;
		FloatingVertex m;
		FloatingVertex l;
		if (a.depth <= b.depth && a.depth <= c.depth)
		{
			t = a;
			if (b.depth < c.depth)
			{
				m = b;
				l = c;
			}
			else
			{
				m = c;
				l = b;
			}
		}
		else if (b.depth <= a.depth && b.depth <= c.depth)
		{
			t = b;
			if (a.depth < c.depth)
			{
				m = a;
				l = c;
			}
			else
			{
				m = c;
				l = a;
			}
		}
		else
		{
			t = c;
			if (a.depth < b.depth)
			{
				m = a;
				l = b;
			}
			else
			{
				m = b;
				l = a;
			}
		}
		FloatingVertex floatingVertex;
		if (a.depth >= 0f && b.depth >= 0f && c.depth >= 0f)
		{
			ApplySumbergedTriangleForces(t, m, l, areaNormal);
		}
		else if (m.depth <= 0f)
		{
			float d = (0f - l.depth) / (m.depth - l.depth);
			float d2 = (0f - l.depth) / (t.depth - l.depth);
			floatingVertex = default(FloatingVertex);
			floatingVertex.pos = l.pos + d2 * (t.pos - l.pos);
			floatingVertex.velocity = l.velocity + d2 * (t.velocity - l.velocity);
			floatingVertex.depth = 0f;
			FloatingVertex t2 = floatingVertex;
			floatingVertex = default(FloatingVertex);
			floatingVertex.pos = l.pos + d * (m.pos - l.pos);
			floatingVertex.velocity = l.velocity + d * (m.velocity - l.velocity);
			floatingVertex.depth = 0f;
			FloatingVertex m2 = floatingVertex;
			ApplySumbergedTriangleForces(t2, m2, l, areaNormal);
		}
		else
		{
			float d3 = (0f - t.depth) / (m.depth - t.depth);
			float d4 = (0f - t.depth) / (l.depth - t.depth);
			floatingVertex = default(FloatingVertex);
			floatingVertex.pos = t.pos + d4 * (l.pos - t.pos);
			floatingVertex.velocity = t.velocity + d4 * (l.velocity - t.velocity);
			floatingVertex.depth = 0f;
			FloatingVertex t3 = floatingVertex;
			floatingVertex = default(FloatingVertex);
			floatingVertex.pos = t.pos + d3 * (m.pos - t.pos);
			floatingVertex.velocity = t.velocity + d3 * (m.velocity - t.velocity);
			floatingVertex.depth = 0f;
			FloatingVertex floatingVertex2 = floatingVertex;
			ApplySumbergedTriangleForces(floatingVertex2, m, l, areaNormal);
			ApplySumbergedTriangleForces(t3, floatingVertex2, l, areaNormal);
		}
	}

	private void ApplySumbergedTriangleForces(FloatingVertex t, FloatingVertex m, FloatingVertex l, Vector3 areaNormal)
	{
		if (m.depth == t.depth || t.depth == l.depth)
		{
			TrianglePointingDown(t, m, l, areaNormal);
			return;
		}
		if (m.depth == l.depth)
		{
			TrianglePointingUp(t, m, l, areaNormal);
			return;
		}
		float d = (m.depth - l.depth) / (t.depth - l.depth);
		FloatingVertex floatingVertex = default(FloatingVertex);
		floatingVertex.pos = l.pos + d * (t.pos - l.pos);
		floatingVertex.velocity = l.velocity + d * (t.velocity - l.velocity);
		floatingVertex.depth = m.depth;
		FloatingVertex floatingVertex2 = floatingVertex;
		TrianglePointingUp(t, m, floatingVertex2, areaNormal);
		TrianglePointingDown(floatingVertex2, m, l, areaNormal);
	}

	private void DrawTriangleGizmos(FloatingVertex t, FloatingVertex m, FloatingVertex l)
	{
	}

	private void TrianglePointingDown(FloatingVertex t, FloatingVertex m, FloatingVertex l, Vector3 areaNormal)
	{
		Hydrostatic(t, m, l, areaNormal);
		Hydrodynamic(m, l, t, areaNormal);
	}

	private void TrianglePointingUp(FloatingVertex t, FloatingVertex m, FloatingVertex l, Vector3 areaNormal)
	{
		Hydrostatic(m, l, t, areaNormal);
		Hydrodynamic(m, l, t, areaNormal);
	}

	private void Hydrostatic(FloatingVertex a, FloatingVertex b, FloatingVertex c, Vector3 areaNormal)
	{
		Vector3 pos = c.pos;
		Vector3 a2 = (a.pos + b.pos) / 2f;
		Vector3 a3 = Math3d.ProjectPointOnLine(a.pos, (b.pos - a.pos).normalized, c.pos);
		float magnitude = (a3 - pos).magnitude;
		float magnitude2 = (a.pos - b.pos).magnitude;
		float depth = c.depth;
		float depth2 = a.depth;
		float num = density * 9.81f * magnitude2 * magnitude * (depth / 2f + (depth2 - depth) / 3f);
		float num2 = density * 9.81f * magnitude2 * magnitude * magnitude * (depth / 3f + (depth2 - depth) / 4f);
		float d = num2 / num;
		if (!(magnitude2 < 0.001f) && !(magnitude < 0.001f) && !(num < 0.0001f))
		{
			Vector3 force = (0f - num) * areaNormal.normalized;
			Vector3 pos2 = pos + (a2 - pos) * d / magnitude;
			AddForceAtPosition(force, pos2, isDynamic: false);
		}
	}

	private void Hydrodynamic(FloatingVertex a, FloatingVertex b, FloatingVertex c, Vector3 areaNormal)
	{
		if (!(body == null))
		{
			float magnitude = areaNormal.magnitude;
			Vector3 a2 = areaNormal / magnitude;
			magnitude /= 3f;
			FloatingVertex floatingVertex = a;
			Vector3 velocity = floatingVertex.velocity;
			float magnitude2 = velocity.magnitude;
			if (magnitude2 != 0f)
			{
				velocity /= magnitude2;
			}
			float num = velocity.x * a2.x + velocity.y * a2.y + velocity.z * a2.z;
			float num2 = 0f;
			if (bendWind > 0f)
			{
				a2 = (a2 - velocity * bendWind).normalized;
			}
			num2 = ((!(num > 0f)) ? ((suctionLinear * magnitude2 + suctionSquare * magnitude2 * magnitude2) * magnitude * ((falloffPower == 1f) ? (0f - num) : Mathf.Pow(0f - num, falloffPower))) : ((0f - (pressureLinear * magnitude2 + pressureSquare * magnitude2 * magnitude2)) * magnitude * ((falloffPower == 1f) ? num : Mathf.Pow(num, falloffPower))));
			AddForceAtPosition(num2 * a2, floatingVertex.pos, isDynamic: true);
			FloatingVertex floatingVertex2 = b;
			Vector3 velocity2 = floatingVertex2.velocity;
			float magnitude3 = velocity2.magnitude;
			if (magnitude3 != 0f)
			{
				velocity2 /= magnitude3;
			}
			float num3 = velocity2.x * a2.x + velocity2.y * a2.y + velocity2.z * a2.z;
			float num4 = 0f;
			if (bendWind > 0f)
			{
				a2 = (a2 - velocity2 * bendWind).normalized;
			}
			num4 = ((!(num3 > 0f)) ? ((suctionLinear * magnitude3 + suctionSquare * magnitude3 * magnitude3) * magnitude * ((falloffPower == 1f) ? (0f - num3) : Mathf.Pow(0f - num3, falloffPower))) : ((0f - (pressureLinear * magnitude3 + pressureSquare * magnitude3 * magnitude3)) * magnitude * ((falloffPower == 1f) ? num3 : Mathf.Pow(num3, falloffPower))));
			AddForceAtPosition(num4 * a2, floatingVertex2.pos, isDynamic: true);
			FloatingVertex floatingVertex3 = c;
			Vector3 velocity3 = floatingVertex3.velocity;
			float magnitude4 = velocity3.magnitude;
			if (magnitude4 != 0f)
			{
				velocity3 /= magnitude4;
			}
			float num5 = velocity3.x * a2.x + velocity3.y * a2.y + velocity3.z * a2.z;
			float num6 = 0f;
			if (bendWind > 0f)
			{
				a2 = (a2 - velocity3 * bendWind).normalized;
			}
			num6 = ((!(num5 > 0f)) ? ((suctionLinear * magnitude4 + suctionSquare * magnitude4 * magnitude4) * magnitude * ((falloffPower == 1f) ? (0f - num5) : Mathf.Pow(0f - num5, falloffPower))) : ((0f - (pressureLinear * magnitude4 + pressureSquare * magnitude4 * magnitude4)) * magnitude * ((falloffPower == 1f) ? num5 : Mathf.Pow(num5, falloffPower))));
			AddForceAtPosition(num6 * a2, floatingVertex3.pos, isDynamic: true);
		}
	}

	private void Hydrodynamic(FloatingVertex v, float surface, Vector3 normal)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Private Hydro dynamic ");
		}
		Vector3 velocity = v.velocity;
		float magnitude = velocity.magnitude;
		if (magnitude != 0f)
		{
			velocity /= magnitude;
		}
		float num = velocity.x * normal.x + velocity.y * normal.y + velocity.z * normal.z;
		float num2 = 0f;
		if (bendWind > 0f)
		{
			normal = (normal - velocity * bendWind).normalized;
		}
		num2 = ((!(num > 0f)) ? ((suctionLinear * magnitude + suctionSquare * magnitude * magnitude) * surface * ((falloffPower == 1f) ? (0f - num) : Mathf.Pow(0f - num, falloffPower))) : ((0f - (pressureLinear * magnitude + pressureSquare * magnitude * magnitude)) * surface * ((falloffPower == 1f) ? num : Mathf.Pow(num, falloffPower))));
		AddForceAtPosition(num2 * normal, v.pos, isDynamic: true);
	}

	private void AddForceAtPosition(Vector3 force, Vector3 pos, bool isDynamic)
	{
		resultantMoment += Vector3.Cross(pos - center, force);
		resultantForce += force;
		if (float.IsNaN(resultantMoment.x) || float.IsNaN(resultantMoment.y) || float.IsNaN(resultantMoment.z))
		{
			if (showDebug)
			{
				Debug.Log(base.name + " resultantMoment is NaN ");
			}
			resultantMoment = Vector3.zero;
		}
		else if (!isDynamic)
		{
			resultantStaticForce += force;
		}
	}

	private void FixedUpdate()
	{
		if (!ReplayRecorder.isPlaying && !NetGame.isClient && index % FloatingMeshSync.skipFrames == FloatingMeshSync.frame % FloatingMeshSync.skipFrames)
		{
			ApplyForces();
		}
	}
}
