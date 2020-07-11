using System.Collections.Generic;
using UnityEngine;

public class CreditsText : MonoBehaviour
{
	public static CreditsText instance;

	private Dictionary<char, List<CreditsLetter>> repository = new Dictionary<char, List<CreditsLetter>>();

	private Dictionary<char, CreditsLetter> masters = new Dictionary<char, CreditsLetter>();

	public CreditsBlock blockPrefab;

	private List<CreditsBlock> blockRepository = new List<CreditsBlock>();

	private void OnEnable()
	{
		instance = this;
		InitData();
	}

	public void OnDestroy()
	{
		instance = null;
	}

	public void InitData()
	{
		if (masters.Count > 0)
		{
			return;
		}
		CreditsLetter[] componentsInChildren = GetComponentsInChildren<CreditsLetter>(includeInactive: true);
		foreach (CreditsLetter creditsLetter in componentsInChildren)
		{
			if (masters.ContainsKey(creditsLetter.character))
			{
				Debug.LogError("Multiple instances of letter", creditsLetter);
			}
			masters[creditsLetter.character] = creditsLetter;
			repository[creditsLetter.character] = new List<CreditsLetter>
			{
				creditsLetter
			};
			creditsLetter.gameObject.SetActive(value: false);
		}
		blockPrefab.gameObject.SetActive(value: false);
	}

	public float GetLetterWidth(char character)
	{
		if (masters.ContainsKey(character))
		{
			return masters[character].width;
		}
		return 0f;
	}

	public CreditsLetter GetLetter(char character)
	{
		if (!repository.ContainsKey(character))
		{
			Debug.LogError("Letter missing: " + character);
			if (character == 'é')
			{
				character = 'e';
			}
		}
		List<CreditsLetter> list = repository[character];
		CreditsLetter creditsLetter = null;
		if (list.Count > 0)
		{
			creditsLetter = list[0];
			list.RemoveAt(0);
		}
		else
		{
			creditsLetter = Object.Instantiate(masters[character].gameObject).GetComponent<CreditsLetter>();
		}
		return creditsLetter;
	}

	public void ReleaseLetter(CreditsLetter letter)
	{
		List<CreditsLetter> list = repository[letter.character];
		list.Add(letter);
		letter.gameObject.SetActive(value: false);
		letter.GetComponent<Rigidbody>().isKinematic = true;
	}
}
