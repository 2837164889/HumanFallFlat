using UnityEngine;

public class WaterPlane : WaterBody
{
	[Tooltip("Reference to the down vector for this game object")]
	public Vector3 down;

	[Tooltip("Reference to the direction the water is moving in")]
	public Vector3 flow;

	[Tooltip("Reference to the direction all water should be moving in")]
	public Vector3 globalFlow;

	private float D;

	private Vector3 normal;

	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	[Tooltip("Water height changes runtime")]
	public bool dynamicHeight;

	[SerializeField]
	private bool SetPlaneHeight;

	[SerializeField]
	private float PlaneHeight;

	private void Start()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Setting up the directions for stuff ");
		}
		normal = base.transform.TransformDirection(down);
		Vector3 lhs = (!SetPlaneHeight || dynamicHeight) ? base.transform.position : new Vector3(0f, PlaneHeight, 0f);
		D = Vector3.Dot(lhs, normal);
		globalFlow = base.transform.TransformVector(flow);
	}

	public override float SampleDepth(Vector3 pos, out Vector3 velocity)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Getting the direction we should be moving in ");
		}
		velocity = globalFlow;
		if (dynamicHeight)
		{
			D = Vector3.Dot(base.transform.position, normal);
		}
		return Vector3.Dot(pos, normal) - D;
	}
}
