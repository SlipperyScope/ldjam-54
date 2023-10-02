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

		["sunfish::smol"] = GD.Load<Texture2D>("res://art/Cards/smol/smol-sunfish.png"),
		["swordfish::smol"] = GD.Load<Texture2D>("res://art/Cards/smol/smol-swordfish.png"),
		["oldBoot::smol"] = GD.Load<Texture2D>("res://art/Cards/smol/smol-boot.png"),
		["bullKelp::smol"] = GD.Load<Texture2D>("res://art/Cards/smol/smol-bull-kelp.png"),
		["underwaterCuttingTorch::smol"] = GD.Load<Texture2D>("res://art/Cards/smol/smol-underwater-cutting-torch.png"),
	};
	[Export]
	public string Face;
	[Export]
	public bool Smol = false;
	public int idx = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print(GetChildCount());
		foreach (var c in GetChildren()) {
			GD.Print(c);
		}

		if (Face is not null) {
			var key = Smol ? $"{Face}::smol" : Face;
			TextureNormal = Textures[key] ?? throw new Exception($"Could not find texture for '{Face}'");
		}

		SizeFlagsHorizontal = (SizeFlags)((int)SizeFlags.Expand + (int)SizeFlags.ShrinkCenter);
		SizeFlagsVertical = SizeFlags.Fill;
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
