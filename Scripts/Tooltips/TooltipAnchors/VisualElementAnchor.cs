using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class VisualElementAnchor : ITooltipAnchor
{
    public readonly VisualElement Element;

    public VisualElementAnchor(){}
    public VisualElementAnchor(VisualElement element)
    {
        this.Element = element;
    }

    public Vector2 GetScreenPosition() => Element.worldBound.center;

    public bool IsValid() => Element != null;
}