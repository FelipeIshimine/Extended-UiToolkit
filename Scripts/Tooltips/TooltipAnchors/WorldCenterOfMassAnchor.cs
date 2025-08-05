using UnityEngine;

[System.Serializable]
public class WorldCenterOfMassAnchor : ITooltipAnchor
{
	[SerializeField] public Rigidbody2D target;
	[SerializeField] private Camera camera;
	public WorldCenterOfMassAnchor()
	{
	}

	public WorldCenterOfMassAnchor(Rigidbody2D target)
	{
		this.target = target;
		camera = Camera.main;
	}
        
	public WorldCenterOfMassAnchor(Rigidbody2D target, Camera camera)
	{
		this.target = target;
		this.camera = camera;
	}

	public Vector2 GetScreenPosition() => camera.WorldToScreenPoint(target.worldCenterOfMass);
}