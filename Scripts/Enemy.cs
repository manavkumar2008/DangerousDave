using Godot;
using System;
using GodotPlugins.Game;
using Microsoft.VisualBasic;

public partial class Enemy : Area2D
{
	[Export] 
	private int type = 1;
	[Export]
	private float[] progress = Array.Empty<float>();
	
	private Dave dave;

	private PackedScene BulletScene = ResourceLoader.Load<PackedScene>("res://Scenes/bullet.tscn");
	
	AnimatedSprite2D sprite;
	private Main main;
	
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		sprite.Play(type.ToString());
		dave = GetNode<Dave>("/root/Node/Main/Dave");
		main = GetNode<Main>("/root/Node/Main");
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.Name == "Dave")
		{
			body.GetNode("Health").Call("Damage");
			QueueFree();
		}
	}

	private void Shoot()
	{
		for(int i = 0; i < progress.Length; i++)
		{
			if(Math.Round(GetParent<PathFollow2D>().Progress).Equals(progress[i]))
			{
				Bullet bullet = BulletScene.Instantiate<Bullet>();
				bullet.Position = GlobalPosition;
				bullet.Spawner = this;
				
				if (dave.Position.X < GlobalPosition.X)
				{
					bullet.FlipH = true;
					bullet.velocity = new Vector2(-1, 0);
				}
				else if(dave.Position.X > GlobalPosition.X)
				{
					bullet.FlipH = false;
					bullet.velocity = new Vector2(1, 0);
				}
				
				main.AddChild(bullet);
			}
		}
	}

	public override void _Process(double delta)
	{
		Shoot();
	}
}
