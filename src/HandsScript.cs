using UnityEngine;

public class HandsScript : MonoBehaviour
{
	public GameObject box1;

	public GameObject box2;

	private void Update()
	{
		if (Game.GetKeyDown(KeyCode.Keypad0))
		{
			box1.tag = "Target";
			box2.tag = "Target";
		}
	}
}
