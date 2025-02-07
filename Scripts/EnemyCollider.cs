using Godot;
using System;

public partial class EnemyCollider : Area2D
{
	
	public override void _Ready()
	{
		
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.Name == "Dave")
		{
			body.Call("Damage");
			GetParent().QueueFree();
		}
	}
	
	public override void _Process(double delta)
	{
		
	}
}
