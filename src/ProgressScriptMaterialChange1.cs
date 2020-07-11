using HumanAPI;
using System.Collections;
using UnityEngine;

public class ProgressScriptMaterialChange1 : Node
{
	[Tooltip("Reference to the Render of the progress bar")]
	public MeshRenderer progressBar;

	[Tooltip("The amount of progree to show on the bar as a float of 1")]
	public NodeInput incomingProgress;

	private uint total;

	private uint lastGoal;

	private uint goal;

	private float phase;

	private float fromProgress;

	private float toProgress;

	private float currentProgress;

	private Material[] materials;

	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	private void Awake()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Awake ");
		}
		materials = progressBar.materials;
		progressBar.sharedMaterials = materials;
	}

	private IEnumerator Start()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Start ");
		}
		if (GiftService.instance != null)
		{
			GiftService.instance.RefreshStatus();
		}
		while (GiftService.status == null)
		{
			yield return null;
		}
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(30, 90));
			if (GiftService.instance != null)
			{
				GiftService.instance.RefreshStatus();
			}
		}
	}

	private void Update()
	{
		materials[1].mainTextureOffset = materials[1].mainTextureOffset + new Vector2(Time.deltaTime / 10f, 0f);
		if (phase < 1f)
		{
			phase = Mathf.MoveTowards(phase, 1f, Time.deltaTime / 2f);
			Sync();
		}
	}

	public override void Process()
	{
		toProgress = incomingProgress.value;
		phase = 0f;
	}

	private void Sync()
	{
		currentProgress = Mathf.Lerp(fromProgress, toProgress, Ease.easeInOutQuad(0f, 1f, phase));
		Material obj = materials[1];
		Vector2 mainTextureOffset = materials[1].mainTextureOffset;
		obj.mainTextureOffset = new Vector2(mainTextureOffset.y, Mathf.Lerp(0.985f, 0.525f, currentProgress));
	}

	public int CalculateMaxDeltaSizeInBits()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " CalculateMaxDeltaSizeInBits ");
		}
		return 232;
	}

	public void SetMaster(bool isMaster)
	{
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
	}

	public void ResetStateProgressBat(int checkpoint)
	{
		phase = 0f;
		fromProgress = 0f;
		toProgress = 0f;
		currentProgress = 0f;
	}

	private IEnumerator SkinUnlockChutes()
	{
		for (int i = 0; i < 100; i++)
		{
			Fireworks.instance.ShootFirework();
			yield return new WaitForSeconds(Random.Range(0.05f, 0.7f));
		}
	}
}
