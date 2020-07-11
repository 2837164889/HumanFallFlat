using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XmasLandAchievement : MonoBehaviour
{
	public List<GameObject> playersJumping = new List<GameObject>();

	private bool haveSnowBoard;

	public void SetJumping(GameObject user)
	{
		StartCoroutine(Jumping(user));
	}

	public void SetJumpingWithSnowBoard()
	{
		StartCoroutine(WithSnowBoard());
	}

	private IEnumerator Jumping(GameObject user)
	{
		playersJumping.Add(user);
		for (int i = 0; i < 120; i++)
		{
			yield return null;
		}
		yield return null;
		playersJumping.Remove(user);
	}

	private IEnumerator WithSnowBoard()
	{
		haveSnowBoard = true;
		for (int i = 0; i < 120; i++)
		{
			yield return null;
		}
		yield return null;
		haveSnowBoard = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && playersJumping.Contains(other.transform.parent.gameObject) && haveSnowBoard)
		{
			StatsAndAchievements.UnlockAchievement(Achievement.ACH_XMAS_SKI_LAND);
		}
	}
}
