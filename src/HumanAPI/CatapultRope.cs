using System;
using UnityEngine;

namespace HumanAPI
{
	public class CatapultRope : RopeRender
	{
		public Transform target;

		public float windThickness = 0.1f;

		public float windCoreRadius = 0.15f;

		private float totalLen = 6.5f;

		private float windRadius;

		private float windAngle;

		private Vector3 localTargetPos;

		private Vector3 exitWindPos;

		private Vector3 straightNormal;

		private Vector3 straightBinormal;

		private float straightT;

		public float catapultWindCount;

		private float dataWindCount;

		public bool autoWind;

		public bool isDynamic;

		private Vector3 initialWorldVectorX;

		public Rigidbody attachedBody;

		public Rigidbody wheel;

		public bool applyForceToWheel = true;

		public bool applyForceToAttachedBody = true;

		private float lengthChange;

		private Vector3 prevAttachedBodyPos;

		[Tooltip("Use this in order to show the prints coming from the script")]
		public bool showDebug;

		public override void CheckDirty()
		{
			base.CheckDirty();
			if (isDynamic || dataWindCount != catapultWindCount)
			{
				isDirty = true;
			}
		}

		public override void OnEnable()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Enable ");
			}
			if (autoWind)
			{
				totalLen = float.MaxValue;
				initialWorldVectorX = base.transform.TransformDirection(new Vector3(1f, 0f, 0f));
				windAngle = 0f;
			}
			if (attachedBody != null)
			{
				prevAttachedBodyPos = attachedBody.transform.position;
			}
			base.OnEnable();
		}

		public override void ReadData()
		{
			if (autoWind)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Auto Wind Compare vectors");
				}
				Vector3 referenceVector = base.transform.TransformDirection(new Vector3(1f, 0f, 0f));
				Vector3 normal = base.transform.TransformDirection(new Vector3(0f, 0f, 1f));
				float num;
				for (num = Math3d.SignedVectorAngle(referenceVector, initialWorldVectorX, normal); num - windAngle < -180f; num += 360f)
				{
				}
				while (num - windAngle > 180f)
				{
					num -= 360f;
				}
				windAngle = num;
				catapultWindCount = windAngle / 360f;
			}
			dataWindCount = catapultWindCount;
			windRadius = windCoreRadius + radius;
			localTargetPos = base.transform.InverseTransformPoint(target.position);
			Vector2 vector = localTargetPos.SetZ(0f);
			float num2 = Mathf.Acos(windRadius / vector.magnitude);
			Vector2 vector2 = vector.Rotate(0f - num2).normalized * windRadius;
			float num3 = Math2d.NormalizeAnglePositive(Math2d.SignedAngle(new Vector2(0f, 1f), vector2) * ((float)Math.PI / 180f));
			float num4 = num3 / ((float)Math.PI * 2f);
			int num5 = Mathf.RoundToInt(catapultWindCount - num4);
			float num6 = num4 + (float)num5 + 2f;
			exitWindPos = new Vector3(vector2.x, vector2.y, num6 * windThickness);
			if (autoWind)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Auto Wind Calc Total Length");
				}
				straightT = 0.9f;
				float num7 = num6 * 2f * (float)Math.PI * windRadius / straightT;
				lengthChange = totalLen - num7;
				totalLen = num7;
			}
			else
			{
				straightT = num6 * 2f * (float)Math.PI * windRadius / totalLen;
			}
			straightNormal = vector2.normalized;
			straightBinormal = Vector3.Cross((vector - vector2).normalized, straightNormal);
		}

		public override void GetPoint(float dist, out Vector3 pos, out Vector3 normal, out Vector3 binormal)
		{
			if (dist < straightT)
			{
				float num = dist * totalLen / windRadius;
				float num2 = Mathf.Sin(num);
				float y = Mathf.Cos(num);
				normal = new Vector3(0f - num2, y, 0f);
				binormal = new Vector3(0f, 0f, -1f);
				pos = normal * windRadius + new Vector3(0f, 0f, num / 2f / (float)Math.PI * windThickness);
			}
			else
			{
				float d = (dist - straightT) / (1f - straightT);
				pos = d * (localTargetPos - exitWindPos) + exitWindPos;
				normal = straightNormal;
				binormal = straightBinormal;
			}
		}

		public override void LateUpdate()
		{
			base.LateUpdate();
			if (applyForceToWheel && attachedBody != null && wheel != null)
			{
				if (prevAttachedBodyPos != attachedBody.transform.position)
				{
					Vector3 b = base.transform.TransformPoint(exitWindPos);
					Vector3 a = base.transform.TransformPoint(localTargetPos);
					float num = Vector3.Dot(rhs: (a - b).normalized, lhs: (attachedBody.transform.position - prevAttachedBodyPos).normalized);
					float num2 = (attachedBody.transform.position - prevAttachedBodyPos).magnitude * num;
					if (num2 > 0f)
					{
						float num3 = (float)Math.PI * 2f * (radius + windCoreRadius);
						float d = 360f * (num2 / num3);
						Quaternion rotation = wheel.transform.rotation;
						rotation *= Quaternion.Euler(Vector3.up * d);
						wheel.GetComponent<Rigidbody>().MoveRotation(rotation);
					}
				}
				prevAttachedBodyPos = attachedBody.transform.position;
			}
			if (applyForceToAttachedBody && wheel != null && lengthChange < 0f)
			{
				Vector3 b2 = base.transform.TransformPoint(exitWindPos);
				Vector3 a2 = base.transform.TransformPoint(localTargetPos);
				attachedBody.MovePosition(attachedBody.transform.position + (a2 - b2).normalized * lengthChange);
			}
		}
	}
}
