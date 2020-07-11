using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarouselPageButtonHandler : MonoBehaviour
{
	public Button leftButton;

	public TextMeshProUGUI leftLabel;

	public Button rightButton;

	public TextMeshProUGUI rightLabel;

	private void Start()
	{
		leftLabel.text = "Q/ℂ";
		rightLabel.text = "℅/E";
	}

	private bool Left()
	{
		InputDevice activeDevice = InputManager.ActiveDevice;
		return Input.GetKeyDown(KeyCode.Q) || (activeDevice.LeftBumper.IsPressed && activeDevice.LeftBumper.HasChanged);
	}

	private bool Right()
	{
		InputDevice activeDevice = InputManager.ActiveDevice;
		return Input.GetKeyDown(KeyCode.E) || (activeDevice.RightBumper.IsPressed && activeDevice.RightBumper.HasChanged);
	}

	private void Update()
	{
		if (Left())
		{
			leftButton.onClick.Invoke();
		}
		if (Right())
		{
			rightButton.onClick.Invoke();
		}
	}
}
