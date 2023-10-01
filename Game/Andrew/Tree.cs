using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Math;
using Godot;

namespace Game.Andrew;

/// <summary>
/// It's a tree, man
/// </summary>
public partial class Tree : Node2D
{
    /// <summary>
    /// Notifies that a tree is attempting to plant a sapling
    /// </summary>
    public event EventHandler<SaplingPlantedEventArgs> SaplingPlanted;

    /// <summary>
    /// Tree config resource
    /// </summary>
    [Export]
    private TreeConfig Config;

    private Sprite2D Sprite;
    private Area2D Bounds;
    private ShapeCast2D Spawner;
    private NavigationObstacle2D Nav;

    private Double PhysicsTime = 0d;
    private Double NextSpawnAttempt = 0d;

    private int Health = 10;

    //TODO: Make it not need to be set before the node is ready
    /// <summary>
    /// Sets the config.
    /// <br/><br/>MUST BE SET BEFORE THE NODE IS READY
    /// </summary>
    /// <param name="config">Config to use</param>
    /// <exception cref="ArgumentNullException"></exception>
    public void SetConfig(TreeConfig config) => Config = config ?? throw new ArgumentNullException(nameof(config));

    public override void _Ready()
    {
        _ = Config ?? throw new NullReferenceException("Config not set");

        Sprite = GetNode<Sprite2D>($"%{nameof(Sprite)}");
        Bounds = GetNode<Area2D>($"%{nameof(Bounds)}");
        Spawner = GetNode<ShapeCast2D>($"%{nameof(Spawner)}");
        Nav = GetNode<NavigationObstacle2D>($"%{nameof(Nav)}");

        Sprite.Texture = Config.Texture;
        
        Bounds.GetNode<CollisionShape2D>("CollisionShape2D").Shape = Config.CollisionShape;
        Bounds.InputEvent += InputEvent;
        

        // TODO: Non-circle obstacle? = use collision shape
        Nav.Radius = Config.NavObstacleRadius;

        Spawner.Enabled = true;
        Spawner.Shape = Config.CollisionShape;
        Spawner.TargetPosition = new();

        //NextSpawnAttempt = Config.SpawnInterval;
    }

    // TODO: Move spawning to level
    // TODO: Either never allow two spawns in one physics frame or queue them and remove one of any pair that overlap

    public override void _PhysicsProcess(Double delta)
    {
        //PhysicsTime += delta;

        //if (PhysicsTime >= NextSpawnAttempt && InBounds(GlobalPosition) is true)
        //{
        //    for (var attempt = 0; attempt < Config.SpawnAttempts; attempt++)
        //    {
        //        Spawner.Position =  RandomSpawnPosition();
        //        Spawner.ForceShapecastUpdate();

        //        if (Spawner.IsColliding() is false && InBounds(Spawner.GlobalPosition))
        //        {
        //            SpawnSapling(Spawner.GlobalPosition);
        //            //Hits.Add(Spawner.Position);
        //            //QueueRedraw();
        //            NextSpawnAttempt += Config.SpawnInterval;

        //            // return; // <--- NOTE
        //            break;
        //        }
        //    }

        //    NextSpawnAttempt += Config.SpawnInterval;

            //GD.Print("aborted spawn attempt");
        //}
    }

    //public void Spread()
    public SaplingPlantedEventArgs Spread()
    {
        for (var attempt = 0; attempt < Config.SpawnAttempts; attempt++)
        {
            Spawner.Position = RandomSpawnPosition();
            Spawner.ForceShapecastUpdate();

            // TODO: Move conditions checking to configuration (out of scope)

            if (Spawner.IsColliding() is false && InBounds(Spawner) is true)
            {
                //SaplingPlanted?.Invoke(this, new(worldPosition, GD.Randf() * Mathf.Tau, new(Config)));
                return new(Spawner.GlobalPosition, GD.Randf() * Mathf.Tau, new(Config));
                //break;

            }
        }

        return null;
    }

    private static Boolean InBounds(Node2D node) => node.GlobalPosition.X is > -1920 and < 1920 && node.GlobalPosition.Y is > -1080 and < 500; //500 is apprx the shoreline. G-G-G-GAME JAM

    private void SpawnSapling(Vector2 globalPosition)
    {
        var sapling = Config.SaplingScene.Instantiate<Tree>();
        sapling.GlobalPosition = globalPosition;
        sapling.RotationDegrees = GD.Randf() * 360;
        sapling.SetConfig(Config);
        GetParent().AddChild(sapling);
        CallDeferred(nameof(SwapZ), this, sapling); // Is this actually working?
    }

    private void SwapZ(Node2D a, Node2D b) => (a.ZIndex, b.ZIndex) = (b.ZIndex, a.ZIndex);

    private Vector2 RandomSpawnPosition()
    {
        Vector2 point;
        UInt32 attempts = 0;
        var annulus = Config.SpawnRange;
        do
        {
            point = new Vector2(GD.Randf()-0.5f, GD.Randf()-0.5f) * 2f * annulus.R;
        }
        while (attempts++ < 100u && annulus.Contains(point) is false);

        if (attempts > 100u)
            GD.Print("Did not find a point in annulus");

        return point;
    }

    public void InputEvent(Node viewpport, InputEvent @event, long shapeIdx)
    {
		if (@event is InputEventMouseButton eventMouseButton && Health > 0)
		{
			if (eventMouseButton.IsReleased()) {
                this.Global().SetTargetTree(this);
                GD.Print("Tree clicked");
            }
        }
    }

    public Boolean DoAHit(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Sprite.Texture = Config.StumpTexture;
            Nav.Radius = Config.StumpNavObstacleRadius;
            Spawner.Enabled = false;
            //kill tree
        }
        return Health <= 0;
    }

    //private List<Vector2> Hits = new();

    //public override void _Draw()
    //{
    //    DrawCircle(Vector2.Zero, Config.SpawnRange.R, new Color("FF000022"));
    //    DrawCircle(Vector2.Zero, Config.SpawnRange.r, new Color("0000FF22"));

    //    foreach (var hit in Hits)
    //    {
    //        DrawCircle(hit, 150, new Color("00FF0022"));
    //    }
    //}
}

/// <summary>
/// Arguments for a make tree event
/// </summary>
public class SaplingPlantedEventArgs : EventArgs
{
    /// <summary>
    /// Position in the same worldspace as the tree
    /// </summary>
    public Vector2 Position { get; init; }

    /// <summary>
    /// Rotation in the same worldspace as the tree
    /// </summary>
    public Single Rotation { get; init; }

    /// <summary>
    /// Buider for the sapling
    /// </summary>
    public TreeBuilder Builder { get; init; }

    /// <summary>
    /// Creates new sapling planted event args
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public SaplingPlantedEventArgs(Vector2 position, Single rotation, TreeBuilder builder)
    {
        Position = position;
        Rotation = rotation;
        Builder = builder;
    }
}