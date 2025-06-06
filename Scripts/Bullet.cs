using Godot;

public partial class Bullet : AnimatedSprite2D
{

	public int speed = 120;
	public Vector2 velocity = new Vector2(1,0);
	public Node2D Spawner;
	private Main main;
	public string animation = "default";
	
	public override void _Ready()
	{
		main = (Main)GetParent();
		Play(animation);
	}

	private void OnBodyShapeEntered(Rid bodyRid, Node2D body, int bodyShapeIndex, int localShapeIndex)
	{
		if(body.Name=="TileMap")
			QueueFree();

		if (body.Name == "Dave" && body.Name != Spawner.Name)
		{
			body.Call("OnDamage");
			main.OnBulletHit();
			QueueFree();
		}
	}

	private void OnAreaEntered(Area2D area)
	{
		if(!(area.GetParent() is Bullet) && !(area.GetParent().Name == Spawner.Name || area.Name == Spawner.Name) && area is Enemy)
		{
			area.Call("OnDamage");
			main.OnBulletHit();
			QueueFree();
		}
	}

	private void ScreenExited()
	{
		QueueFree();
	}
	
	public override void _Process(double delta)
	{
		Position += velocity*(float)delta*speed;
	}
}
