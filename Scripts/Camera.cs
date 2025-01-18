using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;

public partial class Camera : Camera2D
{
	private Dave dave;
	
	[Export]
	private Vector2[] positions = Array.Empty<Vector2>();
	
	int cameraTransitionSpeed = 20;
	int currentCameraPositionIndex = 0;
	bool isCameraTransitioning = false;
	
	public override void _Ready()
	{
		dave = GetNode<Dave>("/root/Main/Dave");
	}
	
	private void UpdateCameraPosition()
	{
		if(isCameraTransitioning) return;
		
		float nextCameraPositionXMidPoint;
		float backCameraPositionXMidPoint;
		
		if(currentCameraPositionIndex!=positions.Length-1)
			nextCameraPositionXMidPoint =
				(positions[currentCameraPositionIndex].X + positions[currentCameraPositionIndex + 1].X)/2;
		else
			nextCameraPositionXMidPoint = dave.Position.X + 1000;

		if (currentCameraPositionIndex != 0)
			backCameraPositionXMidPoint =
				(positions[currentCameraPositionIndex].X + positions[currentCameraPositionIndex - 1].X)/2;
		else
			backCameraPositionXMidPoint = dave.Position.X - 1000;

		if (dave.Position.X > nextCameraPositionXMidPoint)
		{
			//Position = positions[currentCameraPositionIndex+1];
			currentCameraPositionIndex++;
			isCameraTransitioning = true;
		}
		else if(dave.Position.X < backCameraPositionXMidPoint)
		{
			//Position = positions[currentCameraPositionIndex - 1];
			currentCameraPositionIndex--;
			isCameraTransitioning = true;
		}
	}

	private void DoCameraTransition()
	{
		if(!isCameraTransitioning) return;
		
		Position = Position.MoveToward(positions[currentCameraPositionIndex], cameraTransitionSpeed);
		
		if(Position.Equals(positions[currentCameraPositionIndex]))
			isCameraTransitioning = false;
	}
	
	public override void _Process(double delta)
	{
		UpdateCameraPosition();
		DoCameraTransition();
	}
}
