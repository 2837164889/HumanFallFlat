using System;
using System.Collections.Generic;
using UnityEngine;

public class Listener : MonoBehaviour
{
	public static Listener instance;

	private Transform originalParent;

	[NonSerialized]
	public List<Transform> earList = new List<Transform>();

	private bool transformOverride;

	private void Awake()
	{
		if (originalParent == null)
		{
			originalParent = base.transform.parent;
		}
	}

	private void OnEnable()
	{
		instance = this;
	}

	private void OnDisable()
	{
		Update();
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	public void OverrideTransform(Transform t)
	{
		transformOverride = true;
		base.transform.SetParent(t, worldPositionStays: false);
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
	}

	public void EndTransfromOverride()
	{
		transformOverride = false;
		UpdateHierarchy();
	}

	public void AddListenTransform(Transform ears)
	{
		earList.Add(ears);
		UpdateHierarchy();
	}

	public void RemoveListenTransform(Transform ears)
	{
		earList.Remove(ears);
		UpdateHierarchy();
	}

	public void AddHuman(Human human)
	{
		AddListenTransform(human.ragdoll.partHead.transform);
	}

	public void RemoveHuman(Human human)
	{
		RemoveListenTransform(human.ragdoll.partHead.transform);
	}

	private void UpdateHierarchy()
	{
		if (transformOverride)
		{
			return;
		}
		earList.Remove(null);
		if (earList.Count == 1)
		{
			if (base.transform.parent != earList[0])
			{
				base.transform.SetParent(earList[0], worldPositionStays: false);
				base.transform.localPosition = Vector3.zero;
				base.transform.localRotation = Quaternion.identity;
			}
		}
		else if (base.transform.parent != originalParent && originalParent != null)
		{
			base.transform.SetParent(originalParent);
		}
	}

	private void Update()
	{
		if (!transformOverride && earList.Count > 0)
		{
			Vector3 vector = Vector3.zero;
			Quaternion quaternion = Quaternion.identity;
			for (int i = 0; i < earList.Count; i++)
			{
				vector = Vector3.Lerp(vector, earList[i].position, 1f / (float)(i + 1));
				quaternion = Quaternion.Slerp(quaternion, earList[i].rotation, 1f / (float)(i + 1));
			}
			base.transform.position = vector;
			base.transform.rotation = quaternion;
		}
	}
}
