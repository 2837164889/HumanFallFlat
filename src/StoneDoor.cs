using UnityEngine;

public class StoneDoor : MonoBehaviour, IReset
{
	[Tooltip("Reference to the thing barring the door")]
	public GameObject block;

	[Tooltip("A Container for a bunch of phs boulders")]
	public GameObject stoneContainer;

	[Tooltip("Scaffoding the player uses to get to the door")]
	public GameObject scaffold;

	[Tooltip("How far the bar can be moved before the doors open with force")]
	public float distTreshold;

	[Tooltip("The velocity to add to the boulders when the door is opened")]
	public Vector3 spitVelocity;

	private Vector3 startPos;

	private Rigidbody[] stones;

	private Rigidbody[] scaffoldPieces;

	[Tooltip("Use this in order to see the activity within the script")]
	public bool showDebug;

	private void Start()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Start ");
		}
		startPos = block.transform.position;
		if (stoneContainer != null)
		{
			stones = stoneContainer.GetComponentsInChildren<Rigidbody>();
		}
		if (scaffold != null)
		{
			scaffoldPieces = scaffold.GetComponentsInChildren<Rigidbody>();
		}
	}

	private void FixedUpdate()
	{
		if (!((block.transform.position - startPos).magnitude > distTreshold))
		{
			return;
		}
		if (showDebug)
		{
			Debug.Log(base.name + " Doors should burst open ");
		}
		if (stoneContainer != null)
		{
			SetKinematic(kinematic: false);
			for (int i = 0; i < stones.Length; i++)
			{
				stones[i].SafeAddForce(spitVelocity, ForceMode.VelocityChange);
			}
			base.enabled = false;
			StatsAndAchievements.UnlockAchievement(Achievement.ACH_BREAK_SURPRISE);
		}
	}

	private void SetKinematic(bool kinematic)
	{
		base.enabled = kinematic;
		if (stones != null)
		{
			for (int i = 0; i < stones.Length; i++)
			{
				stones[i].isKinematic = kinematic;
			}
		}
		if (scaffoldPieces != null)
		{
			for (int j = 0; j < scaffoldPieces.Length; j++)
			{
				scaffoldPieces[j].isKinematic = kinematic;
			}
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Reset State for Checkpoints ");
		}
		SetKinematic(kinematic: true);
	}
}
