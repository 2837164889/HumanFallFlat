public class ChatUser
{
	private bool updateOnce;

	public bool IsMuted
	{
		get;
		private set;
	}

	public bool IsTalking
	{
		get;
		private set;
	}

	public bool IsHost
	{
		get;
		private set;
	}

	public bool IsLocal
	{
		get;
		private set;
	}

	public string GamerTag
	{
		get;
		private set;
	}

	public string XboxUserId
	{
		get;
		private set;
	}

	public ChatUser(string userId)
	{
		XboxUserId = userId;
		IsMuted = true;
		IsTalking = false;
	}

	public bool UpdateStatus()
	{
		if (!updateOnce)
		{
			updateOnce = true;
			return true;
		}
		return false;
	}

	public void UpdateGamerTag(string gamerTag)
	{
		GamerTag = gamerTag;
	}
}
