using Godot;
using System;

public partial class Player : CharacterBody2D
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
		if (GetGlobalTransformWithCanvas().Origin.DistanceTo(nav.TargetPosition) > 10) {
			Vector2 direction = nav.GetNextPathPosition() - GetGlobalTransformWithCanvas().Origin;
			direction = direction.Normalized();
			Velocity = Velocity.Lerp(direction * speed, (float)(accel * delta));
			nav.Velocity = Velocity;
		} else {
			nav.Velocity = new Vector2(0, 0);
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton eventMouseButton)
		{
			if (eventMouseButton.IsReleased()) {
				nav.TargetPosition = eventMouseButton.Position;
			}
		}
	}

	private void Move(Vector2 navVelocity)
	{
		Velocity = navVelocity;
		MoveAndSlide();
		if (Velocity.X != 0 || Velocity.Y != 0) {
			Rotation = Velocity.Angle();
		}
	}
}
