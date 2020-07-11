using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

namespace UnityEngine.Rendering.PostProcessing
{
	public static class RuntimeUtilities
	{
		private static Texture2D m_WhiteTexture;

		private static Texture3D m_WhiteTexture3D;

		private static Texture2D m_BlackTexture;

		private static Texture3D m_BlackTexture3D;

		private static Texture2D m_TransparentTexture;

		private static Texture3D m_TransparentTexture3D;

		private static Dictionary<int, Texture2D> m_LutStrips = new Dictionary<int, Texture2D>();

		private static Mesh s_FullscreenTriangle;

		private static Material s_CopyStdMaterial;

		private static Material s_CopyMaterial;

		private static PropertySheet s_CopySheet;

		private static IEnumerable<Type> m_AssemblyTypes;

		public static Texture2D whiteTexture
		{
			get
			{
				if (m_WhiteTexture == null)
				{
					Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, mipmap: false);
					texture2D.name = "White Texture";
					m_WhiteTexture = texture2D;
					m_WhiteTexture.SetPixel(0, 0, Color.white);
					m_WhiteTexture.Apply();
				}
				return m_WhiteTexture;
			}
		}

		public static Texture3D whiteTexture3D
		{
			get
			{
				if (m_WhiteTexture3D == null)
				{
					Texture3D texture3D = new Texture3D(1, 1, 1, TextureFormat.ARGB32, mipmap: false);
					texture3D.name = "White Texture 3D";
					m_WhiteTexture3D = texture3D;
					m_WhiteTexture3D.SetPixels(new Color[1]
					{
						Color.white
					});
					m_WhiteTexture3D.Apply();
				}
				return m_WhiteTexture3D;
			}
		}

		public static Texture2D blackTexture
		{
			get
			{
				if (m_BlackTexture == null)
				{
					Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, mipmap: false);
					texture2D.name = "Black Texture";
					m_BlackTexture = texture2D;
					m_BlackTexture.SetPixel(0, 0, Color.black);
					m_BlackTexture.Apply();
				}
				return m_BlackTexture;
			}
		}

		public static Texture3D blackTexture3D
		{
			get
			{
				if (m_BlackTexture3D == null)
				{
					Texture3D texture3D = new Texture3D(1, 1, 1, TextureFormat.ARGB32, mipmap: false);
					texture3D.name = "Black Texture 3D";
					m_BlackTexture3D = texture3D;
					m_BlackTexture3D.SetPixels(new Color[1]
					{
						Color.black
					});
					m_BlackTexture3D.Apply();
				}
				return m_BlackTexture3D;
			}
		}

		public static Texture2D transparentTexture
		{
			get
			{
				if (m_TransparentTexture == null)
				{
					Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, mipmap: false);
					texture2D.name = "Transparent Texture";
					m_TransparentTexture = texture2D;
					m_TransparentTexture.SetPixel(0, 0, Color.clear);
					m_TransparentTexture.Apply();
				}
				return m_TransparentTexture;
			}
		}

		public static Texture3D transparentTexture3D
		{
			get
			{
				if (m_TransparentTexture3D == null)
				{
					Texture3D texture3D = new Texture3D(1, 1, 1, TextureFormat.ARGB32, mipmap: false);
					texture3D.name = "Transparent Texture 3D";
					m_TransparentTexture3D = texture3D;
					m_TransparentTexture3D.SetPixels(new Color[1]
					{
						Color.clear
					});
					m_TransparentTexture3D.Apply();
				}
				return m_TransparentTexture3D;
			}
		}

		public static Mesh fullscreenTriangle
		{
			get
			{
				if (s_FullscreenTriangle != null)
				{
					return s_FullscreenTriangle;
				}
				Mesh mesh = new Mesh();
				mesh.name = "Fullscreen Triangle";
				s_FullscreenTriangle = mesh;
				s_FullscreenTriangle.SetVertices(new List<Vector3>
				{
					new Vector3(-1f, -1f, 0f),
					new Vector3(-1f, 3f, 0f),
					new Vector3(3f, -1f, 0f)
				});
				s_FullscreenTriangle.SetIndices(new int[3]
				{
					0,
					1,
					2
				}, MeshTopology.Triangles, 0, calculateBounds: false);
				s_FullscreenTriangle.UploadMeshData(markNoLogerReadable: false);
				return s_FullscreenTriangle;
			}
		}

		public static Material copyStdMaterial
		{
			get
			{
				if (s_CopyStdMaterial != null)
				{
					return s_CopyStdMaterial;
				}
				Shader shader = Shader.Find("Hidden/PostProcessing/CopyStd");
				Material material = new Material(shader);
				material.name = "PostProcess - CopyStd";
				material.hideFlags = HideFlags.HideAndDontSave;
				s_CopyStdMaterial = material;
				return s_CopyStdMaterial;
			}
		}

		public static Material copyMaterial
		{
			get
			{
				if (s_CopyMaterial != null)
				{
					return s_CopyMaterial;
				}
				Shader shader = Shader.Find("Hidden/PostProcessing/Copy");
				Material material = new Material(shader);
				material.name = "PostProcess - Copy";
				material.hideFlags = HideFlags.HideAndDontSave;
				s_CopyMaterial = material;
				return s_CopyMaterial;
			}
		}

		public static PropertySheet copySheet
		{
			get
			{
				if (s_CopySheet == null)
				{
					s_CopySheet = new PropertySheet(copyMaterial);
				}
				return s_CopySheet;
			}
		}

		public static bool scriptableRenderPipelineActive => GraphicsSettings.renderPipelineAsset != null;

		public static bool supportsDeferredShading => scriptableRenderPipelineActive || GraphicsSettings.GetShaderMode(BuiltinShaderType.DeferredShading) != BuiltinShaderMode.Disabled;

		public static bool supportsDepthNormals => scriptableRenderPipelineActive || GraphicsSettings.GetShaderMode(BuiltinShaderType.DepthNormals) != BuiltinShaderMode.Disabled;

		public static bool isSinglePassStereoEnabled => XRSettings.eyeTextureDesc.vrUsage == VRTextureUsage.TwoEyes;

		public static bool isVREnabled => XRSettings.enabled;

		public static bool isAndroidOpenGL => Application.platform == RuntimePlatform.Android && SystemInfo.graphicsDeviceType != GraphicsDeviceType.Vulkan;

		public static RenderTextureFormat defaultHDRRenderTextureFormat => RenderTextureFormat.DefaultHDR;

		public static bool isLinearColorSpace => QualitySettings.activeColorSpace == ColorSpace.Linear;

		public static Texture2D GetLutStrip(int size)
		{
			if (!m_LutStrips.TryGetValue(size, out Texture2D value))
			{
				int num = size * size;
				Color[] array = new Color[num * size];
				float num2 = 1f / ((float)size - 1f);
				for (int i = 0; i < size; i++)
				{
					int num3 = i * size;
					float b = (float)i * num2;
					for (int j = 0; j < size; j++)
					{
						float g = (float)j * num2;
						for (int k = 0; k < size; k++)
						{
							float r = (float)k * num2;
							array[j * num + num3 + k] = new Color(r, g, b);
						}
					}
				}
				TextureFormat format = TextureFormat.RGBAHalf;
				if (!format.IsSupported())
				{
					format = TextureFormat.ARGB32;
				}
				Texture2D texture2D = new Texture2D(size * size, size, format, mipmap: false, linear: true);
				texture2D.name = "Strip Lut" + size;
				texture2D.hideFlags = HideFlags.DontSave;
				texture2D.filterMode = FilterMode.Bilinear;
				texture2D.wrapMode = TextureWrapMode.Clamp;
				texture2D.anisoLevel = 0;
				value = texture2D;
				value.SetPixels(array);
				value.Apply();
				m_LutStrips.Add(size, value);
			}
			return value;
		}

		public static void SetRenderTargetWithLoadStoreAction(this CommandBuffer cmd, RenderTargetIdentifier rt, RenderBufferLoadAction loadAction, RenderBufferStoreAction storeAction)
		{
			cmd.SetRenderTarget(rt);
		}

		public static void SetRenderTargetWithLoadStoreAction(this CommandBuffer cmd, RenderTargetIdentifier color, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderTargetIdentifier depth, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction)
		{
			cmd.SetRenderTarget(color, depth);
		}

		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, bool clear = false)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			cmd.SetRenderTargetWithLoadStoreAction(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
			if (clear)
			{
				cmd.ClearRenderTarget(clearDepth: true, clearColor: true, Color.clear);
			}
			cmd.DrawMesh(fullscreenTriangle, Matrix4x4.identity, copyMaterial, 0, 0);
		}

		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, PropertySheet propertySheet, int pass, RenderBufferLoadAction loadAction)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			bool flag = false;
			cmd.SetRenderTargetWithLoadStoreAction(destination, (!flag) ? loadAction : RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
			if (flag)
			{
				cmd.ClearRenderTarget(clearDepth: true, clearColor: true, Color.clear);
			}
			cmd.DrawMesh(fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
		}

		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, PropertySheet propertySheet, int pass, bool clear = false)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			cmd.SetRenderTargetWithLoadStoreAction(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
			if (clear)
			{
				cmd.ClearRenderTarget(clearDepth: true, clearColor: true, Color.clear);
			}
			cmd.DrawMesh(fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
		}

		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, RenderTargetIdentifier depth, PropertySheet propertySheet, int pass, bool clear = false)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			if (clear)
			{
				cmd.SetRenderTargetWithLoadStoreAction(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, depth, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
				cmd.ClearRenderTarget(clearDepth: true, clearColor: true, Color.clear);
			}
			else
			{
				cmd.SetRenderTargetWithLoadStoreAction(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, depth, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
			}
			cmd.DrawMesh(fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
		}

		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier[] destinations, RenderTargetIdentifier depth, PropertySheet propertySheet, int pass, bool clear = false)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			cmd.SetRenderTarget(destinations, depth);
			if (clear)
			{
				cmd.ClearRenderTarget(clearDepth: true, clearColor: true, Color.clear);
			}
			cmd.DrawMesh(fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
		}

		public static void BlitFullscreenTriangle(Texture source, RenderTexture destination, Material material, int pass)
		{
			RenderTexture active = RenderTexture.active;
			material.SetPass(pass);
			if (source != null)
			{
				material.SetTexture(ShaderIDs.MainTex, source);
			}
			if (destination != null)
			{
				destination.DiscardContents(discardColor: true, discardDepth: false);
			}
			Graphics.SetRenderTarget(destination);
			Graphics.DrawMeshNow(fullscreenTriangle, Matrix4x4.identity);
			RenderTexture.active = active;
		}

		public static void BuiltinBlit(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier dest)
		{
			cmd.Blit(source, dest);
		}

		public static void BuiltinBlit(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier dest, Material mat, int pass = 0)
		{
			cmd.Blit(source, dest, mat, pass);
		}

		public static void CopyTexture(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination)
		{
			if (SystemInfo.copyTextureSupport > CopyTextureSupport.None)
			{
				cmd.CopyTexture(source, destination);
			}
			else
			{
				cmd.BlitFullscreenTriangle(source, destination);
			}
		}

		public static bool isFloatingPointFormat(RenderTextureFormat format)
		{
			return format == RenderTextureFormat.DefaultHDR || format == RenderTextureFormat.ARGBHalf || format == RenderTextureFormat.ARGBFloat || format == RenderTextureFormat.RGFloat || format == RenderTextureFormat.RGHalf || format == RenderTextureFormat.RFloat || format == RenderTextureFormat.RHalf || format == RenderTextureFormat.RGB111110Float;
		}

		public static void Destroy(Object obj)
		{
			if (obj != null)
			{
				Object.Destroy(obj);
			}
		}

		public static bool IsResolvedDepthAvailable(Camera camera)
		{
			GraphicsDeviceType graphicsDeviceType = SystemInfo.graphicsDeviceType;
			return camera.actualRenderingPath == RenderingPath.DeferredShading && (graphicsDeviceType == GraphicsDeviceType.Direct3D11 || graphicsDeviceType == GraphicsDeviceType.Direct3D12 || graphicsDeviceType == GraphicsDeviceType.XboxOne);
		}

		public static void DestroyProfile(PostProcessProfile profile, bool destroyEffects)
		{
			if (destroyEffects)
			{
				foreach (PostProcessEffectSettings setting in profile.settings)
				{
					Destroy(setting);
				}
			}
			Destroy(profile);
		}

		public static void DestroyVolume(PostProcessVolume volume, bool destroyProfile, bool destroyGameObject = false)
		{
			if (destroyProfile)
			{
				DestroyProfile(volume.profileRef, destroyEffects: true);
			}
			GameObject gameObject = volume.gameObject;
			Destroy(volume);
			if (destroyGameObject)
			{
				Destroy(gameObject);
			}
		}

		public static bool IsPostProcessingActive(PostProcessLayer layer)
		{
			return layer != null && layer.enabled;
		}

		public static bool IsTemporalAntialiasingActive(PostProcessLayer layer)
		{
			return IsPostProcessingActive(layer) && layer.antialiasingMode == PostProcessLayer.Antialiasing.TemporalAntialiasing && layer.temporalAntialiasing.IsSupported();
		}

		public static IEnumerable<T> GetAllSceneObjects<T>() where T : Component
		{
			Queue<Transform> queue = new Queue<Transform>();
			GameObject[] roots = SceneManager.GetActiveScene().GetRootGameObjects();
			GameObject[] array = roots;
			foreach (GameObject root in array)
			{
				queue.Enqueue(root.transform);
				T comp = root.GetComponent<T>();
				if ((Object)comp != (Object)null)
				{
					yield return comp;
				}
			}
			while (queue.Count > 0)
			{
				IEnumerator enumerator = queue.Dequeue().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Transform child = (Transform)enumerator.Current;
						queue.Enqueue(child);
						T comp2 = child.GetComponent<T>();
						if ((Object)comp2 != (Object)null)
						{
							yield return comp2;
						}
					}
				}
				finally
				{
					IDisposable disposable;
					IDisposable disposable2 = disposable = (enumerator as IDisposable);
					if (disposable != null)
					{
						disposable2.Dispose();
					}
				}
			}
		}

		public static void CreateIfNull<T>(ref T obj) where T : class, new()
		{
			if (obj == null)
			{
				obj = new T();
			}
		}

		public static float Exp2(float x)
		{
			return Mathf.Exp(x * 0.6931472f);
		}

		public static Matrix4x4 GetJitteredPerspectiveProjectionMatrix(Camera camera, Vector2 offset)
		{
			float nearClipPlane = camera.nearClipPlane;
			float farClipPlane = camera.farClipPlane;
			float num = Mathf.Tan((float)Math.PI / 360f * camera.fieldOfView) * nearClipPlane;
			float num2 = num * camera.aspect;
			offset.x *= num2 / (0.5f * (float)camera.pixelWidth);
			offset.y *= num / (0.5f * (float)camera.pixelHeight);
			Matrix4x4 projectionMatrix = camera.projectionMatrix;
			projectionMatrix[0, 2] += offset.x / num2;
			projectionMatrix[1, 2] += offset.y / num;
			return projectionMatrix;
		}

		public static Matrix4x4 GetJitteredOrthographicProjectionMatrix(Camera camera, Vector2 offset)
		{
			float orthographicSize = camera.orthographicSize;
			float num = orthographicSize * camera.aspect;
			offset.x *= num / (0.5f * (float)camera.pixelWidth);
			offset.y *= orthographicSize / (0.5f * (float)camera.pixelHeight);
			float left = offset.x - num;
			float right = offset.x + num;
			float top = offset.y + orthographicSize;
			float bottom = offset.y - orthographicSize;
			return Matrix4x4.Ortho(left, right, bottom, top, camera.nearClipPlane, camera.farClipPlane);
		}

		public static Matrix4x4 GenerateJitteredProjectionMatrixFromOriginal(PostProcessRenderContext context, Matrix4x4 origProj, Vector2 jitter)
		{
			FrustumPlanes decomposeProjection = origProj.decomposeProjection;
			float num = Math.Abs(decomposeProjection.top) + Math.Abs(decomposeProjection.bottom);
			float num2 = Math.Abs(decomposeProjection.left) + Math.Abs(decomposeProjection.right);
			Vector2 vector = new Vector2(jitter.x * num2 / (float)context.screenWidth, jitter.y * num / (float)context.screenHeight);
			decomposeProjection.left += vector.x;
			decomposeProjection.right += vector.x;
			decomposeProjection.top += vector.y;
			decomposeProjection.bottom += vector.y;
			return Matrix4x4.Frustum(decomposeProjection);
		}

		public static IEnumerable<Type> GetAllAssemblyTypes()
		{
			if (m_AssemblyTypes == null)
			{
				m_AssemblyTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(delegate(Assembly t)
				{
					Type[] result = new Type[0];
					try
					{
						result = t.GetTypes();
						return result;
					}
					catch
					{
						return result;
					}
				});
			}
			return m_AssemblyTypes;
		}

		public static T GetAttribute<T>(this Type type) where T : Attribute
		{
			return (T)type.GetCustomAttributes(typeof(T), inherit: false)[0];
		}

		public static Attribute[] GetMemberAttributes<TType, TValue>(Expression<Func<TType, TValue>> expr)
		{
			Expression expression = expr;
			if (expression is LambdaExpression)
			{
				expression = ((LambdaExpression)expression).Body;
			}
			ExpressionType nodeType = expression.NodeType;
			if (nodeType == ExpressionType.MemberAccess)
			{
				FieldInfo fieldInfo = (FieldInfo)((MemberExpression)expression).Member;
				return fieldInfo.GetCustomAttributes(inherit: false).Cast<Attribute>().ToArray();
			}
			throw new InvalidOperationException();
		}

		public static string GetFieldPath<TType, TValue>(Expression<Func<TType, TValue>> expr)
		{
			ExpressionType nodeType = expr.Body.NodeType;
			if (nodeType == ExpressionType.MemberAccess)
			{
				MemberExpression memberExpression = expr.Body as MemberExpression;
				List<string> list = new List<string>();
				while (memberExpression != null)
				{
					list.Add(memberExpression.Member.Name);
					memberExpression = (memberExpression.Expression as MemberExpression);
				}
				StringBuilder stringBuilder = new StringBuilder();
				for (int num = list.Count - 1; num >= 0; num--)
				{
					stringBuilder.Append(list[num]);
					if (num > 0)
					{
						stringBuilder.Append('.');
					}
				}
				return stringBuilder.ToString();
			}
			throw new InvalidOperationException();
		}

		public static object GetParentObject(string path, object obj)
		{
			string[] array = path.Split('.');
			if (array.Length == 1)
			{
				return obj;
			}
			FieldInfo field = obj.GetType().GetField(array[0], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			obj = field.GetValue(obj);
			return GetParentObject(string.Join(".", array, 1, array.Length - 1), obj);
		}
	}
}
