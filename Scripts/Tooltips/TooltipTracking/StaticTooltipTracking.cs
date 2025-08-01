using Tooltips;
using TypeSelector;
using UnityEngine;

[System.Serializable]
public class StaticTooltipTracking : TooltipTracking
{
    [TypeSelector, SerializeReference] public ITooltipAnchor Anchor;
    public TooltipPlacement Placement = TooltipPlacement.Bottom;
    [Min(10)]
    public float Margin = 20;

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
        tooltipElement.SetAnchorPosition(screenPosition + GetMarginVector());
        tooltipElement.SetTranslateOffset(CalculateTranslationDelta());
    }

}