using HumanAPI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RagdollTexture : MonoBehaviour
{
	private WorkshopItemType part;

	private RagdollModel model;

	public Texture texture;

	public Texture2D baseTexture;

	public Texture2D bakedTexture;

	public RenderTexture renderTexture;

	private bool baseTextureIsAsset;

	private bool maskTextureIsAsset;

	public bool paintingEnabled;

	public int width;

	public int height;

	private string savePath;

	private bool textureLoadSuppressed;

	private Color appliedColor1;

	private Color appliedColor2;

	private Color appliedColor3;

	private Coroutine bakeCoroutine;

	private List<Texture2D> undo = new List<Texture2D>();

	private List<Texture2D> redo = new List<Texture2D>();

	public Texture paintBase;

	public RenderTexture paintSource;

	public RenderTexture paintTarget;

	private RenderBuffer[] buffers = new RenderBuffer[2];

	[CompilerGenerated]
	private static TextureTracker.CleanUpFunc _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static TextureTracker.CleanUpFunc _003C_003Ef__mg_0024cache1;

	private void ChangeBaseTexture(Texture2D newRes, bool isAsset)
	{
		if (!(newRes == baseTexture))
		{
			if (baseTexture != null)
			{
				ReleaseBaseTexture();
			}
			else if (baseTextureIsAsset)
			{
				TextureTracker.instance.RemoveMapping(this, null);
			}
			baseTexture = newRes;
			baseTextureIsAsset = isAsset;
			if (isAsset && newRes != null)
			{
				TextureTracker.instance.AddMapping(this, newRes, TextureTracker.DontUnloadAsset);
			}
		}
	}

	private void ChangeMaskTexture(Texture2D newRes, bool isAsset)
	{
		if (!(model == null) && !(newRes == model.maskTexture))
		{
			ReleaseMaskTexture();
			model.maskTexture = newRes;
			maskTextureIsAsset = isAsset;
			if (isAsset && newRes != null)
			{
				TextureTracker.instance.AddMapping(model, newRes, TextureTracker.DontUnloadAsset);
			}
		}
	}

	public void BindToModel(RagdollModel model)
	{
		part = model.ragdollPart;
		this.model = model;
	}

	public void LoadFromPreset(RagdollPresetMetadata preset)
	{
		ReleaseBaseTexture();
		savePath = FileTools.Combine(preset.folder, part.ToString() + ".png");
		textureLoadSuppressed = preset.GetPart(part).suppressCustomTexture;
		if (!textureLoadSuppressed)
		{
			RagdollPresetPartMetadata ragdollPresetPartMetadata = preset.GetPart(part);
			if (ragdollPresetPartMetadata != null && ragdollPresetPartMetadata.bytes != null)
			{
				ChangeBaseTexture(FileTools.TextureFromBytes(part.ToString(), ragdollPresetPartMetadata.bytes), isAsset: false);
			}
			else if (!string.IsNullOrEmpty(savePath))
			{
				bool isAsset;
				Texture2D newRes = FileTools.ReadTexture(savePath, out isAsset);
				ChangeBaseTexture(newRes, isAsset);
			}
			if (baseTexture != null)
			{
				baseTexture.Compress(highQuality: true);
				baseTexture.Apply(updateMipmaps: true);
			}
		}
		if (model.meta.metaPath.StartsWith("builtin"))
		{
			if (baseTexture == null)
			{
				ChangeBaseTexture(HFFResources.instance.FindTextureResource("SkinTextures/" + model.meta.modelPrefab.name + "Color"), isAsset: true);
			}
			if (model.maskTexture == null)
			{
				ChangeMaskTexture(HFFResources.instance.FindTextureResource("SkinTextures/" + model.meta.modelPrefab.name + "Mask"), isAsset: true);
			}
		}
		if (baseTexture != null)
		{
			width = baseTexture.width;
			height = baseTexture.height;
			paintingEnabled = true;
		}
		else if (model.maskTexture != null)
		{
			width = model.maskTexture.width;
			height = model.maskTexture.height;
			paintingEnabled = true;
		}
		else
		{
			width = (height = 2048);
			paintingEnabled = true;
		}
	}

	public void SaveToPreset()
	{
		for (int i = 0; i < undo.Count; i++)
		{
			Object.Destroy(undo[i]);
		}
		undo.Clear();
		for (int j = 0; j < redo.Count; j++)
		{
			Object.Destroy(redo[j]);
		}
		redo.Clear();
		FileTools.WriteTexture(savePath, bakedTexture);
		ReleaseBaseTexture();
		ChangeBaseTexture(bakedTexture, isAsset: false);
		bakedTexture = new Texture2D(width, height, TextureFormat.RGB24, mipmap: false);
		bakedTexture.name = "SavedClone" + base.name;
		Graphics.CopyTexture(baseTexture, bakedTexture);
	}

	private void ApplyColors(Color color1, Color color2, Color color3, bool bake, bool compress = false)
	{
		if (!paintingEnabled)
		{
			return;
		}
		appliedColor1 = color1;
		appliedColor2 = color2;
		appliedColor3 = color3;
		if (renderTexture == null)
		{
			renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
			renderTexture.name = "Colors" + base.name;
		}
		Material material = new Material(Shaders.instance.applyMaskedColors);
		material.mainTexture = baseTexture;
		material.SetTexture("_MaskTex", model.maskTexture);
		material.SetColor("_Color1", (!model.mask1) ? default(Color) : color1.linear);
		material.SetColor("_Color2", (!model.mask2) ? default(Color) : color2.linear);
		material.SetColor("_Color3", (!model.mask3) ? default(Color) : color3.linear);
		Graphics.Blit(null, renderTexture, material);
		Object.Destroy(material);
		if (bake)
		{
			BakeTexture(renderTexture, compress);
			Object.Destroy(renderTexture);
			return;
		}
		ApplyTexture(renderTexture);
		if (bakeCoroutine != null)
		{
			StopCoroutine(bakeCoroutine);
		}
		bakeCoroutine = StartCoroutine(BakeDelayed());
	}

	private IEnumerator BakeDelayed()
	{
		yield return new WaitForSeconds(1f);
		EndStrokeFinalize();
	}

	public void ApplyPresetColors(RagdollPresetMetadata preset, bool bake, bool compress)
	{
		string b = FileTools.Combine(preset.folder, part.ToString() + ".png");
		bool suppressCustomTexture = preset.GetPart(part).suppressCustomTexture;
		if (savePath != b || textureLoadSuppressed != suppressCustomTexture)
		{
			LoadFromPreset(preset);
		}
		RagdollPresetPartMetadata ragdollPresetPartMetadata = preset.GetPart(part);
		ApplyColors(HexConverter.HexToColor(ragdollPresetPartMetadata.color1, default(Color)), HexConverter.HexToColor(ragdollPresetPartMetadata.color2, default(Color)), HexConverter.HexToColor(ragdollPresetPartMetadata.color3, default(Color)), bake, compress);
	}

	private void ApplyTexture(Texture texture)
	{
		if (texture != null && texture.wrapMode != 0)
		{
			texture.wrapMode = TextureWrapMode.Repeat;
		}
		this.texture = texture;
		model.SetTexture(texture);
	}

	public void BakeTexture(RenderTexture rt, bool compress)
	{
		if (bakedTexture == null)
		{
			bakedTexture = new Texture2D(width, height, TextureFormat.RGB24, mipmap: true);
			bakedTexture.name = "Baked" + base.name;
		}
		RenderTexture.active = rt;
		bakedTexture.ReadPixels(new Rect(0f, 0f, width, height), 0, 0, recalculateMipMaps: true);
		if (compress)
		{
			bakedTexture.Compress(highQuality: true);
			bakedTexture.Apply(updateMipmaps: true, makeNoLongerReadable: true);
			ReleaseBaseTexture();
			ReleaseMaskTexture();
		}
		else
		{
			bakedTexture.Apply(updateMipmaps: true);
		}
		RenderTexture.active = null;
		ApplyTexture(bakedTexture);
	}

	public void BakeTextureNoMip(RenderTexture rt)
	{
		if (bakedTexture == null)
		{
			bakedTexture = new Texture2D(width, height, TextureFormat.RGB24, mipmap: false);
			bakedTexture.name = "BakedNoMIP" + base.name;
		}
		RenderTexture.active = rt;
		bakedTexture.ReadPixels(new Rect(0f, 0f, width, height), 0, 0, recalculateMipMaps: true);
		bakedTexture.Apply(updateMipmaps: true);
		RenderTexture.active = null;
	}

	public void EnterPaint()
	{
		paintSource = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
		paintTarget = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
		paintSource.name = "PaintSource" + base.name;
		paintTarget.name = "PaintTarget" + base.name;
		ClearPaintTextures();
	}

	public void BeginStroke()
	{
		EnsureBaked();
		paintBase = bakedTexture;
		undo.Add(bakedTexture);
		bakedTexture = null;
		if (undo.Count > 8)
		{
			Object.Destroy(undo[0]);
			undo.RemoveAt(0);
		}
		if (redo.Count > 0)
		{
			for (int i = 0; i < redo.Count; i++)
			{
				Object.Destroy(redo[i]);
			}
			redo.Clear();
		}
		renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
		while (renderTexture == null && undo.Count > 0)
		{
			Object.Destroy(undo[0]);
			undo.RemoveAt(0);
			renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
		}
		renderTexture.name = "Stroke" + base.name;
		ApplyTexture(renderTexture);
		if (!paintSource.IsCreated())
		{
			paintSource.Create();
			ClearPaintTextures();
		}
		Graphics.Blit(paintBase, renderTexture);
	}

	private void EnsureBaked()
	{
		if (bakeCoroutine != null)
		{
			EndStrokeFinalize();
		}
	}

	private void ClearPaintTextures()
	{
		Graphics.SetRenderTarget(paintSource);
		GL.Clear(clearDepth: false, clearColor: true, default(Color));
		Graphics.SetRenderTarget(null);
	}

	public void EndStroke()
	{
		ClearPaintTextures();
		bakeCoroutine = StartCoroutine(EndStrokeFinalizeWait());
	}

	public void CancelStroke()
	{
		ClearPaintTextures();
		bakedTexture = undo[undo.Count - 1];
		ApplyTexture(bakedTexture);
		undo.RemoveAt(undo.Count - 1);
		Object.Destroy(renderTexture);
		renderTexture = null;
	}

	private IEnumerator EndStrokeFinalizeWait()
	{
		for (int i = 0; i < (int)part; i++)
		{
			yield return null;
		}
		EndStrokeFinalize();
	}

	private void EndStrokeFinalize()
	{
		if (!renderTexture.IsCreated())
		{
			ApplyColors(appliedColor1, appliedColor2, appliedColor3, bake: true);
			return;
		}
		BakeTextureNoMip(renderTexture);
		ApplyTexture(bakedTexture);
		Object.Destroy(renderTexture);
		renderTexture = null;
		if (bakeCoroutine != null)
		{
			StopCoroutine(bakeCoroutine);
		}
		bakeCoroutine = null;
	}

	public void PaintStep(Camera camera, Material unprojectMaterial, int mask)
	{
		unprojectMaterial.SetFloat("_TexSize", width);
		unprojectMaterial.SetTexture("_ModelTex", paintBase);
		unprojectMaterial.SetTexture("_BaseTex", paintSource);
		unprojectMaterial.SetTexture("_MaskTex", model.maskTexture);
		unprojectMaterial.SetFloat("_Mask1", ((mask & 1) == 1) ? 1 : 0);
		unprojectMaterial.SetFloat("_Mask2", ((mask & 2) == 2) ? 1 : 0);
		unprojectMaterial.SetFloat("_Mask3", ((mask & 4) == 4) ? 1 : 0);
		buffers[0] = this.renderTexture.colorBuffer;
		buffers[1] = paintTarget.colorBuffer;
		camera.SetTargetBuffers(buffers, this.renderTexture.depthBuffer);
		RenderTexture renderTexture = paintSource;
		paintSource = paintTarget;
		paintTarget = renderTexture;
	}

	public void Undo()
	{
		if (undo.Count != 0)
		{
			EnsureBaked();
			redo.Add(bakedTexture);
			bakedTexture = undo[undo.Count - 1];
			ApplyTexture(bakedTexture);
			undo.RemoveAt(undo.Count - 1);
		}
	}

	public void Redo()
	{
		if (redo.Count != 0)
		{
			undo.Add(bakedTexture);
			bakedTexture = redo[redo.Count - 1];
			ApplyTexture(bakedTexture);
			redo.RemoveAt(redo.Count - 1);
		}
	}

	public void LeavePaint()
	{
		for (int i = 0; i < undo.Count; i++)
		{
			Object.Destroy(undo[i]);
		}
		undo.Clear();
		for (int j = 0; j < redo.Count; j++)
		{
			Object.Destroy(redo[j]);
		}
		redo.Clear();
		if (paintSource != null)
		{
			Object.Destroy(paintSource);
		}
		if (paintTarget != null)
		{
			Object.Destroy(paintTarget);
		}
	}

	private void ReleaseBaseTexture()
	{
		if (baseTextureIsAsset)
		{
			TextureTracker.instance.RemoveMapping(this, baseTexture);
		}
		else if (baseTexture != null)
		{
			Object.Destroy(baseTexture);
		}
		baseTexture = null;
		baseTextureIsAsset = false;
	}

	private void ReleaseMaskTexture()
	{
		if (maskTextureIsAsset)
		{
			TextureTracker.instance.RemoveMapping(model, model.maskTexture);
		}
		model.maskTexture = null;
		maskTextureIsAsset = false;
	}

	private void OnDestroy()
	{
		LeavePaint();
		ReleaseBaseTexture();
		ReleaseMaskTexture();
		if (bakedTexture != null)
		{
			Object.Destroy(bakedTexture);
		}
		if (renderTexture != null)
		{
			Object.Destroy(renderTexture);
		}
	}
}
