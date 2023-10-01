using Godot;
using System;

public partial class Stump : Node2D
{
    private Sprite2D StumpSprite;
    private Sprite2D CircleSprite;
    private Tween tween;
    private bool waitingForTween = true;
    public double secondsToKeep = 20;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        StumpSprite = GetNode<Sprite2D>("StumpSprite");
        CircleSprite = GetNode<Sprite2D>("CircleSprite");
        Color stumpColor = StumpSprite.Modulate;
        stumpColor.A = 0;

        Color circleHalfColor = CircleSprite.Modulate;
        circleHalfColor.A = 0.7f;

        Color circleColor = CircleSprite.Modulate;
        circleColor.A = 0;

        tween = GetTree().CreateTween();
        tween.Pause();
        tween.TweenProperty(CircleSprite, "scale", new Vector2(1.2f, 1.2f), 0.5f);
        tween.Parallel().TweenProperty(CircleSprite, "modulate", circleHalfColor, 0.5f);
        tween.TweenProperty(StumpSprite, "modulate", stumpColor, 2.0f);
        tween.Parallel().TweenProperty(CircleSprite, "modulate", circleColor, 1.0f);
        tween.TweenCallback(Callable.From(this.QueueFree));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _Process(double delta)
	{
        if (waitingForTween)
        {
            waitingForTween = false;
            await ToSignal(GetTree().CreateTimer(secondsToKeep), "timeout");
            CircleSprite.Visible = true;
            tween.Play();
        }
	}
}
