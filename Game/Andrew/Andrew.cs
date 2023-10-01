using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Game.Andrew;

/// <summary>
/// Andrew's test scene
/// </summary>
public partial class Andrew : Node2D
{
	[Export]
	private Resource TreeConfig;

	/// <summary>
	///  Number of seconds between tree spawn attempts
	/// </summary>
	[Export]
	private Single SpawnRate = 1f;

	private TreeBuilder TreeBuilder;

	private List<Tree> Trees = new();

	private Double NextSpawn = 99d;
	private Double CurrentTime = 0d;
	
	public override void _Ready()
	{
		this.Global().HelloWorld();
		
		TreeBuilder = new(TreeConfig as TreeConfig ?? throw new InvalidCastException($"{nameof(TreeConfig)} is not a valid config type or is null"));

		PlantFirstTree(new());
		//PlantFirstTree(new(500f,0f));

        NextSpawn = SpawnRate;
	}

	public override void _PhysicsProcess(double delta)
	{
		CurrentTime += delta;

		if (CurrentTime >= NextSpawn)
		{
			// spawn tree
			if (Trees.Count is > 0)
			{
				var spread = Trees[(Int32)(GD.Randi() % Trees.Count)].Spread();
				if (spread is not null)
				{
					OnPlantSapling(this, spread);
				}
			}
			NextSpawn += SpawnRate;
		}
	}

	private void PlantFirstTree(Vector2 position)
	{
		var tree = TreeBuilder.Make();
		//tree.SaplingPlanted += OnPlantSapling;
		Trees.Add(tree);
		AddChild(tree);
		tree.Position = position;
		tree.ZIndex -= 2;
	}

    private void OnPlantSapling(Object sender, SaplingPlantedEventArgs e)
	{
		var tree = e.Builder.Make();
		tree.GlobalPosition = e.Position;
		tree.Rotation = e.Rotation;
		Trees.Add(tree);
		AddChild(tree);
	}
}
