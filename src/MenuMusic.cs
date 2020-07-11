using HumanAPI;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{
	private bool canPlayMenuTheme = true;

	public float droneMinTime = 30f;

	public Music menuMusic1;

	public Music menuMusic2;

	public string[] menuSongList;

	public string[] songList;

	public float minPause = 10f;

	public float maxPause = 60f;

	private float pause;

	public ProceduralDroneMix drones;

	private bool shuffle = true;

	private void Update()
	{
	}
}
