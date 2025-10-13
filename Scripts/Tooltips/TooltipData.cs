using System.Text;

namespace Core.Data.Tooltips
{
	[System.Serializable]
	public struct TooltipData
	{
		public string Title;
		public string Body;
		public string Footer;
		public string AltFooter;
		
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
			if (!string.IsNullOrEmpty(AltFooter))
			{
				builder.AppendLine($"<i>{AltFooter}</i>");
			}
			return builder.ToString().Trim();
		}
	}
}