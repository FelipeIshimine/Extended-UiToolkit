using UnityEngine;
using UnityEngine.UIElements;

public class HoverColorManipulator : MouseManipulator
{
    private readonly Color hoverColor;
    private readonly Color normalColor;
    
    public HoverColorManipulator(Color hoverColor, Color normalColor)
    {
        this.hoverColor = hoverColor;
        this.normalColor = normalColor;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        target.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseEnterEvent>(OnMouseEnter);
        target.UnregisterCallback<MouseLeaveEvent>(OnMouseLeave);
    }

    private void OnMouseEnter(MouseEnterEvent evt)
    {
        (target as VisualElement).style.color = hoverColor;
    }

    private void OnMouseLeave(MouseLeaveEvent evt)
    {
        (target as VisualElement).style.color = normalColor;
    }
}