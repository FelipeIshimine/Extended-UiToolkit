using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Tooltips
{
    [System.Serializable]
    public class DynamicTooltipTracking : TooltipTracking
    {
        public OrientationFocusMode focusMode;
        
        public VisualElement Target;
        private MousePositionAnchor _mousePositionAnchor = new();
        
        [Min(10)] public float Margin = 20;

        public bool discretePosition;

        public DynamicTooltipTracking() { }
        
        public DynamicTooltipTracking(float offset = 0)
        {
            Margin = offset;
        }

        public static TooltipTracking DefaultDynamic => new DynamicTooltipTracking()
        {
            focusMode = DynamicTooltipTracking.OrientationFocusMode.LowerScreenCenter,
            Margin = 25,
            discretePosition = false
        };
        

        public override void ApplyTo(TooltipElement tooltipElement)
        {
            Vector2 screenPosition;
            switch (focusMode)
            {
                case OrientationFocusMode.Mouse:
                    screenPosition = _mousePositionAnchor.GetScreenPosition();
                    break;
                case OrientationFocusMode.ScreenCenter:
                    screenPosition = new Vector2(Screen.width/2f,Screen.height/2f);
                    break;
                case OrientationFocusMode.LowerScreenCenter:
                    screenPosition = new Vector2(Screen.width/2f,Screen.height/3f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            screenPosition.y = Screen.height - screenPosition.y;
            screenPosition = RuntimePanelUtils.ScreenToPanel(tooltipElement.panel, screenPosition);
            
            Rect rect = Target.contentRect;
            
            var targetWorldCenter=            Target.LocalToWorld(rect.center);
            
            var diff = screenPosition - targetWorldCenter;
            
            var dir = diff.normalized;
            
            var unitDir = dir / Mathf.Max(Mathf.Abs(dir.x),Mathf.Abs(dir.y));
            
            
            var normalizedLocalPosition = (unitDir + Vector2.one) / 2f ;

     
            normalizedLocalPosition.x = 1 -(normalizedLocalPosition.x);
            normalizedLocalPosition.y = 1 -(normalizedLocalPosition.y);

            if (discretePosition)
            {
                normalizedLocalPosition = Snapping.Snap(normalizedLocalPosition, Vector2.one * .5f);
            }
            
            
            tooltipElement.SetAnchorPosition(targetWorldCenter + rect.size * unitDir/2f + unitDir * Margin);

            tooltipElement.SetTranslateOffset(-(normalizedLocalPosition * 100));
        }

        public enum OrientationFocusMode
        {
            Mouse,
            ScreenCenter,
            LowerScreenCenter,
        }

    }
    
}