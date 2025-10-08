using UnityEngine;

[System.Serializable]
public class WorldObjectRendererBoundsAnchor : ITooltipAnchor,ITooltipAnchorWithCamera
{
	[SerializeField] public Renderer target;
	[SerializeField] private Camera camera;
	[SerializeField, DirectionDropdown] private Vector2 dir;
	public Camera Cam { get => camera; set => camera = value; }
	
	public WorldObjectRendererBoundsAnchor()
	{
	}

	public WorldObjectRendererBoundsAnchor(Renderer target)
	{
		this.target = target;
		camera = Camera.main;
	}
        
	public WorldObjectRendererBoundsAnchor(Renderer target, Camera camera)
	{
		this.target = target;
		this.camera = camera;
	}

	public Vector2 GetScreenPosition()
	{
		Bounds bounds = target.bounds;
		Vector3 screenPos = camera.WorldToScreenPoint(bounds.center + (Vector3)(bounds.extents * dir));
		return new Vector2(screenPos.x, screenPos.y); // Flip Y
	}
}