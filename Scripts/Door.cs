using Godot;
using System;

public partial class Door : Sprite2D
{
	[Signal]
	public delegate void ProceedToNextLevelEventHandler(Vector2 position);
	[Export]
	public Vector2 nextLevelPosition;
	
	public override void _Ready()
	{
		
	}

	private void OnBodyEntered(Node body)
	{
		if (body.Name == "Dave")
			EmitSignal(SignalName.ProceedToNextLevel, nextLevelPosition);
	}
	
}
