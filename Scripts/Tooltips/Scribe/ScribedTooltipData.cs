using Core.Data.Tooltips;
using Scribe;
using Tooltips.Manipulators;

#if HAS_SCRIBE
namespace Tooltips.Scribe
{
	[System.Serializable]
	public class ScribedTooltipData : TooltipInfoSource
	{
		[ScribeKey] public string title;
		[ScribeKey] public string body;
		[ScribeKey] public string footer;

		public override TooltipData GetTooltipInfo()
		{
			return new TooltipData()
			{
				Title = title.Scribed(),
				Body = body.Scribed(),
				Footer = footer.Scribed(),
			};
		}
	}
}
#endif
