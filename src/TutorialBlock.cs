using HumanAPI;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TutorialBlock : Node
{
	public float dismissTime = 1.5f;

	public int maxShowCount = 5;

	public float maxDisplayTime = 30f;

	public float activateNonsenseTime = 10f;

	public int activateEnterCount = 3;

	public float deactivateMeaningfulTime = 1f;

	public float nonsenseTimer;

	public float meaningfulTimer;

	public float entryCount;

	public bool show;

	public bool nonsenseDetectedExternally;

	private float displayStart = -1f;

	private string oldText = string.Empty;

	private static List<TutorialBlock> allBlocks = new List<TutorialBlock>();

	public bool playerInside;

	private float leaveTime;

	public void OnFail()
	{
		if (base.enabled && displayStart == -1f)
		{
			Show();
			show = true;
		}
	}

	public void OnSucceed()
	{
		Hide();
		base.gameObject.SetActive(value: false);
		show = false;
	}

	public void OnDestroy()
	{
		Hide();
	}

	public static void RefreshTextOnAllBlocks()
	{
		int i = 0;
		for (int count = allBlocks.Count; i < count; i++)
		{
			TutorialBlock tutorialBlock = allBlocks[i];
			if (tutorialBlock != null && tutorialBlock.displayStart != -1f && SubtitleManager.instance.instructionText.text == tutorialBlock.oldText)
			{
				string tutorialText = TutorialRepository.instance.GetTutorialText(tutorialBlock.name);
				if (tutorialText != tutorialBlock.oldText)
				{
					tutorialBlock.oldText = tutorialText;
					SubtitleManager.instance.SetInstruction(tutorialText);
				}
			}
		}
	}

	private void Show()
	{
		if (maxShowCount != 0)
		{
			string instruction = oldText = TutorialRepository.instance.GetTutorialText(base.name);
			allBlocks.Add(this);
			SubtitleManager.instance.SetInstruction(instruction);
			if (displayStart == -1f)
			{
				displayStart = Time.time;
			}
		}
	}

	private void Hide()
	{
		allBlocks.Remove(this);
		if (displayStart != -1f)
		{
			maxShowCount--;
			float num = displayStart + dismissTime - Time.time;
			if (num <= 0f)
			{
				SubtitleManager.instance.ClearInstruction();
			}
			else
			{
				SubtitleManager.instance.ClearInstruction(num);
			}
			oldText = string.Empty;
			displayStart = -1f;
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Player")
		{
			return;
		}
		playerInside = true;
		if (show)
		{
			Show();
		}
		else if (Time.time - leaveTime > 0.5f)
		{
			entryCount += 1f;
			if (entryCount >= (float)activateEnterCount && !IsPlayerActivityMakingSense())
			{
				OnFail();
			}
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (!(other.tag != "Player"))
		{
			playerInside = false;
			leaveTime = Time.time;
			Hide();
		}
	}

	public virtual bool IsPlayerActivityMakingSense()
	{
		return false;
	}

	public virtual bool CheckInstantSuccess(bool playerInside)
	{
		return false;
	}

	public void Update()
	{
		if (CheckInstantSuccess(playerInside))
		{
			OnSucceed();
		}
		else
		{
			if (!playerInside && !nonsenseDetectedExternally)
			{
				return;
			}
			if (displayStart != -1f && Time.time - displayStart > maxDisplayTime)
			{
				OnSucceed();
			}
			else if (IsPlayerActivityMakingSense())
			{
				meaningfulTimer += Time.deltaTime;
				if (meaningfulTimer > deactivateMeaningfulTime)
				{
					OnSucceed();
				}
			}
			else
			{
				nonsenseTimer += Time.deltaTime;
				if (nonsenseTimer > activateNonsenseTime)
				{
					OnFail();
				}
			}
		}
	}

	public void ReportNonsense()
	{
		nonsenseDetectedExternally = true;
	}

	public void UnreportNonsense()
	{
		if (nonsenseDetectedExternally)
		{
			nonsenseDetectedExternally = false;
			if (!playerInside)
			{
				leaveTime = Time.time;
				Hide();
			}
		}
	}
}
