using System.Collections.Generic;
using UnityEngine;

public class TextureTracker
{
	public delegate void CleanUpFunc(Object res);

	public class TrackedResource
	{
		public Object resource;

		public CleanUpFunc cleanUp;

		public List<Object> owners = new List<Object>();

		public TrackedResource(Object res, CleanUpFunc func, Object initOwner)
		{
			resource = res;
			cleanUp = func;
			owners.Add(initOwner);
		}

		public void AddOwner(Object newOwner)
		{
			if (newOwner != null && !owners.Contains(newOwner))
			{
				owners.Add(newOwner);
			}
		}

		public int RemoveOwner(Object oldOwner)
		{
			int count = owners.Count;
			for (int num = count - 1; num >= 0; num--)
			{
				if (owners[num] == oldOwner || owners[num] == null)
				{
					owners.RemoveAt(num);
				}
			}
			return owners.Count;
		}

		public int Prune()
		{
			int count = owners.Count;
			for (int num = count - 1; num >= 0; num--)
			{
				if (owners[num] == null)
				{
					owners.RemoveAt(num);
				}
			}
			return owners.Count;
		}
	}

	public static TextureTracker instance = new TextureTracker();

	private List<TrackedResource> resourceList = new List<TrackedResource>();

	public static void DontUnloadAsset(Object res)
	{
	}

	public static void UnloadAsset(Object res)
	{
		if (res != null)
		{
			Resources.UnloadAsset(res);
		}
	}

	public void AddMapping(Object owner, Object resource, CleanUpFunc cleanUp)
	{
		lock (this)
		{
			if (!(resource == null))
			{
				int count = resourceList.Count;
				for (int i = 0; i < count; i++)
				{
					if (resourceList[i].resource == resource)
					{
						resourceList[i].AddOwner(owner);
						return;
					}
				}
				resourceList.Add(new TrackedResource(resource, cleanUp, owner));
			}
		}
	}

	public void RemoveMapping(Object owner, Object resource)
	{
		lock (this)
		{
			int num = resourceList.Count;
			int num2 = 0;
			TrackedResource trackedResource;
			while (true)
			{
				if (num2 >= num)
				{
					return;
				}
				trackedResource = resourceList[num2];
				if (trackedResource.resource == null)
				{
					resourceList.RemoveAt(num2);
					num2--;
					num--;
					if (trackedResource.cleanUp != null)
					{
						trackedResource.cleanUp(trackedResource.resource);
					}
				}
				else if (trackedResource.resource == resource && trackedResource.RemoveOwner(owner) == 0)
				{
					break;
				}
				num2++;
			}
			resourceList.RemoveAt(num2);
			if (trackedResource.cleanUp != null)
			{
				trackedResource.cleanUp(resource);
			}
		}
	}

	public int GetNumOwners(Object resource)
	{
		lock (this)
		{
			if (resource != null)
			{
				int count = resourceList.Count;
				for (int i = 0; i < count; i++)
				{
					if (resourceList[i].resource == resource)
					{
						return resourceList[i].owners.Count;
					}
				}
			}
			return 0;
		}
	}
}
