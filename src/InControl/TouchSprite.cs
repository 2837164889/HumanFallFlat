using System;
using UnityEngine;

namespace InControl
{
	[Serializable]
	public class TouchSprite
	{
		[SerializeField]
		private Sprite idleSprite;

		[SerializeField]
		private Sprite busySprite;

		[SerializeField]
		private Color idleColor = new Color(1f, 1f, 1f, 0.5f);

		[SerializeField]
		private Color busyColor = new Color(1f, 1f, 1f, 1f);

		[SerializeField]
		private TouchSpriteShape shape;

		[SerializeField]
		private TouchUnitType sizeUnitType;

		[SerializeField]
		private Vector2 size = new Vector2(10f, 10f);

		[SerializeField]
		private bool lockAspectRatio = true;

		[SerializeField]
		[HideInInspector]
		private Vector2 worldSize;

		private Transform spriteParentTransform;

		private GameObject spriteGameObject;

		private SpriteRenderer spriteRenderer;

		private bool state;

		public bool Dirty
		{
			get;
			set;
		}

		public bool Ready
		{
			get;
			set;
		}

		public bool State
		{
			get
			{
				return state;
			}
			set
			{
				if (state != value)
				{
					state = value;
					Dirty = true;
				}
			}
		}

		public Sprite BusySprite
		{
			get
			{
				return busySprite;
			}
			set
			{
				if (busySprite != value)
				{
					busySprite = value;
					Dirty = true;
				}
			}
		}

		public Sprite IdleSprite
		{
			get
			{
				return idleSprite;
			}
			set
			{
				if (idleSprite != value)
				{
					idleSprite = value;
					Dirty = true;
				}
			}
		}

		public Sprite Sprite
		{
			set
			{
				if (idleSprite != value)
				{
					idleSprite = value;
					Dirty = true;
				}
				if (busySprite != value)
				{
					busySprite = value;
					Dirty = true;
				}
			}
		}

		public Color BusyColor
		{
			get
			{
				return busyColor;
			}
			set
			{
				if (busyColor != value)
				{
					busyColor = value;
					Dirty = true;
				}
			}
		}

		public Color IdleColor
		{
			get
			{
				return idleColor;
			}
			set
			{
				if (idleColor != value)
				{
					idleColor = value;
					Dirty = true;
				}
			}
		}

		public TouchSpriteShape Shape
		{
			get
			{
				return shape;
			}
			set
			{
				if (shape != value)
				{
					shape = value;
					Dirty = true;
				}
			}
		}

		public TouchUnitType SizeUnitType
		{
			get
			{
				return sizeUnitType;
			}
			set
			{
				if (sizeUnitType != value)
				{
					sizeUnitType = value;
					Dirty = true;
				}
			}
		}

		public Vector2 Size
		{
			get
			{
				return size;
			}
			set
			{
				if (size != value)
				{
					size = value;
					Dirty = true;
				}
			}
		}

		public Vector2 WorldSize => worldSize;

		public Vector3 Position
		{
			get
			{
				return (!spriteGameObject) ? Vector3.zero : spriteGameObject.transform.position;
			}
			set
			{
				if ((bool)spriteGameObject)
				{
					spriteGameObject.transform.position = value;
				}
			}
		}

		public TouchSprite()
		{
		}

		public TouchSprite(float size)
		{
			this.size = Vector2.one * size;
		}

		public void Create(string gameObjectName, Transform parentTransform, int sortingOrder)
		{
			spriteGameObject = CreateSpriteGameObject(gameObjectName, parentTransform);
			spriteRenderer = CreateSpriteRenderer(spriteGameObject, idleSprite, sortingOrder);
			spriteRenderer.color = idleColor;
			Ready = true;
		}

		public void Delete()
		{
			Ready = false;
			UnityEngine.Object.Destroy(spriteGameObject);
		}

		public void Update()
		{
			Update(forceUpdate: false);
		}

		public void Update(bool forceUpdate)
		{
			if (Dirty || forceUpdate)
			{
				if (spriteRenderer != null)
				{
					spriteRenderer.sprite = ((!State) ? idleSprite : busySprite);
				}
				if (sizeUnitType == TouchUnitType.Pixels)
				{
					Vector2 a = TouchUtility.RoundVector(size);
					ScaleSpriteInPixels(spriteGameObject, spriteRenderer, a);
					worldSize = a * TouchManager.PixelToWorld;
				}
				else
				{
					ScaleSpriteInPercent(spriteGameObject, spriteRenderer, size);
					if (lockAspectRatio)
					{
						worldSize = size * TouchManager.PercentToWorld;
					}
					else
					{
						worldSize = Vector2.Scale(size, TouchManager.ViewSize);
					}
				}
				Dirty = false;
			}
			if (spriteRenderer != null)
			{
				Color color = (!State) ? idleColor : busyColor;
				if (spriteRenderer.color != color)
				{
					spriteRenderer.color = Utility.MoveColorTowards(spriteRenderer.color, color, 5f * Time.deltaTime);
				}
			}
		}

		private GameObject CreateSpriteGameObject(string name, Transform parentTransform)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.parent = parentTransform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
			gameObject.layer = parentTransform.gameObject.layer;
			return gameObject;
		}

		private SpriteRenderer CreateSpriteRenderer(GameObject spriteGameObject, Sprite sprite, int sortingOrder)
		{
			SpriteRenderer spriteRenderer = spriteGameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = sprite;
			spriteRenderer.sortingOrder = sortingOrder;
			spriteRenderer.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
			spriteRenderer.sharedMaterial.SetFloat("PixelSnap", 1f);
			return spriteRenderer;
		}

		private void ScaleSpriteInPixels(GameObject spriteGameObject, SpriteRenderer spriteRenderer, Vector2 size)
		{
			if (!(spriteGameObject == null) && !(spriteRenderer == null) && !(spriteRenderer.sprite == null))
			{
				float width = spriteRenderer.sprite.rect.width;
				Vector3 vector = spriteRenderer.sprite.bounds.size;
				float num = width / vector.x;
				float num2 = TouchManager.PixelToWorld * num;
				float x = num2 * size.x / spriteRenderer.sprite.rect.width;
				float y = num2 * size.y / spriteRenderer.sprite.rect.height;
				spriteGameObject.transform.localScale = new Vector3(x, y);
			}
		}

		private void ScaleSpriteInPercent(GameObject spriteGameObject, SpriteRenderer spriteRenderer, Vector2 size)
		{
			if (!(spriteGameObject == null) && !(spriteRenderer == null) && !(spriteRenderer.sprite == null))
			{
				if (lockAspectRatio)
				{
					Vector3 viewSize = TouchManager.ViewSize;
					float x = viewSize.x;
					Vector3 viewSize2 = TouchManager.ViewSize;
					float num = Mathf.Min(x, viewSize2.y);
					float num2 = num * size.x;
					Vector3 vector = spriteRenderer.sprite.bounds.size;
					float x2 = num2 / vector.x;
					float num3 = num * size.y;
					Vector3 vector2 = spriteRenderer.sprite.bounds.size;
					float y = num3 / vector2.y;
					spriteGameObject.transform.localScale = new Vector3(x2, y);
				}
				else
				{
					Vector3 viewSize3 = TouchManager.ViewSize;
					float num4 = viewSize3.x * size.x;
					Vector3 vector3 = spriteRenderer.sprite.bounds.size;
					float x3 = num4 / vector3.x;
					Vector3 viewSize4 = TouchManager.ViewSize;
					float num5 = viewSize4.y * size.y;
					Vector3 vector4 = spriteRenderer.sprite.bounds.size;
					float y2 = num5 / vector4.y;
					spriteGameObject.transform.localScale = new Vector3(x3, y2);
				}
			}
		}

		public bool Contains(Vector2 testWorldPoint)
		{
			if (shape == TouchSpriteShape.Oval)
			{
				float x = testWorldPoint.x;
				Vector3 position = Position;
				float num = (x - position.x) / worldSize.x;
				float y = testWorldPoint.y;
				Vector3 position2 = Position;
				float num2 = (y - position2.y) / worldSize.y;
				return num * num + num2 * num2 < 0.25f;
			}
			float x2 = testWorldPoint.x;
			Vector3 position3 = Position;
			float num3 = Utility.Abs(x2 - position3.x) * 2f;
			float y2 = testWorldPoint.y;
			Vector3 position4 = Position;
			float num4 = Utility.Abs(y2 - position4.y) * 2f;
			return num3 <= worldSize.x && num4 <= worldSize.y;
		}

		public bool Contains(Touch touch)
		{
			return Contains(TouchManager.ScreenToWorldPoint(touch.position));
		}

		public void DrawGizmos(Vector3 position, Color color)
		{
			if (shape == TouchSpriteShape.Oval)
			{
				Utility.DrawOvalGizmo(position, WorldSize, color);
			}
			else
			{
				Utility.DrawRectGizmo(position, WorldSize, color);
			}
		}
	}
}
