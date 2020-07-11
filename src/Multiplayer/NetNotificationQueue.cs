using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Multiplayer
{
	public class NetNotificationQueue
	{
		private struct NotifyClientTask
		{
			public int timeID;

			public int frameID;

			public NetStream full;

			public NetHost client;

			public NetScope scope;
		}

		public List<NetHost> clients = new List<NetHost>();

		private Thread thread;

		private bool threaded;

		private object notifyQueueLock = new object();

		private int frameCapacity = 1000;

		private Queue<NotifyClientTask> notifyQueue = new Queue<NotifyClientTask>();

		private bool mKillThread;

		public void StartNotifyThread()
		{
			threaded = (NetGame.instance.serverThreads != 0);
			if (threaded)
			{
				thread = new Thread(NotifyWorker);
				thread.IsBackground = true;
				thread.Start();
			}
		}

		private void NotifyWorker()
		{
			while (NetGame.isNetStarted || NetGame.isNetStarting)
			{
				Thread.Sleep(5);
				HandleNotifyQueue();
				if (mKillThread)
				{
					break;
				}
			}
			thread = null;
			mKillThread = false;
		}

		private void HandleNotifyQueue()
		{
			while (true)
			{
				NotifyClientTask notifyClientTask;
				lock (notifyQueueLock)
				{
					if (notifyQueue.Count > frameCapacity * 10)
					{
						int num = notifyQueue.Count - frameCapacity * 6;
						while (num-- > 0)
						{
							notifyClientTask = notifyQueue.Dequeue();
							notifyClientTask.full = notifyClientTask.full.Release();
						}
					}
					if (notifyQueue.Count == 0)
					{
						return;
					}
					notifyClientTask = notifyQueue.Dequeue();
				}
				notifyClientTask.scope.NotifyClients(notifyClientTask.frameID, notifyClientTask.timeID, notifyClientTask.full, notifyClientTask.client);
				notifyClientTask.full = notifyClientTask.full.Release();
			}
		}

		public void NotifyClients(NetScope scope, int frameId, int timeId, NetStream full)
		{
			lock (notifyQueueLock)
			{
				frameCapacity = clients.Count * NetScope.all.Count;
				for (int i = 0; i < clients.Count; i++)
				{
					if (clients[i].isReady)
					{
						full.AddRef();
						notifyQueue.Enqueue(new NotifyClientTask
						{
							frameID = frameId,
							timeID = timeId,
							full = full,
							client = clients[i],
							scope = scope
						});
					}
				}
			}
			if (!threaded)
			{
				HandleNotifyQueue();
			}
		}

		public void StopNotifyThreadPhase1()
		{
			if (thread != null)
			{
				thread.Abort();
				thread = null;
			}
		}

		[Conditional("NEVER_SET_THIS_DEFINE_1234")]
		public void StopNotifyThreadPhase2()
		{
		}
	}
}
