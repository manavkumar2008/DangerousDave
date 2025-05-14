using Godot;
using System;

public partial class Main : Node2D
{
	private Label scoreLabel;
	private Label levelLabel;
	private TextureRect gunRect;
	private TextureRect jetpackRect;
	private TextureRect goThruDoor;
	private TextureRect levelTexture;
	private TextureRect davesTexture;
	private TextureRect davesHeadTexture1;
	private TextureRect davesHeadTexture2;
	private TextureRect davesHeadTexture3;
	private TextureProgressBar jetpackBar;
	private TextureRect scoreTexture;
	private Label endingScoreLabel;
	private Panel deathScreenPanel;
	private Dave dave;
	private AudioStreamPlayer audioPlayer;
	private int score;
	private byte currentLevel;
	private ulong blinkTimer;
	
	public override void _Ready()
	{
		Engine.MaxFps = 60;
		scoreLabel = GetNode<Label>("/root/Node/CanvasLayer/Control/ScoreLabel");
		levelLabel = GetNode<Label>("/root/Node/CanvasLayer/Control/LevelLabel");
		gunRect = GetNode<TextureRect>("/root/Node/CanvasLayer/Control/Gun");
		jetpackRect = GetNode<TextureRect>("/root/Node/CanvasLayer/Control/Jetpack");
		jetpackBar = GetNode<TextureProgressBar>("/root/Node/CanvasLayer/Control/JetpackBar");
		goThruDoor = GetNode<TextureRect>("/root/Node/CanvasLayer/Control/GoThruDoor");
		levelTexture = GetNode<TextureRect>("/root/Node/CanvasLayer/Control/TextureRect2");
		scoreTexture = GetNode<TextureRect>("/root/Node/CanvasLayer/Control/TextureRect");
		davesTexture = GetNode<TextureRect>("/root/Node/CanvasLayer/Control/TextureRect3");
		davesHeadTexture1 = GetNode<TextureRect>("/root/Node/CanvasLayer/Control/TextureRect4");
		davesHeadTexture2 = GetNode<TextureRect>("/root/Node/CanvasLayer/Control/TextureRect5");
		davesHeadTexture3 = GetNode<TextureRect>("/root/Node/CanvasLayer/Control/TextureRect6");
		endingScoreLabel = GetNode<Label>("/root/Node/Main/Control/ScoreContainer/Score");
		dave = GetNode<Dave>("/root/Node/Main/Dave");
		audioPlayer = GetNode<AudioStreamPlayer>("/root/Node/Main/AudioStreamPlayer");
		deathScreenPanel = GetNode<Panel>("/root/Node/CanvasLayer/Control/Panel");
		gunRect.Visible = false;
		jetpackRect.Visible = false;
		goThruDoor.Visible = false;
		jetpackBar.Visible = false;
		ProcessMode = ProcessModeEnum.Always;
		blinkTimer = Time.GetTicksMsec();
		currentLevel = 0;
	}

	public void PauseGame()
	{
		if (currentLevel == 11)
		{
			dave.SetProcess(false);
			dave.Visible = false;
		}
		Engine.TimeScale = 0.0;
		dave.SetPhysicsProcess(false);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (Engine.TimeScale != 0.0) return;
		if (@event is InputEventKey key)
			ResumeGame();
	}
	
	public void ResumeGame()
	{
		Engine.TimeScale = 1.0;
		dave.Visible = true;
		dave.SetPhysicsProcess(true);
	}

	private void PlayerBlink(double deltaTime)
	{
		if(Engine.TimeScale != 0.0) return;
		
		ulong now = Time.GetTicksMsec();
		if (now - blinkTimer >= 500)
		{
			dave.Visible = !dave.Visible;
			dave.animatedSprite.Play("STAND");
			blinkTimer = now;
		}
	}

	public void OnDaveDamage()
	{
		switch (dave.daves)
		{
			case 3:
				davesHeadTexture3.Visible = false;
				break;
			case 2:
				davesHeadTexture2.Visible = false;
				break;
			case 1:
				davesHeadTexture1.Visible = false;
				break;
		}
	}

	public void OnDaveDeath()
	{
		dave.SetPhysicsProcess(false);
		deathScreenPanel.Visible = true;
	}
	
	public void UpdateGunStatus(bool status = true)
	{
		gunRect.Visible = status;
	}

	public void UpdateJetpackStatus(bool status = true)
	{
		jetpackRect.Visible = status;
		jetpackBar.Visible = status;
		jetpackBar.Value = dave.jetpack;
	}

	public void UpdateTrophieStatus()
	{
		goThruDoor.Visible = true;
	}
	
	public void UpdateLevel()
	{
		if (currentLevel == 10)
		{
			currentLevel += 1;
			gunRect.Visible = false;
			jetpackRect.Visible = false;
			jetpackBar.Visible = false;
			goThruDoor.Visible = false;
			levelLabel.Visible = false;
			scoreLabel.Visible = false;
			levelTexture.Visible = false;
			scoreTexture.Visible = false;
			davesTexture.Visible = false;
			davesHeadTexture1.Visible = false;
			davesHeadTexture2.Visible = false;
			davesHeadTexture3.Visible = false;
			endingScoreLabel.Text = scoreLabel.Text;
			return;
		}
		currentLevel += 1;
		levelLabel.Text = currentLevel.ToString();
		gunRect.Visible = false;
		jetpackRect.Visible = false;
		jetpackBar.Visible = false;
		goThruDoor.Visible = false;
	}

	public void UpdateScore(int amount)
	{
		score += amount;
		string zeros = "00000";
		int tempScore = score;
		while (tempScore > 0)
		{
			zeros = zeros.Remove(zeros.Length - 1);
			tempScore = tempScore / 10;
		}
		
		scoreLabel.Text = zeros + score;
	}

	public void UpdateJetpackProgress()
	{
		if (!jetpackBar.Visible) return;

		jetpackBar.Value = dave.jetpack;
	}

	public void OnBulletHit()
	{
		audioPlayer.Play();
	}

	private void OnRestartButtonPressed()
	{
		GetTree().ReloadCurrentScene();
	}

	private void OnMainMenuButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/main_menu.tscn");
	}
	
	public override void _Process(double delta)
	{
		UpdateJetpackProgress();
		PlayerBlink(delta);
	}
}
