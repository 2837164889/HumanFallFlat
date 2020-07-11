using HumanAPI;
using System.Collections.Generic;
using UnityEngine;

public class FlammableSourceBoiler1 : Node
{
	[Tooltip("The value output by the boiler in line with the level of fuel")]
	public NodeOutput output;

	[Tooltip("The amount of fuel needed for max output")]
	public float capacity = 5f;

	[Tooltip("Whether or not the boiler is lit")]
	public bool isHot;

	[Tooltip("A list of flammable things inside the boiler")]
	private List<Flammable> flammableList = new List<Flammable>();

	private int flammbleElementCount;

	[Tooltip("a List of the flames inside the boiler")]
	private List<Flame> flameList = new List<Flame>();

	private int flameElementCount;

	[Tooltip("The sound to play when the boiler is lit")]
	public Sound2 sound;

	[Tooltip("The sound to play when the boiler is lit")]
	public AudioSource FlameSound;

	[Tooltip("The volume the sound should play at")]
	public float fireVolume = 1f;

	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	private bool initialHot;

	private int currentAudio;

	protected override void OnEnable()
	{
		base.OnEnable();
		initialHot = isHot;
	}

	public void AddFlame(Flame flame)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Add Flame ");
		}
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
		if (showDebug)
		{
			Debug.Log(base.name + " Ignite ");
		}
		isHot = true;
		flameElementCount++;
		if (flammableList.Count != 0)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Flammable list is not 0 ");
				Debug.Log(base.name + " flammableList.Count =" + flammableList.Count);
			}
			for (int i = 0; i < flammableList.Count; i++)
			{
				Flammable flammable = flammableList[i];
				FlammableScriptHeatColourChange1 component = flammable.GetComponent<FlammableScriptHeatColourChange1>();
				if (showDebug)
				{
					Debug.Log(base.name + " Doing Entry " + i);
				}
				if (component != null)
				{
					component.Ignite();
				}
			}
		}
		if (flameList.Count != 0)
		{
			for (int j = 0; j < flameList.Count; j++)
			{
				flameList[j].Ignite();
			}
		}
		if (showDebug)
		{
			Debug.Log(base.name + " flameElementCount = " + flameElementCount);
		}
		UpdateValue();
	}

	public void Extinguish()
	{
		if (flameList.Count != 0)
		{
			return;
		}
		for (int i = 0; i < flammableList.Count; i++)
		{
			Flammable flammable = flammableList[i];
			FlammableScriptHeatColourChange1 component = flammable.GetComponent<FlammableScriptHeatColourChange1>();
			if (showDebug)
			{
				Debug.Log(base.name + " Doing Entry " + i);
			}
			if (component != null)
			{
				component.Extinguish();
			}
		}
	}

	public void RemoveFlame(Flame flame)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Remove Flame ");
		}
		if (flameList.Contains(flame))
		{
			flameList.Remove(flame);
			flameElementCount--;
			if (showDebug)
			{
				Debug.Log(base.name + " Flame list contains this flame ");
				Debug.Log(base.name + " flameElementCount = " + flameElementCount);
			}
		}
		if (isHot && (flammbleElementCount == 0 || flameElementCount == 0))
		{
			isHot = false;
			if (showDebug)
			{
				Debug.Log(base.name + " We have cooled down ");
			}
		}
		UpdateValue();
	}

	public void AddFuel(Flammable fuel)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Add Fuel ");
		}
		if (flammableList.Contains(fuel))
		{
			return;
		}
		flammableList.Add(fuel);
		flammbleElementCount++;
		if (isHot)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Going to change the colour of the fuel ");
			}
			FlammableScriptHeatColourChange1 component = fuel.GetComponent<FlammableScriptHeatColourChange1>();
			if (component != null)
			{
				component.Ignite();
			}
		}
		UpdateValue();
	}

	public void RemoveFuel(Flammable fuel)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Remove fuel ");
		}
		flammableList.Remove(fuel);
		flammbleElementCount--;
		if (isHot)
		{
			FlammableScriptHeatColourChange1 component = fuel.GetComponent<FlammableScriptHeatColourChange1>();
			component.Extinguish();
			if (flammableList.Count == 0 && flameList.Count == 0)
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
			output.SetValue(Mathf.Clamp01((float)flammbleElementCount / capacity));
		}
		SyncAudio();
	}

	private void SyncAudio()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Sync Audio ");
		}
		if (sound == null)
		{
			return;
		}
		int num = isHot ? Mathf.Clamp(flammableList.Count, 0, 5) : 0;
		if (currentAudio == num)
		{
			return;
		}
		currentAudio = num;
		if (num == 0)
		{
			if (FlameSound.isPlaying)
			{
				FlameSound.Stop();
			}
		}
		else
		{
			FlameSound.pitch.Equals(0.9f + (float)currentAudio * 0.1f);
			FlameSound.volume.Equals(0.5f + fireVolume * 0.1f);
		}
	}

	public void PostResetState(int checkpoint)
	{
		isHot = initialHot;
		UpdateValue();
	}
}
