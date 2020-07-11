using UnityEngine;

namespace InterfaceMovement
{
	public class ButtonFocus : MonoBehaviour
	{
		private void Update()
		{
			Button focusedButton = base.transform.parent.GetComponent<ButtonManager>().focusedButton;
			base.transform.position = Vector3.MoveTowards(base.transform.position, focusedButton.transform.position, Time.deltaTime * 10f);
		}
	}
}
