public static class StringUtils
{
	public static bool StartsWith(string toSearch, string subString)
	{
		int length = toSearch.Length;
		int length2 = subString.Length;
		if (length < length2)
		{
			return false;
		}
		for (int i = 0; i < length2; i++)
		{
			if (toSearch[i] != subString[i])
			{
				return false;
			}
		}
		return true;
	}

	public static bool EndsWith(string toSearch, string subString)
	{
		int length = toSearch.Length;
		int length2 = subString.Length;
		if (length < length2)
		{
			return false;
		}
		int num = length - length2;
		for (int i = 0; i < length2; i++)
		{
			if (toSearch[num + i] != subString[i])
			{
				return false;
			}
		}
		return true;
	}

	public static bool Contains(string toSearch, string subString)
	{
		int length = toSearch.Length;
		int length2 = subString.Length;
		if (length2 == 0)
		{
			return true;
		}
		if (length < length2)
		{
			return false;
		}
		int num = length - length2;
		int i = 0;
		char c = subString[0];
		int num2 = 0;
		while (num2 < length2)
		{
			if (toSearch[num2 + i] == subString[num2])
			{
				num2++;
				continue;
			}
			num2 = 0;
			for (i++; i <= num && toSearch[i] != c; i++)
			{
			}
			if (i <= num)
			{
				continue;
			}
			return false;
		}
		return true;
	}
}
