using Multiplayer;
using Steamworks;
using System;
using UnityEngine;

public class GiftBase : MonoBehaviour, IGrabbable
{
	public uint giftId;

	[NonSerialized]
	public ulong userId;

	public void OnGrab()
	{
		if (NetGame.isServer)
		{
			Human human = GrabManager.GrabbedBy(base.gameObject);
			CSteamID cSteamID = (CSteamID)human.player.host.connection;
			userId = cSteamID.m_SteamID;
		}
	}

	public void OnRelease()
	{
	}
}
