using Godot;
using System;
using Microsoft.VisualBasic;

public partial class Enemy : Area2D
{
	[Export] private int type = 1;
	AnimatedSprite2D sprite;
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		sprite.Play(type.ToString());
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.Name == "Dave")
		{
			body.GetNode("Health").Call("Damage");
			QueueFree();
		}
	}

	public override void _Process(double delta)
	{
		
	}
}
