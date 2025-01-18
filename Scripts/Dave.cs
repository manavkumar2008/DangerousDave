using Godot;
using System;
using System.Data.Common;
using System.Runtime.Intrinsics.X86;
using System.Xml.Linq;

public partial class Dave : CharacterBody2D
{
	private Vector2 velocity;
	private AnimatedSprite2D animatedSprite;
	private Area2D area2d;
	private TileMap tilemap;
	
	[Export]
	private int speed = 110;
	[Export]
	private float terminalYVelocity = 170;
	[Export]
	private float jump = 100;
	
	public override void _Ready()
	{
		velocity = Velocity;
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		area2d = GetNode<Area2D>("Area2D");
		tilemap = GetNode<TileMap>("/root/Main/TileMap");
	}

	private void Move()
	{
		if(Input.IsActionPressed("left"))
		{
			velocity.X = -speed;
		}else if(Input.IsActionPressed("right"))
		{
			velocity.X = speed;
		}else if(Input.IsActionJustReleased("left") || Input.IsActionJustReleased("right"))
		{
			velocity.X = 0;
		}
		
		if(Input.IsActionPressed("jump")&&IsOnFloor())
		{ 
			velocity.Y = -jump;
		}
		if(IsOnCeilingOnly())
		{
			velocity.Y = 0;
		}
	}

	private void Gravity(double delta)
	{
		if(!IsOnFloor()&&velocity.Y<terminalYVelocity) 
			velocity.Y += (float)delta * ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area == null)
		{
			GD.PrintErr("nothing is here");
			return;
		}
		
		GD.Print("Area2D Global Position: ", area.GlobalPosition);
		
		Vector2 localPos = tilemap.ToLocal(area.GlobalPosition);
		GD.Print("Local Position: ", localPos);
		
		Vector2I tilePos = tilemap.LocalToMap(localPos);
		GD.Print("Tile Position: ", tilePos);
	}

	private void OnProceedingToNextLevel(Vector2 position)
	{
		Position = position;
	}

	private void PlayAnimations()
	{
		if(Input.IsActionPressed("left"))
		{
			animatedSprite.FlipH = true; 
		}else if(Input.IsActionPressed("right"))
		{
			animatedSprite.FlipH = false;
		}

		string animation = "IDLE";

		if((Input.IsActionPressed("left")||Input.IsActionPressed("right"))&&IsOnFloor())
		{
			animation = "WALK";
		}

		if(Input.IsActionPressed("jump")||!IsOnFloor())
		{
			animation = "JUMP";
		}

		animatedSprite.Play(animation);
	}

	public override void _PhysicsProcess(double delta)
	{
		Move();
		Gravity(delta);	

		PlayAnimations();
		
		MoveAndSlide();
		Velocity = velocity;
	}
}
