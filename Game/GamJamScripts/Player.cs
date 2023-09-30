using Game.Andrew;
using Godot;
using System;

namespace Game;

public partial class Player : CharacterBody2D
{
    private NavigationAgent2D nav;
    int speed = 300;
    float accel = 7.0f;

    bool chopping = false;
    int axeDamage = 1;
    float axeTime = 1.0f;

    public Game.Andrew.Tree targetTree;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Global().SetPlayer(this);
        nav = GetNode<NavigationAgent2D>("PlayerNav");
        nav.VelocityComputed += Move;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override async void _Process(double delta)
    {
        Vector2 pos = GetGlobalTransformWithCanvas().Origin;
        if (targetTree != null && pos.DistanceTo(targetTree.GetGlobalTransformWithCanvas().Origin) < 85) {
            nav.TargetPosition = pos;
            if (!chopping)
            {
                chopping = true;
                //start animation?
                await ToSignal(GetTree().CreateTimer(axeTime), "timeout");
                GD.Print("chop");
                if (targetTree.DoAHit(axeDamage)) {
                    GD.Print("tree felled!");
                   // targetTree.CallDeferred("queue_free"); //should eventually change to stump and stop spawning
                    targetTree = null;
                }
                chopping = false;
            }
        }

        if (pos.DistanceTo(nav.TargetPosition) > 10)
        {
            Vector2 direction = nav.GetNextPathPosition() - pos;
            direction = direction.Normalized();
            Velocity = Velocity.Lerp(direction * speed, (float)(accel * delta));
            nav.Velocity = Velocity;
        }
        else
        {
            nav.Velocity = new Vector2(0, 0);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton)
        {
            if (eventMouseButton.IsReleased())
            {
                targetTree = null;
                nav.TargetPosition = eventMouseButton.Position;
            }
        }
    }

    private void Move(Vector2 navVelocity)
    {
        Velocity = navVelocity;
        MoveAndSlide();
        if (Velocity.X != 0 || Velocity.Y != 0)
        {
            Rotation = Velocity.Angle();
        }
    }
}
