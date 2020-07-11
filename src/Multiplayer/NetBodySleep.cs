using System;
using UnityEngine;

namespace Multiplayer
{
	public class NetBodySleep
	{
		private Rigidbody body;

		private Quaternion freezeRot;

		private Vector3 freezePos;

		private int framesFrozen;

		private float freezeDrag = 1f;

		private float freezeDragAngular = 0.5f;

		private float sleepTreshold = 0.005f;

		private float dampFrom = 0.005f;

		private float dampTo = 0.05f;

		private float maxDrag = 0.5f;

		private float maxAngularDrag = 0.5f;

		private float drag;

		private float angularDrag;

		private float mass;

		private float momentumOverMass;

		private float energyOverMass;

		private Vector3 inertia;

		private Quaternion inertiaRotation;

		public NetBodySleep(Rigidbody body)
		{
			this.body = body;
			mass = body.mass;
			float num = 5f / mass;
			sleepTreshold *= num;
			dampFrom *= num;
			dampTo *= num;
			sleepTreshold = Mathf.Max(sleepTreshold, 0.001f);
			float num2 = 2f / Mathf.Sqrt(mass);
			freezeDrag = drag + (freezeDrag - drag) * num2;
			freezeDragAngular = angularDrag + (freezeDragAngular - angularDrag) * num2;
			freezeDrag = Mathf.Min(freezeDrag, maxDrag);
			freezeDragAngular = Mathf.Min(freezeDragAngular, maxAngularDrag);
			drag = body.drag;
			angularDrag = body.angularDrag;
			freezeDrag = Mathf.Max(freezeDrag, drag);
			freezeDragAngular = Mathf.Max(freezeDragAngular, angularDrag);
			NetBodySleepOverride componentInParent = body.GetComponentInParent<NetBodySleepOverride>();
			if (componentInParent != null)
			{
				sleepTreshold = componentInParent.sleepTreshold;
				dampFrom = componentInParent.dampFrom;
				dampTo = componentInParent.dampTo;
				freezeDrag = componentInParent.freezeDrag;
				freezeDragAngular = componentInParent.freezeDragAngular;
			}
			body.sleepThreshold = sleepTreshold;
			inertia = body.inertiaTensor;
			inertiaRotation = body.inertiaTensorRotation;
		}

		public void HandleSleep()
		{
			if (!NetGame.isClient && !body.IsSleeping())
			{
				Vector3 position = body.position;
				Quaternion rotation = body.rotation;
				float num = (position - freezePos).sqrMagnitude / Time.fixedDeltaTime;
				(rotation * Quaternion.Inverse(freezeRot)).ToAngleAxis(out float angle, out Vector3 axis);
				axis *= angle / Time.fixedDeltaTime * ((float)Math.PI / 180f);
				Vector3 point = body.transform.InverseTransformVector(axis);
				Vector3 b = Quaternion.Inverse(inertiaRotation) * point;
				float sqrMagnitude = Vector3.Scale(inertia, b).sqrMagnitude;
				energyOverMass = num * 0.5f + sqrMagnitude / mass * 0.5f;
				float num2 = Mathf.InverseLerp(dampTo, dampFrom, energyOverMass);
				body.drag = Mathf.Lerp(drag, freezeDrag, num2);
				body.angularDrag = Mathf.Lerp(angularDrag, freezeDragAngular, num2 * num2);
				freezePos = position;
				freezeRot = rotation;
			}
		}
	}
}
