using Godot;
using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Xml.Linq;
using GodotPlugins.Game;

public partial class Dave : CharacterBody2D
{
	private Vector2 velocity;
	public AnimatedSprite2D animatedSprite;
	private Area2D area2d;
	private TileMap tilemap;
	private Camera camera;
	private Main main;
	private AudioStreamPlayer audioStream;
	private AudioStreamPlayer audioPlayerSpecial;
	private bool hasTrophy;
	public int jetpack;
	private Vector2 lastCheckpoint;
	private bool hasGun;
	private int currentLevelFirstCameraPositionIndex;
	private bool isBulletPresent;
	private int isOnTreeCooldown = 0;
	public byte daves = 4;
	
	private PackedScene bulletScene = ResourceLoader.Load<PackedScene>("res://Scenes/bullet.tscn");
	
	private AudioStream jumpingSound = GD.Load<AudioStream>("res://Assest/sounds/jump.wav");
	private AudioStream walkingSound = GD.Load<AudioStream>("res://Assest/sounds/footstep.wav");
	private AudioStream climbingSound = GD.Load<AudioStream>("res://Assest/sounds/climbing.wav");
	private AudioStream jetpackSound = GD.Load<AudioStream>("res://Assest/sounds/jetpack_humming.wav");
	private AudioStream pickupSound = GD.Load<AudioStream>("res://Assest/sounds/pickup.wav");
	private AudioStream specialPickupSound = GD.Load<AudioStream>("res://Assest/sounds/special_pickup.wav");
	private AudioStream transitionSound = GD.Load<AudioStream>("res://Assest/sounds/TRANSITION.wav");
	private AudioStream deathSound = GD.Load<AudioStream>("res://Assest/sounds/death.wav");
	private AudioStream trophieSound = GD.Load<AudioStream>("res://Assest/sounds/trophie.wav");
	
	
	[Export]
	private int speed = 110;
	[Export]
	private float terminalYVelocity = 170;
	[Export]
	private float jump = 100;
	
	public bool isDoingLevelTransition;
	
	private Vector2I[] treeTiles =
	{
		new Vector2I(9, 3), new Vector2I(10, 3), new Vector2I(11, 3), new Vector2I(10, 4), 
		new Vector2I(11, 4), new Vector2I(12, 4), new Vector2I(4,5), new Vector2I(8,4), new Vector2I(9,4),
	};

	private enum MotionState
	{
		ONGROUND,
		CLIMBING,
		FLYING,
		INAIR
	}

	private MotionState state;

	public override void _Ready()
	{
		velocity = Velocity;
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		area2d = GetNode<Area2D>("Area2D");
		tilemap = GetNode<TileMap>("/root/Node/Main/TileMap");
		camera = GetNode<Camera>("/root/Node/Main/Camera");
		main = GetParent<Main>();
		audioStream = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		audioPlayerSpecial = GetNode<AudioStreamPlayer>("AudioStreamPlayer2");
		hasTrophy = false;
		hasGun = false;
		jetpack = 0;
		isDoingLevelTransition = false;
		currentLevelFirstCameraPositionIndex = 0;
		isBulletPresent = false;
	}

	private void CheckForState()
	{
		if(IsOnFloor() && state != MotionState.FLYING) state = MotionState.ONGROUND;
		
		if(!IsOnFloor() && state == MotionState.ONGROUND) state = MotionState.INAIR;
		
		if (Input.IsActionJustPressed("deployjetpack") && jetpack != 0)
		{
			if (state == MotionState.FLYING)
				state = MotionState.INAIR;
			else
			{
				velocity = Vector2.Zero;
				state = MotionState.FLYING;
			}
		}
	}

	private void MoveOnGround()
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
		
		if(Input.IsActionPressed("jump"))
		{ 
			velocity.Y = -jump;
		}
		
		if (Input.IsActionJustPressed("shoot")&&hasGun)
		{
			if(!isBulletPresent)
				SpawnBullet();
		}
	}

	private void MoveWhileFlying()
	{
		if (jetpack == 0)
		{
			state = MotionState.INAIR;
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

		jetpack--;
	}

	private void MoveWhileClimbing()
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

		velocity.X = 0f;
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
	}

	private void MoveWhileInAir()
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

		if (IsOnCeilingOnly())
			velocity.Y = 0f;
		
		if (Input.IsActionJustPressed("shoot")&&hasGun)
		{
			if(!isBulletPresent)
				SpawnBullet();
		}
	}

	private void Move()
	{
		CheckForState();
		switch (state)
		{
			case MotionState.ONGROUND:
				MoveOnGround();
				break;
			case MotionState.CLIMBING:
				MoveWhileClimbing();
				break;
			case MotionState.FLYING:
				MoveWhileFlying();
				break;
			case MotionState.INAIR:
				MoveWhileInAir();
				break;
		}
	}

	private void PlayStreamInLoop(AudioStream stream)
	{
		if ((audioStream.Playing && audioStream.Stream == stream) || audioPlayerSpecial.Playing)
			return;
		audioStream.Stream = stream;
		audioStream.Play();
	}

	private void PlaySpecialStream(AudioStream stream)
	{
		audioStream.Stop();
		audioPlayerSpecial.Stream = stream;
		audioPlayerSpecial.Play();
	}

	private void PlayAnimations()
	{
		switch (state)
		{ 
			case MotionState.ONGROUND:
				if (velocity.X == 0)
				{
					animatedSprite.Play("IDLE");
					audioStream.Stop();
					break;
				}
				
				if (velocity.X < 0)
					animatedSprite.FlipH = true;
				else if (velocity.X > 0)
					animatedSprite.FlipH = false;
				
				animatedSprite.Play("WALK");
				
				PlayStreamInLoop(walkingSound);
				break;
			
			case MotionState.CLIMBING:
				if (velocity.X != 0 || velocity.Y != 0)
				{
					animatedSprite.Play("CLIMB");
					PlayStreamInLoop(climbingSound);
				}
				else
				{
					animatedSprite.Play("CLIMB_IDLE");
					audioStream.Stop();
				}
					
				break;
			
			case MotionState.FLYING:
				if (velocity.X < 0)
					animatedSprite.FlipH = true;
				else if (velocity.X > 0)
					animatedSprite.FlipH = false;
				
				animatedSprite.Play("JETPACK");
				PlayStreamInLoop(jetpackSound);
				break;
			
			case MotionState.INAIR:
				if (velocity.X < 0)
					animatedSprite.FlipH = true;
				else if (velocity.X > 0)
					animatedSprite.FlipH = false;
				
				animatedSprite.Play("JUMP");
				
				if(velocity.Y < 0)
					PlayStreamInLoop(jumpingSound);

				if (audioStream.Stream != jumpingSound)
					audioStream.Stop();
				
				break;
		}
	}
	
	private void Gravity(double delta)
	{
		if(velocity.Y<terminalYVelocity&&state == MotionState.INAIR) 
			velocity.Y += (float)delta * ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	}

	private void OnBodyShapeEntered(Rid bodyRid, Node2D body, int bodyShapeIndex, int localShapeIndex)
	{
		if(isDoingLevelTransition) return;
		Vector2I tileCoordinate = tilemap.GetCoordsForBodyRid(bodyRid);

		if (tilemap.GetCellAtlasCoords(0, tileCoordinate).Equals(new Vector2I(5, 0)))
		{
			TileData tileData = tilemap.GetCellTileData(0, tileCoordinate);
			OnProceedingToNextLevel((Vector2)tileData.GetCustomData("Teleport"));
			currentLevelFirstCameraPositionIndex = (int)tileData.GetCustomData("CameraIndex");
			return;
		}
		
		if(tilemap.GetCellAtlasCoords(0, tileCoordinate).Equals(new Vector2I(4, 1)))
		{
			hasTrophy = true;
			main.UpdateTrophieStatus();
			PlaySpecialStream(trophieSound);
			tilemap.SetCell(0, tileCoordinate);
			return;
		}

		if (tilemap.GetCellAtlasCoords(0, tileCoordinate).Equals(new Vector2I(7, 0)))
		{
			jetpack = 2000;
			main.UpdateJetpackStatus(true);
			PlaySpecialStream(specialPickupSound);
			tilemap.SetCell(0, tileCoordinate);
			return;
		}

		if (tilemap.GetCellAtlasCoords(0, tileCoordinate).Equals(new Vector2I(5, 2)))
		{
			hasGun = true;
			main.UpdateGunStatus(hasGun);
			PlaySpecialStream(specialPickupSound);
			tilemap.SetCell(0, tileCoordinate);
			return;
		}

		if (tilemap.GetCellAtlasCoords(0, tileCoordinate).Equals(new Vector2I(10, 2)))
		{
			Teleport((Vector2)tilemap.GetCellTileData(0, tileCoordinate).GetCustomData("Teleport"));
			return;
		}
		
		if(tilemap.GetCellAtlasCoords(0, tileCoordinate).Equals(new Vector2I(0, 0)) || tilemap.GetCellAtlasCoords(0, tileCoordinate).Equals(new Vector2I(9, 0)))
		{
			camera.CompleteLevelTransition(currentLevelFirstCameraPositionIndex);
			OnDamage();
			return;
		}
		
		CheckForCollectibles(tileCoordinate);
	}

	private void OnBodyShapeEnteredForTree(Rid bodyRid, Node2D body, int bodyShapeIndex, int localShapeIndex)
	{
		if(isDoingLevelTransition) return;
		Vector2I tileCoordinate = tilemap.GetCoordsForBodyRid(bodyRid);
		
		if (treeTiles.Contains(tilemap.GetCellAtlasCoords(0, tileCoordinate)))
		{
			if (state == MotionState.ONGROUND || state == MotionState.FLYING) return;
			state = MotionState.CLIMBING;
		}
	}

	private void OnBodyShapeExitedForTree(Rid bodyRid, Node2D body, int bodyShapeIndex, int localShapeIndex)
	{
		Vector2I tileCoordinate = tilemap.GetCoordsForBodyRid(bodyRid);
		
		if (treeTiles.Contains(tilemap.GetCellAtlasCoords(0, tileCoordinate)))
		{
			if (state == MotionState.ONGROUND || state == MotionState.FLYING) return;
			state = MotionState.INAIR;
			velocity.Y -= 10;
		}
	}

	private void CheckForCollectibles(Vector2I coordinate)
	{
		Vector2I tileAtlasCoords = tilemap.GetCellAtlasCoords(0, coordinate);
		if (!(tileAtlasCoords.Y == 5 && tileAtlasCoords.X >= 5)) return;
		
		main.UpdateScore((int)tilemap.GetCellTileData(0, coordinate).GetCustomData("Score"));
		PlaySpecialStream(pickupSound);
		tilemap.SetCell(0, coordinate);
	}

	private void DoLevelTransition()
	{
		if (!isDoingLevelTransition) return;

		camera.DoLevelTransition();
		
		animatedSprite.FlipH = false;
		animatedSprite.Play("WALK");
		
		Position = Position.MoveToward(new Vector2(321, -184), 0.78f);
		
		if(Position.Equals(new Vector2(321, -184)))
		{
			isDoingLevelTransition = false;
			camera.CompleteLevelTransition(currentLevelFirstCameraPositionIndex);
			Position = lastCheckpoint;
			main.PauseGame();
		}
	}
	
	private void OnProceedingToNextLevel(Vector2 teleportCoordinate)
	{
		if(hasTrophy)
		{
			PlaySpecialStream(transitionSound);
			isDoingLevelTransition = true;
			lastCheckpoint = teleportCoordinate;
			hasTrophy = false;
			jetpack = 0;
			hasGun = false;
			velocity = Vector2.Zero;
			main.UpdateLevel();
			Position = new Vector2(27, -184);
		}
	}
	
	private void Teleport(Vector2 teleportCoordinate)
	{
		Position = teleportCoordinate;
	}

	public void OnDamage()
	{
		daves--;
		PlaySpecialStream(deathSound);
		if (daves != 0)
		{
			Position = lastCheckpoint;
			main.OnDaveDamage();
			main.PauseGame();
		}
		else
			main.OnDaveDeath();
	}

	private void SpawnBullet()
	{
		Bullet bullet = bulletScene.Instantiate<Bullet>();
		bullet.Position = animatedSprite.GlobalPosition;
		bullet.FlipH = animatedSprite.FlipH;
		bullet.velocity = animatedSprite.FlipH ? new Vector2(-1, 0) : new Vector2(1, 0);
		bullet.Spawner = this;
		isBulletPresent = true;
		bullet.ChildExitingTree += OnBulletDestruction;
		
		main.AddChild(bullet);
	}

	private void OnBulletDestruction(Node node)
	{
		isBulletPresent = false;
	}

	public override void _PhysicsProcess(double delta)
	{
		if(!isDoingLevelTransition)
		{
			Move();
			PlayAnimations();
			Gravity(delta);	
		}
		DoLevelTransition();
		
		MoveAndSlide();
		Velocity = velocity;
	}
}
