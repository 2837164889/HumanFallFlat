using System.Collections.Generic;
using UnityEngine;

public class VideoRepository : MonoBehaviour
{
	public static VideoRepository instance;

	public VideoRepositoryItem[] items;

	private void OnEnable()
	{
		instance = this;
		items = GetComponentsInChildren<VideoRepositoryItem>();
	}

	public List<VideoRepositoryItem> ListAvailableItems()
	{
		GameSave.GetProgress(out int levelNumber, out int checkpoint);
		List<VideoRepositoryItem> list = new List<VideoRepositoryItem>();
		for (int i = 0; i < items.Length; i++)
		{
			VideoRepositoryItem videoRepositoryItem = items[i];
			if ((videoRepositoryItem.level < levelNumber || (videoRepositoryItem.level == levelNumber && videoRepositoryItem.checkpoint < checkpoint)) && videoRepositoryItem.IsVisible())
			{
				list.Add(videoRepositoryItem);
			}
		}
		return list;
	}

	public bool HasAvailableItems()
	{
		GameSave.GetProgress(out int levelNumber, out int checkpoint);
		for (int i = 0; i < items.Length; i++)
		{
			VideoRepositoryItem videoRepositoryItem = items[i];
			if (videoRepositoryItem.level < levelNumber || (videoRepositoryItem.level == levelNumber && videoRepositoryItem.checkpoint < checkpoint))
			{
				return true;
			}
		}
		return false;
	}
}
