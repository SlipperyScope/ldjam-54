using Game;
using Godot;
using System;
using System.Collections.Generic;

public partial class Inventory : Control
{
	public string[] Cards = new string[]{};
	private PackedScene CardScene;
	private HBoxContainer Tray;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CardScene = GD.Load<PackedScene>("res://NotTreesOrAxes/Card.tscn");
		Tray = GetNode<HBoxContainer>("Tray");
		SetCards(Cards);
		this.Global().InventoryUI = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetCards(string[] cards) {
		var g = this.Global();

		Cards = cards;
		while (Tray.GetChildCount() > 0) {
			Tray.RemoveChild(Tray.GetChild(0));	
		}

		for (var i = 0; i < Cards.Length; i++) {
			var c = Cards[i];
			var CardInstance = CardScene.Instantiate<Card>();
			CardInstance.Face = c;
			CardInstance.Smol = true;
			CardInstance.idx = i;
			CardInstance.Pressed += () => {
				// Remove card from inventory
				g.RemoveFromInventoryAt(CardInstance.idx);
				GD.Print(g.inventory.ToArray());
			};
			Tray.AddChild(CardInstance);
		}
	}
}
