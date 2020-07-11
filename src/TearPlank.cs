using UnityEngine;

public class TearPlank : MonoBehaviour
{
	public Flame TearPlankEnd1;

	public Flame TearPlankEnd2;

	private GameObject particles1;

	private GameObject particles2;

	public void RespawnExtra()
	{
		TearPlankEnd1.Extinguish();
		TearPlankEnd2.Extinguish();
		DisableParticles();
		EnablePlanks();
	}

	private void EnablePlanks()
	{
		TearPlankEnd1.gameObject.SetActive(value: true);
		TearPlankEnd2.gameObject.SetActive(value: true);
	}

	private void DisableParticles()
	{
		if (particles1 != null)
		{
			particles1.SetActive(value: false);
		}
		if (particles2 != null)
		{
			particles2.SetActive(value: false);
		}
	}

	public void OnColdChange()
	{
		RespawnExtra();
	}

	public void OnHotChange(Flame endPlank)
	{
		if (endPlank.Equals(TearPlankEnd1))
		{
			particles1.SetActive(value: true);
			TearPlankEnd2.gameObject.SetActive(value: false);
		}
		else
		{
			particles2.SetActive(value: true);
			TearPlankEnd1.gameObject.SetActive(value: false);
		}
	}

	private void Start()
	{
		if (TearPlankEnd1 != null)
		{
			particles1 = TearPlankEnd1.gameObject.transform.GetChild(0).transform.gameObject;
			particles2 = TearPlankEnd2.gameObject.transform.GetChild(0).transform.gameObject;
		}
	}
}
