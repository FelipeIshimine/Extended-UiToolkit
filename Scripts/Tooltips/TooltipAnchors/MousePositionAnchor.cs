using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class MousePositionAnchor : ITooltipAnchor
{
    private Vector2 _lastMousePosition;
        
    public Vector2 GetScreenPosition()
    {
        if (Pointer.current == null)
        {
            return _lastMousePosition;
        }
        _lastMousePosition = Pointer.current.position.ReadValue();
        _lastMousePosition.y = Screen.height - _lastMousePosition.y;
        return _lastMousePosition;
    }

    public ITooltipAnchor Clone() => new MousePositionAnchor();
}