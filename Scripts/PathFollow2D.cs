using Godot;
using System;

public partial class PathFollow2D : Godot.PathFollow2D
{
	[Export] float speed = 10f;
	public override void _Ready()
	{

	}

	
	public override void _Process(double delta)
	{
		Progress += (float)delta*speed;
	}
}
