using HumanAPI;
using System.Collections.Generic;
using UnityEngine;

public class Boiler : Node, IPostReset
{
	public NodeOutput output;

	public float capacity = 5f;

	public bool isHot;

	public List<Coal> coalList = new List<Coal>();

	public List<Flame> flameList = new List<Flame>();

	public Sound2 sound;

	public float fireVolume = 1f;

	public bool infiniteFlames;

	private bool initialHot;

	private int currentAudio;

	protected override void OnEnable()
	{
		base.OnEnable();
		initialHot = isHot;
	}

	public void AddFlame(Flame flame)
	{
		if (!flameList.Contains(flame))
		{
			flameList.Add(flame);
			if (flame.isHot.value > 0.5f && !isHot)
			{
				Ignite();
			}
			else if (flame.isHot.value < 0.5f && isHot)
			{
				flame.Ignite();
			}
		}
	}

	public void Ignite()
	{
		isHot = true;
		for (int i = 0; i < coalList.Count; i++)
		{
			coalList[i].Ignite();
		}
		for (int j = 0; j < flameList.Count; j++)
		{
			flameList[j].Ignite();
		}
		UpdateValue();
	}

	public void Extinguish()
	{
		isHot = false;
		for (int i = 0; i < coalList.Count; i++)
		{
			coalList[i].Extinguish(instant: true);
		}
		for (int j = 0; j < flameList.Count; j++)
		{
			flameList[j].Extinguish();
		}
		UpdateValue();
	}

	public void RemoveFlame(Flame flame)
	{
		if (flameList.Contains(flame))
		{
			flameList.Remove(flame);
			if (isHot && coalList.Count == 0 && flameList.Count == 0 && !infiniteFlames)
			{
				isHot = false;
			}
			UpdateValue();
		}
	}

	public void AddCoal(Coal coal)
	{
		if (!coalList.Contains(coal))
		{
			coalList.Add(coal);
			if (isHot)
			{
				coal.Ignite();
			}
			UpdateValue();
		}
	}

	public void RemoveCoal(Coal coal)
	{
		if (!coalList.Contains(coal))
		{
			return;
		}
		coalList.Remove(coal);
		if (isHot)
		{
			coal.Extinguish();
			if (coalList.Count == 0 && flameList.Count == 0)
			{
				isHot = false;
			}
		}
		UpdateValue();
	}

	private void UpdateValue()
	{
		if (!isHot)
		{
			output.SetValue(0f);
		}
		else
		{
			output.SetValue(Mathf.Clamp01((float)coalList.Count / capacity));
		}
		SyncAudio();
	}

	private void SyncAudio()
	{
		if (sound == null)
		{
			return;
		}
		int num = isHot ? Mathf.Clamp(coalList.Count, 0, 5) : 0;
		if (currentAudio == num)
		{
			return;
		}
		currentAudio = num;
		if (num == 0)
		{
			if (sound.isPlaying)
			{
				sound.Stop();
			}
			return;
		}
		sound.SetPitch(0.9f + (float)currentAudio * 0.1f);
		sound.SetVolume(0.5f + fireVolume * 0.1f);
		sound.Switch((char)(65 + num - 1));
		if (!sound.isPlaying)
		{
			sound.Play(forceLoop: true);
		}
	}

	public void PostResetState(int checkpoint)
	{
		isHot = initialHot;
		if (isHot)
		{
			Ignite();
		}
		else
		{
			Extinguish();
		}
		UpdateValue();
	}
}
