using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Math;
using Godot;

namespace Game.Andrew;

/// <summary>
/// A tree configuration resource
/// </summary>
public partial class TreeConfig : Resource
{
	/// <summary>
	/// Name of the tree type
	/// </summary>
	[Export]
	public String Name { get; private set; } = "Unspecified Tree";

	///// <summary>
	///// Minimum distance away from the tree a child can spawn
	///// </summary>
	//[Export]
	//public Single OuterSpawnRadius { get; private set; } = 200f;

	///// <summary>
	///// Maximum distance away from the tree a child can spawn
	///// </summary>
	//[Export]
	//public Single InnerSpawnRadius { get; private set; } = 300f;

	/// <summary>
	/// Disc in which saplings can spawn
	/// </summary>
	public Annulus SpawnRange { get; private set; }


	private Godot.Collections.Dictionary<String, Single> __SpawnRange = new() { { "r", 200f }, { "R", 300f } };

	[Export]
	private Godot.Collections.Dictionary<String, Single> _SpawnRange
	{
		get => __SpawnRange;
		set
		{
			__SpawnRange = new() { { "r", (Single?)value["r"] ?? 0f }, { "R", (Single?)value["R"] ?? 0f } };
			SpawnRange = value is null ? new() : new((Single?)value["r"] ?? 0f, (Single?)value["R"] ?? 0f);
		}
	}

	/// <summary>
	/// Tree texture to use
	/// </summary>
	[Export]
	public Texture2D Texture { get; private set; }

	/// <summary>
	/// Tree texture to use for stump
	/// </summary>
	[Export]
	public Texture2D StumpTexture { get; private set; }

	/// <summary>
	/// Shape for the tree's collision
	/// </summary>
	[Export]
	public Shape2D CollisionShape { get; private set; }

	/// <summary>
	/// The number of times the tree will try to find a valid place to spawn a sapling before aborting
	/// </summary>
	[Export]
	public UInt32 SpawnAttempts { get; private set; } = 10u;

	/// <summary>
	/// How often the tree should try to spawn a sapling (in seconds)
	/// </summary>
	[Export]
	public Double SpawnInterval { get; private set; } = 1d;

	/// <summary>
	/// Scene template for saplings
	/// </summary>
	[Export]
	public PackedScene SaplingScene { get; private set; }

	/// <summary>
	/// The radius of the navigation obstacle area
	/// </summary>
	[Export]
	public Single NavObstacleRadius { get; private set; } = 200f;

	/// <summary>
	/// The radius of the navigation obstacle area for stump
	/// </summary>
	[Export]
	public Single StumpNavObstacleRadius { get; private set; } = 100f;
}
