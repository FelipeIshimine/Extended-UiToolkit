using System.Text;

namespace Core.Data.Tooltips
{
	[System.Serializable]
	public struct TooltipData
	{
		public string Title;
		public string Body;
		public string Footer;
		
		public bool IsEmpty =>string.IsNullOrEmpty(Body);

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			if (!string.IsNullOrEmpty(Title))
			{
				builder.AppendLine($"<b>{Title}</b>");
			}
			if (!string.IsNullOrEmpty(Body))
			{
				builder.AppendLine(Body);
			}
			if (!string.IsNullOrEmpty(Footer))
			{
				builder.AppendLine($"<i>{Footer}</i>");
			}
			return builder.ToString().Trim();
		}
	}
}