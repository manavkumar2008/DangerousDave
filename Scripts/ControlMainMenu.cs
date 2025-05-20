using Godot;

public partial class ControlMainMenu : Control
{
	private Panel controlsPanel;
	public override void _Ready()
	{
		controlsPanel = GetNode<Panel>("Panel");
		controlsPanel.Visible = false;
	}

	private void Pressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/main.tscn");
	}

	private void PresssedExit()
	{
		GetTree().Quit();
	}

	private void PressedControls()
	{
		controlsPanel.Visible = true;
	}

	private void PressedClosePanel()
	{
		controlsPanel.Visible = false;
	}
	
	public override void _Process(double delta)
	{
		
	}
}
