using HumanAPI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Flammable : Node, IReset
{
	public NodeOutput output;

	public float ignitionPoint = 0.8f;

	public float maximumHeat = 1f;

	public float heat;

	private float initialHeat;

	public float heatUpSpeed = 0.5f;

	private List<Flammable> flammableList = new List<Flammable>();

	private List<Flame> flameList = new List<Flame>();

	private List<FlammableExtinguisher> extinguisherList = new List<FlammableExtinguisher>();

	protected override void OnEnable()
	{
		base.OnEnable();
		initialHeat = heat;
	}

	public void AddFlammable(Flammable flammable)
	{
		flammableList.Add(flammable);
	}

	public void RemoveFlammable(Flammable flammable)
	{
		flammableList.Remove(flammable);
	}

	public void AddFlame(Flame flame)
	{
		flameList.Add(flame);
	}

	public void RemoveFlame(Flame flame)
	{
		flameList.Remove(flame);
	}

	public void AddExtinguisher(FlammableExtinguisher extinguisher)
	{
		extinguisherList.Add(extinguisher);
		heat -= extinguisher.cooling;
	}

	public void RemoveExtinguisher(FlammableExtinguisher extinguisher)
	{
		extinguisherList.Add(extinguisher);
	}

	public void Update()
	{
		float num;
		if (heat > ignitionPoint)
		{
			num = maximumHeat;
			flameList.ForEach(delegate(Flame f)
			{
				f.Ignite();
			});
		}
		else
		{
			num = flameList.Sum((Flame f) => f.isHot.value);
			num += flammableList.Sum((Flammable f) => f.heat);
		}
		float num2 = extinguisherList.Sum((FlammableExtinguisher e) => e.cooling);
		num -= num2;
		num = Mathf.Clamp(num, 0f, maximumHeat);
		heat = Mathf.MoveTowards(heat, num, heatUpSpeed * Time.deltaTime);
		UpdateValue();
	}

	private void UpdateValue()
	{
		output.SetValue(heat);
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		heat = initialHeat;
		UpdateValue();
	}
}
