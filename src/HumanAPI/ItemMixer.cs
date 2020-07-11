using Multiplayer;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace HumanAPI
{
	[RequireComponent(typeof(NetIdentity))]
	public class ItemMixer : Node, IReset
	{
		public enum MixEffect
		{
			Destroy,
			DestroyFirst,
			Retain
		}

		[Serializable]
		public struct MixItem
		{
			[Tooltip("Item to add. This item or any of its children will be mixed.")]
			public GameObject item;

			[Tooltip("What to do with the mixed items when complete")]
			public MixEffect mixEffect;

			public UnityEvent itemAdded;
		}

		public NodeInput input;

		public NodeOutput allItems;

		[Tooltip("Set to true to allow mixing to work. If false, mix won't happen until ActivateMixing() has been called or input is held at 1.0 for long enough.")]
		public bool mixingActive;

		[Tooltip("Number of seconds input must be 1.0 in order to activate.")]
		public float activationTime;

		private float activationTimer;

		public MixItem[] items;

		public UnityEvent badItemAdded;

		public UnityEvent allItemsAdded;

		private int[] itemAdded;

		private bool allItemsHaveBeenAdded;

		private bool doneMixing;

		private uint evtMixed;

		private uint evtDestroyIngredient;

		private NetIdentity identity;

		private void Start()
		{
			ResetState(0, 0);
			identity = GetComponent<NetIdentity>();
			evtMixed = identity.RegisterEvent(OnMixed);
			evtDestroyIngredient = identity.RegisterEvent(OnDestroyIngredient);
		}

		public void Update()
		{
			if (!doneMixing)
			{
				if (Mathf.Abs(input.value) < 1f)
				{
					activationTimer = 0f;
					mixingActive = false;
				}
				else
				{
					activationTimer += Time.deltaTime;
					if (activationTimer >= activationTime)
					{
						mixingActive = true;
					}
				}
			}
			if (allItemsHaveBeenAdded && mixingActive && !doneMixing)
			{
				SendMixed();
				doneMixing = true;
			}
		}

		public void OnTriggerEnter(Collider other)
		{
			if (doneMixing || other == null)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < items.Length; i++)
			{
				if (!other.gameObject.transform.IsChildOf(items[i].item.transform))
				{
					continue;
				}
				flag = true;
				if (items[i].mixEffect == MixEffect.Destroy || (itemAdded[i] == 0 && items[i].mixEffect == MixEffect.DestroyFirst))
				{
					if (other.gameObject == items[i].item)
					{
						GrabManager.Release(other.gameObject);
						SendDestroyIngredient(i, -1);
					}
					else
					{
						for (int j = 0; j < items[i].item.transform.childCount; j++)
						{
							Transform child = items[i].item.transform.GetChild(j);
							if (other.gameObject.transform.IsChildOf(child))
							{
								GrabManager.Release(other.gameObject);
								GrabManager.Release(child.gameObject);
								SendDestroyIngredient(i, j);
								break;
							}
						}
					}
				}
				itemAdded[i]++;
				items[i].itemAdded.Invoke();
				if (itemAdded.All((int x) => x != 0))
				{
					allItemsHaveBeenAdded = true;
					allItems.SetValue(1f);
				}
				break;
			}
			if (!flag)
			{
				badItemAdded.Invoke();
			}
		}

		public void OnTriggerExit(Collider other)
		{
			if (doneMixing)
			{
				return;
			}
			for (int i = 0; i < items.Length; i++)
			{
				if (other.gameObject.transform.IsChildOf(items[i].item.transform))
				{
					itemAdded[i]--;
					if (itemAdded[i] < 0)
					{
						itemAdded[i] = 0;
					}
					allItemsHaveBeenAdded = false;
					allItems.SetValue(0f);
				}
			}
		}

		private void OnMixed(NetStream stream)
		{
			DoMix();
		}

		private void SendMixed()
		{
			DoMix();
			if (NetGame.isServer || ReplayRecorder.isRecording)
			{
				NetStream netStream = identity.BeginEvent(evtMixed);
				identity.EndEvent();
			}
		}

		private void DoMix()
		{
			allItemsAdded.Invoke();
		}

		private void OnDestroyIngredient(NetStream stream)
		{
			uint itemIndex = stream.ReadUInt32(8);
			int childIndex = stream.ReadInt32(8);
			DoDestroyIngredient((int)itemIndex, childIndex);
		}

		private void SendDestroyIngredient(int itemIndex, int childIndex)
		{
			DoDestroyIngredient(itemIndex, childIndex);
			if (NetGame.isServer || ReplayRecorder.isRecording)
			{
				NetStream netStream = identity.BeginEvent(evtDestroyIngredient);
				netStream.Write((uint)itemIndex, 8);
				netStream.Write(childIndex, 8);
				identity.EndEvent();
			}
		}

		private void DoDestroyIngredient(int itemIndex, int childIndex)
		{
			GameObject item = items[itemIndex].item;
			GameObject gameObject = (childIndex >= 0) ? item.transform.GetChild(childIndex).gameObject : item;
			NetBody component = gameObject.GetComponent<NetBody>();
			if ((bool)component)
			{
				component.SetVisible(visible: false);
			}
			else
			{
				gameObject.SetActive(value: false);
			}
		}

		public void ResetState(int checkpoint, int subObjectives)
		{
			activationTimer = 0f;
			itemAdded = new int[items.Length];
			allItemsHaveBeenAdded = false;
			doneMixing = false;
			allItems.SetValue(0f);
		}
	}
}
