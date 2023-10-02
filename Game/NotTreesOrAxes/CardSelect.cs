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

		var layer = GetNode<CanvasLayer>("Bull");
		var bgButton = GetNode<TextureButton>("Bull/Shit/BgButton");
		bgButton.Pressed += () => {
			this.Global().DismissCardSelectScreen();
		};
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

    public void SetCards(string[] cards) {
		var g = this.Global();

		Cards = cards;
		while (CardCollection.GetChildCount() > 0) {
			CardCollection.RemoveChild(CardCollection.GetChild(0));	
		}

		for (var i = 0; i < Cards.Length; i++) {
			var c = Cards[i];
			var CardInstance = CardScene.Instantiate<Card>();
			CardInstance.Face = c;
			CardInstance.idx = i;
			CardInstance.Pressed += () => {
				// Set card in inventory
				if (g.inventory.Count < g.maxInventorySize) {
					g.AddToInventory(CardInstance.Face);
					GD.Print(g.inventory.ToArray());

					// Dismiss early if this is the last card
					if (Cards.Length == 1) {
						g.DismissCardSelectScreen();
					}

					var newCards = new List<string>(Cards);
					newCards.RemoveAt(CardInstance.idx);
					SetCards(newCards.ToArray());
				} else {
					GD.Print("Bonk, inventory full");
				}
			};
			CardCollection.AddChild(CardInstance);
		}
	}
}