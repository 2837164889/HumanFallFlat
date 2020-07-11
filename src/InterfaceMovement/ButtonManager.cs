using InControl;
using UnityEngine;

namespace InterfaceMovement
{
	public class ButtonManager : MonoBehaviour
	{
		public Button focusedButton;

		private TwoAxisInputControl filteredDirection;

		private void Awake()
		{
			filteredDirection = new TwoAxisInputControl();
			filteredDirection.StateThreshold = 0.5f;
		}

		private void Update()
		{
			InputDevice activeDevice = InputManager.ActiveDevice;
			filteredDirection.Filter(activeDevice.Direction, Time.deltaTime);
			if (filteredDirection.Left.WasRepeated)
			{
				Debug.Log("!!!");
			}
			if (filteredDirection.Up.WasPressed)
			{
				MoveFocusTo(focusedButton.up);
			}
			if (filteredDirection.Down.WasPressed)
			{
				MoveFocusTo(focusedButton.down);
			}
			if (filteredDirection.Left.WasPressed)
			{
				MoveFocusTo(focusedButton.left);
			}
			if (filteredDirection.Right.WasPressed)
			{
				MoveFocusTo(focusedButton.right);
			}
		}

		private void MoveFocusTo(Button newFocusedButton)
		{
			if (newFocusedButton != null)
			{
				focusedButton = newFocusedButton;
			}
		}
	}
}
