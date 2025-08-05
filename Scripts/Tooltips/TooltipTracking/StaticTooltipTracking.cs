using Tooltips;
using TypeSelector;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class StaticTooltipTracking : TooltipTracking
{
    [TypeSelector, SerializeReference] public ITooltipAnchor Anchor;
    public TooltipPlacement Placement = TooltipPlacement.Bottom;
    [Min(10)]
    public float Margin = 20;

    public float damping = 0;
    
    public StaticTooltipTracking()
    {
    }
        
    public StaticTooltipTracking(ITooltipAnchor anchor)
    {
        Anchor = anchor;
    }
        
    public StaticTooltipTracking(ITooltipAnchor anchor, TooltipPlacement placement = TooltipPlacement.Bottom, float offset = 0)
    {
        Anchor = anchor;
        Placement = placement;
        Margin = offset;
    }

    public static TooltipTracking DefaultMouseTop => new StaticTooltipTracking(new MousePositionAnchor(), TooltipPlacement.Top)
    {
        Margin = 25,
    };

    public Vector2 GetMarginVector() => GetPlacementUnitVector(Placement) * Margin;
    public Vector2 GetPlacementUnitVector() => GetPlacementUnitVector(Placement);

    public Vector2 CalculateTranslationDelta() => CalculateTranslationDelta(Placement);


    public override void ApplyTo(TooltipElement tooltipElement)
    {
        var screenPosition = Anchor.GetScreenPosition();

        Vector2 pos = new Vector2(tooltipElement.style.left.value.value,tooltipElement.style.top.value.value);
        
        screenPosition.y = Screen.height - screenPosition.y;
        screenPosition = RuntimePanelUtils.ScreenToPanel(tooltipElement.panel, screenPosition);

        var targetPos = screenPosition + GetMarginVector();

		targetPos = Vector2.Lerp(pos, targetPos, 1-damping);
        
        tooltipElement.SetAnchorPosition(targetPos);
        tooltipElement.SetTranslateOffset(CalculateTranslationDelta());
    }

}