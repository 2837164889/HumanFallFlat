namespace I2.Loc
{
	public static class ScriptLocalization
	{
		public static class CUSTOMIZATION
		{
			public static string MIRROR_OFF => Get("CUSTOMIZATION/MIRROR_OFF");

			public static string MIRROR_ON => Get("CUSTOMIZATION/MIRROR_ON");

			public static string TIP_MANIPULATE => Get("CUSTOMIZATION/TIP_MANIPULATE");

			public static string TIP_MASK => Get("CUSTOMIZATION/TIP_MASK");

			public static string TIP_PAINT => Get("CUSTOMIZATION/TIP_PAINT");

			public static string PartBody => Get("CUSTOMIZATION/PartBody");

			public static string PartHead => Get("CUSTOMIZATION/PartHead");

			public static string PartUpper => Get("CUSTOMIZATION/PartUpper");

			public static string PartLower => Get("CUSTOMIZATION/PartLower");
		}

		public static class TUTORIAL
		{
			public static string COOP => Get("TUTORIAL/COOP");

			public static string CHEAT => Get("TUTORIAL/CHEAT");

			public static string CAMERADISABLED => Get("TUTORIAL/CAMERADISABLED");

			public static string RECORDERDISABLED => Get("TUTORIAL/RECORDERDISABLED");

			public static string SAVING => Get("TUTORIAL/SAVING");

			public static string LOADING => Get("TUTORIAL/LOADING");
		}

		public static class MULTIPLAYER
		{
			public static string Relayed => Get("MULTIPLAYER/Relayed");
		}

		public static string Get(string Term)
		{
			return Get(Term, FixForRTL: false, 0);
		}

		public static string Get(string Term, bool FixForRTL)
		{
			return Get(Term, FixForRTL, 0);
		}

		public static string Get(string Term, bool FixForRTL, int maxLineLengthForRTL)
		{
			return LocalizationManager.GetTermTranslation(Term);
		}
	}
}
