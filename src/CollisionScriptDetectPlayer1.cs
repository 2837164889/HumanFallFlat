using HumanAPI;
using UnityEngine;

public class CollisionScriptDetectPlayer1 : Node
{
	[Tooltip("Output from the node to tell other nodes a player has entered")]
	public NodeOutput playerEntered;

	[Tooltip("Signal this node when we should start checking for humans")]
	public NodeInput startChecking;

	[Tooltip("Whether or not the node is checking for humans")]
	public bool checkingNow;

	[Tooltip("Whether the node should always check for humans")]
	public bool alwaysCheck;

	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	private void Start()
	{
	}

	private void OnTriggerEnter(Collider humanBodyPart)
	{
		if (checkingNow || alwaysCheck)
		{
			foreach (Human item in Human.all)
			{
				if (item.GetComponent<Collider>() == humanBodyPart)
				{
					SetOuputVar();
				}
			}
		}
	}

	private void Update()
	{
		if (startChecking.value >= 0.5f)
		{
			checkingNow = true;
		}
	}

	private void SetOuputVar()
	{
		playerEntered.SetValue(1f);
	}
}
