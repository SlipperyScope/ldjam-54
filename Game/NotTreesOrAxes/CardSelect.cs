using Godot;
using System;
using System.Collections.Generic;

public partial class CardSelect : Control
{
	[Export]
	public string[] Cards;
	private PackedScene CardScene;
	private HBoxContainer CardCollection;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CardScene = GD.Load<PackedScene>("res://NotTreesOrAxes/Card.tscn");
		CardCollection = GetNode<HBoxContainer>("Bull/Shit/Cards");
		SetCards(Cards);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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