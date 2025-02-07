using Godot;
using System;

public partial class Health : Node2D
{
	[Export]
	private int lives = 1;
	
	public override void _Ready()
	{
		
	}

	public void Damage()
	{
		lives--;
		if(lives == 0)
			GetParent().QueueFree();
		GetParent().Call("OnDamage");
	}
	
	public override void _Process(double delta)
	{
		
	}
}
