using InControl;
using System;
using UnityEngine;

namespace BindingsExample
{
	public class PlayerActions : PlayerActionSet
	{
		public PlayerAction Fire;

		public PlayerAction Jump;

		public PlayerAction Left;

		public PlayerAction Right;

		public PlayerAction Up;

		public PlayerAction Down;

		public PlayerTwoAxisAction Move;

		public PlayerActions()
		{
			Fire = CreatePlayerAction("Fire");
			Jump = CreatePlayerAction("Jump");
			Left = CreatePlayerAction("Move Left");
			Right = CreatePlayerAction("Move Right");
			Up = CreatePlayerAction("Move Up");
			Down = CreatePlayerAction("Move Down");
			Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);
		}

		public static PlayerActions CreateWithDefaultBindings()
		{
			PlayerActions playerActions = new PlayerActions();
			playerActions.Fire.AddDefaultBinding(Key.A);
			playerActions.Fire.AddDefaultBinding(InputControlType.Action1);
			playerActions.Fire.AddDefaultBinding(Mouse.LeftButton);
			playerActions.Jump.AddDefaultBinding(Key.Space);
			playerActions.Jump.AddDefaultBinding(InputControlType.Action3);
			playerActions.Jump.AddDefaultBinding(InputControlType.Back);
			playerActions.Up.AddDefaultBinding(Key.UpArrow);
			playerActions.Down.AddDefaultBinding(Key.DownArrow);
			playerActions.Left.AddDefaultBinding(Key.LeftArrow);
			playerActions.Right.AddDefaultBinding(Key.RightArrow);
			playerActions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
			playerActions.Right.AddDefaultBinding(InputControlType.LeftStickRight);
			playerActions.Up.AddDefaultBinding(InputControlType.LeftStickUp);
			playerActions.Down.AddDefaultBinding(InputControlType.LeftStickDown);
			playerActions.Left.AddDefaultBinding(InputControlType.DPadLeft);
			playerActions.Right.AddDefaultBinding(InputControlType.DPadRight);
			playerActions.Up.AddDefaultBinding(InputControlType.DPadUp);
			playerActions.Down.AddDefaultBinding(InputControlType.DPadDown);
			playerActions.Up.AddDefaultBinding(Mouse.PositiveY);
			playerActions.Down.AddDefaultBinding(Mouse.NegativeY);
			playerActions.Left.AddDefaultBinding(Mouse.NegativeX);
			playerActions.Right.AddDefaultBinding(Mouse.PositiveX);
			playerActions.ListenOptions.IncludeUnknownControllers = true;
			playerActions.ListenOptions.MaxAllowedBindings = 4u;
			playerActions.ListenOptions.UnsetDuplicateBindingsOnSet = true;
			playerActions.ListenOptions.OnBindingFound = delegate(PlayerAction action, BindingSource binding)
			{
				if (binding == new KeyBindingSource(Key.Escape))
				{
					action.StopListeningForBinding();
					return false;
				}
				return true;
			};
			BindingListenOptions listenOptions = playerActions.ListenOptions;
			listenOptions.OnBindingAdded = (Action<PlayerAction, BindingSource>)Delegate.Combine(listenOptions.OnBindingAdded, (Action<PlayerAction, BindingSource>)delegate(PlayerAction action, BindingSource binding)
			{
				Debug.Log("Binding added... " + binding.DeviceName + ": " + binding.Name);
			});
			BindingListenOptions listenOptions2 = playerActions.ListenOptions;
			listenOptions2.OnBindingRejected = (Action<PlayerAction, BindingSource, BindingSourceRejectionType>)Delegate.Combine(listenOptions2.OnBindingRejected, (Action<PlayerAction, BindingSource, BindingSourceRejectionType>)delegate(PlayerAction action, BindingSource binding, BindingSourceRejectionType reason)
			{
				Debug.Log("Binding rejected... " + reason);
			});
			return playerActions;
		}
	}
}
