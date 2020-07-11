using HumanAPI;
using UnityEngine;

public class NodeScriptWaveOutput1 : Node
{
	[Tooltip("The value being output by the curve")]
	public NodeOutput curveValue;

	[Tooltip("Whether or not to start using the curve value")]
	public NodeInput outputValue;

	[Tooltip("Whether or not the node is outputting the curve value")]
	public bool outputting;

	[Tooltip("Whether or not this node always outputs its value")]
	public bool alwaysOuput;

	[Tooltip("The curve the node will use for outputting a value")]
	public AnimationCurve animationCurve;

	private float currentCurveValue;

	private float time;

	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	private void Start()
	{
	}

	private void Update()
	{
		if (outputting || alwaysOuput)
		{
			time += Time.deltaTime;
			if (showDebug)
			{
				Debug.Log(base.name + " time = " + time);
			}
			currentCurveValue = animationCurve.Evaluate(time);
			if (showDebug)
			{
				Debug.Log(base.name + " currentCurveValue = " + currentCurveValue);
			}
			curveValue.SetValue(currentCurveValue);
		}
	}

	public override void Process()
	{
		if (outputValue.value >= 0.5f)
		{
			outputting = true;
		}
		else
		{
			outputting = false;
		}
	}
}
