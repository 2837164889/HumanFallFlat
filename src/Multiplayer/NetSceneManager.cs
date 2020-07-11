using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Multiplayer
{
	[RequireComponent(typeof(NetIdentity))]
	public class NetSceneManager : MonoBehaviour
	{
		public static NetSceneManager instance;

		private NetIdentity identity;

		private uint evtCheckpoint;

		private uint evtResetLevel;

		[CompilerGenerated]
		private static Action<NetStream> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Action<NetStream> _003C_003Ef__mg_0024cache1;

		private void Start()
		{
			identity = GetComponent<NetIdentity>();
			if ((bool)identity)
			{
				instance = this;
				evtCheckpoint = identity.RegisterEvent(OnEnterCheckpoint);
				evtResetLevel = identity.RegisterEvent(OnResetLevel);
			}
		}

		private void OnDestroy()
		{
			if (instance == this)
			{
				instance = null;
			}
		}

		public static void EnterCheckpoint(int number, int subObjectives)
		{
			if ((bool)instance)
			{
				NetStream netStream = instance.identity.BeginEvent(instance.evtCheckpoint);
				netStream.Write((uint)number, 6);
				netStream.Write((uint)subObjectives, 6);
				instance.identity.EndEvent();
				Debug.Log("Send Enter Checkpoint Message");
			}
		}

		public static void ResetLevel(int checkpoint, int subObjectives)
		{
			if ((bool)instance)
			{
				NetStream netStream = instance.identity.BeginEvent(instance.evtResetLevel);
				netStream.Write((uint)checkpoint, 6);
				netStream.Write((uint)subObjectives, 6);
				instance.identity.EndEvent();
				Debug.Log("Send Reset Level Message");
			}
		}

		public static void OnEnterCheckpoint(NetStream stream)
		{
			int checkpoint = (int)stream.ReadUInt32(6);
			int subObjectives = (int)stream.ReadUInt32(6);
			Game.instance.EnterCheckpoint(checkpoint, subObjectives);
			Debug.Log("Received Checkpoint Message");
		}

		public static void OnResetLevel(NetStream stream)
		{
			int checkpoint = (int)stream.ReadUInt32(6);
			int subObjectives = (int)stream.ReadUInt32(6);
			Game.currentLevel.Reset(checkpoint, subObjectives);
			Game.currentLevel.PostEndReset(checkpoint);
			Debug.Log("Received Reset Level Message");
		}
	}
}
