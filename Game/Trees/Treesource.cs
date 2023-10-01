using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Math;
using Godot;

namespace Game.Trees;

/// <summary>
/// It's a tree-resource, geddit? All the information used to make a tree is in here
/// </summary>
public partial class Treesource : Resource
{
    private const String Look = "Aesthetic";
    private const String Prop = "Propagation";
    private const String Life = "Lifecycle";
    private const String Spawn = "Spawning";
    private const String Radius = "SpawnRadius";

    /// <summary>
    /// The name of the tree defined by this configuration
    /// </summary>
    [Export, ExportGroup(Look)]
    public String Name { get; private set; } = "Jerry";

    /// <summary>
    /// Tree texture to use
    /// </summary>
    [Export]
    public SpriteFrames Textures { get; private set; }

    [Export]
    public Shape2D CollisionShape { get; private set; } = new CircleShape2D() { Radius = 150f };

    /// <summary>
    /// Scene used to create saplings
    /// <br/><br/>Note: Must be type ITree
    /// </summary>
    [Export, ExportGroup(Prop)]
    public String SaplingPath { get; private set; }

    /// <summary>
    /// Number of times the tree will try to find a suitible planting area before giving up
    /// </summary>
    [Export]
    public UInt32 SpawnAttempts { get; private set; } = 10u;

    /// <summary>
    /// Maximum distance away from center a sapling can grow
    /// </summary>
    [Export, ExportSubgroup(Radius)]
    private Single Minimum = 350f;

    /// <summary>
    /// Minimum distance from center a sapling can grow
    /// </summary>
    [Export]
    private Single Maximum = 600f;

    /// <summary>
    /// Zone in which saplings can spawn
    /// </summary>
    public Annulus SpawnZone => new(Minimum, Maximum);

    /// <summary>
    /// Scene used to create death remnants.
    /// <br/><br/>Note: Must be type IStump
    /// </summary>
    [Export, ExportGroup(Prop)]
    public PackedScene Stump { get; private set; }

    /// <summary>
    /// Amount of hits a tree can take before dying
    /// </summary>
    [Export, ExportGroup(Life)]
    public Single HitPoints { get; private set; } = 100f;
}
