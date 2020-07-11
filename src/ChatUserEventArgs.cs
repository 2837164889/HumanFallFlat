using System;

public class ChatUserEventArgs : EventArgs
{
	public int Index
	{
		get;
		private set;
	}

	public ChatUserEventArgs(int index)
	{
		Index = index;
	}
}
