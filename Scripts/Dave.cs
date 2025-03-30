using Godot;
using System;
using System.Data.Common;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Xml.Linq;
using GodotPlugins.Game;

public partial class Dave : CharacterBody2D
{
	private Vector2 velocity;
	private AnimatedSprite2D animatedSprite;
	private Area2D area2d;
	private TileMap tilemap;
	private int score;
	private bool hasTrophy;
	private bool isNotWalking;
	private int jetpack;
	private Vector2 lastCheckpoint;
	private bool isOnTree;
	private bool hasGun;
	
	private PackedScene bulletScene = ResourceLoader.Load<PackedScene>("res://Scenes/bullet.tscn");
	
	[Export]
	private int speed = 110;
	[Export]
	private float terminalYVelocity = 170;
	[Export]
	private float jump = 100;
	
	private Vector2I[] treeTiles =
	{
		new Vector2I(9, 3), new Vector2I(10, 3), new Vector2I(11, 3), new Vector2I(10, 4), 
		new Vector2I(11, 4), new Vector2I(12, 4), new Vector2I(4,5)
	};
	
	public override void _Ready()
	{
		velocity = Velocity;
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		area2d = GetNode<Area2D>("Area2D");
		tilemap = GetNode<TileMap>("/root/Main/TileMap");
		score = 0;
		hasTrophy = false;
		isNotWalking = false;
		isOnTree = false;
		hasGun = false;

		Engine.MaxFps = 60;
	}

	private void Move()
	{
		if (Input.IsActionJustPressed("deployjetpack")&&jetpack!=0&&!isOnTree)
		{
			velocity = Vector2.Zero;
			isNotWalking = !isNotWalking;
		}
		
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

		if (isOnTree)
		{
			velocity.Y = 0f;
			if (Input.IsActionPressed("jump"))
			{
				velocity.Y = -speed;
			}else if (Input.IsActionPressed("down"))
			{
				velocity.Y = speed;
			}else if (Input.IsActionJustReleased("jump") || Input.IsActionJustReleased("down"))
			{
				velocity.Y = 0;
			}
		}

		if (isNotWalking)
		{
			if (jetpack == 0)
			{
				isNotWalking = false;
				return;
			}
			
			if (Input.IsActionJustPressed("jump"))
			{
				velocity.Y = -speed;
			}else if (Input.IsActionJustPressed("down"))
			{
				velocity.Y = speed;
			}else if (Input.IsActionJustReleased("jump") || Input.IsActionJustReleased("down"))
			{
				velocity.Y = 0;
			}

			jetpack--;
		}
		
		if(isOnTree||isNotWalking) return;
		
		if(Input.IsActionPressed("jump")&&IsOnFloor())
		{ 
			velocity.Y = -jump;
		}
		if(IsOnCeilingOnly())
		{
			velocity.Y = 0;
		}

		if (Input.IsActionJustPressed("shoot")&&hasGun)
		{
			SpawnBullet();
		}
	}
	
	private void Gravity(double delta)
	{
		if(!IsOnFloor()&&velocity.Y<terminalYVelocity&&!isNotWalking&&!isOnTree) 
			velocity.Y += (float)delta * ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	}

	private void OnBodyShapeEntered(Rid bodyRid, Node2D body, int bodyShapeIndex, int localShapeIndex)
	{
		Vector2I tileCoordinate = tilemap.GetCoordsForBodyRid(bodyRid);

		if (tilemap.GetCellAtlasCoords(0, tileCoordinate).Equals(new Vector2I(5, 0)))
		{
			OnProceedingToNextLevel((Vector2)tilemap.GetCellTileData(0,tileCoordinate).GetCustomData("Teleport"));
			return;
		}
		
		if(tilemap.GetCellAtlasCoords(0, tileCoordinate).Equals(new Vector2I(4, 1)))
		{
			hasTrophy = true;
			tilemap.SetCell(0, tileCoordinate);
			return;
		}

		if (tilemap.GetCellAtlasCoords(0, tileCoordinate).Equals(new Vector2I(7, 0)))
		{
			jetpack = 2000;
			tilemap.SetCell(0, tileCoordinate);
			return;
		}

		if (treeTiles.Contains(tilemap.GetCellAtlasCoords(0, tileCoordinate)))
		{
			if (IsOnFloor()) return;
			isOnTree = true;
			return;
		}

		if (tilemap.GetCellAtlasCoords(0, tileCoordinate).Equals(new Vector2I(5, 2)))
		{
			hasGun = true;
			tilemap.SetCell(0, tileCoordinate);
			return;
		}
		
		CheckForCollectibles(tileCoordinate);
	}

	private void OnBodyShapeExited(Rid bodyRid, Node2D body, int bodyShapeIndex, int localShapeIndex)
	{
		Vector2I tileCoordinate = tilemap.GetCoordsForBodyRid(bodyRid);
		
		if (treeTiles.Contains(tilemap.GetCellAtlasCoords(0, tileCoordinate)))
		{
			isOnTree = false;
		}
	}

	private void CheckForCollectibles(Vector2I coordinate)
	{
		Vector2I tileAtlasCoords = tilemap.GetCellAtlasCoords(0, coordinate);
		if (!(tileAtlasCoords.Y == 5 && tileAtlasCoords.X >= 5)) return;
		
		score += (int)tilemap.GetCellTileData(0, coordinate).GetCustomData("Score");
		tilemap.SetCell(0, coordinate);
	}

	private void OnProceedingToNextLevel(Vector2 teleportCoordinate)
	{
		if(hasTrophy)
		{
			Position = teleportCoordinate;
			lastCheckpoint = teleportCoordinate;
			hasTrophy = false;
		}
	}

	public void OnDamage()
	{
		Position = lastCheckpoint;
	}

	private void SpawnBullet()
	{
		Bullet bullet = bulletScene.Instantiate<Bullet>();
		bullet.Position = animatedSprite.GlobalPosition;
		bullet.Spawner = this;
		GetParent().AddChild(bullet);
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
