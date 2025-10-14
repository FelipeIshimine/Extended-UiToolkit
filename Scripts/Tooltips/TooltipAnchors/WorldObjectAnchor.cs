using UnityEngine;

[System.Serializable]
public class WorldObjectAnchor : ITooltipAnchor,ITooltipAnchorWithCamera
{
    [SerializeField] public Transform target;
    [SerializeField] private Camera camera;
	public Camera Cam { get => camera; set => camera = value; }

    public WorldObjectAnchor()
    {
    }

    public WorldObjectAnchor(Transform target)
    {
        this.target = target;
        camera = Camera.main;
    }
        
    public WorldObjectAnchor(Transform target, Camera camera)
    {
        this.target = target;
        this.camera = camera;
    }

    public Vector2 GetScreenPosition()
    {
        Vector3 screenPos = camera.WorldToScreenPoint(target.position);
        return new Vector2(screenPos.x, Screen.height -screenPos.y); // Flip Y
    }
}