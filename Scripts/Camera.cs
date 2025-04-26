using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

public partial class Camera : Camera2D
{
	private Dave dave;
	
	[Export]
	public Vector2[] positions = Array.Empty<Vector2>();
	[Export]
	public Vector2 TransitionLevelPosition = new Vector2(160, -192);
	
	[Export]
	int cameraTransitionSpeed = 8;
	private int currentCameraPositionIndex;
	private bool isCameraTransitioning;
	private bool isLevelTransitioning;
	
	public override void _Ready()
	{
		dave = GetNode<Dave>("/root/Node/Main/Dave");
		ProcessMode = ProcessModeEnum.Always;
		currentCameraPositionIndex = 0;
		isCameraTransitioning = false;
		isLevelTransitioning = false;
	}
	
	private void UpdateCameraPosition()
	{
		if(isCameraTransitioning||isLevelTransitioning) return;
		
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
			currentCameraPositionIndex++;
			isCameraTransitioning = true;
		}
		else if(dave.Position.X < backCameraPositionXMidPoint)
		{
			currentCameraPositionIndex--;
			isCameraTransitioning = true;
		}
	}

	public void DoLevelTransition()
	{
		isLevelTransitioning = true;
		Position = TransitionLevelPosition;
	}

	public void CompleteLevelTransition(int levelFirstCameraPositionIndex)
	{
		currentCameraPositionIndex = levelFirstCameraPositionIndex;
		Position = positions[levelFirstCameraPositionIndex];
		isLevelTransitioning = false;
	}
	
	private void DoCameraTransition()
	{
		if(!isCameraTransitioning||isLevelTransitioning) return;
		
		Position = Position.MoveToward(positions[currentCameraPositionIndex], cameraTransitionSpeed);
		
		if(Position.DistanceTo(positions[currentCameraPositionIndex])<0.5f)
			isCameraTransitioning = false;
	}
	
	public override void _Process(double delta)
	{
		UpdateCameraPosition();
		DoCameraTransition();
	}
}
