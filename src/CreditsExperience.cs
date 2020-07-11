using HumanAPI;
using Multiplayer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsExperience : MonoBehaviour, INetBehavior
{
	public class BlockBucket
	{
		public enum State
		{
			Init,
			Active,
			Pausing,
			Paused
		}

		public int bitsLeft;

		public NetScope scope;

		public GameObject go;

		public List<CreditsBlock> blocks = new List<CreditsBlock>();

		public State scopeState;

		public bool IsNeeded
		{
			get
			{
				int i = 0;
				for (int count = blocks.Count; i < count; i++)
				{
					if (blocks[i].isSpawned)
					{
						return true;
					}
				}
				return false;
			}
		}

		public void Collecting()
		{
			switch (scopeState)
			{
			case State.Active:
			case State.Paused:
				break;
			case State.Init:
				scopeState = State.Active;
				break;
			case State.Pausing:
				scope.SuspendCollect = true;
				scopeState = State.Paused;
				break;
			}
		}
	}

	private enum State
	{
		Falling,
		NearGround,
		Ground,
		FadeOut
	}

	internal const uint DataFormatVersion = 1u;

	public GameObject startupDoor;

	private float startupDoorY;

	private float topY;

	private float currentDrag;

	private HashSet<Human> humansInLevel = new HashSet<Human>();

	private List<BlockBucket> buckets = new List<BlockBucket>();

	private bool hasFinishedCredits;

	private bool parsed;

	private bool skipDrag;

	private bool snapAlign;

	private State state;

	private Transform spawn;

	private Vector3 originalSpawn;

	private bool cloudsFiddled;

	private bool canSetClounds;

	private bool lastDrag = true;

	private bool fadeRunning;

	public CreditsGround ground;

	public CreditsText ctext;

	public float blockSpacing = 20f;

	public float showAhead = 100f;

	public float showBehind = 100f;

	public float spawnAbove = 20f;

	[HideInInspector]
	public TextAsset ActiveContent;

	[HideInInspector]
	public uint ContentHash;

	public TextAsset contentSA;

	public TextAsset contentPS;

	public TextAsset contentPSJP;

	public TextAsset contentXB;

	public TextAsset contentSwitch;

	public TextAsset contentSwitchJP;

	private List<CreditsBlock> blocks = new List<CreditsBlock>();

	private const ushort bitsForState = 2;

	private void ParseContent()
	{
		if (parsed)
		{
			return;
		}
		parsed = true;
		ctext.InitData();
		TextAsset activeContent = ActiveContent;
		List<CreditsBlock> list = new List<CreditsBlock>();
		string[] array = activeContent.text.Split('\n');
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = array[i].Trim();
		}
		int j = 0;
		float num = 0f;
		while (j < array.Length)
		{
			for (; j < array.Length && string.IsNullOrEmpty(array[j]); j++)
			{
			}
			if (j < array.Length)
			{
				string text = array[j++];
				while (j < array.Length && !string.IsNullOrEmpty(array[j]))
				{
					text = text + "\r\n" + array[j++];
				}
				CreditsBlock component = UnityEngine.Object.Instantiate(ctext.blockPrefab.gameObject).GetComponent<CreditsBlock>();
				component.transform.SetParent(base.transform, worldPositionStays: false);
				component.transform.position = num * Vector3.up;
				component.Initialize(num, text, ctext);
				blocks.Add(component);
				list.Add(component);
				num -= blockSpacing;
				NetIdentity netIdentity = component.gameObject.AddComponent<NetIdentity>();
				netIdentity.sceneId = (uint)blocks.Count;
				component.blockId = netIdentity;
				component.gameObject.name = "CrBlock" + netIdentity.sceneId.ToString();
			}
		}
		startupDoorY = num - showAhead;
		topY = 0f;
		uint num2 = (uint)NetHost.CalcMaxPossibleSizeForContainerContentsTier0();
		buckets.Clear();
		uint num3 = 288u;
		int k = 0;
		for (int count = list.Count; k < count; k++)
		{
			CreditsBlock creditsBlock = list[k];
			int bitsRequired = creditsBlock.bitsRequired;
			int count2 = buckets.Count;
			int num4 = count2 - 1;
			if (num4 < 0 || buckets[num4].bitsLeft < bitsRequired)
			{
				num4++;
			}
			BlockBucket blockBucket;
			if (num4 < count2)
			{
				blockBucket = buckets[num4];
			}
			else
			{
				blockBucket = new BlockBucket();
				buckets.Add(blockBucket);
				if (num3 > 1054)
				{
					throw new ApplicationException("Run out of auxilliary net ids");
				}
				blockBucket.go = new GameObject("LettersScope" + num3.ToString());
				blockBucket.scope = blockBucket.go.AddComponent<NetScope>();
				blockBucket.scope.enabled = false;
				blockBucket.scope.netId = num3++;
				blockBucket.scope.AllowSuspendCollect = true;
				blockBucket.go.transform.SetParent(base.gameObject.transform, worldPositionStays: false);
				blockBucket.scope.enabled = true;
				blockBucket.bitsLeft = (int)(num2 * 8) - NetScope.CalculateDeltaPrefixSize(blockBucket.scope.netId);
			}
			blockBucket.blocks.Add(creditsBlock);
			blockBucket.bitsLeft -= bitsRequired;
			creditsBlock.transform.SetParent(blockBucket.go.transform, worldPositionStays: true);
			creditsBlock.bucket = blockBucket;
		}
		int l = 0;
		for (int count3 = buckets.Count; l < count3; l++)
		{
			BlockBucket blockBucket2 = buckets[l];
			blockBucket2.scope.StartNetwork();
		}
		CheckScopes();
	}

	public void CheckScopes()
	{
		int i = 0;
		for (int count = buckets.Count; i < count; i++)
		{
			BlockBucket blockBucket = buckets[i];
			switch (blockBucket.scopeState)
			{
			case BlockBucket.State.Active:
			case BlockBucket.State.Pausing:
				blockBucket.scopeState = (blockBucket.IsNeeded ? BlockBucket.State.Active : BlockBucket.State.Pausing);
				break;
			case BlockBucket.State.Paused:
				if (blockBucket.IsNeeded)
				{
					blockBucket.scopeState = BlockBucket.State.Active;
					blockBucket.scope.SuspendCollect = false;
				}
				break;
			}
		}
	}

	private void Awake()
	{
		ParseContent();
		Level componentInParent = GetComponentInParent<Level>();
		componentInParent.netHash ^= ContentHash;
		ContentHash = 0u;
		spawn = componentInParent.spawnPoint;
		spawn.position = Vector3.up * (topY + spawnAbove);
		ReinitDoor(init: true);
		originalSpawn = spawn.transform.position;
		componentInParent.prerespawn = delegate(int cp, bool startingLevel)
		{
			if (cp == -1 || startingLevel)
			{
				spawn.transform.position = originalSpawn;
			}
			foreach (Human item in Human.all)
			{
				item.ControlVelocity(0.1f, killHorizontal: false);
			}
		};
	}

	private IEnumerator Start()
	{
		canSetClounds = false;
		StartingPlaces(initDoor: true);
		UpdateFromState();
		yield return new WaitForSeconds(0.1f);
		if (!hasFinishedCredits)
		{
			canSetClounds = true;
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		if (checkpoint != 0)
		{
			return;
		}
		StartingPlaces(initDoor: false);
		if (!NetGame.isClient)
		{
			for (int i = 0; i < blocks.Count; i++)
			{
				CreditsBlock creditsBlock = blocks[i];
				creditsBlock.DespawnAll();
			}
			CheckScopes();
		}
		UpdateFromState();
	}

	public void StartingPlaces(bool initDoor)
	{
		skipDrag = false;
		state = State.Falling;
		snapAlign = true;
		currentDrag = 0.4f;
		humansInLevel.Clear();
		for (int i = 0; i < Human.all.Count; i++)
		{
			if (!skipDrag)
			{
				Human.all[i].SetDrag(currentDrag);
			}
			humansInLevel.Add(Human.all[i]);
		}
		lastDrag = true;
		hasFinishedCredits = false;
		ReinitDoor(initDoor);
	}

	public void ReinitDoor(bool init)
	{
		if (!NetGame.isClient || init)
		{
			startupDoor.transform.position = Vector3.up * startupDoorY;
			startupDoor.transform.rotation = Quaternion.identity;
		}
		if (!NetGame.isClient)
		{
			for (int i = 0; i < blocks.Count; i++)
			{
				CreditsBlock creditsBlock = blocks[i];
				creditsBlock.transform.position = creditsBlock.lineY * Vector3.up;
				creditsBlock.transform.rotation = Quaternion.identity;
			}
		}
	}

	private void OnDestroy()
	{
		if (NetGame.isClient)
		{
			SubtitleManager.instance.ClearProgress();
		}
		ClearDrag();
		SetCloudState(newState: false);
		canSetClounds = false;
		hasFinishedCredits = true;
	}

	private void ClearDrag()
	{
		skipDrag = true;
		for (int i = 0; i < Human.all.Count; i++)
		{
			Human.all[i].ResetDrag();
		}
	}

	private void FixedUpdate()
	{
		if (ReplayRecorder.isPlaying || NetGame.isClient)
		{
			return;
		}
		AnalyzeHumans(out Human lowHuman, out Human _, out Vector3 center, out Vector3 avgSpeed);
		if (!(lowHuman != null))
		{
			return;
		}
		if (state < State.Ground)
		{
			for (int i = 0; i < Human.all.Count; i++)
			{
				Human human = Human.all[i];
				Vector3 a = center - human.transform.position;
				Vector3 a2 = avgSpeed - human.ragdoll.partBall.rigidbody.velocity;
				if (a.magnitude > 1f)
				{
					human.ragdoll.partChest.rigidbody.SafeAddForce(a * 40f + a2 * 40f);
				}
			}
		}
		if (state < State.Ground)
		{
			Vector3 position = lowHuman.transform.position;
			float num = position.y - startupDoorY;
			if (num > 2f && !lowHuman.onGround)
			{
				spawn.transform.position = lowHuman.transform.position;
			}
		}
	}

	private void Update()
	{
		if (hasFinishedCredits || ReplayRecorder.isPlaying)
		{
			return;
		}
		AnalyzeHumans(out Human lowHuman, out Human highHuman, out Vector3 _, out Vector3 _);
		for (int i = 0; i < Human.all.Count; i++)
		{
			Human human = Human.all[i];
			if (humansInLevel.Contains(human))
			{
				if (!skipDrag)
				{
					human.SetDrag(currentDrag);
				}
				continue;
			}
			float num;
			if (highHuman == null)
			{
				num = -1f;
			}
			else
			{
				Vector3 position = human.transform.position;
				float y = position.y;
				Vector3 position2 = highHuman.transform.position;
				num = y - position2.y - showBehind;
			}
			float num2 = num;
			if (num2 < 0f)
			{
				humansInLevel.Add(human);
				if (!skipDrag)
				{
					human.SetDrag(currentDrag);
				}
			}
		}
		if (!NetGame.isClient && highHuman != null)
		{
			Despawn(highHuman.transform.position);
			if (state == State.Falling)
			{
				Spawn(lowHuman.transform.position, highHuman.transform.position);
			}
			Align(lowHuman, highHuman);
		}
		if (!NetGame.isClient)
		{
			float num3;
			if (lowHuman == null)
			{
				num3 = 1000f;
			}
			else
			{
				Vector3 position3 = lowHuman.transform.position;
				num3 = position3.y - startupDoorY;
			}
			float num4 = num3;
			if (state == State.Falling && num4 < 20f)
			{
				state = State.NearGround;
			}
			if (state == State.NearGround && num4 < 2f && lowHuman != null && lowHuman.onGround)
			{
				state = State.Ground;
			}
			if (state == State.Ground && num4 < -20f)
			{
				state = State.FadeOut;
			}
		}
		UpdateFromState();
		if (!NetGame.isClient)
		{
			CheckScopes();
		}
	}

	private void SetCloudState(bool newState)
	{
		if (canSetClounds && cloudsFiddled != newState)
		{
			cloudsFiddled = newState;
			float num = (!newState) ? 0.5f : 2f;
			CloudSystem.instance.nearClipStart *= num;
			CloudSystem.instance.nearClipEnd *= num;
		}
	}

	private void UpdateFromState()
	{
		if (state < State.Ground)
		{
			if (MenuCameraEffects.instance.creditsAdjust == 0f && MenuSystem.instance.state == MenuSystemState.Inactive)
			{
				MenuCameraEffects.FadeInCredits();
			}
		}
		else if (MenuCameraEffects.instance.creditsAdjust != 0f && !MenuCameraEffects.instance.CreditsAdjustInTransition && MenuSystem.instance.state == MenuSystemState.Inactive)
		{
			MenuCameraEffects.FadeOut(1.5f);
		}
		SetCloudState(state < State.Ground);
		if (!skipDrag)
		{
			bool flag = state < State.NearGround;
			if (flag != lastDrag)
			{
				lastDrag = flag;
				currentDrag = ((!flag) ? 0.05f : 0.4f);
				for (int i = 0; i < Human.all.Count; i++)
				{
					Human human = Human.all[i];
					if (humansInLevel.Contains(human) && !skipDrag)
					{
						human.SetDrag(currentDrag);
					}
					human.state = HumanState.Spawning;
				}
			}
		}
		bool flag2 = state >= State.FadeOut;
		if (fadeRunning == flag2)
		{
			return;
		}
		fadeRunning = flag2;
		if (flag2)
		{
			CreditsFadeOutAndEnd.Trigger(base.gameObject, delegate
			{
				GetComponentInParent<Level>().respawnLocked = true;
				ClearDrag();
				hasFinishedCredits = true;
				startupDoor.SetActive(value: false);
				for (int j = 0; j < blocks.Count; j++)
				{
					blocks[j].gameObject.SetActive(value: false);
				}
			});
			return;
		}
		CreditsFadeOutAndEnd component = base.gameObject.GetComponent<CreditsFadeOutAndEnd>();
		if (component != null)
		{
			component.Cancel();
		}
	}

	private void AnalyzeHumans(out Human lowHuman, out Human highHuman, out Vector3 center, out Vector3 avgSpeed)
	{
		bool flag = true;
		lowHuman = null;
		highHuman = null;
		center = Vector3.zero;
		avgSpeed = Vector3.zero;
		int num = 0;
		for (int i = 0; i < Human.all.Count; i++)
		{
			Human human = Human.all[i];
			if (!humansInLevel.Contains(human))
			{
				continue;
			}
			num++;
			if (flag)
			{
				flag = false;
				lowHuman = human;
				highHuman = human;
				center = human.transform.position;
				avgSpeed = human.ragdoll.partBall.rigidbody.velocity;
				continue;
			}
			Vector3 position = human.transform.position;
			float y = position.y;
			Vector3 position2 = lowHuman.transform.position;
			if (y < position2.y)
			{
				lowHuman = human;
			}
			Vector3 position3 = human.transform.position;
			float y2 = position3.y;
			Vector3 position4 = highHuman.transform.position;
			if (y2 > position4.y)
			{
				highHuman = human;
			}
			center += human.transform.position;
			avgSpeed = human.ragdoll.partBall.rigidbody.velocity;
		}
		if (num > 0)
		{
			center /= (float)num;
			avgSpeed /= (float)num;
		}
	}

	public void AlignTransform(Transform transform, Human lowHuman, Human highHuman, bool snap)
	{
		Vector3 position = highHuman.transform.position;
		float y = position.y;
		Vector3 position2 = lowHuman.transform.position;
		float t = Mathf.InverseLerp(y - position2.y, 0f, 20f) * 0.5f + 0.5f;
		Vector3 vector = Vector3.Lerp(highHuman.transform.position, lowHuman.transform.position, t);
		Vector3 position3 = transform.position;
		if (!(vector.y - position3.y < 1f))
		{
			float num;
			if (snap)
			{
				num = 1f;
			}
			else
			{
				float b = showAhead;
				Vector3 position4 = lowHuman.transform.position;
				num = Mathf.InverseLerp(5f, b, position4.y - position3.y) * 0.3f;
			}
			float num2 = num;
			if (num2 != 0f)
			{
				position3.x = vector.x;
				position3.z = vector.z;
				transform.position = Vector3.Lerp(transform.position, position3, num2);
				Quaternion b2 = Quaternion.Euler(0f, Mathf.LerpAngle(highHuman.controls.targetYawAngle, lowHuman.controls.targetYawAngle, t), 0f);
				transform.rotation = Quaternion.Lerp(transform.rotation, b2, Mathf.Pow(num2, 2f));
			}
		}
	}

	private void Spawn(Vector3 lowHumanPos, Vector3 highHumanPos)
	{
		float num = highHumanPos.y + showBehind;
		float num2 = lowHumanPos.y - showAhead;
		for (int i = 0; i < blocks.Count; i++)
		{
			if (!blocks[i].isSpawned && blocks[i].lineY > num2 && blocks[i].lineY < num)
			{
				blocks[i].SpawnLetters(lowHumanPos.SetY(blocks[i].lineY));
			}
		}
	}

	private void Despawn(Vector3 highHumanPos)
	{
		float num = highHumanPos.y + showBehind;
		for (int i = 0; i < blocks.Count; i++)
		{
			if (blocks[i].isSpawned && blocks[i].lineY > num)
			{
				blocks[i].Despawn(num);
			}
		}
	}

	private void Align(Human lowHuman, Human highHuman)
	{
		for (int i = 0; i < blocks.Count; i++)
		{
			AlignTransform(blocks[i].transform, lowHuman, highHuman, snapAlign);
		}
		AlignTransform(startupDoor.transform, lowHuman, highHuman, snapAlign);
		snapAlign = false;
	}

	public void StartNetwork(NetIdentity identity)
	{
		ParseContent();
	}

	public void CollectState(NetStream stream)
	{
		stream.Write((uint)state, 2);
		NetBoolEncoder.CollectState(stream, ground != null && ground.isOpen);
	}

	public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
	{
		state0.ReadUInt32(2);
		State state2 = (State)state1.ReadUInt32(2);
		if (state != state2)
		{
			state = state2;
			UpdateFromState();
		}
		state0.ReadBool();
		bool newState = state1.ReadBool();
		if (ground != null)
		{
			ground.ChangeState(newState);
		}
	}

	public void ApplyState(NetStream state)
	{
		State state2 = (State)state.ReadUInt32(2);
		if (this.state != state2)
		{
			this.state = state2;
			UpdateFromState();
		}
		bool newState = state.ReadBool();
		if (ground != null)
		{
			ground.ChangeState(newState);
		}
	}

	public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
	{
		uint num = 0u;
		bool flag = false;
		if (state0 != null)
		{
			num = state0.ReadUInt32(2);
			flag = state0.ReadBool();
		}
		uint num2 = state1.ReadUInt32(2);
		bool flag2 = state1.ReadBool();
		if (num == num2 && flag == flag2)
		{
			delta.Write(v: false);
			return;
		}
		delta.Write(v: true);
		delta.Write(num2, 2);
		delta.Write(flag2);
	}

	public void AddDelta(NetStream state0, NetStream delta, NetStream result)
	{
		uint x = 0u;
		bool v = false;
		if (state0 != null)
		{
			x = state0.ReadUInt32(2);
			v = state0.ReadBool();
		}
		if (delta.ReadBool())
		{
			x = delta.ReadUInt32(2);
			v = delta.ReadBool();
		}
		result.Write(x, 2);
		result.Write(v);
	}

	public int CalculateMaxDeltaSizeInBits()
	{
		int num = 0;
		return num + 4;
	}

	public void SetMaster(bool isMaster)
	{
	}
}
