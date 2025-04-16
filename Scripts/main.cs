using Godot;
using System;

public partial class main : Node2D
{

	public void PauseGame()
	{
		Engine.TimeScale = 0.0;
	}
	
	public void ResumeGame()
	{
		Engine.TimeScale = 1.0;
	}

	public void DoLevelTransition()
	{
		
	}
	
	public override void _Ready()
	{
		Engine.MaxFps = 60;
	}
	
	public override void _Process(double delta)
	{
		
	}
}
