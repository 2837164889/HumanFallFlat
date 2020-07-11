using InControl;
using UnityEngine;

[RequireComponent(typeof(InControlInputModule))]
public class InputModuleActionAdapter : MonoBehaviour
{
	public class InputModuleActions : PlayerActionSet
	{
		public PlayerAction Submit;

		public PlayerAction Cancel;

		public PlayerAction Left;

		public PlayerAction Right;

		public PlayerAction Up;

		public PlayerAction Down;

		public PlayerTwoAxisAction Move;

		public InputModuleActions()
		{
			Submit = CreatePlayerAction("Submit");
			Cancel = CreatePlayerAction("Cancel");
			Left = CreatePlayerAction("Move Left");
			Right = CreatePlayerAction("Move Right");
			Up = CreatePlayerAction("Move Up");
			Down = CreatePlayerAction("Move Down");
			Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);
		}
	}

	public static InputModuleActions actions;

	private void OnEnable()
	{
		CreateActions();
		InControlInputModule component = GetComponent<InControlInputModule>();
		if (component != null)
		{
			component.SubmitAction = actions.Submit;
			component.CancelAction = actions.Cancel;
			component.MoveAction = actions.Move;
		}
	}

	private void OnDisable()
	{
		DestroyActions();
	}

	private void CreateActions()
	{
		actions = new InputModuleActions();
		bool flag = false;
		actions.Submit.AddDefaultBinding((!flag) ? InputControlType.Action1 : InputControlType.Action2);
		actions.Submit.AddDefaultBinding(Key.Space);
		actions.Submit.AddDefaultBinding(Key.Return);
		actions.Submit.AddDefaultBinding(Key.PadEnter);
		actions.Cancel.AddDefaultBinding((!flag) ? InputControlType.Action2 : InputControlType.Action1);
		actions.Cancel.AddDefaultBinding(Key.Escape);
		actions.Up.AddDefaultBinding(InputControlType.LeftStickUp);
		actions.Up.AddDefaultBinding(InputControlType.DPadUp);
		actions.Up.AddDefaultBinding(Key.UpArrow);
		actions.Up.AddDefaultBinding(Key.W);
		actions.Down.AddDefaultBinding(InputControlType.LeftStickDown);
		actions.Down.AddDefaultBinding(InputControlType.DPadDown);
		actions.Down.AddDefaultBinding(Key.DownArrow);
		actions.Down.AddDefaultBinding(Key.S);
		actions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
		actions.Left.AddDefaultBinding(InputControlType.DPadLeft);
		actions.Left.AddDefaultBinding(Key.LeftArrow);
		actions.Left.AddDefaultBinding(Key.A);
		actions.Right.AddDefaultBinding(InputControlType.LeftStickRight);
		actions.Right.AddDefaultBinding(InputControlType.DPadRight);
		actions.Right.AddDefaultBinding(Key.RightArrow);
		actions.Right.AddDefaultBinding(Key.D);
	}

	private void DestroyActions()
	{
		actions.Destroy();
	}
}
