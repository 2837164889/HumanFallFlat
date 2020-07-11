using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SwitchAssetBundle
{
	public class StandardBundle
	{
		public string mSceneBundle;

		public string[] mRequires;
	}

	public enum LoadingScenePhase
	{
		kNone,
		kRequiredFileLoad,
		kBundleLoad,
		kSceneLoad,
		kRequiredFileLoadWait,
		kBundleLoadWait,
		kSceneLoadWait,
		kDone
	}

	public class LoadingCurrentScene
	{
		public LoadingScenePhase mCurrentPhase;

		public AssetBundleCreateRequest mBundleLoader;

		public AsyncOperation mSceneLoader;

		public StandardBundle mCurrentBundle;

		public int mCurrentRequired;

		public string mSceneName;

		public string mCurrentRequiredName;

		public string mCurrentBundleName;

		public bool mActive;

		private bool _allowSceneActivation = true;

		public bool allowSceneActivation
		{
			get
			{
				return _allowSceneActivation;
			}
			set
			{
				_allowSceneActivation = value;
			}
		}

		public float progress
		{
			get
			{
				return (!isDone) ? 0f : 1f;
			}
			private set
			{
			}
		}

		public bool isDone
		{
			get
			{
				switch (mCurrentPhase)
				{
				case LoadingScenePhase.kNone:
					Debug.Log("Should never to in kNone phase!");
					break;
				case LoadingScenePhase.kRequiredFileLoad:
					LoadNextRequired();
					break;
				case LoadingScenePhase.kSceneLoad:
					LoadScene();
					break;
				case LoadingScenePhase.kBundleLoad:
					LoadBundle();
					break;
				case LoadingScenePhase.kRequiredFileLoadWait:
					if (mBundleLoader.isDone)
					{
						SetLoadedBundle(mCurrentRequiredName, mBundleLoader.assetBundle);
						mCurrentPhase = LoadingScenePhase.kRequiredFileLoad;
					}
					break;
				case LoadingScenePhase.kSceneLoadWait:
					if (!_allowSceneActivation && mSceneLoader.progress > 0.9f)
					{
						_allowSceneActivation = true;
						mSceneLoader.allowSceneActivation = true;
					}
					if (mSceneLoader.isDone)
					{
						mCurrentPhase = LoadingScenePhase.kDone;
					}
					break;
				case LoadingScenePhase.kBundleLoadWait:
					if (mBundleLoader.isDone)
					{
						SetLoadedBundle(mCurrentBundleName, mBundleLoader.assetBundle);
						mCurrentPhase = LoadingScenePhase.kSceneLoad;
					}
					break;
				case LoadingScenePhase.kDone:
					Shutdown();
					return true;
				}
				return false;
			}
			private set
			{
			}
		}

		private void LoadNextRequired()
		{
			if (mCurrentBundle.mRequires == null || mCurrentRequired == mCurrentBundle.mRequires.Length)
			{
				mCurrentPhase = LoadingScenePhase.kBundleLoad;
				return;
			}
			AssetBundle returnedBundle;
			AssetBundleCreateRequest assetBundleCreateRequest = SwitchAssetBundle.LoadBundle(mCurrentBundle.mRequires[mCurrentRequired], out returnedBundle, out mCurrentRequiredName, async: true);
			mCurrentRequired++;
			if (assetBundleCreateRequest == null)
			{
				LoadNextRequired();
				return;
			}
			mBundleLoader = assetBundleCreateRequest;
			mCurrentPhase = LoadingScenePhase.kRequiredFileLoadWait;
		}

		private void LoadBundle()
		{
			AssetBundle returnedBundle;
			AssetBundleCreateRequest assetBundleCreateRequest = SwitchAssetBundle.LoadBundle(mCurrentBundle.mSceneBundle, out returnedBundle, out mCurrentBundleName, async: true);
			if (assetBundleCreateRequest == null)
			{
				mCurrentPhase = LoadingScenePhase.kSceneLoad;
				return;
			}
			mBundleLoader = assetBundleCreateRequest;
			mCurrentPhase = LoadingScenePhase.kBundleLoadWait;
		}

		private void LoadScene()
		{
			mSceneLoader = SceneManager.LoadSceneAsync(mSceneName);
			mCurrentPhase = LoadingScenePhase.kSceneLoadWait;
		}

		public void Shutdown()
		{
			if (!_allowSceneActivation)
			{
				mSceneLoader.allowSceneActivation = true;
			}
			mBundleLoader = null;
			mSceneLoader = null;
			mCurrentBundle = null;
			mActive = false;
			mCurrentPhase = LoadingScenePhase.kNone;
		}
	}

	public const string kBundleSkinTextures = "skintextures";

	public const string kBundleLevelImages = "levelimages";

	public const string kBundleVideo = "video";

	public const string kBundleLocalisation = "localisation";

	public const string kBundleFonts = "fonts";

	public const string kBundleMusic = "music";

	public const string kBundleShared = "shared";

	public const string kLocalisationFile = "hff_master_localisation";

	private static string sVariantName = "switch";

	private static Dictionary<string, AssetBundle> mLoadedBundles = new Dictionary<string, AssetBundle>();

	private static StandardBundle sThermalScene = new StandardBundle
	{
		mSceneBundle = "thermalscene"
	};

	private static StandardBundle sFactoryScene = new StandardBundle
	{
		mSceneBundle = "factoryscene"
	};

	private static Dictionary<string, StandardBundle> sStandardBundles = new Dictionary<string, StandardBundle>
	{
		{
			"Thermal",
			sThermalScene
		},
		{
			"Factory",
			sFactoryScene
		}
	};

	private static LoadingCurrentScene sCurrentLoad = new LoadingCurrentScene();

	private static AssetBundleCreateRequest loader = null;

	private static StandardBundle GetBundleForScene(string scene)
	{
		StandardBundle value = null;
		sStandardBundles.TryGetValue(scene, out value);
		return value;
	}

	public static void LoadScene(string scene)
	{
		StandardBundle bundleForScene = GetBundleForScene(scene);
		if (bundleForScene == null)
		{
			Debug.Log("Scene not found: " + scene);
			return;
		}
		int num = bundleForScene.mRequires.Length;
		AssetBundle returnedBundle;
		string variantBundleName;
		for (int i = 0; i < num; i++)
		{
			LoadBundle(bundleForScene.mRequires[i], out returnedBundle, out variantBundleName, async: false);
			if (returnedBundle == null)
			{
				Debug.Log("Failed to load bundle: " + bundleForScene.mRequires[i]);
				return;
			}
		}
		LoadBundle(bundleForScene.mSceneBundle, out returnedBundle, out variantBundleName, async: false);
		if (returnedBundle == null)
		{
			Debug.Log("Failed to load main scene bundle: " + bundleForScene.mSceneBundle);
		}
		string text = FindScene(returnedBundle, scene);
		if (text == null)
		{
			Debug.Log("Failed to find scene in bundle: " + text);
		}
		SceneManager.LoadScene(text);
	}

	private static void StartNewLoad(string scene, StandardBundle bundle)
	{
		if (sCurrentLoad.mActive)
		{
			sCurrentLoad.Shutdown();
		}
		sCurrentLoad.mSceneName = scene;
		sCurrentLoad.mCurrentBundle = bundle;
		sCurrentLoad.mCurrentRequired = 0;
		sCurrentLoad.mCurrentPhase = LoadingScenePhase.kRequiredFileLoad;
		sCurrentLoad.mBundleLoader = null;
		sCurrentLoad.mSceneLoader = null;
		sCurrentLoad.mActive = true;
	}

	public static LoadingCurrentScene LoadSceneAsync(string scene)
	{
		StandardBundle bundleForScene = GetBundleForScene(scene);
		if (bundleForScene == null)
		{
			Debug.Log("Scene not found: " + scene);
			return null;
		}
		StartNewLoad(scene, bundleForScene);
		return sCurrentLoad;
	}

	private static AssetBundleCreateRequest LoadBundle(string bundleName, out AssetBundle returnedBundle, out string variantBundleName, bool async)
	{
		variantBundleName = null;
		if (mLoadedBundles.TryGetValue(bundleName, out AssetBundle value))
		{
			returnedBundle = value;
			return null;
		}
		byte[] array = null;
		string streamingAssetsPath = Application.streamingAssetsPath;
		streamingAssetsPath = Path.Combine(streamingAssetsPath, "PCBundles/");
		variantBundleName = string.Format(bundleName, sVariantName);
		streamingAssetsPath = Path.Combine(streamingAssetsPath, variantBundleName);
		array = File.ReadAllBytes(streamingAssetsPath);
		if (!async)
		{
			value = AssetBundle.LoadFromMemory(array);
			array = null;
			mLoadedBundles.Add(variantBundleName, value);
			returnedBundle = value;
			return null;
		}
		returnedBundle = null;
		return AssetBundle.LoadFromMemoryAsync(array);
	}

	private static string FindScene(AssetBundle bundle, string scene)
	{
		string[] allScenePaths = bundle.GetAllScenePaths();
		for (int i = 0; i < allScenePaths.Length; i++)
		{
			if (allScenePaths[i].Contains(scene))
			{
				return allScenePaths[i];
			}
		}
		Debug.Log("Scene not found in bundle: " + scene + " " + bundle);
		return null;
	}

	private static void SetLoadedBundle(string bundleName, AssetBundle bundle)
	{
		if (!mLoadedBundles.ContainsKey(bundleName))
		{
			mLoadedBundles.Add(bundleName, bundle);
		}
	}

	public static AssetBundle GetAssetBundle(string scene)
	{
		if (mLoadedBundles.TryGetValue(scene, out AssetBundle value))
		{
			return value;
		}
		return null;
	}
}
