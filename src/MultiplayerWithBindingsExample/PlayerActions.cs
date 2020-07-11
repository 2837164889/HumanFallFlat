using InControl;

namespace MultiplayerWithBindingsExample
{
	public class PlayerActions : PlayerActionSet
	{
		public PlayerAction Green;

		public PlayerAction Red;

		public PlayerAction Blue;

		public PlayerAction Yellow;

		public PlayerAction Left;

		public PlayerAction Right;

		public PlayerAction Up;

		public PlayerAction Down;

		public PlayerTwoAxisAction Rotate;

		public PlayerActions()
		{
			Green = CreatePlayerAction("Green");
			Red = CreatePlayerAction("Red");
			Blue = CreatePlayerAction("Blue");
			Yellow = CreatePlayerAction("Yellow");
			Left = CreatePlayerAction("Left");
			Right = CreatePlayerAction("Right");
			Up = CreatePlayerAction("Up");
			Down = CreatePlayerAction("Down");
			Rotate = CreateTwoAxisPlayerAction(Left, Right, Down, Up);
		}

		public static PlayerActions CreateWithKeyboardBindings()
		{
			PlayerActions playerActions = new PlayerActions();
			playerActions.Green.AddDefaultBinding(Key.A);
			playerActions.Red.AddDefaultBinding(Key.S);
			playerActions.Blue.AddDefaultBinding(Key.D);
			playerActions.Yellow.AddDefaultBinding(Key.F);
			playerActions.Up.AddDefaultBinding(Key.UpArrow);
			playerActions.Down.AddDefaultBinding(Key.DownArrow);
			playerActions.Left.AddDefaultBinding(Key.LeftArrow);
			playerActions.Right.AddDefaultBinding(Key.RightArrow);
			return playerActions;
		}

		public static PlayerActions CreateWithJoystickBindings()
		{
			PlayerActions playerActions = new PlayerActions();
			playerActions.Green.AddDefaultBinding(InputControlType.Action1);
			playerActions.Red.AddDefaultBinding(InputControlType.Action2);
			playerActions.Blue.AddDefaultBinding(InputControlType.Action3);
			playerActions.Yellow.AddDefaultBinding(InputControlType.Action4);
			playerActions.Up.AddDefaultBinding(InputControlType.LeftStickUp);
			playerActions.Down.AddDefaultBinding(InputControlType.LeftStickDown);
			playerActions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
			playerActions.Right.AddDefaultBinding(InputControlType.LeftStickRight);
			playerActions.Up.AddDefaultBinding(InputControlType.DPadUp);
			playerActions.Down.AddDefaultBinding(InputControlType.DPadDown);
			playerActions.Left.AddDefaultBinding(InputControlType.DPadLeft);
			playerActions.Right.AddDefaultBinding(InputControlType.DPadRight);
			return playerActions;
		}
	}
}
