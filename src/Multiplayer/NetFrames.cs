using System;
using System.Collections.Generic;

namespace Multiplayer
{
	public class NetFrames
	{
		public object framesLock = new object();

		public List<FrameState> frameQueue = new List<FrameState>();

		public List<FrameState> eventQueue = new List<FrameState>();

		public bool AllowDiscontinuous;

		public void DropOldStates(int frameId)
		{
			while (frameQueue.Count > 0)
			{
				FrameState frameState = frameQueue[0];
				if (frameState.frameId < frameId)
				{
					FrameState frameState2 = frameQueue[0];
					frameState2.stream.Release();
					frameQueue.RemoveAt(0);
					continue;
				}
				break;
			}
		}

		public void DropOldEvents(int frameId)
		{
			while (eventQueue.Count > 0)
			{
				FrameState frameState = eventQueue[0];
				if (frameState.frameId < frameId)
				{
					FrameState frameState2 = eventQueue[0];
					frameState2.stream.Release();
					eventQueue.RemoveAt(0);
					continue;
				}
				break;
			}
		}

		public void PushState(int frameId, NetStream state)
		{
			int num = frameQueue.Count;
			while (num - 1 > 0)
			{
				FrameState frameState = frameQueue[num - 1];
				if (frameState.frameId <= frameId)
				{
					break;
				}
				num--;
			}
			frameQueue.Insert(num, new FrameState
			{
				frameId = frameId,
				stream = state
			});
		}

		public void PushEvents(int frameId, NetStream eventStream)
		{
			eventQueue.Add(new FrameState
			{
				frameId = frameId,
				stream = eventStream
			});
		}

		public void LimitHistory()
		{
			int num = 1024;
			if (frameQueue.Count > num)
			{
				FrameState frameState = frameQueue[frameQueue.Count - num];
				DropOldStates(frameState.frameId);
			}
			if (eventQueue.Count > num)
			{
				FrameState frameState2 = eventQueue[eventQueue.Count - num];
				DropOldEvents(frameState2.frameId);
			}
		}

		public NetStream GetState(int frameId, bool rewind = false)
		{
			for (int i = 0; i < frameQueue.Count; i++)
			{
				FrameState frameState = frameQueue[i];
				if (frameState.frameId == frameId)
				{
					FrameState frameState2 = frameQueue[i];
					NetStream stream = frameState2.stream;
					if (rewind)
					{
						stream.Seek(0);
					}
					return stream;
				}
			}
			return null;
		}

		public int TestForState(int frameId)
		{
			int count = frameQueue.Count;
			if (count == 0)
			{
				return -1;
			}
			int num = count - 1;
			FrameState frameState = frameQueue[num];
			int frameId2 = frameState.frameId;
			if (frameId >= frameId2)
			{
				return (frameId != frameId2) ? (-1) : num;
			}
			FrameState frameState2 = frameQueue[0];
			int frameId3 = frameState2.frameId;
			if (frameId <= frameId3)
			{
				return (frameId != frameId3) ? (-1) : 0;
			}
			int num2 = 0;
			while (num2 < num)
			{
				int num3 = num + num2 >> 1;
				FrameState frameState3 = frameQueue[num3];
				int frameId4 = frameState3.frameId;
				if (frameId == frameId4)
				{
					return num3;
				}
				if (frameId < frameId4)
				{
					num = num3 - 1;
				}
				else
				{
					num2 = num3 + 1;
				}
			}
			if (num2 == num)
			{
				FrameState frameState4 = frameQueue[num2];
				if (frameState4.frameId == frameId)
				{
					return num2;
				}
			}
			return -1;
		}

		public bool GetState(int frame, float fraction, out int frame0id, out NetStream frame0, out NetStream frame1, out float mix)
		{
			frame0 = null;
			frame1 = null;
			frame0id = -1;
			try
			{
				for (int num = frameQueue.Count - 1; num >= 0; num--)
				{
					FrameState frameState = frameQueue[num];
					if (frameState.frameId <= frame)
					{
						FrameState frameState2 = frameQueue[num];
						frame0id = frameState2.frameId;
						if (num < frameQueue.Count - 1)
						{
							FrameState frameState3 = frameQueue[num + 1];
							int frameId = frameState3.frameId;
							if (AllowDiscontinuous && frameId >= frame0id + 60)
							{
								if ((float)(frameId - frame) <= 0.25f + fraction)
								{
									FrameState frameState4 = frameQueue[num + 1];
									frame1 = NetStream.AllocStream(frameState4.stream);
									frame0 = NetStream.AllocStream(frame1);
									frame0id = frameId;
									mix = 1f;
									return true;
								}
								FrameState frameState5 = frameQueue[num];
								frame0 = NetStream.AllocStream(frameState5.stream);
								frame1 = NetStream.AllocStream(frame0);
								mix = 0f;
								return true;
							}
							FrameState frameState6 = frameQueue[num];
							frame0 = NetStream.AllocStream(frameState6.stream);
							FrameState frameState7 = frameQueue[num + 1];
							frame1 = NetStream.AllocStream(frameState7.stream);
							mix = ((float)(frame - frame0id) + fraction) / (float)(frameId - frame0id);
							return true;
						}
						FrameState frameState8 = frameQueue[num];
						frame0 = NetStream.AllocStream(frameState8.stream);
						frame1 = NetStream.AllocStream(frame0);
						mix = 0f;
						return true;
					}
				}
				mix = 0f;
				return false;
			}
			catch (Exception)
			{
				if (frame0 != null)
				{
					frame0 = frame0.Release();
				}
				if (frame1 != null)
				{
					frame1 = frame1.Release();
				}
				throw;
			}
		}
	}
}
