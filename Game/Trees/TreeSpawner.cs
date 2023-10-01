using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Game.Trees;
public partial class TreeSpawner : Node
{
    private List<ITree> Trees = new();

    /// <summary>
    /// Seconds between spawn attempts
    /// </summary>
    [Export]
    private Single SpawnInterval = 1f;

    [Export]
    private Node2D Stumptown;

    private Double Time = 0d;
    private Double Next = 0f;

    public override void _EnterTree()
    {
        if (Stumptown is null) throw new NullReferenceException($"{nameof(Stumptown)} is not set");

        Next = SpawnInterval;
    }

    public override void _Ready()
    {
        foreach( var child in GetChildren())
        {
            if (child is ITree tree)
            {
                Trees.Add(tree);
                tree.Felled += OnTreeFelled;
            }
        }
    }

    private void OnTreeFelled(Object sender, Node2D e)
    {
        if (sender is ITree tree && Trees.Contains(tree))
        {
            Stumptown.AddChild(e);
            e.GlobalTransform = tree.AsNode.GlobalTransform;
            Trees.Remove(tree);
            RemoveChild(tree.AsNode);
            tree.AsNode.QueueFree();
        }
        else
        {
            GD.Print("Hmmm... 🤔");
        }
    }

    public override void _PhysicsProcess(Double delta)
    {
        Time += delta;

        if (Time >= Next && Trees.Count > 0)
        {
            var seed = Trees[(Int32)(GD.Randi() % Trees.Count)].Spread();

            if (seed is not null)
            {
                SpawnTree(seed);        
                Next += SpawnInterval;
            }
            else
            {
                Next += SpawnInterval / 2f;
            }
        }
    }

    private void SpawnTree(Seed seed)
    {
        var tree = seed.Scene.InstantiateOrNull<ITree>() ?? throw new InvalidCastException(nameof(seed));
        Trees.Add(tree);
        tree.Felled += OnTreeFelled;
        AddChild(tree.AsNode);
        tree.AsNode.GlobalTransform = seed.GlobalTransform;
    }
}