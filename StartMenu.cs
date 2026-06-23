using Godot;
using System;

public partial class StartMenu : Control
{

	[Export] 
	public PackedScene GameScene;
	private LineEdit _nameInput;

	public override void _Ready()
	{
		_nameInput = GetNode<LineEdit>("NameInput");
		GetNode<Button>("StartButton").Pressed += OnStartPressed;
	}
	private void OnStartPressed()
	{
		string playerName = _nameInput.Text;
		if (string.IsNullOrEmpty(playerName))
			playerName = "Player";

		GameData.PlayerName = playerName;
		GetTree().ChangeSceneToPacked(GameScene);
    }
	
	public override void _Process(double delta)
	{
	}
}
