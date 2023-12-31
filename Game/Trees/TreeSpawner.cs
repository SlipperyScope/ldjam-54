﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.NotTreesOrAxes;
using Game.Sounds;
using Godot;

namespace Game.Trees;
public partial class TreeSpawner : Node
{
    private Dictionary<String, ITree> Trees = new();

    /// <summary>
    /// Seconds between spawn attempts
    /// </summary>
    [Export]
    private Single SpawnInterval = 2.5f;
    private Single NormalInterval;
    private Boolean TreeGrowthBypass = false;

    [Export]
    private Node2D Stumptown;

    [Export]
    private Camp Camp;

    private Double Time = 0d;
    private Double Next = 0f;
    private Double Lose = Double.MaxValue;

    private RandomSound2D GrowSound;
    private RandomSound2D Felled;

    public override void _EnterTree()
    {
        if (Stumptown is null) throw new NullReferenceException($"{nameof(Stumptown)} is not set");

        GrowSound = GetNodeOrNull<RandomSound2D>($"{nameof(GrowSound)}") ?? throw new NullReferenceException($"Could not find {nameof(RandomSound2D)} named {nameof(GrowSound)}");
        Felled = GetNodeOrNull<RandomSound2D>($"{nameof(Felled)}") ?? throw new NullReferenceException($"Could not find {nameof(RandomSound2D)} named {nameof(Felled)}");
        
        NormalInterval = SpawnInterval;
        Next = SpawnInterval;

        this.Global().FireLit += FireLit;
    }

    private void FireLit(Object sender, EventArgs e)
    {
        //GrowFast();
        Next = Double.MaxValue;
    }

    private void Camp_AreaEntered(Area2D area)
    {


        Lose = Time + 5d;
        Next = Double.MaxValue;
        var tree = area.GetParent<ITree>();
        tree.GodMode = true;
        tree.AsNode.Modulate = Colors.Red;
        GetNode<AudioStreamPlayer>("AudioStreamPlayer").Play();
        Trees.Remove(Trees.First(p => p.Value == tree).Key);
        GrowFast();
    }

    private void GrowFast()
    {
        SpawnInterval = .125f;
        Next = 0f;
        TreeGrowthBypass = true;
        foreach(var tree in Trees.Values)
        {
            tree.OverrideSpawn = true;
        }
    }

    private void GrowSlow()
    {
        SpawnInterval = NormalInterval;
        Next = Time + SpawnInterval;
        TreeGrowthBypass = false;
        foreach (var tree in Trees.Values)
        {
            tree.OverrideSpawn = false;
        }
    }

    public override void _Ready()
    {
        Camp.CampArea.AreaEntered += Camp_AreaEntered;
        foreach( var child in GetChildren())
        {
            if (child is ITree tree)
            {
                var name = GetRandomUniqueName();
                tree.TreeName = name;
                Trees.Add(name, tree);
                tree.Felled += OnTreeFelled;
            }
        }
    }

    public override void _Input(InputEvent e)
    {
        if (e.IsActionPressed("SpeedTree"))
        {
            GrowFast();
        }
        else if (e.IsActionReleased("SpeedTree"))
        {
            GrowSlow();
        }
    }

    private void OnTreeFelled(Object sender, Node2D e)
    {
        if (sender is ITree tree && Trees.Values.ToList().Contains(tree))
        {
            Stumptown.AddChild(e);
            e.GlobalTransform = tree.AsNode.GlobalTransform;
            Felled.GlobalPosition = tree.AsNode.GlobalPosition;
            var pair = Trees.FirstOrDefault(p => p.Value == tree);
            Trees.Remove(pair.Key);
            RemoveChild(tree.AsNode);
            tree.AsNode.QueueFree();
            Felled.Play();
            StopSpawn = false;
        }
        else
        {
            GD.Print("Hmmm... 🤔");
        }

        if (Trees.Count is 0)
        {
            CallDeferred(nameof(SendWin));
        }
    }

    private void SendWin() => this.Global().WinGame();

    public override void _Process(Double delta)
    {
        Time += delta;

        var dohalf = true;

        if (Time >= Next && Trees.Count > 0)
        {

            var attempts = TreeGrowthBypass ? 100 : 0;
            do
            {
                Seed seed = Trees.Values.ToList()[(Int32)(GD.Randi() % Trees.Count)].Spread();

                if (seed is not null)
                {
                    SpawnTree(seed);
                    dohalf = false;
                    break;
                }
            } while (--attempts > 0);
            while(Next < Time) Next += dohalf is true ? SpawnInterval / 2f : SpawnInterval;
        }

        if (Time >= Lose)
        {
            while (Next < Time) Next += SpawnInterval;
            Lose = Double.MaxValue;
            GD.Print("💀💀 Lol, you lose 💀💀");
            Camp.CampArea.AreaEntered -= Camp_AreaEntered;
            this.Global().LoseGame();
            GrowFast();
        }
    }

    private Boolean StopSpawn = false;

    private void SpawnTree(Seed seed)
    {
        //GD.Print(Trees.Count);

        if (StopSpawn) return;

        var tree = seed.Scene.InstantiateOrNull<ITree>() ?? throw new InvalidCastException(nameof(seed));
        var name = GetRandomUniqueName();
        tree.TreeName = name;
        tree.OverrideSpawn = TreeGrowthBypass;
        Trees.Add(name, tree);
        tree.Felled += OnTreeFelled;
        AddChild(tree.AsNode);
        MoveChild(tree.AsNode, 0);
        tree.AsNode.GlobalTransform = seed.GlobalTransform;
        GrowSound.GlobalPosition = tree.AsNode.GlobalPosition;
        GrowSound.Play();
    }

    private String GetRandomUniqueName()
    {
        var unusedNames = NameSetA.Except(Trees.Keys).ToList();
        if (unusedNames.Count is > 0)
        {
            return unusedNames[(Int32)(GD.Randi() % unusedNames.Count)];
        }
        else
        {
            unusedNames = NameSetB.Except(Trees.Keys).ToList();
            if (unusedNames.Count is > 0)
            {
                return unusedNames[(Int32)(GD.Randi() % unusedNames.Count)];
            }
            else
            {
                unusedNames = NameSetC.Except(Trees.Keys).ToList();
                if (unusedNames.Count is > 0)
                {
                    return unusedNames[(Int32)(GD.Randi() % unusedNames.Count)];
                }
                else
                {
                    unusedNames = NormalNames.Except(Trees.Keys).ToList();
                    if (unusedNames.Count is > 0)
                    {
                        return unusedNames[(Int32)(GD.Randi() % unusedNames.Count)];
                    }
                    else
                    {
                        var i = 0ul;
                        String name;
                        var c = 0ul;
                        do
                        {
                            name = $"bonus name {i++:00}";
                        } while (Trees.ContainsKey(name) is true && ++c < 100000ul);

                        if (c >= 100000ul)
                            GD.PushWarning("holy balls man");

                        return name;
                    }
                }
            }
        }
    }

    private List<String> NameSetA = new()
    {
        "Bertha", 
        "Ebenezer", 
        "Mildred", 
        "Horace", 
        "Gertrude", 
        "Archibald", 
        "Ethel", 
        "Wilfred", 
        "Agatha", 
        "Cuthbert", 
        "Maude", 
        "Barnaby", 
        "Gladys", 
        "Percival", 
        "Doris", 
        "Rupert", 
        "Edna", 
        "Algernon", 
        "Mabel", 
        "Clement", 
        "Zara",
    };

    private List<String> NameSetB = new()
    {
        "Myrtle", 
        "Basil", 
        "Eunice", 
        "Edgar",
        "Blanche", 
        "Ernest", 
        "Hilda", 
        "Herbert", 
        "Fanny", 
        "Clarence", 
        "Enid", 
        "Gilbert", 
        "Gloria", 
        "Harold", 
        "Ida", 
        "Lionel", 
        "Muriel", 
        "Melvin" , 
        "Prudence" ,
    };

    private List<String> NameSetC = new()
    {
        "Norman","Adonis", "Astrid", "Beatrix", "Blaine", "Bodhi", "Briar", "Caspian", "Coraline", "Daphne", "Darius", "Delilah", "Dorian", "Elodie", "Emrys", "Esme", "Finnick", "Freya", "Gideon", "Harlow", "Hugo", "Imogen", "Iris", "Jasper", "Juno", "Kaius", "Lara", "Leander", "Lennox", "Lila", "Lorelei", "Magnus", "Maren", "Matteo", "Mira", "Nadia", "Nico", "Odette", "Orion", "Oscar", "Phoebe", "Remy", "Rhea", "Ronan", "Ruby", "Sasha", "Sawyer", "Seraphina", "Talia", "Thalia"
    };

    private List<String> NormalNames = new()
    {
        "Emma", 
        "Oliver", 
        "Ava", 
        "Noah", 
        "Sophia", 
        "Liam", 
        "Isabella", 
        "Lucas", 
        "Charlotte", 
        "Ethan", 
        "Amelia", 
        "Mason", 
        "Harper", 
        "Logan", 
        "Evelyn", 
        "James", 
        "Abigail", 
        "Alexander", 
        "Emily", 
        "Elijah", 
        "Avery", 
        "Jacob", 
        "Mila", 
        "Michael", 
        "Ella", 
        "Daniel", 
        "Scarlett", 
        "Henry", 
        "Chloe", 
        "Jackson", 
        "Lily", 
        "Sebastian", 
        "Madison", 
        "Aiden", 
        "Sofia", 
        "Matthew", 
        "Aria", 
        "David", 
        "Riley", 
        "Owen", 
        "Zoe", 
        "Carter", 
        "Hannah", 
        "Wyatt", 
        "Nora", 
        "Jayden", 
        "Luna", 
        "John"
    };
}