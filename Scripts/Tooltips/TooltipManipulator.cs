using System;
using Core.Data.Tooltips;
using Tooltips;
using Tooltips.Manipulators;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace UI.Manipulators
{
    public class TooltipManipulator : MouseManipulator
    {
        private readonly TooltipElement _tooltip;
        private Func<TooltipData> _generateTooltip;
        private IVisualElementScheduledItem _scheduleItem;
        private VisualElement _root;

        private TooltipTracking _tracking;

        public TooltipManipulator(TooltipData tooltip, TooltipTracking tracking) : this(new TooltipInfoSource.Instance(tooltip), tracking) { }

        public TooltipManipulator(TooltipInfoSource tooltipSource, TooltipTracking tracking)
        {
         
            
            _tracking = tracking;
            _generateTooltip = tooltipSource.GetTooltipInfo;

            _tooltip = new TooltipElement();
            _tooltip.Hide(); // start hidden
        }

        protected override void RegisterCallbacksOnTarget()
        {
            if(_tracking is StaticTooltipTracking tracking)
            {
                if (tracking.Anchor is VisualElementAnchor visualElementAnchor && !visualElementAnchor.IsValid())
                {
                    tracking.Anchor = new VisualElementAnchor(target);
                }
                else if (tracking.Anchor is VisualElementBoundsAnchor anchor && !anchor.IsValid())
                {
                    tracking.Anchor = new VisualElementBoundsAnchor(target, anchor.AnchorPoint);
                }
            }
            else if (_tracking is DynamicTooltipTracking dynamicTooltipTracking)
            {
                dynamicTooltipTracking.Target = target;
            }
            
            target.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            target.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
            target.RegisterCallback<DetachFromPanelEvent>(OnDetach);

            TooltipLayer.Add(_tooltip);

            _scheduleItem = _tooltip.schedule.Execute(UpdateAnchor).Every(0);
            _scheduleItem.Pause();
            
            target.style.overflow = Overflow.Visible;

            _root = target;
            while (_root.parent != null)
                _root = _root.parent;
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            _scheduleItem?.Pause();
            _scheduleItem = null;

            target.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
            target.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);

            TooltipLayer.Remove(_tooltip);
        }

        private void OnPointerEnter(PointerEnterEvent evt)
        {
            _scheduleItem?.Resume();
            UpdateAnchor();
            _tooltip.SetTooltipInfo(_generateTooltip.Invoke());
            _tooltip.Show();
            
        }

        private void OnPointerLeave(PointerLeaveEvent evt)
        {
            _scheduleItem?.Pause();
            _tooltip.Hide();
        }

        private void OnDetach(DetachFromPanelEvent evt) => _tooltip.Hide();
        private void UpdateAnchor()
        {
            _tooltip?.SetAnchorPosition(_tracking);
        }
 

        public void SetTooltip(Func<TooltipData> generator) => _generateTooltip = generator;

        public void SetExpandVisibility(bool visible) => _tooltip.SetExpandVisibility(visible);
        
        public void SetTracking(StaticTooltipTracking config)
        {
            _tracking = config;
        }

    }
}
