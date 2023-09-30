using Godot;
using System;

public partial class movement : CharacterBody2D
{
	private NavigationAgent2D nav;
	int speed = 300;
	float accel = 7.0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		nav = GetNode<NavigationAgent2D>("PlayerNav");
		nav.VelocityComputed += Move;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (GlobalPosition.DistanceTo(nav.TargetPosition) > 10) {
			var direction = new Vector2();
			direction = nav.GetNextPathPosition() - GlobalPosition;
			direction = direction.Normalized();
			Velocity = Velocity.Lerp(direction * speed, (float)(accel * delta));
			nav.Velocity = Velocity;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton eventMouseButton)
		{
			if (eventMouseButton.IsReleased()) {
				GD.Print("Mousce click at: ", eventMouseButton.Position);
				nav.TargetPosition = eventMouseButton.Position;
			}
		}
	}

	private void Move(Vector2 navVelocity)
	{
		Velocity = navVelocity;
		MoveAndSlide();
		Rotation = Velocity.Angle();
	}
}
