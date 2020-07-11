using Multiplayer;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SnowBoard : MonoBehaviour, IGrabbable, IPreReset, IRespawnable, IPreRespawn
{
	[Serializable]
	public class Binding
	{
		public static List<Binding> all = new List<Binding>();

		public Collider col;

		[ReadOnly]
		public Joint joint;

		[HideInInspector]
		public SnowBoard body;

		public Human boundTo;

		public AudioSource SFXConnect;

		public AudioSource SFXDisconnect;

		private bool isFixed;

		private const float accumulatedRelease = 2f;

		private float currentAccumulation;

		public bool occupied => joint != null;

		public void Init(SnowBoard parent)
		{
			body = parent;
			all.Add(this);
		}

		public void FixedUpdate(float forceRelief)
		{
			if (joint == null)
			{
				return;
			}
			if (isFixed)
			{
				float num = Vector3.Dot(body.transform.forward, -joint.transform.up);
				bool flag = body.grabbedCount >= 1;
				currentAccumulation = Mathf.MoveTowards(currentAccumulation, (!flag) ? 0f : 2f, Time.fixedDeltaTime + 1f * forceRelief);
				if (currentAccumulation >= 2f)
				{
					Disconnect();
					SFXDisconnect.PlayOneShot(SFXDisconnect.clip);
				}
			}
			else if ((joint.transform.position - col.transform.position).magnitude < 0.15f)
			{
				AttachFixed();
				SFXConnect.PlayOneShot(SFXConnect.clip);
			}
		}

		public void AttachSpring(GameObject foot, Human human)
		{
			foreach (Binding item in all)
			{
				if (item.joint != null && item.joint.gameObject == foot)
				{
					return;
				}
			}
			SpringJoint springJoint = foot.AddComponent<SpringJoint>();
			springJoint.spring = 10000f;
			springJoint.damper = 141.42f;
			springJoint.breakForce = 6000f;
			springJoint.breakTorque = 300f;
			springJoint.connectedBody = body.body;
			springJoint.autoConfigureConnectedAnchor = false;
			springJoint.anchor = Vector3.zero;
			springJoint.connectedAnchor = body.transform.InverseTransformPoint(col.transform.position);
			isFixed = false;
			boundTo = human;
			joint = springJoint;
		}

		public void AttachFixed()
		{
			GameObject gameObject = joint.gameObject;
			UnityEngine.Object.Destroy(joint);
			ConfigurableJoint configurableJoint = gameObject.AddComponent<ConfigurableJoint>();
			ConfigurableJointMotion configurableJointMotion2 = configurableJoint.zMotion = ConfigurableJointMotion.Locked;
			configurableJointMotion2 = (configurableJoint.xMotion = (configurableJoint.yMotion = configurableJointMotion2));
			configurableJointMotion2 = (configurableJoint.angularZMotion = ConfigurableJointMotion.Free);
			configurableJointMotion2 = (configurableJoint.angularXMotion = (configurableJoint.angularYMotion = configurableJointMotion2));
			configurableJoint.anchor = Vector3.zero;
			configurableJoint.connectedBody = body.body;
			configurableJoint.autoConfigureConnectedAnchor = false;
			configurableJoint.connectedAnchor = body.transform.InverseTransformPoint(col.transform.position);
			configurableJoint.breakForce = 15000f;
			configurableJoint.breakTorque = 10000f;
			isFixed = true;
			joint = configurableJoint;
		}

		public void Disconnect()
		{
			boundTo = null;
			if (joint != null)
			{
				UnityEngine.Object.DestroyImmediate(joint);
				joint = null;
			}
		}
	}

	public Binding[] bindings;

	public Rigidbody body;

	public Vector3 breakNormal = Vector3.down;

	[HideInInspector]
	public int grabbedCount;

	private RaycastHit[] info = new RaycastHit[2];

	private bool grabbed;

	private void Start()
	{
		if (!body)
		{
			body = GetComponent<Rigidbody>();
		}
		Binding[] array = bindings;
		foreach (Binding binding in array)
		{
			binding.Init(this);
		}
	}

	private void FixedUpdate()
	{
		bool flag = false;
		grabbedCount = 0;
		float num = 0f;
		if (grabbed)
		{
			for (int i = 0; i < Human.all.Count; i++)
			{
				Human human = Human.all[i];
				human.grabManager.grabbedObjects.ForEach(delegate(GameObject g)
				{
					if (base.gameObject == g)
					{
						grabbedCount++;
					}
				});
				num += GetForceRelief(human, human.ragdoll.partLeftHand) + GetForceRelief(human, human.ragdoll.partRightHand);
			}
		}
		Binding[] array = bindings;
		foreach (Binding binding in array)
		{
			if (binding.occupied)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		Binding[] array2 = bindings;
		foreach (Binding binding2 in array2)
		{
			binding2.FixedUpdate(num);
		}
		Vector3 velocity = body.velocity;
		Vector3 vector = velocity;
		vector.Scale(base.transform.up);
		Vector3 vector2 = velocity;
		vector2.Scale(base.transform.right);
		if (Physics.RaycastNonAlloc(new Ray(base.transform.position, Vector3.down), info, 1f) <= 0)
		{
			return;
		}
		Vector3 vector3 = default(Vector3);
		for (int l = 0; l < info.Length; l++)
		{
			if (!(info[l].transform == body.transform) && !(info[l].transform == null) && info[l].transform.gameObject.layer != 9)
			{
				if (vector2.magnitude < vector.magnitude)
				{
					float num2 = Vector3.Dot(info[l].normal, -base.transform.forward);
					num2 += 1f;
					Debug.DrawLine(info[l].point, info[l].point - body.velocity * num2 * 1000f * Time.fixedDeltaTime, Color.green);
					body.AddForceAtPosition(-body.velocity * num2 * 200f, info[l].point);
				}
				float num3 = Vector3.Dot(body.transform.right, body.velocity.normalized);
				float num4 = Mathf.Lerp(1f, 0f, Mathf.Abs(num3));
				Vector3 velocity2 = body.velocity;
				float x = velocity2.x;
				Vector3 velocity3 = body.velocity;
				vector3 = new Vector3(x, 0f, velocity3.z);
				vector3.Scale(body.velocity);
				num4 *= vector3.magnitude / 5f;
				float num5 = 0f;
				num5 = ((!(num3 < 0f)) ? Vector3.SignedAngle(body.transform.right, body.velocity, -body.transform.forward) : Vector3.SignedAngle(body.transform.right, body.velocity, body.transform.forward));
				body.SafeAddTorque(-body.transform.forward * num4 * num5);
			}
		}
	}

	private float GetForceRelief(Human human, HumanSegment hand)
	{
		if (!hand.sensor.IsGrabbed(base.gameObject))
		{
			return 0f;
		}
		Vector3 normalized = base.transform.TransformVector(breakNormal).normalized;
		float b = Mathf.InverseLerp(0.5f, 0.9f, Vector3.Dot(normalized, human.controls.walkDirection));
		return Mathf.Max(0f, b);
	}

	private void OnDrowning(Human human)
	{
		Binding[] array = bindings;
		foreach (Binding binding in array)
		{
			if (binding.boundTo != null && binding.boundTo.Equals(human))
			{
				Debug.Log("Disconnect");
				Disconnect();
			}
		}
	}

	private void OnEnable()
	{
		Game.OnDrowning = (Action<Human>)Delegate.Remove(Game.OnDrowning, new Action<Human>(OnDrowning));
		Game.OnDrowning = (Action<Human>)Delegate.Combine(Game.OnDrowning, new Action<Human>(OnDrowning));
	}

	private void OnDisable()
	{
		Game.OnDrowning = (Action<Human>)Delegate.Remove(Game.OnDrowning, new Action<Human>(OnDrowning));
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!(other.gameObject.GetComponent<FootCollisionAudioSensor>() != null))
		{
			return;
		}
		Binding binding = FindClosest(other);
		if (binding != null && !binding.occupied && grabbedCount < 1)
		{
			Human componentInParent = other.GetComponentInParent<Human>();
			binding.AttachSpring(other.gameObject, componentInParent);
			if (componentInParent != null && SnowboardAchievement.instance != null)
			{
				SnowboardAchievement.instance.RegisterAttach(this, componentInParent);
			}
			if (!allBindingsUsed())
			{
			}
		}
	}

	private bool allBindingsUsed()
	{
		bool result = true;
		Binding[] array = bindings;
		foreach (Binding binding in array)
		{
			if (!binding.occupied)
			{
				result = false;
			}
		}
		return result;
	}

	public void CreateHipsSpring(GameObject ball)
	{
	}

	private Binding FindClosest(Collider feet)
	{
		Binding[] array = bindings;
		foreach (Binding binding in array)
		{
			if (Vector3.Distance(binding.col.transform.position, feet.transform.position) < 0.2f)
			{
				return binding;
			}
		}
		return null;
	}

	public void OnGrab()
	{
		grabbed = true;
	}

	public void OnRelease()
	{
		grabbed = false;
	}

	public void Disconnect()
	{
		Binding[] array = bindings;
		foreach (Binding binding in array)
		{
			binding.Disconnect();
		}
	}

	public void PreResetState(int checkpoint)
	{
		Disconnect();
	}

	public void Respawn(Vector3 offset)
	{
		Disconnect();
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(base.transform.position, base.transform.position + Vector3.down);
	}

	void IPreRespawn.PreRespawn()
	{
		Disconnect();
	}
}
