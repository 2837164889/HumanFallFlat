using UnityEngine;

public class BuildPaddedDebug : MonoBehaviour
{
	private void Start()
	{
		GetComponent<MeshFilter>().sharedMesh = BuildPaddedMesh.GeneratePadded(GetComponent<MeshFilter>().sharedMesh, 0.1f);
	}
}
