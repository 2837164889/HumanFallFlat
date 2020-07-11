using HumanAPI;
using System.Linq;
using UnityEngine;

public class ContactSignal : Node
{
	public NodeOutput value;

	private int contacts;

	public bool OneShotOn;

	public GameObject[] contactObjects;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (contactObjects.Any((GameObject x) => collision.gameObject.transform.IsChildOf(x.transform)))
		{
			contacts++;
			value.SetValue(1f);
			Debug.Log("OCEn");
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (contactObjects.Any((GameObject x) => collision.gameObject.transform.IsChildOf(x.transform)))
		{
			contacts--;
			if (contacts == 0 && !OneShotOn)
			{
				Debug.Log("OCEx");
				value.SetValue(0f);
			}
		}
	}
}
