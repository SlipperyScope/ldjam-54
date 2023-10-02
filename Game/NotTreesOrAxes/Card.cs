using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

public partial class Card : TextureButton
{
	private Dictionary<string,Texture2D> Textures = new Dictionary<string, Texture2D> {
		["sunfish"] = GD.Load<Texture2D>("res://art/Cards/sunfish.png"),
		["swordfish"] = GD.Load<Texture2D>("res://art/Cards/swordfish.png"),
		["oldBoot"] = GD.Load<Texture2D>("res://art/Cards/boot.png"),
		["bullKelp"] = GD.Load<Texture2D>("res://art/Cards/bull-kelp.png"),
		["underwaterCuttingTorch"] = GD.Load<Texture2D>("res://art/Cards/underwater-cutting-torch.png"),
	};
	[Export]
	public string Face;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print(GetChildCount());
		foreach (var c in GetChildren()) {
			GD.Print(c);
		}

		if (Face is not null) {
			TextureNormal = Textures[Face] ?? throw new Exception($"Could not find texture for '{Face}'");
		}

		SizeFlagsHorizontal = (SizeFlags)((int)SizeFlags.Expand + (int)SizeFlags.ShrinkCenter);
		SizeFlagsVertical = SizeFlags.Fill;
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
