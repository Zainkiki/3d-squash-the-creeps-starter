using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

public partial class LeaderboardLabel : Label
{
	private HttpRequest _httpRequest;
	public override void _Ready()
	{
		_httpRequest = new HttpRequest();
		AddChild(_httpRequest);

        _httpRequest.RequestCompleted += OnRequestCompleted;
		var error = _httpRequest.Request("http://localhost:5049/leaderboard");

		if (error != Error.Ok)
		{
			Text = $"Request faild: {error}";
		}

    }
	private void OnRequestCompleted(
		long result,
		long responseCode,
		string[] headers,
		byte[] body)
	{
		string json = System.Text.Encoding.UTF8.GetString(body);
		GD.Print(json);

		try
		{
			List<PlayerScore> leaderboard = JsonSerializer.Deserialize<List<PlayerScore>>(json);

			string displayText = "Leaderboard\n\n";

			foreach (var player in leaderboard)
			{
				displayText += $"{player.playerName}: {player.score}\n";
			}
			Text = displayText;
		}
		catch (Exception ex)
		{
			Text = $"JSON Error: {ex.Message}";
		}
	}

	public class PlayerScore
	{
		public string playerName { get; set; }
		public int score { get; set; }
	}
    public override void _Process(double delta)
	{
	}
}
