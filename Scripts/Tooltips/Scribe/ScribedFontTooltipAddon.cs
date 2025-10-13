using System;
using System.Linq;
using Tooltips.Scribe;
using UI.Manipulators;
using UnityEngine.UIElements;

namespace Tooltips.Scribe
{
	public class ScribedFontTooltipAddon : IDisposable
	{
		private readonly TooltipManipulator manipulator;

		public ScribedFontTooltipAddon(TooltipManipulator manipulator)
		{
			this.manipulator = manipulator;
			manipulator.OnShow += ApplyFontClass;
			manipulator.Tooltip.RegisterCallbackOnce<DetachFromPanelEvent>(Detached);
		}

		public void Dispose()
		{
			manipulator.OnShow -= ApplyFontClass;
		}

		private void ApplyFontClass()
		{
			foreach (var textElement in manipulator.Tooltip.Query<TextElement>().Build())
			{
				SetLanguageClass(textElement, ScribeManager.Active.FontName);
			}
		}

		private void Detached(DetachFromPanelEvent evt)
		{
			Dispose();
		}

		private void SetLanguageClass(TextElement text, string languageCode)
		{
			var activeLanguage = text.GetClasses().Where(x => x.Contains("lang-")).ToArray();
			foreach (string s in activeLanguage)
			{
				text.RemoveFromClassList(s);
			}
			text.AddToClassList($"lang-{languageCode}");
		}
	}
}

public static class TooltipManipulatorFontExtension
{
	public static TooltipManipulator WithLanguageFontClass(this TooltipManipulator manipulator)
	{
		new ScribedFontTooltipAddon(manipulator);
		return manipulator;
	}
}