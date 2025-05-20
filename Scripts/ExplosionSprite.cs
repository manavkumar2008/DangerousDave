using Godot;

public partial class ExplosionSprite : AnimatedSprite2D
{
	
	public override void _Ready()
	{
		Timer timer = GetNode<Timer>("Timer");
		timer.WaitTime = 3;
		timer.OneShot = true;
		timer.Timeout += QueueFree;
		timer.Start();
	}

	public override void _PhysicsProcess(double delta)
	{
		Position += new Vector2(0, (float)delta * 3);
	}
}
