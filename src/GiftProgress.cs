using I2.Loc;
using Multiplayer;
using System;
using System.Collections;
using UnityEngine;

public class GiftProgress : MonoBehaviour, INetBehavior
{
	public SkinnedDoll doll;

	private Quaternion startRot;

	public Transform[] digits;

	public MeshRenderer progressBar;

	private uint total;

	private uint lastGoal;

	private uint goal;

	private string prize = string.Empty;

	private float[] from = new float[6];

	private float[] to = new float[6];

	private float[] current = new float[6];

	private float phase;

	private float fromProgress;

	private float toProgress;

	private float currentProgress;

	public bool lobbyGuns = true;

	public bool gunsUnlocked;

	private Material[] materials;

	private string lockedPrize;

	private void Awake()
	{
		startRot = digits[0].localRotation;
		materials = progressBar.materials;
		progressBar.sharedMaterials = materials;
	}

	private IEnumerator Start()
	{
		if (!NetGame.isServer)
		{
			lobbyGuns = true;
			yield break;
		}
		if (GiftService.instance != null)
		{
			GiftService.instance.RefreshStatus();
		}
		while (GiftService.status == null)
		{
			yield return null;
		}
		GiftService_statusUpdated(GiftService.status);
		GiftService.statusUpdated += GiftService_statusUpdated;
		yield return null;
		yield return null;
		if (gunsUnlocked)
		{
			NetChat.RegisterCommand(server: true, client: false, "bang", LobbyGuns, ScriptLocalization.Get("XTRA/gunsHelp"));
			lobbyGuns = (PlayerPrefs.GetInt("lobbyGuns", 1) > 0);
			NetChat.serverCommands.OnHelp("bang");
		}
		while (true)
		{
			yield return new WaitForSeconds(UnityEngine.Random.Range(30, 90));
			if (GiftService.instance != null)
			{
				GiftService.instance.RefreshStatus();
			}
		}
	}

	private void OnDisable()
	{
		NetChat.UnRegisterCommand(server: true, client: false, "bang", LobbyGuns, ScriptLocalization.Get("XTRA/gunsHelp"));
	}

	private void LobbyGuns()
	{
		lobbyGuns = !lobbyGuns;
		if (lobbyGuns)
		{
			string str = ScriptLocalization.Get("XTRA/gunsOn");
			NetChat.Print(str);
		}
		else
		{
			string str2 = ScriptLocalization.Get("XTRA/gunsOff");
			NetChat.Print(str2);
		}
		PlayerPrefs.SetInt("lobbyGuns", lobbyGuns ? 1 : 0);
		SetValue(total, goal, lastGoal, prize);
	}

	private void OnDestroy()
	{
		GiftService.statusUpdated -= GiftService_statusUpdated;
	}

	private void GiftService_statusUpdated(XmasStatus status)
	{
		SetValue(status.total, status.goal, status.last_goal, status.prize);
	}

	private void SetValue(uint newTotal, uint newGoal, uint newLastGoal, string newPrize)
	{
		if (newTotal < newGoal)
		{
			lockedPrize = newPrize;
		}
		if (newPrize != prize)
		{
			prize = newPrize;
			doll.ApplySkin(prize);
		}
		if (newGoal - newLastGoal < 0)
		{
			newGoal = (newLastGoal = 0u);
		}
		if (newTotal != total || newGoal != goal || newLastGoal != lastGoal)
		{
			total = newTotal;
			goal = newGoal;
			lastGoal = newLastGoal;
			int num = 100000;
			phase = 0f;
			for (int i = 0; i < 6; i++)
			{
				from[i] = current[i];
				to[i] = (long)total / (long)num;
				num /= 10;
			}
			float num2 = (goal == 0) ? 0f : Mathf.InverseLerp((float)(double)lastGoal, (float)(double)goal, (float)(double)total);
			fromProgress = currentProgress;
			toProgress = num2;
			bool flag = total < goal;
			if (doll.nailed != flag)
			{
				doll.Nail(flag);
				if (!NetGame.isClient && !ReplayRecorder.isPlaying && !flag && string.Equals(prize, lockedPrize))
				{
					StartCoroutine(SkinUnlockChutes());
				}
			}
		}
		gunsUnlocked = (newTotal >= newGoal && "Guns".Equals(prize, StringComparison.InvariantCultureIgnoreCase));
		if (Fireworks.instance != null)
		{
			Fireworks.instance.enableWeapons = (gunsUnlocked && lobbyGuns);
		}
	}

	private void Update()
	{
		materials[1].mainTextureOffset = materials[1].mainTextureOffset + new Vector2(Time.deltaTime / 10f, 0f);
		if (phase < 1f)
		{
			phase = Mathf.MoveTowards(phase, 1f, Time.deltaTime / 2f);
			Sync();
		}
	}

	private void Sync()
	{
		currentProgress = Mathf.Lerp(fromProgress, toProgress, Ease.easeInOutQuad(0f, 1f, phase));
		Material obj = materials[1];
		Vector2 mainTextureOffset = materials[1].mainTextureOffset;
		obj.mainTextureOffset = new Vector2(mainTextureOffset.y, Mathf.Lerp(0.985f, 0.525f, currentProgress));
		for (int i = 0; i < 6; i++)
		{
			current[i] = Mathf.Lerp(from[i], to[i], Ease.easeInOutQuad(0f, 1f, phase));
			float num = current[i] * 360f / 10f;
			digits[i].localRotation = startRot * Quaternion.Euler(0f, 0f, 0f - num);
		}
	}

	public void StartNetwork(NetIdentity identity)
	{
	}

	public void CollectState(NetStream stream)
	{
		stream.Write(total, 24);
		stream.Write(goal, 24);
		stream.Write(lastGoal, 24);
		if ("Guns".Equals(prize, StringComparison.InvariantCultureIgnoreCase) && !lobbyGuns)
		{
			stream.Write("GunsLocked");
		}
		else
		{
			stream.Write(prize);
		}
	}

	public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
	{
		if (state0 != null)
		{
			state0.ReadUInt32(24);
			state0.ReadUInt32(24);
			state0.ReadUInt32(24);
			state0.ReadString();
		}
		ApplyState(state1);
	}

	public void ApplyState(NetStream state)
	{
		uint newTotal = state.ReadUInt32(24);
		uint newGoal = state.ReadUInt32(24);
		uint newLastGoal = state.ReadUInt32(24);
		string newPrize = state.ReadString();
		SetValue(newTotal, newGoal, newLastGoal, newPrize);
	}

	public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
	{
		uint num = state0?.ReadUInt32(24) ?? 0;
		uint num2 = state0?.ReadUInt32(24) ?? 0;
		uint num3 = state0?.ReadUInt32(24) ?? 0;
		string a = (state0 != null) ? state0.ReadString() : string.Empty;
		uint num4 = state1.ReadUInt32(24);
		uint num5 = state1.ReadUInt32(24);
		uint num6 = state1.ReadUInt32(24);
		string text = state1.ReadString();
		if (num == num4 && num2 == num5 && num3 == num6 && string.Equals(a, text))
		{
			delta.Write(v: false);
			return;
		}
		delta.Write(v: true);
		delta.Write(num4, 24);
		delta.Write(num5, 24);
		delta.Write(num6, 24);
		delta.Write(text);
	}

	public void AddDelta(NetStream state0, NetStream delta, NetStream result)
	{
		uint x = state0?.ReadUInt32(24) ?? 0;
		uint x2 = state0?.ReadUInt32(24) ?? 0;
		uint x3 = state0?.ReadUInt32(24) ?? 0;
		string text = (state0 != null) ? state0.ReadString() : string.Empty;
		if (delta.ReadBool())
		{
			x = delta.ReadUInt32(24);
			x2 = delta.ReadUInt32(24);
			x3 = delta.ReadUInt32(24);
			text = delta.ReadString();
		}
		result.Write(x, 24);
		result.Write(x2, 24);
		result.Write(x3, 24);
		result.Write(text);
	}

	public int CalculateMaxDeltaSizeInBits()
	{
		return 232;
	}

	public void SetMaster(bool isMaster)
	{
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
	}

	private IEnumerator SkinUnlockChutes()
	{
		for (int i = 0; i < 100; i++)
		{
			Fireworks.instance.ShootFirework();
			yield return new WaitForSeconds(UnityEngine.Random.Range(0.05f, 0.7f));
		}
	}
}
