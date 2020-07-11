using HumanAPI;
using System.Collections.Generic;

public class WorkshopTypeRepository<T> : List<T> where T : WorkshopItemMetadata
{
	private Dictionary<WorkshopItemSource, List<T>> bySource = new Dictionary<WorkshopItemSource, List<T>>
	{
		{
			WorkshopItemSource.BuiltIn,
			new List<T>()
		},
		{
			WorkshopItemSource.EditorPick,
			new List<T>()
		},
		{
			WorkshopItemSource.Subscription,
			new List<T>()
		},
		{
			WorkshopItemSource.LocalWorkshop,
			new List<T>()
		},
		{
			WorkshopItemSource.BuiltInLobbies,
			new List<T>()
		},
		{
			WorkshopItemSource.SubscriptionLobbies,
			new List<T>()
		}
	};

	public void Clear(WorkshopItemSource source)
	{
		List<T> list = bySource[source];
		for (int i = 0; i < list.Count; i++)
		{
			Remove(list[i]);
		}
		list.Clear();
	}

	public List<T> BySource(WorkshopItemSource source)
	{
		return bySource[source];
	}

	public List<T> AllSources(LevelSelectMenuMode mode, bool isMultiplayer)
	{
		List<T> list = new List<T>();
		foreach (WorkshopItemSource key in bySource.Keys)
		{
			if (!isMultiplayer || key != WorkshopItemSource.LocalWorkshop)
			{
				foreach (T item in bySource[key])
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	public List<T> AllSourcesLobbies(LevelSelectMenuMode mode)
	{
		List<T> list = new List<T>();
		foreach (WorkshopItemSource key in bySource.Keys)
		{
			if (key != WorkshopItemSource.LocalWorkshop && key != 0 && key != WorkshopItemSource.EditorPick)
			{
				foreach (T item in bySource[key])
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	public void AddItem(WorkshopItemSource source, WorkshopItemMetadata item)
	{
		item.levelType = source;
		T item2 = GetItem(item.folder);
		if (item2 != null)
		{
			int index = IndexOf(item2);
			base[index] = (T)item;
			index = bySource[source].IndexOf(item2);
			bySource[source][index] = (T)item;
		}
		else
		{
			bySource[source].Add((T)item);
			Add((T)item);
		}
	}

	protected void RemoveItem(WorkshopItemSource source, WorkshopItemMetadata item)
	{
		if (item != null)
		{
			Remove((T)item);
			foreach (List<T> value in bySource.Values)
			{
				value.Remove((T)item);
			}
		}
	}

	public virtual T GetItem(string path)
	{
		for (int i = 0; i < base.Count; i++)
		{
			if (base[i].folder == path)
			{
				return base[i];
			}
		}
		return (T)null;
	}
}
