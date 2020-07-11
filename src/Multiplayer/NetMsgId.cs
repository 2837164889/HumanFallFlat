namespace Multiplayer
{
	public enum NetMsgId
	{
		Helo = 1,
		ClientHelo = 1,
		AddHost = 2,
		RemoveHost = 3,
		AddPlayer = 4,
		RequestAddPlayer = 4,
		RemovePlayer = 5,
		RequestRemovePlayer = 5,
		Move = 6,
		MoveAck = 6,
		Delta = 7,
		DeltaAck = 7,
		Event = 8,
		EventAck = 8,
		LoadLevel = 9,
		LoadLevelAck = 9,
		RequestSkin = 10,
		SendSkin = 11,
		Container = 12,
		Chat = 13,
		Kick = 14,
		RequestRespawn = 14,
		Achievement = 0xF
	}
}
