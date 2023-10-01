using Godot;
using System;

namespace Game;

public partial class Ocean : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
    {
        base._InputEvent(viewport, @event, shapeIdx);
        if (@event.IsActionReleased("Interact"))
        {
            GD.Print("Clicked the ocean");
            this.Global().GoFishing();
        }
    }
}
