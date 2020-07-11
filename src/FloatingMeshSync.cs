using UnityEngine;

public class FloatingMeshSync : MonoBehaviour
{
	public static int frame;

	public static int skipFrames = 1;

	private void FixedUpdate()
	{
		frame++;
	}
}
