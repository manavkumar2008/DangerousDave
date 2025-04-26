using Godot;
using System;

public partial class ControlMainMenu : Control
{
	
	public override void _Ready()
	{
		
	}

	private void Pressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/main.tscn");
	}

	private void PresssedExit()
	{
		GetTree().Quit();
	}
	
	public override void _Process(double delta)
	{
		
	}
}
