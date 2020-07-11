using HumanAPI;
using InControl;
using System.Collections.Generic;
using UnityEngine;

namespace Multiplayer
{
	public class NetPlayer : NetScope
	{
		public NetHost host;

		public bool isLocalPlayer;

		public Human human;

		public CameraController3 cameraController;

		public RagdollCustomization customization;

		public RagdollPresetMetadata skin;

		public uint localCoopIndex;

		public string skinUserId;

		public byte[] skinCRC;

		public Queue<NetHost> skinQueue = new Queue<NetHost>();

		public HumanControls controls;

		public OverheadNameTag overHeadNameTag;

		public bool ChatUserAdded;

		public static InputDevice Player0Device;

		private object moveLock = new object();

		private int moveFrames;

		private float walkForward;

		private float walkRight;

		private float cameraPitch;

		private float cameraYaw;

		private float leftExtend;

		private float rightExtend;

		private bool jump;

		private bool playDead;

		private bool holding;

		private bool shooting;

		private int nextIdentityId = 1;

		internal NameTag nametag;

		private static bool isPlayingGame()
		{
			return App.state == AppSate.PlayLevel || App.state == AppSate.ClientPlayLevel || App.state == AppSate.ServerPlayLevel;
		}

		public static NetPlayer SpawnPlayer(uint id, NetHost host, bool isLocal, string skinUserId, uint localCoopIndex, byte[] skinCRC)
		{
			NetPlayer component = Object.Instantiate(Game.instance.playerPrefab).GetComponent<NetPlayer>();
			component.human.player = component;
			component.human.ragdoll = Object.Instantiate(Game.instance.ragdollPrefab.gameObject, component.human.transform, worldPositionStays: false).GetComponent<Ragdoll>();
			component.human.ragdoll.BindBall(component.human.transform);
			component.human.Initialize();
			RagdollPresetMetadata ragdollPresetMetadata;
			if (isLocal)
			{
				ragdollPresetMetadata = GetLocalSkin(localCoopIndex);
			}
			else
			{
				ragdollPresetMetadata = RagdollPresetMetadata.LoadNetSkin(localCoopIndex, skinUserId);
				if (ragdollPresetMetadata != null && !ragdollPresetMetadata.CheckCRC(skinCRC))
				{
					ragdollPresetMetadata = null;
				}
			}
			component.netId = id;
			component.host = host;
			component.localCoopIndex = localCoopIndex;
			component.isLocalPlayer = isLocal;
			component.skin = ragdollPresetMetadata;
			component.skinUserId = skinUserId;
			component.skinCRC = skinCRC;
			if (isLocal && ragdollPresetMetadata != null && isPlayingGame())
			{
				App.StartPlaytimeForItem(ragdollPresetMetadata.workshopId);
			}
			host.AddPlayer(component);
			if (ragdollPresetMetadata != null)
			{
				component.ApplyPreset(ragdollPresetMetadata);
			}
			else
			{
				component.ApplyPreset(PresetRepository.CreateDefaultSkin(), bake: false);
			}
			component.SetupBodies();
			if (isLocal)
			{
				MenuCameraEffects.instance.AddHuman(component);
				Listener.instance.AddHuman(component.human);
				PlayerManager.instance.OnLocalPlayerAdded(component);
			}
			else
			{
				component.cameraController.gameObject.SetActive(value: false);
			}
			if (NetGame.netlog)
			{
				Debug.LogFormat("Spawning {0}", component.netId);
			}
			return component;
		}

		public void DespawnPlayer()
		{
			if (NetGame.netlog)
			{
				Debug.LogFormat("Destroying {0}", netId);
			}
			if (isLocalPlayer)
			{
				if (skin != null)
				{
					App.StopPlaytimeForItem(skin.workshopId);
				}
				MenuCameraEffects.instance.RemoveHuman(this);
				Listener.instance.RemoveHuman(human);
				if (App.state == AppSate.Customize)
				{
					Player0Device = controls.device;
				}
			}
			host.RemovePlayer(this);
			if (isLocalPlayer)
			{
				PlayerManager.instance.OnLocalPlayerRemoved(this);
			}
			if (base.gameObject != null)
			{
				GrabManager.Release(base.gameObject);
			}
			Object.Destroy(base.gameObject);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (base.gameObject != null)
			{
				GrabManager.Release(base.gameObject);
			}
		}

		public override void PreFixedUpdate()
		{
			base.PreFixedUpdate();
			lock (moveLock)
			{
				if (isLocalPlayer && !human.disableInput)
				{
					bool flag = true;
					if (MenuSystem.instance.state == MenuSystemState.PauseMenu)
					{
						flag = false;
					}
					else
					{
						if ((App.state == AppSate.ServerLobby || App.state == AppSate.ClientLobby) && MenuSystem.instance.state != 0)
						{
							flag = false;
						}
						if (NetChat.typing && (NetGame.isClient || NetGame.isServer))
						{
							flag = false;
						}
					}
					if (flag)
					{
						controls.ReadInput(out walkForward, out walkRight, out cameraPitch, out cameraYaw, out leftExtend, out rightExtend, out jump, out playDead, out shooting);
					}
					else
					{
						walkForward = 0f;
						walkRight = 0f;
						jump = false;
						shooting = false;
					}
				}
				if (NetGame.isClient)
				{
					SendMove(walkForward, walkRight, cameraPitch, cameraYaw, leftExtend, rightExtend, jump, playDead, shooting);
				}
				else
				{
					holding = human.hasGrabbed;
				}
				controls.HandleInput(walkForward, walkRight, cameraPitch, cameraYaw, leftExtend, rightExtend, jump, playDead, holding, shooting);
				moveFrames = 0;
			}
		}

		public override void PostFixedUpdate()
		{
			base.PostFixedUpdate();
			human.groundManager.PostFixedUpdate();
		}

		public void SendMove(float walkForward, float walkRight, float cameraPitch, float cameraYaw, float leftExtend, float rightExtend, bool jump, bool playDead, bool shooting)
		{
			if (host != null && host.isReady)
			{
				NetStream netStream = NetGame.BeginMessage(NetMsgId.Move);
				try
				{
					netStream.WriteNetId(netId);
					netStream.Write(NetFloat.Quantize(walkForward, 1f, 8), 8);
					netStream.Write(NetFloat.Quantize(walkRight, 1f, 8), 8);
					netStream.Write(NetFloat.Quantize(Math2d.NormalizeAngleDeg(cameraPitch), 180f, 9), 9);
					netStream.Write(NetFloat.Quantize(Math2d.NormalizeAngleDeg(cameraYaw), 180f, 9), 9);
					netStream.Write(NetFloat.Quantize(leftExtend, 1f, 5), 5);
					netStream.Write(NetFloat.Quantize(rightExtend, 1f, 5), 5);
					netStream.Write(jump);
					netStream.Write(playDead);
					netStream.Write(shooting);
					NetGame.instance.SendUnreliableToServer(netStream, -1);
				}
				finally
				{
					if (netStream != null)
					{
						netStream = netStream.Release();
					}
				}
			}
		}

		public void ReceiveMove(NetStream stream)
		{
			float b = NetFloat.Dequantize(stream.ReadInt32(8), 1f, 8);
			float b2 = NetFloat.Dequantize(stream.ReadInt32(8), 1f, 8);
			lock (moveLock)
			{
				moveFrames++;
				walkForward = Mathf.Lerp(walkForward, b, 1f / (float)moveFrames);
				walkRight = Mathf.Lerp(walkRight, b2, 1f / (float)moveFrames);
				cameraPitch = NetFloat.Dequantize(stream.ReadInt32(9), 180f, 9);
				cameraYaw = NetFloat.Dequantize(stream.ReadInt32(9), 180f, 9);
				if (moveFrames == 1)
				{
					leftExtend = (rightExtend = 0f);
					jump = (playDead = false);
					shooting = false;
				}
				leftExtend = Mathf.Max(leftExtend, NetFloat.Dequantize(stream.ReadInt32(5), 1f, 5));
				rightExtend = Mathf.Max(rightExtend, NetFloat.Dequantize(stream.ReadInt32(5), 1f, 5));
				jump |= stream.ReadBool();
				playDead |= stream.ReadBool();
				shooting |= stream.ReadBool();
				if (shooting)
				{
					Debug.LogError("shooting = true in NetPlayer.cs:357");
				}
				NetStream netStream = NetGame.BeginMessage(NetMsgId.Move);
				try
				{
					netStream.WriteNetId(netId);
					netStream.Write(holding);
					NetGame.instance.SendUnreliable(host, netStream, -1);
				}
				finally
				{
					if (netStream != null)
					{
						netStream = netStream.Release();
					}
				}
			}
		}

		public void ReceiveMoveAck(NetStream stream)
		{
			holding = stream.ReadBool();
		}

		private void SetupBodies()
		{
			int num = 3;
			Ragdoll ragdoll = human.ragdoll;
			RegisterBody(ragdoll.partBall, null, pos: true, rot: true, num);
			RegisterBody(ragdoll.partHips, null, pos: true, rot: true, num);
			RegisterBody(ragdoll.partWaist, ragdoll.partHips, pos: false, rot: true, num);
			RegisterBody(ragdoll.partChest, ragdoll.partHips, pos: false, rot: true, num);
			RegisterBody(ragdoll.partHead, ragdoll.partHips, pos: true, rot: true, num + 2);
			RegisterBody(ragdoll.partLeftArm, ragdoll.partHips, pos: false, rot: true, num);
			RegisterBody(ragdoll.partLeftForearm, ragdoll.partHips, pos: false, rot: true, num);
			RegisterBody(ragdoll.partLeftThigh, ragdoll.partHips, pos: false, rot: true, num);
			RegisterBody(ragdoll.partLeftLeg, ragdoll.partHips, pos: false, rot: true, num);
			RegisterBody(ragdoll.partRightArm, ragdoll.partHips, pos: false, rot: true);
			RegisterBody(ragdoll.partRightForearm, ragdoll.partHips, pos: false, rot: true, num);
			RegisterBody(ragdoll.partRightThigh, ragdoll.partHips, pos: false, rot: true, num);
			RegisterBody(ragdoll.partRightLeg, ragdoll.partHips, pos: false, rot: true, num);
		}

		private void RegisterBody(HumanSegment segment, HumanSegment relativeTo, bool pos, bool rot, int precision = 0)
		{
			NetIdentity netIdentity = segment.rigidbody.gameObject.AddComponent<NetIdentity>();
			netIdentity.sceneId = (uint)nextIdentityId++;
			NetBody netBody = segment.rigidbody.gameObject.AddComponent<NetBody>();
			netBody.despawnHeight = float.NegativeInfinity;
			netBody.disableSleep = true;
			netBody.syncPosition = (pos ? ((relativeTo == null) ? NetBodySyncPosition.Absolute : NetBodySyncPosition.Relative) : NetBodySyncPosition.None);
			netBody.syncRotation = (rot ? ((relativeTo == null) ? NetBodySyncRotation.Absolute : NetBodySyncRotation.Relative) : NetBodySyncRotation.None);
			netBody.relativeTo = relativeTo?.transform;
			netBody.rotPrecision = precision;
			netBody.posPrecision = precision;
			if (netBody.syncPosition == NetBodySyncPosition.Absolute)
			{
				netBody.posRangeOverride = 4000f;
				netBody.posPrecision += 2;
			}
			if (relativeTo != null)
			{
				netBody.posRangeOverride = 2f;
				netBody.posPrecision -= 2;
			}
			AddIdentity(netIdentity);
			netBody.Start();
			if (NetGame.isClient && pos)
			{
				segment.rigidbody.gameObject.SetActive(value: false);
			}
		}

		public override void Populate()
		{
		}

		public void ApplySkin(byte[] bytes)
		{
			skin = RagdollPresetMetadata.Deserialize(bytes);
			skin.SaveNetSkin(localCoopIndex, skinUserId);
			ApplyPreset(skin);
		}

		public static RagdollPresetMetadata GetLocalSkin(uint localCoopIndex)
		{
			int @int = PlayerPrefs.GetInt("MainSkinIndex", 0);
			@int = (@int + (int)localCoopIndex) % 2;
			return WorkshopRepository.instance.presetRepo.GetPlayerSkin(@int);
		}

		public void ApplyPreset(RagdollPresetMetadata preset, bool bake = true)
		{
			if (customization == null)
			{
				customization = human.ragdoll.gameObject.AddComponent<RagdollCustomization>();
			}
			customization.ApplyPreset(preset, forceRebuild: true);
			customization.RebindColors(bake, compress: true);
			customization.ClearOutCachedClipVolumes();
		}

		public void ReapplySkin()
		{
			ApplyPreset(customization.preset);
		}
	}
}
