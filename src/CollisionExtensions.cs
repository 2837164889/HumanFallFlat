using HumanAPI;
using UnityEngine;

public static class CollisionExtensions
{
	public static void Analyze(this Collision collision, out Vector3 pos, out float impulse, out float normalVelocity, out float tangentVelocity, out PhysicMaterial mat1, out PhysicMaterial mat2, out int id2, out float volume2, out float pitch2)
	{
		ContactPoint[] contacts = collision.contacts;
		id2 = int.MaxValue;
		volume2 = 1f;
		pitch2 = 1f;
		mat1 = (mat2 = null);
		normalVelocity = 0f;
		tangentVelocity = 0f;
		pos = collision.contacts[0].point;
		Collider collider = null;
		int num = 0;
		Vector3 relativeVelocity = collision.relativeVelocity;
		for (int i = 0; i < contacts.Length; i++)
		{
			ContactPoint contactPoint = contacts[i];
			float num2 = Vector3.Dot(contactPoint.normal, relativeVelocity);
			float magnitude = (relativeVelocity - contactPoint.normal * num2).magnitude;
			if (num2 > normalVelocity)
			{
				normalVelocity = num2;
				tangentVelocity = magnitude;
				pos = contactPoint.point;
				mat1 = contactPoint.thisCollider.sharedMaterial;
				mat2 = contactPoint.otherCollider.sharedMaterial;
				collider = contactPoint.otherCollider;
			}
			num++;
		}
		impulse = collision.impulse.magnitude;
		if (collider != null)
		{
			CollisionAudioSensor componentInParent = collider.GetComponentInParent<CollisionAudioSensor>();
			if (componentInParent != null)
			{
				id2 = componentInParent.id;
				volume2 = componentInParent.volume;
				pitch2 = componentInParent.pitch;
			}
		}
	}

	public static Vector3 GetNormalTangentVelocitiesAndImpulse(this Collision collision, Rigidbody thisBody)
	{
		ContactPoint[] contacts = collision.contacts;
		float num = 0f;
		float y = 0f;
		int num2 = 0;
		Vector3 relativeVelocity = collision.relativeVelocity;
		for (int i = 0; i < contacts.Length; i++)
		{
			ContactPoint contactPoint = contacts[i];
			float num3 = Vector3.Dot(contactPoint.normal, relativeVelocity);
			float magnitude = (relativeVelocity - contactPoint.normal * num3).magnitude;
			if (num3 > num)
			{
				num = num3;
				y = magnitude;
			}
			num2++;
		}
		return new Vector3(num, y, collision.impulse.magnitude);
	}

	public static Vector3 GetPoint(this Collision collision)
	{
		return collision.contacts[0].point;
	}

	public static Vector3 GetImpulse(this Collision collision)
	{
		Vector3 impulse = collision.impulse;
		if (Vector3.Dot(impulse, collision.contacts[0].normal) < 0f)
		{
			return -impulse;
		}
		return impulse;
	}
}
