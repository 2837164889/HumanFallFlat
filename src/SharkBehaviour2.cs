using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkBehaviour2 : MonoBehaviour
{
	public Rigidbody[] m_Tail;

	public float m_Strength = 10f;

	public float m_ShortFlop = 4f;

	public float m_LongFlop = 4f;

	public SharkState2 m_State = SharkState2.Flopping;

	private float m_nextFlop;

	public Vector3 dir;

	private Vector3 newDir;

	private Rigidbody rb;

	[SerializeField]
	private Rigidbody noseRigidbody;

	[SerializeField]
	private FishBox fishBox;

	[SerializeField]
	private List<Transform> sharkTargets;

	private int sharkTarget;

	public float duration = 20f;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		if (m_State == SharkState2.Water)
		{
			rb.useGravity = false;
			Rigidbody[] tail = m_Tail;
			foreach (Rigidbody rigidbody in tail)
			{
				rigidbody.useGravity = false;
			}
		}
		else if (m_nextFlop < Time.time)
		{
			StartCoroutine(Flop());
			m_nextFlop += ((!(Random.value < 0.95f)) ? m_LongFlop : m_ShortFlop) * Random.Range(0.5f, 1.5f) + duration * 2f * Time.fixedDeltaTime;
		}
		if (Vector3.Distance(base.transform.position, sharkTargets[0].position) < 14f && m_State != SharkState2.BreakingTank && fishBox.hasFishOnIt)
		{
			m_State = SharkState2.BreakingTank;
		}
		if (m_State != SharkState2.BreakingTank)
		{
			return;
		}
		noseRigidbody.AddForce((sharkTargets[sharkTarget].position - base.transform.position) * 300f);
		if (Vector3.Distance(base.transform.position, sharkTargets[sharkTarget].position) < 3f)
		{
			if (sharkTarget < sharkTargets.Count - 1)
			{
				sharkTarget++;
			}
			else
			{
				m_State = SharkState2.Idle;
			}
		}
	}

	private IEnumerator Flop()
	{
		float strength2 = m_Strength * Mathf.Sign(Random.Range(-1, 1));
		for (int j = 0; (float)j < duration; j++)
		{
			Rigidbody[] tail = m_Tail;
			foreach (Rigidbody rigidbody in tail)
			{
				base.transform.InverseTransformDirection(dir);
				rigidbody.AddRelativeTorque(dir * strength2, ForceMode.Impulse);
			}
			yield return new WaitForFixedUpdate();
		}
		strength2 *= -0.5f;
		for (int i = 0; (float)i < duration; i++)
		{
			Rigidbody[] tail2 = m_Tail;
			foreach (Rigidbody rigidbody2 in tail2)
			{
				base.transform.InverseTransformDirection(dir);
				rigidbody2.AddRelativeTorque(dir * strength2, ForceMode.Impulse);
			}
			yield return new WaitForFixedUpdate();
		}
		yield return null;
	}
}
