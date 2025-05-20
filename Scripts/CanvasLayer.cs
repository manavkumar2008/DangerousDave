using Godot;

public partial class CanvasLayer : Godot.CanvasLayer
{
	private Camera camera;
	private Vector2 zoom;
	public override void _Ready()
	{
		camera = GetNode<Camera>("/root/Node/Main/Camera");
		zoom = camera.Zoom;
		Vector2 scaleFactor = new Vector2(zoom.X*Scale.X, zoom.Y*Scale.Y);
		Scale = scaleFactor;
	}
	
	public override void _Process(double delta)
	{
		
	}
}
