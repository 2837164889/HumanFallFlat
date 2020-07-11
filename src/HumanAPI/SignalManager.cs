using System;
using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI
{
	public class SignalManager : MonoBehaviour
	{
		public static bool suspendFX = true;

		public static bool skipTransitions = true;

		private static bool isInitialized = false;

		private static SignalManager instance;

		private static Queue<Node>[] dirtyNodes = new Queue<Node>[3]
		{
			new Queue<Node>(),
			new Queue<Node>(),
			new Queue<Node>()
		};

		private static int lowestQueue = 2;

		private static int currentQueue = 0;

		private static bool inProcess = false;

		internal static void BeginReset()
		{
			EnsureInstance();
			isInitialized = true;
			suspendFX = true;
			skipTransitions = true;
		}

		public static void EndReset()
		{
			for (int i = 0; i < Node.all.Count; i++)
			{
				Node.all[i].ResetOutputs();
			}
			for (int j = 0; j < Node.all.Count; j++)
			{
				Node.all[j].ResetInputs();
			}
			for (int k = 0; k < Node.all.Count; k++)
			{
				Node.all[k].SetDirty();
			}
			EnsureInstance();
			instance.Update();
			suspendFX = false;
			skipTransitions = false;
		}

		private static void EnsureInstance()
		{
			if (instance == null)
			{
				GameObject gameObject = new GameObject("SignalManager");
				instance = gameObject.AddComponent<SignalManager>();
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
		}

		public static void AddDirtyNode(Node node)
		{
			EnsureInstance();
			int priority = (int)node.priority;
			dirtyNodes[priority].Enqueue(node);
			if (priority < lowestQueue)
			{
				lowestQueue = priority;
			}
			ProcessQueues();
		}

		private void FixedUpdate()
		{
			currentQueue = 1;
			ProcessQueues();
		}

		private void Update()
		{
			currentQueue = 2;
			ProcessQueues();
		}

		private void LateUpdate()
		{
			currentQueue = 0;
		}

		private static void ProcessQueues()
		{
			if (inProcess)
			{
				return;
			}
			inProcess = true;
			int num = 1000;
			while (lowestQueue <= currentQueue)
			{
				if (num-- <= 0)
				{
					throw new Exception("Infinite loop in Signal chain");
				}
				if (dirtyNodes[lowestQueue].Count > 0)
				{
					Node node = dirtyNodes[lowestQueue].Dequeue();
					if (node != null && node.isDirty)
					{
						node.isDirty = false;
						node.Process();
					}
				}
				else
				{
					lowestQueue++;
				}
			}
			inProcess = false;
		}
	}
}
