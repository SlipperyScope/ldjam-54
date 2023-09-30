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
	}
}
