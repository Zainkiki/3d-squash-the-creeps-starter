using Godot;
using System;

public partial class StartMenu : Control
{

	[Export] 
	public PackedScene GameScene;
	private LineEdit _nameInput;
	private LineEdit _leaderboardIPInput;


	public override void _Ready()
	{
		_nameInput = GetNode<LineEdit>("NameInput");
		_leaderboardIPInput = GetNode<LineEdit>("LeaderboardIPInput");
		GetNode<Button>("StartButton").Pressed += OnStartPressed;
	}
	private void OnStartPressed()
	{
		string playerName = _nameInput.Text;
		if (string.IsNullOrEmpty(playerName))
			playerName = "Player";

		GameData.PlayerName = playerName;

		string leaderboardIP = _leaderboardIPInput.Text;
		if (leaderboardIP == "")
			leaderboardIP = "localhost:5049";
		GameData.LeaderboardIP = leaderboardIP;
        GetTree().ChangeSceneToPacked(GameScene);

    }


    public override void _Process(double delta)
	{
	}
}
