using System.IO;
using System.Text;

public class FileLog
{
	private string path;

	private int batch;

	private int cnt;

	private StringBuilder sb = new StringBuilder();

	public FileLog(string path, int batch)
	{
		this.path = path;
		this.batch = batch;
		File.Delete(path);
	}

	public void Log(string format, params object[] args)
	{
		sb.AppendFormat(format, args);
		cnt++;
		if (cnt == batch)
		{
			File.AppendAllText(path, sb.ToString());
			sb = new StringBuilder();
			cnt = 0;
		}
	}
}
