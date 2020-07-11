using UnityEngine;

public class HumanSegment
{
	public Transform transform;

	public Collider collider;

	public Rigidbody rigidbody;

	public Quaternion startupRotation;

	public CollisionSensor sensor;

	public Transform skeleton;

	public HumanSegment parent;

	public Matrix4x4 bindPose;
}
