using Godot;
using System;

public partial class Bullet : AnimatedSprite2D
{

	private int speed = 88;
	public Vector2 velocity = new Vector2(1,0);
	public Node2D Spawner;
	
	public override void _Ready()
	{
		
	}

	private void OnBodyShapeEntered(Rid bodyRid, Node2D body, int bodyShapeIndex, int localShapeIndex)
	{
		if(body.Name=="TileMap")
			QueueFree();
	}

	private void OnAreaEntered(Area2D area)
	{
		if(area.HasNode("Health") && !(area.GetParent().Equals(Spawner)||area.Equals(Spawner)))
		{
			area.GetNode("Health").Call("Damage");
			QueueFree();
		}else if (area.GetParent() is Bullet)
		{
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
