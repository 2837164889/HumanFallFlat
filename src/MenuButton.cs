using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : Button
{
	private bool _isOn;

	public TextMeshProUGUI label;

	[NonSerialized]
	public Text[] textLabel;

	private int frameUpdated;

	public bool isOn
	{
		get
		{
			return _isOn;
		}
		set
		{
			_isOn = value;
			DoStateTransition(base.currentSelectionState, instant: false);
		}
	}

	public void SetLabel(Text textLabelPara)
	{
		textLabel = new Text[1]
		{
			textLabelPara
		};
		label = null;
	}

	public void SetLabel(TextMeshProUGUI label)
	{
		this.label = label;
		textLabel = null;
	}

	protected override void Awake()
	{
		base.Awake();
		if (label == null)
		{
			label = GetComponentInChildren<TextMeshProUGUI>();
		}
		if (label == null)
		{
			textLabel = GetComponentsInChildren<Text>();
		}
	}

	protected override void DoStateTransition(SelectionState state, bool instant)
	{
		Color a = default(Color);
		Color a2;
		if (isOn)
		{
			a = ((state == SelectionState.Highlighted) ? new Color(0f, 0f, 0f, 1f) : new Color(0f, 0f, 0f, 0.9f));
			a2 = Color.white;
		}
		else
		{
			switch (state)
			{
			case SelectionState.Normal:
			{
				Color normalColor = base.colors.normalColor;
				a = new Color(1f, 1f, 1f, normalColor.a);
				a2 = Color.black;
				break;
			}
			case SelectionState.Highlighted:
				a = new Color(0f, 0f, 0f, 0.75f);
				a2 = Color.white;
				break;
			case SelectionState.Pressed:
				a = new Color(0f, 0f, 0f, 0.75f);
				a2 = Color.white;
				break;
			case SelectionState.Disabled:
				a = base.colors.disabledColor;
				a2 = Color.black;
				break;
			default:
				a = Color.black;
				a2 = Color.white;
				break;
			}
		}
		if (frameUpdated == Time.renderedFrameCount)
		{
			instant = true;
		}
		frameUpdated = Time.renderedFrameCount;
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		base.targetGraphic.CrossFadeColor(a * base.colors.colorMultiplier, (!instant) ? base.colors.fadeDuration : 0f, ignoreTimeScale: true, useAlpha: true);
		if (label != null)
		{
			label.color = Color.white;
			label.CrossFadeColor(a2 * base.colors.colorMultiplier, (!instant) ? base.colors.fadeDuration : 0f, ignoreTimeScale: true, useAlpha: true);
		}
		if (textLabel != null)
		{
			for (int i = 0; i < textLabel.Length; i++)
			{
				textLabel[i].color = Color.white;
				textLabel[i].CrossFadeColor(a2 * base.colors.colorMultiplier, (!instant) ? base.colors.fadeDuration : 0f, ignoreTimeScale: true, useAlpha: true);
			}
		}
	}
}
