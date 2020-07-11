using Multiplayer;
using UnityEngine;
using UnityEngine.UI;

public class OverheadNameTag : MonoBehaviour
{
	public Text textMesh;

	public float maxScale = 0.3f;

	public float minScale = 0.15f;

	public float maxScaleDistance = 10f;

	public float minOffsetFromHead = 0.5f;

	public float maxOffsetFromHead = 5f;

	public float rotateSpeed = 1f;

	public float FadeInDuration = 0.1f;

	public float FadeOutDuration = 0.2f;

	public float minWidth = 337f;

	public GameObject Child;

	public SpriteRenderer SpeakerSprite;

	public Sprite TalkingSprite;

	public Sprite NotTalkingSprite;

	public Sprite MutedSprite;

	public float waitTimeOnForceShow = 5f;

	private NetPlayer player;

	private Image childBackground;

	private static Camera mainCamera;

	private float TransitionTimer;

	private bool TransitionInProgress;

	private bool isTalking;

	public float MinimumBgWidth;

	private bool forceShow;

	private float currentWaitTime;

	private float SpeakerSize;

	private float sizeAddition = 0.075f;

	private float initialBGAlpha;

	private float getChildWidth
	{
		get
		{
			float preferredWidth = textMesh.preferredWidth;
			Vector3 localScale = textMesh.rectTransform.localScale;
			return preferredWidth * localScale.x + SpeakerSize + sizeAddition;
		}
	}

	private void Start()
	{
		player = GetComponentInParent<NetPlayer>();
		childBackground = Child.GetComponent<Image>();
		if ((bool)childBackground)
		{
			Color color = childBackground.color;
			initialBGAlpha = color.a;
			childBackground.color = new Color(1f, 1f, 1f, 0f);
		}
		if (player == null)
		{
			Child.SetActive(value: false);
		}
		else
		{
			player.overHeadNameTag = this;
		}
		SpeakerSprite.sprite = NotTalkingSprite;
		Child.SetActive(value: false);
		SpeakerSprite.enabled = false;
	}

	private void OnEnable()
	{
		textMesh.text = "WW";
		MinimumBgWidth = getChildWidth;
		textMesh.text = string.Empty;
		AdjustTagWidth();
	}

	private void FadeTransition(bool FadeIn)
	{
		if (!Child || !childBackground || !SpeakerSprite || !textMesh)
		{
			return;
		}
		if (FadeIn)
		{
			if (!Child.activeSelf)
			{
				Child.SetActive(value: true);
			}
			if (TransitionTimer < 1f)
			{
				TransitionTimer += Time.unscaledDeltaTime / FadeInDuration;
				TransitionTimer = Mathf.Clamp01(TransitionTimer);
				Image image = childBackground;
				Color color = childBackground.color;
				float r = color.r;
				Color color2 = childBackground.color;
				float g = color2.g;
				Color color3 = childBackground.color;
				image.color = new Color(r, g, color3.b, TransitionTimer * initialBGAlpha);
				SpriteRenderer speakerSprite = SpeakerSprite;
				Color color4 = SpeakerSprite.color;
				float r2 = color4.r;
				Color color5 = SpeakerSprite.color;
				float g2 = color5.g;
				Color color6 = SpeakerSprite.color;
				speakerSprite.color = new Color(r2, g2, color6.b, TransitionTimer);
				Text text = textMesh;
				Color color7 = textMesh.color;
				float r3 = color7.r;
				Color color8 = textMesh.color;
				float g3 = color8.g;
				Color color9 = textMesh.color;
				text.color = new Color(r3, g3, color9.b, TransitionTimer);
			}
			else if (TransitionInProgress)
			{
				TransitionInProgress = false;
			}
		}
		else if (TransitionTimer > 0f)
		{
			TransitionTimer -= Time.unscaledDeltaTime / FadeOutDuration;
			TransitionTimer = Mathf.Clamp01(TransitionTimer);
			Image image2 = childBackground;
			Color color10 = childBackground.color;
			float r4 = color10.r;
			Color color11 = childBackground.color;
			float g4 = color11.g;
			Color color12 = childBackground.color;
			image2.color = new Color(r4, g4, color12.b, TransitionTimer * initialBGAlpha);
			SpriteRenderer speakerSprite2 = SpeakerSprite;
			Color color13 = SpeakerSprite.color;
			float r5 = color13.r;
			Color color14 = SpeakerSprite.color;
			float g5 = color14.g;
			Color color15 = SpeakerSprite.color;
			speakerSprite2.color = new Color(r5, g5, color15.b, TransitionTimer);
			Text text2 = textMesh;
			Color color16 = textMesh.color;
			float r6 = color16.r;
			Color color17 = textMesh.color;
			float g6 = color17.g;
			Color color18 = textMesh.color;
			text2.color = new Color(r6, g6, color18.b, TransitionTimer);
		}
		else if (Child.activeSelf)
		{
			Child.SetActive(value: false);
			TransitionInProgress = false;
		}
	}

	private void Update()
	{
	}

	public void UpdateNameTag(ChatUser user)
	{
		textMesh.text = user.GamerTag;
		SpeakerSprite.enabled = false;
		AdjustTagWidth();
	}

	private void AdjustTagWidth()
	{
		if (!string.IsNullOrEmpty(textMesh.text) && textMesh.text.Length > 16)
		{
			textMesh.text = textMesh.text.Substring(0, 16) + "…";
		}
		RectTransform component = Child.GetComponent<RectTransform>();
		component.sizeDelta = new Vector2((!(getChildWidth < MinimumBgWidth)) ? getChildWidth : MinimumBgWidth, component.rect.height);
	}
}
