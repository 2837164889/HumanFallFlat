using System;
using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI
{
	public class SteamNode : MonoBehaviour
	{
		[NonSerialized]
		public List<SteamPort> ports = new List<SteamPort>();

		private void OnEnable()
		{
			SteamPort[] componentsInChildren = GetComponentsInChildren<SteamPort>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (!ports.Contains(componentsInChildren[i]))
				{
					componentsInChildren[i].node = this;
					ports.Add(componentsInChildren[i]);
				}
			}
			SteamSystem.Recalculate(this);
		}
	}
}
