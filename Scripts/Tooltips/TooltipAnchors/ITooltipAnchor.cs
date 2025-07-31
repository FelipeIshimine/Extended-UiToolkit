using UnityEngine;

public interface ITooltipAnchor
{
    /// <summary>Returns the current screen-space anchor position for the tooltip to track.</summary>
    Vector2 GetScreenPosition();
}