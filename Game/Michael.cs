using Game;
using Godot;
using System;
using System.Diagnostics;

public partial class Michael : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var pos = GetNode<Sprite2D>("Player").Position;
		Debug.WriteLine($"Coordinates (X: {pos.X}, Y: {pos.Y})");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GetNode<RichTextLabel>("GameTime").Text = $"Game Time: {this.Global().gameDuration}";
	}

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
		if (@event is InputEventKey keyEvent && keyEvent.Pressed) {
			if (keyEvent.Keycode == Key.S) this.Global().StartGame();
			if (keyEvent.Keycode == Key.E) this.Global().EndGame();
		}
    }
}
