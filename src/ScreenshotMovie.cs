using System.IO;
using UnityEngine;

public class ScreenshotMovie : MonoBehaviour
{
	public string folder = "ScreenshotMovieOutput";

	public int frameRate = 60;

	public int sizeMultiplier = 1;

	private string realFolder = string.Empty;

	private bool recording;

	private int frame;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl))
		{
			if (!recording)
			{
				Time.captureFramerate = frameRate;
				realFolder = Path.Combine(Application.persistentDataPath, folder);
				int num = 1;
				while (Directory.Exists(realFolder))
				{
					realFolder = Path.Combine(Application.persistentDataPath, folder + num);
					num++;
				}
				Directory.CreateDirectory(realFolder);
				recording = true;
				frame = 0;
			}
			else
			{
				Time.captureFramerate = 0;
				recording = false;
			}
		}
		if (recording)
		{
			string filename = $"{realFolder}/shot {frame++:D04}.png";
			ScreenCapture.CaptureScreenshot(filename, sizeMultiplier);
		}
	}
}
