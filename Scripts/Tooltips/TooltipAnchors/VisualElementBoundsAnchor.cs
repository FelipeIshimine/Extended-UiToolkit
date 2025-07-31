using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class VisualElementBoundsAnchor : ITooltipAnchor
{
    public VisualElement Element;
    public ElementAnchorPoint AnchorPoint;

    public VisualElementBoundsAnchor()
    {
    }
        
    public VisualElementBoundsAnchor(VisualElement element, ElementAnchorPoint anchorPoint)
    {
        Element = element;
        AnchorPoint = anchorPoint;
    }

    public Vector2 GetScreenPosition()
    {
        if (Element.panel == null)
            return Vector2.zero;

        Rect rect = Element.contentRect;
        Vector2 local;
        switch (AnchorPoint)
        {
            case ElementAnchorPoint.TopLeft:     local = new Vector2(rect.xMin, rect.yMin); break;
            case ElementAnchorPoint.Top:         local = new Vector2(rect.center.x, rect.yMin); break;
            case ElementAnchorPoint.TopRight:    local = new Vector2(rect.xMax, rect.yMin); break;
            case ElementAnchorPoint.Right:       local = new Vector2(rect.xMax, rect.center.y); break;
            case ElementAnchorPoint.BottomRight: local = new Vector2(rect.xMax, rect.yMax); break;
            case ElementAnchorPoint.Bottom:      local = new Vector2(rect.center.x, rect.yMax); break;
            case ElementAnchorPoint.BottomLeft:  local = new Vector2(rect.xMin, rect.yMax); break;
            case ElementAnchorPoint.Left:        local = new Vector2(rect.xMin, rect.center.y); break;
            case ElementAnchorPoint.Center:      local = rect.center; break;
            default:                             local = rect.center; break;
        }

        return Element.LocalToWorld(local);
    }

    public bool IsValid() => Element != null;
}