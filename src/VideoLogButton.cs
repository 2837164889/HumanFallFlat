using TMPro;
using UnityEngine;

public class VideoLogButton : MonoBehaviour
{
	public VideoLogMenu menu;

	public VideoRepositoryItem videoRepositoryItem;

	public TextMeshProUGUI label;

	public bool justGotFocus;

	public void GotFocus()
	{
		menu.SelectItem(this);
		justGotFocus = true;
	}

	public void OnPointerClick()
	{
		if (!justGotFocus && menu.SelectedItem == this)
		{
			menu.PlayVideo(this);
		}
		justGotFocus = false;
	}

	public void OnSubmit()
	{
		menu.PlayVideo(this);
		justGotFocus = false;
	}

	public void Bind(VideoRepositoryItem videoRepositoryItem)
	{
		this.videoRepositoryItem = videoRepositoryItem;
		label.text = videoRepositoryItem.title;
	}
}
