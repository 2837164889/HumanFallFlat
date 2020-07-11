using Multiplayer;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CreditsBlock : MonoBehaviour, INetBehavior
{
	private struct Character
	{
		internal char c;

		internal Vector3 spawnPos;

		internal float size;
	}

	public float space = 1f;

	public float lineHeight = 3f;

	[NonSerialized]
	public float lineY;

	[NonSerialized]
	public int bitsRequired;

	[NonSerialized]
	public NetIdentity blockId;

	[NonSerialized]
	public CreditsExperience.BlockBucket bucket;

	private CreditsText ctext;

	private Vector3 centerOffset;

	private int activeLetters;

	private List<Character> characters = new List<Character>();

	public List<CreditsLetter> letters = new List<CreditsLetter>();

	private static NetVector3Encoder posEncoder = new NetVector3Encoder(8000f, 22, 4, 10);

	private static NetQuaternionEncoder rotEncoder = new NetQuaternionEncoder(9, 4, 6);

	private static NetVector3Encoder posEncoderBlock = new NetVector3Encoder(500f, 18, 4, 8);

	private static NetQuaternionEncoder rotEncoderBlock = new NetQuaternionEncoder(16, 6, 10);

	public string text
	{
		get;
		protected set;
	}

	public bool isSpawned
	{
		get;
		protected set;
	}

	public void Initialize(float lineY, string text, CreditsText ctext)
	{
		this.lineY = lineY;
		this.text = text;
		this.ctext = ctext;
		base.gameObject.SetActive(value: false);
		isSpawned = false;
		Bounds bounds = default(Bounds);
		float num = 1f;
		Vector3 a = new Vector3(1f, 0f, 0f);
		Vector3 a2 = new Vector3(0f, 0f, -1f);
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < text.Length; i++)
		{
			char c = text[i];
			if (c == '[' && text[i + 1] == 'H' && text[i + 3] == ']')
			{
				switch (text[i + 2])
				{
				case '1':
					num = 2f;
					break;
				case '2':
					num = 1.5f;
					break;
				case '3':
					num = 0.7f;
					break;
				case '4':
					num = 0.5f;
					break;
				}
				zero += a2 * lineHeight * (num - 1f);
				i += 3;
				continue;
			}
			switch (c)
			{
			case ' ':
				zero += a * space * num;
				break;
			case '\n':
				zero.x = 0f;
				zero += a2 * lineHeight;
				num = 1f;
				break;
			default:
			{
				Character character = default(Character);
				character.c = c;
				character.spawnPos = zero;
				character.size = num;
				Character item = character;
				zero += a * (ctext.GetLetterWidth(c) + 0.1f) * num;
				characters.Add(item);
				letters.Add(null);
				break;
			}
			case '\r':
				break;
			}
			bounds.Encapsulate(zero);
		}
		centerOffset = -bounds.center;
		bitsRequired = CalculateMaxDeltaSizeInBits();
	}

	public void SpawnLetters(Vector3 pos)
	{
		isSpawned = true;
		base.gameObject.SetActive(value: true);
		base.transform.position = pos;
		for (int i = 0; i < characters.Count; i++)
		{
			SpawnLetter(i);
		}
	}

	internal void DespawnAll()
	{
		if (!isSpawned)
		{
			return;
		}
		for (int num = letters.Count - 1; num >= 0; num--)
		{
			if (letters[num] != null)
			{
				DespawnLetter(num);
			}
		}
		CheckEmpty();
	}

	internal void Despawn(float v)
	{
		for (int num = letters.Count - 1; num >= 0; num--)
		{
			if (letters[num] != null)
			{
				Vector3 position = letters[num].transform.position;
				if (position.y > v)
				{
					DespawnLetter(num);
				}
			}
		}
		CheckEmpty();
	}

	public void Scroll(Vector3 offset)
	{
		base.transform.position += offset;
	}

	private void SpawnLetter(int idx)
	{
		if (!(letters[idx] != null))
		{
			Character character = characters[idx];
			CreditsLetter letter = ctext.GetLetter(character.c);
			letter.transform.SetParent(base.transform, worldPositionStays: false);
			letter.transform.localPosition = character.spawnPos;
			letter.transform.localScale = new Vector3(character.size, character.size, character.size);
			letter.Attach(this, centerOffset);
			letters[idx] = letter;
			activeLetters++;
		}
	}

	private void DespawnLetter(int idx)
	{
		if (!(letters[idx] == null))
		{
			ctext.ReleaseLetter(letters[idx]);
			letters[idx] = null;
			activeLetters--;
		}
	}

	private void CheckEmpty()
	{
		if (activeLetters == 0)
		{
			base.gameObject.SetActive(value: false);
			isSpawned = false;
		}
	}

	public void CollectState(NetStream stream)
	{
		stream.Write(isSpawned);
		if (isSpawned)
		{
			posEncoderBlock.CollectState(stream, base.transform.position);
			rotEncoderBlock.CollectState(stream, base.transform.rotation);
			for (int i = 0; i < letters.Count; i++)
			{
				CreditsLetter creditsLetter = letters[i];
				if (creditsLetter == null)
				{
					stream.Write(v: false);
					continue;
				}
				stream.Write(v: true);
				NetVector3Encoder netVector3Encoder = posEncoder;
				Vector3 localPosition = creditsLetter.transform.localPosition;
				Character character = characters[i];
				netVector3Encoder.CollectState(stream, localPosition - character.spawnPos);
				rotEncoder.CollectState(stream, creditsLetter.transform.localRotation);
			}
		}
		bucket.Collecting();
	}

	public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
	{
		bool flag = state0?.ReadBool() ?? false;
		bool flag2 = state1.ReadBool();
		if (flag2 != isSpawned)
		{
			if (flag2)
			{
				SpawnLetters(Vector3.zero);
			}
			else
			{
				DespawnAll();
			}
		}
		if (flag && flag2)
		{
			base.transform.position = posEncoderBlock.ApplyLerpedState(state0, state1, mix).SetY(lineY);
			base.transform.rotation = rotEncoderBlock.ApplyLerpedState(state0, state1, mix);
		}
		else if (flag2)
		{
			base.transform.position = posEncoderBlock.ApplyState(state1).SetY(lineY);
			base.transform.rotation = rotEncoderBlock.ApplyState(state1);
		}
		else if (flag)
		{
			posEncoderBlock.ApplyState(state0);
			rotEncoderBlock.ApplyState(state0);
		}
		for (int i = 0; i < letters.Count; i++)
		{
			bool flag3 = flag && state0.ReadBool();
			bool flag4 = flag2 && state1.ReadBool();
			CreditsLetter creditsLetter = letters[i];
			if (creditsLetter != null && !flag4)
			{
				DespawnLetter(i);
				creditsLetter = null;
			}
			else if (creditsLetter == null && flag4)
			{
				SpawnLetter(i);
				creditsLetter = letters[i];
			}
			if (flag3 && flag4)
			{
				Transform transform = creditsLetter.transform;
				Vector3 a = posEncoder.ApplyLerpedState(state0, state1, mix);
				Character character = characters[i];
				transform.localPosition = a + character.spawnPos;
				creditsLetter.transform.localRotation = rotEncoder.ApplyLerpedState(state0, state1, mix);
			}
			else if (flag4)
			{
				Transform transform2 = creditsLetter.transform;
				Vector3 a2 = posEncoder.ApplyState(state1);
				Character character2 = characters[i];
				transform2.localPosition = a2 + character2.spawnPos;
				creditsLetter.transform.localRotation = rotEncoder.ApplyState(state1);
			}
			else if (flag3)
			{
				posEncoder.ApplyState(state0);
				rotEncoder.ApplyState(state0);
			}
		}
	}

	public void ApplyState(NetStream state)
	{
		bool flag = state.ReadBool();
		if (flag != isSpawned)
		{
			if (flag)
			{
				SpawnLetters(Vector3.zero);
			}
			else
			{
				DespawnAll();
			}
		}
		if (!flag)
		{
			return;
		}
		base.transform.position = posEncoderBlock.ApplyState(state).SetY(lineY);
		base.transform.rotation = rotEncoderBlock.ApplyState(state);
		for (int i = 0; i < letters.Count; i++)
		{
			bool flag2 = state.ReadBool();
			CreditsLetter creditsLetter = letters[i];
			if (creditsLetter != null && !flag2)
			{
				DespawnLetter(i);
				creditsLetter = null;
			}
			else if (creditsLetter == null && flag2)
			{
				SpawnLetter(i);
				creditsLetter = letters[i];
			}
			if (flag2)
			{
				Transform transform = creditsLetter.transform;
				Vector3 a = posEncoder.ApplyState(state);
				Character character = characters[i];
				transform.localPosition = a + character.spawnPos;
				creditsLetter.transform.localRotation = rotEncoder.ApplyState(state);
			}
		}
	}

	public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
	{
		bool flag = state0?.ReadBool() ?? false;
		bool flag2 = state1.ReadBool();
		delta.Write(flag2);
		if (!flag)
		{
			state0 = null;
		}
		if (flag2)
		{
			posEncoderBlock.CalculateDelta(state0, state1, delta);
			rotEncoderBlock.CalculateDelta(state0, state1, delta);
			for (int i = 0; i < letters.Count; i++)
			{
				bool flag3 = flag && state0.ReadBool();
				bool flag4 = flag2 && state1.ReadBool();
				delta.Write(flag4);
				if (flag4)
				{
					NetStream state2 = (!flag3) ? null : state0;
					posEncoder.CalculateDelta(state2, state1, delta);
					rotEncoder.CalculateDelta(state2, state1, delta);
				}
				else if (flag3)
				{
					posEncoder.ApplyState(state0);
					rotEncoder.ApplyState(state0);
				}
			}
		}
		else
		{
			if (!flag)
			{
				return;
			}
			posEncoderBlock.ApplyState(state0);
			rotEncoderBlock.ApplyState(state0);
			for (int j = 0; j < letters.Count; j++)
			{
				if (state0.ReadBool())
				{
					posEncoder.ApplyState(state0);
					rotEncoder.ApplyState(state0);
				}
			}
		}
	}

	public void AddDelta(NetStream state0, NetStream delta, NetStream result)
	{
		bool flag = state0?.ReadBool() ?? false;
		bool flag2 = delta.ReadBool();
		result.Write(flag2);
		if (!flag)
		{
			state0 = null;
		}
		if (flag2)
		{
			posEncoderBlock.AddDelta(state0, delta, result);
			rotEncoderBlock.AddDelta(state0, delta, result);
			for (int i = 0; i < letters.Count; i++)
			{
				bool flag3 = flag && state0.ReadBool();
				bool flag4 = flag2 && delta.ReadBool();
				result.Write(flag4);
				if (flag4)
				{
					NetStream state = (!flag3) ? null : state0;
					posEncoder.AddDelta(state, delta, result);
					rotEncoder.AddDelta(state, delta, result);
				}
				else if (flag3)
				{
					posEncoder.ApplyState(state0);
					rotEncoder.ApplyState(state0);
				}
			}
		}
		else
		{
			if (!flag)
			{
				return;
			}
			posEncoderBlock.ApplyState(state0);
			rotEncoderBlock.ApplyState(state0);
			for (int j = 0; j < letters.Count; j++)
			{
				if (state0.ReadBool())
				{
					posEncoder.ApplyState(state0);
					rotEncoder.ApplyState(state0);
				}
			}
		}
	}

	public int CalculateMaxDeltaSizeInBits()
	{
		return (1 + posEncoder.CalculateMaxDeltaSizeInBits() + 1 + rotEncoder.CalculateMaxDeltaSizeInBits() + 1) * characters.Count + posEncoderBlock.CalculateMaxDeltaSizeInBits() + 1 + rotEncoderBlock.CalculateMaxDeltaSizeInBits() + 1 + 1;
	}

	public void SetMaster(bool isMaster)
	{
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
	}

	public void StartNetwork(NetIdentity identity)
	{
	}
}
