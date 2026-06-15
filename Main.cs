using Godot;

public partial class Main : Node
{
    [Export]
    public PackedScene MobScene { get; set; }

    private void OnMobTimerTimeout()
    {
        Mob mob = MobScene.Instantiate<Mob>();

        var mobSpawnLocation = GetNode<PathFollow3D>("SpawnPath/SpawnLocation");
        mobSpawnLocation.ProgressRatio = GD.Randf();

        Vector3 playerPosition = GetNode<Player>("Player").Position;
        mob.Initialize(mobSpawnLocation.Position, playerPosition);

        AddChild(mob);
    }

    private void OnPlayerHit()
    {
        GetNode<Timer>("MobTimer").Stop();
    }
}