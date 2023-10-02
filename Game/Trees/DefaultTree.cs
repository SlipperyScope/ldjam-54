using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.GamJamScripts;
using Godot;

namespace Game.Trees;

/// <summary>
/// Defines a tree
/// </summary>
public interface ITree 
{
    public event EventHandler<Node2D> Felled;
    public Boolean DoAHit(Int32 damage);
    public Seed Spread();
    public Node2D AsNode { get; }
    public String TreeName { get; set; }
    public Boolean OverrideSpawn { get; set; }
    public Boolean GodMode { get; set; }
    public Area2D Bounds { get; }
}

/// <summary>
/// Yep, it's wood
/// </summary>
public partial class DefaultTree : Node2D, ITree, ITargetable
{
    public event EventHandler<Node2D> Felled;

    /// <summary>
    /// Configuration for setting up the tree
    /// </summary>
    [Export]
    private Treesource Config;

    public String TreeName { get; set; } = null;
    public Boolean OverrideSpawn { get; set; } = false;
    public Boolean GodMode { get; set; } = false;
    public Area2D Bounds { get; private set; }
    private AnimatedSprite2D Sprite;
    private ShapeCast2D Seedler;
    private NavigationObstacle2D Obstacle;
    private Single HitsRemaining;
    private Boolean Growing = true;

    public override void _EnterTree()
    {
        _ = Config ?? throw new NullReferenceException(nameof(Config));
        TreeName ??= Config.Name;
        HitsRemaining = GodMode is true ? Single.MaxValue : Config.HitPointsAtSpawn;
        this.Global().Beat += Wiggle;
    }

    public override void _Process(Double delta)
    {
        if (GodMode is true && HitsRemaining < Single.MaxValue)
        {
            HitsRemaining = Single.MaxValue;
            UpdateSprite();
        }
        else if (HitsRemaining < Config.HitPoints)
        {

            HitsRemaining += (Growing is true ? Config.GrowthRate : Config.HealingRate) * (Single)delta;
            UpdateSprite();

            if (HitsRemaining > Config.HitPoints)
            {
                HitsRemaining = Config.HitPoints;
                Growing = false;
            }
        }
    }

    private void Wiggle(Object sender, EventArgs e)
    {
        RotationDegrees += GD.Randf() * 2f - 1f;
    }

    public override void _Ready()
    {
        Bounds = GetNode<Area2D>($"%{nameof(Bounds)}");
        Bounds.GetNode<CollisionShape2D>(nameof(CollisionShape2D)).Shape = Config.CollisionShape;
        Bounds.InputEvent += OnClick;

        Sprite = GetNode<AnimatedSprite2D>($"%{nameof(Sprite)}");
        Sprite.SpriteFrames = Config.Textures;
        Sprite.Frame = 0;

        Seedler = GetNode<ShapeCast2D>($"%{nameof(Seedler)}");
        Seedler.Shape = Config.CollisionShape;
        Seedler.Enabled = false;

        Obstacle = GetNode<NavigationObstacle2D>($"%{nameof(Obstacle)}");
        var shapeBounds = Config.CollisionShape.GetRect().Size;
        Obstacle.Radius = Mathf.Max(shapeBounds.X, shapeBounds.Y) / 2f;
    }

    public Seed Spread()
    {
        if (OverrideSpawn is false)
        {
            if (Growing is true) return null;
            if (HitsRemaining / Config.HitPoints is < .8f) return null;
        }

        for (var attempt = 0; attempt < Config.SpawnAttempts; attempt++)
        {
            Seedler.Enabled = true;
            Seedler.Position = GetRandomSpawnPosition();
            Seedler.Rotation = GD.Randf() * Mathf.Tau;
            Seedler.ForceShapecastUpdate();

            if (Seedler.IsColliding() is false)// && IsInBounds(Seedler) is true)
            {
                Seedler.Enabled = false;
                return new(GD.Load<PackedScene>(Config.SaplingPath), Seedler.GlobalTransform);
            }
        }

        Seedler.Enabled = false;
        return null;
    }

    private Vector2 GetRandomSpawnPosition()
    {
        var point = Vector2.Zero;
        var attempt = 0u;
        var annulus = Config.SpawnZone;

        do
        {
            point = new Vector2(GD.Randf() - 0.5f, GD.Randf() - 0.5f) * 2f * annulus.R;
        }
        while (attempt++ < 1000u && annulus.Contains(point) is false);

        if (attempt >= 1000u)
        {
            GD.PushWarning("Could not find suitable point");
            point = Vector2.Zero;
        }

        return point;
    }

    //private Boolean IsInBounds(Node2D node) => /*new Rect2(-2500f, -2000f, 5000f, 2500f).HasPoint(node.GlobalPosition);//*/ node.GlobalPosition.X is > -2500f and < 2500f && node.GlobalPosition.Y is > -2000f and < 500f;

    private void OnClick(Node viewport, InputEvent e, Int64 shapeIdx)
    {
        if (e.IsActionReleased("Interact") && HitsRemaining > 0f)
        {
            this.Global().SetTarget(this);
        }
    }

    public Boolean DoAHit(Int32 damage)
    {
        HitsRemaining -= damage;
        Growing = false;
        UpdateSprite();

        if (HitsRemaining <= 0)
        {
            var stump = Config.Stump.Instantiate<Node2D>();
            stump.Transform = Transform;
            this.Global().LogMurder(TreeName);
            this.Global().Beat -= Wiggle;
            Felled?.Invoke(this, stump);
            return true;
        }

        return false;
    }

    private const Int32 HitStates = 3;

    private void UpdateSprite()
    {
        var ratio = HitsRemaining / Config.HitPoints;
        //var frame = (Int32)MathF.Ceil((1 - ratio) * HitStates) - 1;
        var frame = HitStates - HitStates * ratio;
        Sprite.Frame = (Int32)Mathf.Round(frame);
    }

    public Node2D AsNode => this; 
}

/// <summary>
/// Information about where a tree was planted
/// </summary>
/// <param name="Scene">Tree scene to instance</param>
/// <param name="GlobalTransform">Robots in disguise</param>
public record Seed(PackedScene Scene, Transform2D GlobalTransform);
