using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace InControl.Internal
{
	internal class CodeWriter
	{
		private const char NewLine = '\n';

		private int indent;

		private StringBuilder stringBuilder;

		[CompilerGenerated]
		private static Func<char, bool> _003C_003Ef__mg_0024cache0;

		public CodeWriter()
		{
			indent = 0;
			stringBuilder = new StringBuilder(4096);
		}

		public void IncreaseIndent()
		{
			indent++;
		}

		public void DecreaseIndent()
		{
			indent--;
		}

		public void Append(string code)
		{
			Append(trim: false, code);
		}

		public void Append(bool trim, string code)
		{
			if (trim)
			{
				code = code.Trim();
			}
			string[] array = Regex.Split(code, "\\r?\\n|\\n");
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				string text = array[i];
				if (!text.All(char.IsWhiteSpace))
				{
					stringBuilder.Append('\t', indent);
					stringBuilder.Append(text);
				}
				if (i < num - 1)
				{
					stringBuilder.Append('\n');
				}
			}
		}

		public void AppendLine(string code)
		{
			Append(code);
			stringBuilder.Append('\n');
		}

		public void AppendLine(int count)
		{
			stringBuilder.Append('\n', count);
		}

		public void AppendFormat(string format, params object[] args)
		{
			Append(string.Format(format, args));
		}

		public void AppendLineFormat(string format, params object[] args)
		{
			AppendLine(string.Format(format, args));
		}

		public override string ToString()
		{
			return stringBuilder.ToString();
		}
	}
}
