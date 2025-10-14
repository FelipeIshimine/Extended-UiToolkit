using UnityEngine;

[System.Serializable]
public class WorldCenterOfMassAnchor : ITooltipAnchor, ITooltipAnchorWithCamera
{
	[SerializeField] public Rigidbody2D target;
	[SerializeField] private Camera camera;
	public Camera Cam { get => camera; set => camera = value; }
	
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

	public Vector2 GetScreenPosition()
	{
		var pos = camera.WorldToScreenPoint(target.worldCenterOfMass);
		pos.y = Screen.height - pos.y;
		return pos;
	}
}

public interface ITooltipAnchorWithCamera
{
	public Camera Cam { get; set; }
}