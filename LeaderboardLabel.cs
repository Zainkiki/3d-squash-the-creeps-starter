using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

public partial class LeaderboardLabel : Label
{
	private HttpRequest _httpRequest;
    public override void _Ready()
    {
        Visible = false;

        _httpRequest = new HttpRequest();
        AddChild(_httpRequest);

        _httpRequest.RequestCompleted += OnRequestCompleted;
    }
	
	public void ShowLeaderboard()
	{
		Visible = true;

		var error = _httpRequest.Request($"http://{GameData.LeaderboardIP}/leaderboard");
        if (error != Error.Ok)
		{
			Text = $"Request failed: {error}";
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

            leaderboard.Sort((a, b) => b.score.CompareTo(a.score));

            string displayText = "Top 5\n\n";

            int count = Math.Min(5, leaderboard.Count);

            for (int i = 0; i < count; i++)
            {
                displayText += $"{i + 1}. {leaderboard[i].playerName} - {leaderboard[i].score}\n";
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
