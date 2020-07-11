using UnityEngine;

public class RigClipVolume : RigVolumeMesh
{
	public void Clip(bool[] canClip, Vector3[] vertices)
	{
		Build(1f);
		for (int i = 0; i < vertices.Length; i++)
		{
			if (!canClip[i])
			{
				float distInside = GetDistInside(vertices[i]);
				canClip[i] = (distInside > 0f);
			}
		}
	}
}
