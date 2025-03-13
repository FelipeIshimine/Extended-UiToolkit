using UnityEngine;
using UnityEngine.UIElements;

public static class UIToolkitUtils
{
    public static void PositionElementBelowWorldBound(VisualElement element, Rect worldBound, VisualElement root)
    {
        // Convert world position to local UI position
        Vector2 localPosition = root.WorldToLocal(new Vector2(worldBound.x, worldBound.yMax));

        // Apply absolute positioning
        element.style.position = Position.Absolute;
        element.style.left = localPosition.x;
        element.style.top = localPosition.y;
    }
}