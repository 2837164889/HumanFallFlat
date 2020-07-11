using Multiplayer;
using UnityEngine;

public class GasHouse : MonoBehaviour
{
	private Rigidbody[] gasHousePieces;

	private void Awake()
	{
		gasHousePieces = GetComponentsInChildren<Rigidbody>();
	}

	public void PrepareForDestruction()
	{
		if (gasHousePieces != null && !NetGame.isClient)
		{
			Rigidbody[] array = gasHousePieces;
			foreach (Rigidbody rigidbody in array)
			{
				rigidbody.isKinematic = false;
			}
		}
	}
}
