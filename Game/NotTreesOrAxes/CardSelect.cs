using Game;
using Godot;
using System;
using System.Collections.Generic;

public partial class CardSelect : Control
{
	[Export]
	public string[] Cards = new string[]{};
	private PackedScene CardScene;
	private HBoxContainer CardCollection;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CardScene = GD.Load<PackedScene>("res://NotTreesOrAxes/Card.tscn");
		CardCollection = GetNode<HBoxContainer>("Bull/Shit/Cards");
		SetCards(Cards);
		this.Global().cardSelect = this;
		this.Toggle();
	}

	public void Toggle() {
		var layer = GetNode<CanvasLayer>("Bull");
		layer.Visible = !layer.Visible;
		Visible = !Visible;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

		if (@event is InputEventMouseButton mb)
		{
			if (mb.ButtonIndex == MouseButton.Left && mb.Pressed && Visible)
			{
				GD.Print("I've been clicked D:");
				this.Global().DismissCardSelectScreen();
			}
		}
    }

    public void SetCards(string[] cards) {
		Cards = cards;
		while (CardCollection.GetChildCount() > 0) {
			CardCollection.RemoveChild(CardCollection.GetChild(0));	
		}

		foreach (var c in Cards) {
			var CardInstance = CardScene.Instantiate<Card>();
			CardInstance.Face = c;
			CardCollection.AddChild(CardInstance);
		}
	}
}