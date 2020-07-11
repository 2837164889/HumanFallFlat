using HumanAPI;
using UnityEngine;

public class WaterHose : Node
{
	public NodeInput activate;

	public GameObject waterBodyGroup;

	public float emitDelay = 0.5f;

	public float particleLifespan = 1f;

	public Vector3 forceVector = new Vector3(0f, 0f, 5f);

	public Vector3 emitOffset = new Vector3(0f, 0f, 0.5f);

	public LineRenderer line;

	private Rigidbody[] waterBodies;

	private WaterJetParticle[] waterJetParticles;

	private float emitTimer;

	private bool isEmitting;

	private int nextEmitIndex;

	private void DisableParticle(int i)
	{
		waterBodies[i].gameObject.SetActive(value: false);
	}

	private void Start()
	{
		if (waterBodyGroup != null)
		{
			waterBodies = waterBodyGroup.GetComponentsInChildren<Rigidbody>(includeInactive: true);
		}
		waterJetParticles = new WaterJetParticle[waterBodies.Length];
		for (int i = 0; i < waterBodies.Length; i++)
		{
			DisableParticle(i);
			waterJetParticles[i] = waterBodies[i].GetComponent<WaterJetParticle>();
			if (waterJetParticles[i] == null)
			{
				Debug.LogError("Water body missing WaterJetParticle component", waterBodies[i].gameObject);
			}
		}
		line.positionCount = waterBodies.Length + 1;
		line.useWorldSpace = true;
		emitTimer = 0f;
		nextEmitIndex = 0;
		isEmitting = false;
	}

	private void EnableParticle(int i)
	{
		waterBodies[i].gameObject.SetActive(value: true);
	}

	private void Update()
	{
		if (activate.value >= 0.5f)
		{
			isEmitting = true;
		}
		else
		{
			isEmitting = false;
		}
		if (!isEmitting)
		{
			return;
		}
		emitTimer += Time.deltaTime;
		while (emitTimer >= emitDelay)
		{
			EnableParticle(nextEmitIndex);
			waterBodies[nextEmitIndex].transform.position = base.transform.TransformPoint(emitOffset);
			waterBodies[nextEmitIndex].transform.rotation = base.transform.rotation;
			waterBodies[nextEmitIndex].velocity = new Vector3(0f, 0f, 0f);
			waterBodies[nextEmitIndex].AddForce(base.transform.TransformDirection(forceVector));
			waterJetParticles[nextEmitIndex].particleAge = 0f;
			nextEmitIndex++;
			if (nextEmitIndex >= waterBodies.Length)
			{
				nextEmitIndex = 0;
			}
			emitTimer -= emitDelay;
		}
	}

	private void LateUpdate()
	{
		Vector3 position = (!isEmitting) ? line.GetPosition(1) : base.transform.TransformPoint(emitOffset);
		line.SetPosition(0, position);
		for (int i = 0; i < waterBodies.Length; i++)
		{
			int num = (waterBodies.Length - i + nextEmitIndex - 1) % waterBodies.Length;
			if (waterJetParticles[num].particleAge > particleLifespan)
			{
				DisableParticle(num);
			}
			if (waterBodies[num].gameObject.activeSelf)
			{
				position = waterBodies[num].transform.position;
			}
			line.SetPosition(i + 1, position);
		}
	}
}
