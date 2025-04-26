using Godot;
using System;

public partial class Main : Node2D
{
	private Label scoreLabel;
	private Label levelLabel;
	private TextureRect gunRect;
	private TextureRect jetpackRect;
	private TextureRect goThruDoor;
	private TextureProgressBar jetpackBar;
	private Dave dave;
	
	private int score;
	private byte currentLevel;
	public override void _Ready()
	{
		Engine.MaxFps = 60;
		scoreLabel = GetNode<Label>("/root/Node/CanvasLayer/Control/ScoreLabel");
		levelLabel = GetNode<Label>("/root/Node/CanvasLayer/Control/LevelLabel");
		gunRect = GetNode<TextureRect>("/root/Node/CanvasLayer/Control/Gun");
		jetpackRect = GetNode<TextureRect>("/root/Node/CanvasLayer/Control/Jetpack");
		jetpackBar = GetNode<TextureProgressBar>("/root/Node/CanvasLayer/Control/JetpackBar");
		goThruDoor = GetNode<TextureRect>("/root/Node/CanvasLayer/Control/GoThruDoor");
		dave = GetNode<Dave>("/root/Node/Main/Dave");
		gunRect.Visible = false;
		jetpackRect.Visible = false;
		goThruDoor.Visible = false;
		jetpackBar.Visible = false;
	}

	public void PauseGame()
	{
		Engine.TimeScale = 0.0;
	}
	
	public void ResumeGame()
	{
		Engine.TimeScale = 1.0;
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
	
	public override void _Process(double delta)
	{
		UpdateJetpackProgress();
	}
}
