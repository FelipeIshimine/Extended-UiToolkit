using System;
using Core.Data.Tooltips;

using UnityEngine;
using UnityEngine.UIElements;
using Position = UnityEngine.UIElements.Position;

namespace UI.Manipulators
{
	public class TooltipAppendix : Manipulator
    {
        private static Action<bool> _onShowExtendedInfo;
        private static bool _showExtendedInfo;
        private static bool ShowExtendedInfo
        {
            get => _showExtendedInfo;
            set
            {
                _showExtendedInfo = value;
                _onShowExtendedInfo?.Invoke(value);
            }
        }
        
		private readonly VisualElement container;

		private readonly Orientation orientation;

     

		public VisualElement Container => container;

		public TooltipAppendix(TooltipData tooltip, Orientation orientation)
		{
			this.orientation = orientation;
			container = new VisualElement
			{
				pickingMode = PickingMode.Ignore
			};
			container.AddToClassList("tooltip-container");
			container.AddToClassList("tooltip-appendix");
			container.AddToClassList("hide");

			if (!string.IsNullOrEmpty(tooltip.Title))
			{
				var titleLabel = new Label(tooltip.Title)
				{
				};
				titleLabel.AddToClassList("tooltip-title");
				container.Add(titleLabel);
			}
			if (!string.IsNullOrEmpty(tooltip.Body))
			{
				var bodyLabel = new Label(tooltip.Body)
				{
				};
				bodyLabel.AddToClassList("tooltip-body");
				container.Add(bodyLabel);
			}
			
			if (!string.IsNullOrEmpty(tooltip.Footer))
			{
				var footerLabel = new Label(tooltip.Footer)
				{
				};
				footerLabel.AddToClassList("tooltip-footer");
				container.Add(footerLabel);
			}
		}
	
		protected override void RegisterCallbacksOnTarget()
		{
			target.Add(container);
			container.style.opacity = 0;
			container.style.position = Position.Absolute;
			target.style.overflow = Overflow.Visible;

            _onShowExtendedInfo += ShowExtendedInfoUpdate;
            
			WaitAndRefreshAsync();
		}

        private void ShowExtendedInfoUpdate(bool obj)
        {
            if (obj)
            {
                container.AddToClassList("hide");
            }
            else
            {
                container.RemoveFromClassList("hide"); 
            }			
        }

        protected override void UnregisterCallbacksFromTarget()
		{
            _onShowExtendedInfo -= ShowExtendedInfoUpdate;

			target.Remove(container);
		}

		private async void WaitAndRefreshAsync()
		{
            try
            {
                await Awaitable.NextFrameAsync();

                container.style.opacity = 1;
                container.style.top = container.style.bottom =
                    container.style.left = container.style.right = new StyleLength(StyleKeyword.Auto);

                Vector2 size = new Vector2(container.resolvedStyle.width, container.resolvedStyle.height);
                var parent = container.parent;
                switch (orientation)
                {
                    case Orientation.Top:
                        container.style.top = -size.y;
                        break;
                    case Orientation.Down:
                        container.style.bottom = -size.y;
                        break;
                    case Orientation.Left:
                        container.style.left = -size.x;
                        break;
                    case Orientation.Right:
                        container.style.right = -size.x;
                        break;
                    case Orientation.UpLeft:
                        container.style.top = -size.y;
                        container.style.left = 0;
                        break;
                    case Orientation.UpRight:
                        container.style.top = -size.y;
                        container.style.right = 0;
                        break;
                    case Orientation.DownLeft:
                        container.style.bottom = -size.y;
                        container.style.left = 0;
                        break;
                    case Orientation.DownRight:
                        container.style.bottom = -size.y;
                        container.style.right = 0;
                        break;
                    case Orientation.LeftUp:
                        container.style.left = -size.x;
                        container.style.top = 0;
                        break;
                    case Orientation.LeftDown:
                        container.style.left = -size.x;
                        container.style.bottom = 0;
                        break;
                    case Orientation.RightUp:
                        container.style.right = -size.x;
                        container.style.top = 0;
                        break;
                    case Orientation.RightDown:
                        container.style.right = -size.x;
                        container.style.bottom = 0;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (ShowExtendedInfo)
                {
                    container.RemoveFromClassList("hide");
                }
                else
                {
                    container.AddToClassList("hide");
                }
            }
            catch (Exception e)
            {
                if (e is not OperationCanceledException)
                {
                    Debug.LogException(e);
                }
            }
		}

		public enum Orientation
		{
			UpLeft,
			Top,
			UpRight,
			LeftUp,
			Left,
			LeftDown,
			RightUp,
			Right,
			RightDown,
			DownLeft,
			Down,
			DownRight,
		}

		public void Release() => target.RemoveManipulator(this);
	}
}
