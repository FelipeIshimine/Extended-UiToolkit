using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class WorldObjectRendererBoundsAnchor : ITooltipAnchor
{
	[SerializeField] public Renderer target;
	[SerializeField] private Camera camera;
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
		Vector3 screenPos = camera.WorldToScreenPoint(bounds.center + bounds.extents.y * Vector3.up);
		return new Vector2(screenPos.x, screenPos.y); // Flip Y
	}
}