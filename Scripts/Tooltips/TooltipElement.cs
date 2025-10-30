using System;
using System.Threading;
using Core.Data.Tooltips;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tooltips
{
    [UxmlElement("TooltipElement")]
    public partial class TooltipElement : VisualElement
    {
        private readonly Label _titleLabel;
        private readonly Label _bodyLabel;
        private readonly Label _footerLabel;
        private readonly VisualElement _overlay;
        //Used for "EXPAND ICON" for example. Hold ALT to Expand (get more info)
        private readonly Label _altFooter;
        private CancellationTokenSource _cts;

        public TooltipElement()
        {
            pickingMode = PickingMode.Ignore;
            AddToClassList("tooltip-container");
            style.position = Position.Absolute;
                
            _titleLabel = new Label { name = "Title" };
            _titleLabel.AddToClassList("tooltip-title");
            Add(_titleLabel);

            _bodyLabel = new Label { name = "Body", style = { whiteSpace = WhiteSpace.Normal, flexShrink = 1}};
            _bodyLabel.AddToClassList("tooltip-body");
            Add(_bodyLabel);

            _altFooter = new Label("<sprite name=\"icon-expand\"> [ALT]")
            {
                name = "AltFooter"
            };
            _altFooter.AddToClassList("tooltip-expand");

            _footerLabel = new Label { name = "Footer", enableRichText = true, style = { whiteSpace = WhiteSpace.Normal, flexShrink = 1} };
            _footerLabel.AddToClassList("tooltip-footer");

            var footerRow = new VisualElement
            {
                name = "FooterContainer",
                style =
                {
                    flexDirection = FlexDirection.Row,
                    justifyContent = Justify.SpaceBetween
                }
            };
            footerRow.Add(_altFooter);
            footerRow.Add(_footerLabel);
            Add(footerRow);

            Title = "Lorem Ipsum";
            Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras sed vulputate enim, a eleifend risus. Pellentesque habitant morbi tristique";
            Footer = "Neque porro quisquam est qui dolorem ipsum";
            AltFooter = "";

            _overlay = new VisualElement()
            {
	            name = "Overlay"
            };
            _overlay.AddToClassList("tooltip-overlay");
            Add(_overlay);

        }

        // —————————————
        // DESIGN-TIME / UXML
        // —————————————
        [UxmlAttribute("title")]
        public string Title
        {
            get => _titleLabel.text;
            set
            {
                _titleLabel.text = value;
                _titleLabel.style.display = string.IsNullOrEmpty(value)
                    ? DisplayStyle.None
                    : DisplayStyle.Flex;
            }
        }

        [UxmlAttribute("body")]
        public string Body
        {
            get => _bodyLabel.text;
            set
            {
                _bodyLabel.text = value;
                _bodyLabel.style.display = string.IsNullOrEmpty(value)
                    ? DisplayStyle.None
                    : DisplayStyle.Flex;
            }
        }

        [UxmlAttribute("right-footer")]
        public string Footer
        {
            get => _footerLabel.text;
            set
            {
                _footerLabel.text = value;
                _footerLabel.style.display = string.IsNullOrEmpty(value)
                    ? DisplayStyle.None
                    : DisplayStyle.Flex;
            }
        }
        
        
        [UxmlAttribute("left-footer")]
        public string AltFooter
        {
            get => _altFooter.text;
            set
            {
                _altFooter.text = value;
                _altFooter.style.display = string.IsNullOrEmpty(value)
                    ? DisplayStyle.None
                    : DisplayStyle.Flex;
            }
        }

        public bool IsVisible => !ClassListContains("transition");

        public bool IsEmpty
        {
	        get
	        {
		        Debug.Log($"> {_titleLabel.text} {_bodyLabel.text} {_footerLabel.text} {_altFooter.text}");
		        return string.IsNullOrEmpty(_titleLabel.text) &&
			        string.IsNullOrEmpty(_bodyLabel.text)     &&
			        string.IsNullOrEmpty(_footerLabel.text)   &&
			        string.IsNullOrEmpty(_altFooter.text);
	        }
        }

        // —————————————
        // RUNTIME HELPERS
        // —————————————
        public void SetTooltipInfo(TooltipData info)
        {
            Title  = info.Title;
            Body   = info.Body;
            Footer = info.Footer;
            AltFooter = info.Footer;
        }

        public void SetExpandVisibility(bool visible)
        {
            _altFooter.style.display = visible
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }

        public async void Show()
        {
            try
            {
                await ShowAsync();
            }
            catch (Exception e)
            {
                if (e is not OperationCanceledException)
                {
                    Debug.LogException(e);
                }
            }
        }
        
        public async Awaitable ShowAsync(CancellationToken token = default)
        {
            _cts?.Cancel();
            _cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            try
            {
                await this.RemoveFromClassListAsync("transition",_cts.Token);
                pickingMode = PickingMode.Position;
            }
            finally
            {
                _cts.Cancel();
                _cts = null;
            }
        }

        public async void Hide()
        {
            try
            {
                await HideAsync();
            }
            catch (Exception e)
            {
                if (e is not OperationCanceledException)
                {
                    Debug.LogException(e);
                }
            }
        }

        public async Awaitable HideAsync(CancellationToken token = default)
        {
            _cts?.Cancel();
            _cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            try
            {
                pickingMode = PickingMode.Ignore;
                await this.AddToClassListAsync("transition",_cts.Token);
            }
            finally
            {
                _cts.Cancel();
                _cts = null;
            }
        }
        
        internal void SetAnchorPosition(Vector2 panelPos)
        {
            // panelPos is already in PANEL-LOCAL coordinates.
            style.left = panelPos.x;
            style.top  = panelPos.y;
        }

        internal void SetTranslateOffset(Vector2 offset)
        {
            style.translate = new StyleTranslate(new Translate(Length.Percent(offset.x), Length.Percent(offset.y)));
        }
        
        /*public void SetPosition(Vector2 screenPosition)
        {
            // convert screen → GUI world → UI-panel local
            Vector2 guiPoint = GUIUtility.GUIToScreenPoint(screenPosition);
            Vector2 local   = panel.visualTree.WorldToLocal(guiPoint);

            style.left = local.x;
            style.top  = local.y;
        }*/
        
        internal void SetTransformOrigin(Vector2 pivotPercent)
        {
            style.transformOrigin = new StyleTransformOrigin(
                new TransformOrigin(
                    Length.Percent(pivotPercent.x * 100f),
                    Length.Percent(pivotPercent.y * 100f)
                )
            );
        }
        
     
        /// <summary>
        /// Pick the right pivot so scale “grows” in the proper direction.
        /// </summary>
        internal void SetTransformOriginForPlacement(TooltipPlacement placement)
        {
            // map placement to pivot percent (0–1)
            (float px, float py) = placement switch
            {
                TooltipPlacement.Top         => (0.5f, 1f),
                TooltipPlacement.Bottom      => (0.5f, 0f),
                TooltipPlacement.Left        => (1f,   0.5f),
                TooltipPlacement.Right       => (0f,   0.5f),
                TooltipPlacement.TopLeft     => (1f,   1f),
                TooltipPlacement.TopRight    => (0f,   1f),
                TooltipPlacement.BottomLeft  => (1f,   0f),
                TooltipPlacement.BottomRight => (0f,   0f),
                TooltipPlacement.Center      => (0.5f, 0.5f),
                _                            => (0.5f, 0.5f)
            };

            // TransformOrigin takes Length – here as percent
            var origin = new TransformOrigin(
                Length.Percent(px * 100f),
                Length.Percent(py * 100f));
            style.transformOrigin = new StyleTransformOrigin(origin);
        }

        public void SetAnchorPosition(TooltipTracking tracking)
        {
            tracking.ApplyTo(this);
        }
    }
}