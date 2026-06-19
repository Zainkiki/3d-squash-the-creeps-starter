using Godot;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

public partial class Player : CharacterBody3D
{
    [Signal]
    public delegate void HitEventHandler();

    [Export]
    public int Speed { get; set; } = 14;
    [Export]
    public int FallAcceleration { get; set; } = 75;
    [Export]
    public int JumpImpulse { get; set; } = 20;
    [Export]
    public int BounceImpulse { get; set; } = 16;

    private Vector3 _targetVelocity = Vector3.Zero;

    public override void _PhysicsProcess(double delta)
    {
        var direction = Vector3.Zero;

        if (Input.IsActionPressed("move_right"))
        {
            direction.X += 1.0f;
        }
        if (Input.IsActionPressed("move_left"))
        {
            direction.X -= 1.0f;
        }
        if (Input.IsActionPressed("move_back"))
        {
            direction.Z += 1.0f;
        }
        if (Input.IsActionPressed("move_forward"))
        {
            direction.Z -= 1.0f;
        }

        if (direction != Vector3.Zero)
        {
            direction = direction.Normalized();
            GetNode<Node3D>("Pivot").Basis = Basis.LookingAt(direction);
            GetNode<AnimationPlayer>("AnimationPlayer").SpeedScale = 4;
        }
        else
        {
            GetNode<AnimationPlayer>("AnimationPlayer").SpeedScale = 1;
        }


        _targetVelocity.X = direction.X * Speed;
        _targetVelocity.Z = direction.Z * Speed;

        if (!IsOnFloor()) 
        {
            _targetVelocity.Y -= FallAcceleration * (float)delta;
        }

        if (IsOnFloor() && Input.IsActionJustPressed("jump"))
        {
            _targetVelocity.Y = JumpImpulse;
        }

        for (int index = 0; index < GetSlideCollisionCount(); index++)
        {
            KinematicCollision3D collision = GetSlideCollision(index);

            if (collision.GetCollider() is Mob mob)
            {
                if (Vector3.Up.Dot(collision.GetNormal()) > 0.1f)
                {
                    mob.Squash();
                    _targetVelocity.Y = BounceImpulse;
                    break;
                }
            }
        }

        Velocity = _targetVelocity;
        MoveAndSlide();

        var pivot = GetNode<Node3D>("Pivot");
        pivot.Rotation = new Vector3(Mathf.Pi / 6.0f * Velocity.Y / JumpImpulse, pivot.Rotation.Y, pivot.Rotation.Z);
    }
     private async void SendScore(int score)
     {
        var http = new HttpRequest();
        GetParent().AddChild(http);

        var data = new Godot.Collections.Dictionary
        {
            {"playerName", "Player1" },
            {"score", score }
        };
        string json = Json.Stringify(data);
        GD.Print(json);
        Godot.Error error = http.Request(
            "http://localhost:5049/score",
            new string[] { "Content-Type: application/json" }, 
            HttpClient.Method.Post,
            json
        );
        if (error != Godot.Error.Ok)
        {
            GD.PrintErr($"Request failed immediately: {error}");
            http.QueueFree();
            return; // Don't await if request failed
        }
        await ToSignal(http, "request_completed");
        QueueFree();

    }
    private void Die()
 {
     EmitSignal(SignalName.Hit);

     int finalScore =
         GetNode<ScoreLabel>("../UserInterface/ScoreLabel").GetScore();

     SendScore(finalScore);

 }
    
private void OnMobDetectorBodyEntered(Node3D body)
{

    Die();
}
}