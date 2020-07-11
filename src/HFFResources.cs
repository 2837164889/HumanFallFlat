using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HFFResources : MonoBehaviour
{
	public Texture2D[] SkinTextures;

	public Sprite[] LevelImages;

	public Sprite[] LobbyImages;

	public TextAsset[] VideoTextAssets;

	public TMP_Settings TMPSettings;

	public TMP_StyleSheet StyleSheet;

	public TextAsset[] VideoSrts;

	public Texture[] VideoThumbs;

	public AudioClip[] Music;

	public TextAsset HFFMasterLocalisationFile;

	public Material PCLinearMovieFixMenu;

	public Material PCLinearMovieFixGame;

	public static HFFResources instance;

	public const string kLevelImages = "LevelImages";

	public const string kLobbyImages = "LobbyImages";

	public const string kSkinTextures = "SkinTextures";

	public const char kPathSeparator = '/';

	private int mNumberLevelImages;

	private int mNumberLobbyImages;

	private int mNumberSkinTextures;

	private int mNumberVideoSrts;

	private int mNumberVideoThumbs;

	private int mNumberMusic;

	private string[] mLevelNames;

	private string[] mLobbyNames;

	private string[] mSkinNames;

	private string[] mVideoSrtNames;

	private string[] mVideoThumbsNames;

	private string[] mMusicNames;

	private List<string> skinMasksProcessed = new List<string>();

	private void BuildNameArray(out string[] nameArray, UnityEngine.Object[] objects, int numberObjects)
	{
		nameArray = new string[numberObjects];
		for (int i = 0; i < numberObjects; i++)
		{
			if (objects[i] != null)
			{
				nameArray[i] = objects[i].name;
			}
		}
	}

	private void Awake()
	{
		instance = this;
		mNumberLevelImages = LevelImages.Length;
		mNumberLobbyImages = LobbyImages.Length;
		mNumberSkinTextures = SkinTextures.Length;
		mNumberVideoSrts = VideoSrts.Length;
		mNumberVideoThumbs = VideoThumbs.Length;
		mNumberMusic = Music.Length;
		BuildNameArray(out mLevelNames, LevelImages, mNumberLevelImages);
		BuildNameArray(out mLobbyNames, LobbyImages, mNumberLobbyImages);
		BuildNameArray(out mSkinNames, SkinTextures, mNumberSkinTextures);
		BuildNameArray(out mVideoSrtNames, VideoSrts, mNumberVideoSrts);
		BuildNameArray(out mVideoThumbsNames, VideoThumbs, mNumberVideoThumbs);
		BuildNameArray(out mMusicNames, Music, mNumberMusic);
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private T FindResource<T>(string[] names, T[] objects, string name, int numberObjects) where T : class
	{
		for (int i = 0; i < numberObjects; i++)
		{
			if (string.Equals(names[i], name, StringComparison.OrdinalIgnoreCase))
			{
				return objects[i];
			}
		}
		return (T)null;
	}

	private string GetName(string path)
	{
		int num = path.LastIndexOf('/');
		if (num != -1)
		{
			return path.Substring(num + 1);
		}
		return path;
	}

	public Texture2D FindTextureResource(string path)
	{
		if (path.Contains("LevelImages"))
		{
			string name = GetName(path);
			for (int i = 0; i < mNumberLevelImages; i++)
			{
				if (string.Equals(mLevelNames[i], name, StringComparison.OrdinalIgnoreCase))
				{
					return LevelImages[i].texture;
				}
			}
		}
		if (path.Contains("LobbyImages"))
		{
			string name2 = GetName(path);
			for (int j = 0; j < mNumberLobbyImages; j++)
			{
				if (string.Equals(mLobbyNames[j], name2, StringComparison.OrdinalIgnoreCase))
				{
					return LobbyImages[j].texture;
				}
			}
		}
		if (path.Contains("SkinTextures"))
		{
			string name3 = GetName(path);
			Texture2D texture2D = FindResource(mSkinNames, SkinTextures, name3, mNumberSkinTextures);
			if (texture2D != null && path.Contains("Mask") && !skinMasksProcessed.Contains(path) && texture2D.format == TextureFormat.ARGB4444)
			{
				Color32[] pixels = texture2D.GetPixels32(0);
				int num = texture2D.width * texture2D.height;
				skinMasksProcessed.Add(path);
				for (int k = 0; k < num; k++)
				{
					int num2 = pixels[k].r + pixels[k].g + pixels[k].b;
					if (pixels[k].r > 0)
					{
						pixels[k].r = byte.MaxValue;
					}
					if (pixels[k].g > 0)
					{
						pixels[k].g = byte.MaxValue;
					}
					if (pixels[k].b > 0)
					{
						pixels[k].b = byte.MaxValue;
					}
					if (num2 > 255)
					{
						num2 = 255;
					}
					pixels[k].a = (byte)num2;
				}
				texture2D.SetPixels32(pixels);
				texture2D.Apply(updateMipmaps: false, makeNoLongerReadable: false);
			}
			return texture2D;
		}
		return null;
	}

	public TextAsset GetVideoSrt(string path)
	{
		string name = GetName(path);
		return FindResource(mVideoSrtNames, VideoSrts, name, mNumberVideoSrts);
	}

	public Texture GetVideoThumb(string path)
	{
		string name = GetName(path);
		return FindResource(mVideoThumbsNames, VideoThumbs, name, mNumberVideoThumbs);
	}

	public AudioClip GetMusicTrack(string path)
	{
		string name = GetName(path);
		return FindResource(mMusicNames, Music, name, mNumberMusic);
	}
}
