using Godot;
using System;

public partial class Title : AnimatedSprite2D
{
	
	public override void _Ready()
	{
		
	}

	
	public override void _Process(double delta)
	{
		float a = (float)Math.Ceiling(Math.Sin(Time.GetTicksMsec())*0.001) <= 0
			? (float)(Math.Floor(Math.Sin(Time.GetTicksMsec() * 0.001)) * 0.15)
			: (float)(Math.Ceiling(Math.Sin(Time.GetTicksMsec() * 0.001)) * 0.15);
		Position += new Vector2(0, a);
	}
}
