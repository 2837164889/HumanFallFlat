using UnityEngine;

public class RecordingMovieClip : MonoBehaviour
{
	public TextAsset asset;

	public byte[] bytes;

	public int beginOnFrame;

	public float beginOnTime;

	public float startTime;

	public int startFrame;

	private void OnEnable()
	{
		bytes = asset.bytes;
	}
}
