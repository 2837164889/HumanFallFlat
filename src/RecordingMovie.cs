using UnityEngine;

public class RecordingMovie : MonoBehaviour
{
	public bool disableHuman;

	public AudioClip soundtrack;

	private RecordingManager recordingManager;

	private Camera playerCam;

	private RecordingMovieCamera[] cameraPositions;

	private RecordingMovieClip[] clips;

	public bool playing;

	public int currentFrame;

	public int startOffset;

	public float playbackStarted;

	public float lastTime;

	public float currentTime;

	public Animation recordedAnimation;

	public int animationFrame;

	public int fileFrame;

	private void Start()
	{
		playerCam = Camera.main;
		cameraPositions = GetComponentsInChildren<RecordingMovieCamera>();
		clips = GetComponentsInChildren<RecordingMovieClip>();
		recordingManager = Object.FindObjectOfType<RecordingManager>();
	}

	private void LateUpdate()
	{
		if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			if (recordedAnimation != null)
			{
				animationFrame++;
				ScreenCapture.CaptureScreenshot("RealSenseA" + fileFrame.ToString("00") + ".png");
				recordedAnimation.clip.SampleAnimation(recordedAnimation.gameObject, 1f * (float)animationFrame / 60f);
				fileFrame++;
			}
			else
			{
				ScreenCapture.CaptureScreenshot("Screenshot.png");
			}
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			GetComponent<AudioSource>().PlayOneShot(soundtrack);
		}
		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			playbackStarted = Time.time;
			lastTime = 0f;
			playerCam.orthographic = true;
			playerCam.GetComponent<CameraController3>().enabled = false;
			playing = true;
			currentFrame = startOffset;
			AudioSource component = GetComponent<AudioSource>();
			if (component != null)
			{
				component.clip = soundtrack;
				component.timeSamples = soundtrack.frequency * startOffset / 60;
				component.Play();
			}
			if (disableHuman)
			{
				Human.instance.gameObject.SetActive(value: false);
			}
		}
		if (!playing)
		{
			return;
		}
		currentTime = Time.time - playbackStarted;
		for (int i = 0; i < clips.Length; i++)
		{
			if (lastTime <= clips[i].beginOnTime && clips[i].beginOnTime < currentTime)
			{
				Object.FindObjectOfType<RecordingManager>().BeginPlayback(clips[i].bytes, clips[i].startTime);
			}
		}
		lastTime = currentTime;
		for (int j = 0; j < cameraPositions.Length; j++)
		{
			if (cameraPositions[j].startTime <= currentTime && cameraPositions[j].endTime >= currentTime)
			{
				PlaceCam(cameraPositions[j]);
				currentFrame++;
				return;
			}
		}
		RecordingMovieCamera startCam = null;
		for (int k = 0; k < cameraPositions.Length; k++)
		{
			if (cameraPositions[k].endTime < currentTime)
			{
				startCam = cameraPositions[k];
			}
		}
		RecordingMovieCamera endCam = null;
		for (int num = cameraPositions.Length - 1; num >= 0; num--)
		{
			if (cameraPositions[num].startTime > currentTime)
			{
				endCam = cameraPositions[num];
			}
		}
		InterpolateCam(startCam, endCam);
		currentFrame++;
	}

	private void InterpolateCam(RecordingMovieCamera startCam, RecordingMovieCamera endCam)
	{
		if (!(startCam == null) && !(endCam == null))
		{
			float value = (1f * currentTime - 1f * startCam.endTime) / (1f * endCam.startTime - 1f * startCam.endTime);
			float t = Ease.easeInOutQuad(0f, 1f, value);
			playerCam.transform.rotation = Quaternion.Lerp(startCam.transform.rotation, endCam.transform.rotation, t);
			Vector3 a = Vector3.Lerp(startCam.transform.position, endCam.transform.position, t);
			playerCam.transform.position = a - playerCam.transform.forward * Mathf.Lerp(startCam.dist, endCam.dist, t);
			playerCam.orthographicSize = Mathf.Lerp(startCam.ortographicsSize, endCam.ortographicsSize, t);
			playerCam.nearClipPlane = Mathf.Lerp(startCam.nearClip, endCam.nearClip, t);
			playerCam.farClipPlane = Mathf.Lerp(startCam.farClip, endCam.farClip, t);
		}
	}

	private void PlaceCam(RecordingMovieCamera cam)
	{
		playerCam.transform.rotation = cam.transform.rotation;
		Vector3 position = cam.transform.position;
		playerCam.transform.position = position - playerCam.transform.forward * cam.dist;
		playerCam.orthographicSize = cam.ortographicsSize;
		playerCam.nearClipPlane = cam.nearClip;
		playerCam.farClipPlane = cam.farClip;
	}
}
