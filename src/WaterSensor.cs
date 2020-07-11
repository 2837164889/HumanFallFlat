using System.Collections.Generic;
using UnityEngine;

public class WaterSensor : MonoBehaviour
{
	protected List<WaterBody> waterBodies = new List<WaterBody>();

	[Tooltip("Reference to the water body to float on")]
	public WaterBody waterBody;

	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	public void OnEnterBody(WaterBody waterBody)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Something has touched us  ");
		}
		waterBodies.Add(waterBody);
		this.waterBody = waterBodies[0];
	}

	public void OnLeaveBody(WaterBody waterBody)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Something has stopped touching us ");
		}
		waterBodies.Remove(waterBody);
		if (waterBodies.Count > 0)
		{
			this.waterBody = waterBodies[0];
		}
		else
		{
			this.waterBody = null;
		}
	}
}
