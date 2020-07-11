using System;
using System.Collections.Generic;
using UnityEngine;

namespace Multiplayer
{
	public abstract class NetTransport : MonoBehaviour
	{
		[Flags]
		public enum LobbyInfoFlags : uint
		{
			kJoinGameInProgress = 0x1,
			kLockLevel = 0x2,
			kInviteOnly = 0x4,
			kStarted = 0x8,
			kFull = 0x10,
			kCanJoin = 0x20,
			kAllFlags = 0x3F
		}

		[Flags]
		public enum LobbyInfoAttrs : uint
		{
			kNumPlayersField = 0x80000000,
			kMaxPlayersField = 0x40000000,
			kLevelIDField = 0x20000000,
			kLevelTypeField = 0x10000000,
			kLobbyTitleField = 0x8000000,
			kAllFields = 0xF8000000,
			kPlayersFields = 0xC0000000
		}

		public struct LobbyDisplayInfo
		{
			public uint FeaturesMask;

			public uint NumPlayersForDisplay;

			public uint MaxPlayers;

			public ulong LevelID;

			public WorkshopItemSource LevelType;

			public string LobbyTitle;

			public uint Flags;

			public void InitBlank()
			{
				FeaturesMask = 0u;
				NumPlayersForDisplay = 0u;
				MaxPlayers = 0u;
				LevelID = 0uL;
				LevelType = WorkshopItemSource.BuiltIn;
				LobbyTitle = string.Empty;
				Flags = 0u;
			}

			public static uint CorrectPlayerCountForDisplay(uint pc)
			{
				return pc + 1;
			}

			public bool ShouldDisplayAllAttr(uint mask)
			{
				return (FeaturesMask & mask) == mask;
			}

			public bool ShouldDisplayAnyAttr(uint mask)
			{
				return (FeaturesMask & mask) != 0;
			}

			public uint Compare(ref LobbyDisplayInfo other, bool includeFeatures = true)
			{
				uint num = Flags ^ other.Flags;
				if (includeFeatures)
				{
					num |= (FeaturesMask ^ other.FeaturesMask);
				}
				if (NumPlayersForDisplay != other.NumPlayersForDisplay)
				{
					num = (uint)((int)num | int.MinValue);
				}
				if (MaxPlayers != other.MaxPlayers)
				{
					num |= 0x40000000;
				}
				if (LevelID != other.LevelID)
				{
					num |= 0x20000000;
				}
				if (LevelType != other.LevelType)
				{
					num |= 0x10000000;
				}
				if (!LobbyTitle.Equals(other.LobbyTitle))
				{
					num |= 0x8000000;
				}
				return num;
			}
		}

		public interface ILobbyEntry
		{
			string name();

			object lobbyId();

			bool isSameLobbyID(object lobbyID);

			bool getDisplayInfo(out LobbyDisplayInfo info);

			void setDisplayInfo(ref LobbyDisplayInfo info);
		}

		public delegate void OnGameOverlayActivationDelegate(byte active);

		public delegate void OnLobbyDataUpdateDelegate(object lobbyID, uint playersMax, uint playersCurrent, ulong levelID, WorkshopItemSource levelType, string levelTitle, uint flags, bool error);

		public delegate void OnLobbyDataUpdateDelegate2(object lobbyID, LobbyDisplayInfo dispInfo, bool error);

		public delegate void OnStartHostDelegate(string error);

		public delegate void OnJoinGameDelegate(object serverConnection, string error);

		public delegate void OnConnectDelegate(object connectionId);

		public delegate void OnDisconnectDelegate(object connectionId);

		public delegate void OnDataDelegate(object connectionId, byte[] buffer, int dataSize);

		[Obsolete]
		public virtual string userId => string.Empty;

		public virtual uint GetSupportedLobbyData()
		{
			return 0u;
		}

		public virtual void RequestLobbyDataRefresh(ILobbyEntry lobbyEntry, bool inSession)
		{
		}

		public virtual float GetLobbyDataRefreshThrottleTime()
		{
			return 2f;
		}

		public abstract void SendReliable(NetHost host, byte[] data, int len);

		public abstract void SendUnreliable(NetHost host, byte[] data, int len);

		public virtual void FlushTxBufers(bool fixedUpdate)
		{
		}

		public abstract void Init();

		public abstract void StartServer(OnStartHostDelegate callback, object sessionArgs = null);

		public abstract void StopServer();

		public abstract void JoinGame(object serverAddress, OnJoinGameDelegate callback);

		public abstract void StartThread();

		public abstract void StopThread();

		public abstract void LeaveGame();

		public abstract void OnUpdate();

		public abstract bool ConnectionEquals(object connection, NetHost host);

		public abstract string GetMyName();

		public virtual string getUserId(int localPlayerIndex)
		{
			return string.Empty;
		}

		public virtual bool CanSendInvite()
		{
			return false;
		}

		public virtual bool ShouldInhibitUIExceptCancel()
		{
			return false;
		}

		public virtual void SendInvite()
		{
			throw new NotImplementedException();
		}

		public virtual void RegisterForGameOverlayActivation(OnGameOverlayActivationDelegate callback)
		{
		}

		public virtual void RegisterForLobbyData(OnLobbyDataUpdateDelegate2 callback)
		{
		}

		public virtual void UnregisterForLobbyData(OnLobbyDataUpdateDelegate2 callback)
		{
		}

		public virtual void SetLobbyStatus(bool status)
		{
		}

		public virtual void UpdateServerLevel(ulong levelID, WorkshopItemSource levelType)
		{
		}

		public virtual void UpdateOptionsLobbyData()
		{
		}

		public virtual int BuildLobbyFlagsFromOptions()
		{
			return 0;
		}

		public virtual int GetNumberRemoteUsers()
		{
			return 0;
		}

		public virtual void UpdateLobbyTitle()
		{
		}

		public virtual void UpdateJoinInProgress()
		{
		}

		public virtual void UpdateLobbyType()
		{
		}

		public virtual void UpdateLobbyPlayers()
		{
		}

		public virtual void LobbyConnectedFixup()
		{
		}

		public virtual bool IsRelayed(NetHost client)
		{
			return false;
		}

		public virtual void ListLobbies(Action<List<ILobbyEntry>> onListLobbies)
		{
			throw new NotImplementedException();
		}

		public virtual bool SupportsLobbyListings()
		{
			return true;
		}

		public virtual object FetchLaunchInvitation()
		{
			return null;
		}

		public virtual void SetJoinable(bool joinable, bool haveStarted)
		{
		}
	}
}
