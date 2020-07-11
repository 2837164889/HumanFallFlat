using UnityEngine;

public class LockCursorEditor : MonoBehaviour
{
	[SerializeField]
	private bool LockCursor = true;

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown("l"))
		{
			Cursor.visible = !Cursor.visible;
			if (Cursor.visible)
			{
				Cursor.lockState = CursorLockMode.None;
			}
			else
			{
				Cursor.lockState = CursorLockMode.Locked;
			}
		}
	}
}
