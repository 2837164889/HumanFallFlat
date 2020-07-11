using Multiplayer;
using System;
using UnityEngine;

public class NameTag : MonoBehaviour
{
	public delegate void NameTagActivatedCallBack(bool active);

	public TextMesh textMesh;

	public Shader textShader;

	public GameObject textBG;

	public float maxScale = 0.7f;

	public float minScale = 0.5f;

	public float maxScaleDistance = 30f;

	private const int kMaxNameChars = 16;

	private NetPlayer player;

	private MeshFilter meshFilter;

	private Renderer rendererBG;

	private Renderer rendererText;

	private int[] kIndices = new int[6]
	{
		0,
		1,
		2,
		2,
		1,
		3
	};

	private Vector3[] vertices = new Vector3[4];

	private const float kPixelToPolyWidth = 266.6f;

	private const float kPixelToPolyHeight = 200f;

	private float kInitialZ;

	private const float kWaitTimeOnForceShow = 5f;

	private bool forceShow;

	private float currentWaitTime;

	private static NameTagActivatedCallBack nameTagCallback;

	private static bool previousRenderState;

	public static void RegisterForNameTagCallbacks(NameTagActivatedCallBack callback)
	{
		nameTagCallback = (NameTagActivatedCallBack)Delegate.Combine(nameTagCallback, callback);
	}

	public static void UnRegisterForNameTagCallbacks(NameTagActivatedCallBack callback)
	{
		nameTagCallback = (NameTagActivatedCallBack)Delegate.Remove(nameTagCallback, callback);
	}

	private int CalculateStringWidth()
	{
		string text = textMesh.text;
		int length = text.Length;
		int num = 0;
		textMesh.font.RequestCharactersInTexture(text, textMesh.font.fontSize);
		for (int i = 0; i < length; i++)
		{
			textMesh.font.GetCharacterInfo(text[i], out CharacterInfo info, textMesh.font.fontSize);
			num += info.advance;
		}
		return num;
	}

	private void TruncateName()
	{
		player.nametag = this;
		string text = player.host.name;
		if (text.Length > 16)
		{
			text = text.Substring(0, 16);
			text += "...";
		}
		textMesh.text = text;
	}

	private void BuildMesh(int stringWidth, int stringHeight)
	{
		float num = (float)stringWidth / 266.6f;
		float num2 = (float)stringHeight / 200f;
		vertices[0] = new Vector3(0f - num, num2, kInitialZ);
		vertices[1] = new Vector3(num, num2, kInitialZ);
		vertices[2] = new Vector3(0f - num, 0f - num2, kInitialZ);
		vertices[3] = new Vector3(num, 0f - num2, kInitialZ);
	}

	private void Start()
	{
		player = GetComponentInParent<NetPlayer>();
		if (player == null)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		TruncateName();
		int stringWidth = CalculateStringWidth();
		int lineHeight = textMesh.font.lineHeight;
		rendererText = textMesh.GetComponent<Renderer>();
		rendererText.sharedMaterial.shader = textShader;
		rendererBG = textBG.GetComponent<Renderer>();
		meshFilter = textBG.transform.GetComponent<MeshFilter>();
		Mesh mesh = new Mesh();
		BuildMesh(stringWidth, lineHeight);
		mesh.vertices = vertices;
		mesh.SetIndices(kIndices, MeshTopology.Triangles, 0);
		mesh.MarkDynamic();
		mesh.UploadMeshData(markNoLogerReadable: false);
		meshFilter.sharedMesh = mesh;
		EnableRenderers(enable: false);
	}

	private void EnableRenderers(bool enable)
	{
		if (previousRenderState != enable)
		{
			nameTagCallback(enable);
			previousRenderState = enable;
		}
		rendererBG.enabled = enable;
		rendererText.enabled = enable;
	}

	private bool ShouldDisplay()
	{
		return !player.isLocalPlayer && MenuSystem.instance.activeMenu == null;
	}

	private void Update()
	{
		if (currentWaitTime > 0f)
		{
			currentWaitTime -= Time.deltaTime;
			if (currentWaitTime < 0f)
			{
				EnableRenderers(enable: false);
			}
		}
		else if (ShouldDisplay() && (Options.controllerBindings.ViewPlayerNames.IsPressed || Options.keyboardBindings.ViewPlayerNames.IsPressed))
		{
			forceShow = true;
			currentWaitTime = 5f;
			EnableRenderers(enable: true);
		}
		else
		{
			forceShow = false;
		}
	}

	public void Align(Transform transform)
	{
		if (forceShow && ShouldDisplay())
		{
			base.transform.rotation = transform.rotation;
			float num = Vector3.Distance(player.human.ragdoll.transform.position, Camera.current.transform.position);
			Camera component = transform.GetComponent<Camera>();
			float f = component.fieldOfView * 0.5f * ((float)Math.PI / 180f);
			float num2 = Mathf.Tan(f);
			float num3 = num * num2;
			base.transform.localScale = Vector3.one * num * num2 * Mathf.Lerp(maxScale, minScale, num / maxScaleDistance);
		}
	}
}
