using Multiplayer;
using UnityEngine;

namespace HumanAPI.LightLevel
{
	[RequireComponent(typeof(LightConsume), typeof(NetBody))]
	public class MeltingObject : Node, IReset
	{
		public float meltingSpeed = 1f;

		public float RespawnDelay;

		private NetBody netbody;

		public NodeInput lightIntensity;

		public NodeOutput output;

		[SerializeField]
		private GameObject translateObject;

		[SerializeField]
		private float translateDistance;

		private LightConsume consume;

		private Vector3 startingHeight;

		private float startingMass;

		private Rigidbody rb;

		private float timeMelting;

		private void Awake()
		{
			startingHeight = base.transform.localScale;
			netbody = GetComponent<NetBody>();
			consume = GetComponent<LightConsume>();
			rb = GetComponent<Rigidbody>();
			startingMass = rb.mass;
		}

		private void DoTranslate()
		{
			Vector3 localPosition = translateObject.transform.localPosition;
			Vector3 localScale = base.transform.localScale;
			localPosition.y = (0f - (1f - localScale.z)) * translateDistance;
			translateObject.transform.localPosition = localPosition;
		}

		private void FixedUpdate()
		{
			if (!NetGame.isClient && !(lightIntensity.value < 0.5f))
			{
				Vector3 vector = base.transform.InverseTransformDirection(Vector3.up);
				Vector3 vector2 = new Vector3((0f - lightIntensity.value) * meltingSpeed * Time.fixedDeltaTime * Mathf.Abs(vector.x), (0f - lightIntensity.value) * meltingSpeed * Time.fixedDeltaTime * Mathf.Abs(vector.y), (0f - lightIntensity.value) * meltingSpeed * Time.fixedDeltaTime * Mathf.Abs(vector.z));
				Vector3 localScale = base.transform.localScale + vector2;
				AdjustHandAnchors(base.transform.TransformVector(vector2));
				base.transform.localScale = localScale;
				if (translateObject != null)
				{
					DoTranslate();
				}
				Vector3 vector3 = new Vector3(localScale.x / startingHeight.x, localScale.y / startingHeight.y, localScale.z / startingHeight.z);
				rb.mass = (vector3.x + vector3.y + vector3.z) / 3f * startingMass;
				float num = Mathf.Min(vector3.x, vector3.y, vector3.z);
				output.SetValue(1f - num);
				if (output.value >= 0.99f)
				{
					Melted();
				}
				consume.RecalculateAll();
			}
		}

		private void Melted()
		{
			base.gameObject.SetActive(value: false);
			if (netbody.respawn)
			{
				Invoke("Respawn", RespawnDelay);
			}
		}

		public void ResetScale()
		{
			base.transform.localScale = startingHeight;
			rb.mass = startingMass;
			output.SetValue(0f);
		}

		private void Respawn()
		{
			ResetScale();
			base.gameObject.SetActive(value: true);
			netbody.Respawn();
		}

		public void ResetState(int checkpoint, int subObjectives)
		{
			ResetScale();
		}

		public void AdjustHandAnchors(Vector3 scaleDif)
		{
			for (int i = 0; i < Human.all.Count; i++)
			{
				Human human = Human.all[i];
				AdjustHandAnchors(human.ragdoll.partLeftHand.sensor.grabJoint, scaleDif);
				AdjustHandAnchors(human.ragdoll.partRightHand.sensor.grabJoint, scaleDif);
			}
		}

		public void AdjustHandAnchors(ConfigurableJoint joint, Vector3 scaleDif)
		{
			if (!(joint == null) && !(joint.connectedBody != rb))
			{
				joint.autoConfigureConnectedAnchor = false;
				joint.connectedAnchor -= joint.transform.InverseTransformVector(scaleDif);
			}
		}
	}
}
