using TMPro;
using UnityEngine;

namespace Multiplayer
{
	public class NetStats : MonoBehaviour
	{
		public NetGraph sendGraph;

		public NetGraph recvGraph;

		public NetGraph latencyGraph;

		public TextMeshProUGUI recvLabel;

		public TextMeshProUGUI sendLabel;

		public TextMeshProUGUI latencyLabel;

		private float recvSum;

		private float sendSum;

		private int frames;

		private int baseFrameId;

		private void Start()
		{
			Shell.RegisterCommand("netstats", OnNetStats, "netstats\r\nToggle netwok statistics display");
			base.transform.parent.gameObject.SetActive(value: false);
		}

		private void OnNetStats()
		{
			base.transform.parent.gameObject.SetActive(!base.transform.parent.gameObject.activeSelf);
			if (base.transform.parent.gameObject.activeSelf)
			{
				Shell.Print("netstats on");
			}
			else
			{
				Shell.Print("netstats off");
			}
		}

		private void Update()
		{
			frames++;
			sendSum = NetGame.instance.sendBps.kbps;
			recvSum = NetGame.instance.recvBps.kbps;
			if (frames == 4)
			{
				recvGraph.PushValue(recvSum);
				sendGraph.PushValue(sendSum);
				float max = sendGraph.GetMax();
				float max2 = recvGraph.GetMax();
				float range = Mathf.Max(max, max2);
				recvGraph.SetRange(range);
				sendGraph.SetRange(range);
				recvLabel.text = $"recv \t{recvSum:0.0}kbps \t{max2:0.0}kbps";
				sendLabel.text = $"send \t{sendSum:0.0}kbps \t{max:0.0}kbps";
				latencyLabel.text = $"buf \t {NetGame.instance.clientBuffer.latency:0.0}frames \tlag \t{NetGame.instance.clientLatency.latency * 1000f / 60f:0.0}ms";
				frames = 0;
				sendSum = (recvSum = 0f);
			}
		}

		private void OnGUI()
		{
			if (Shell.visible)
			{
				GUILayout.BeginArea(new Rect(10f, Screen.height / 2, Screen.width - 20, Screen.height / 2));
			}
			GUILayout.BeginVertical();
			GUI.color = Color.black;
			GUIStyle label = GUI.skin.label;
			GUILayout.Label($"Send: {NetGame.instance.sendBps.kbps:00.0} kbps / Recv: {NetGame.instance.recvBps.kbps:00.0} kbps", label);
			for (int i = 0; i < NetScope.all.Count; i++)
			{
				NetScope.all[i].RenderGUI(label);
			}
			GUILayout.EndVertical();
			if (Shell.visible)
			{
				GUILayout.EndArea();
			}
		}
	}
}
