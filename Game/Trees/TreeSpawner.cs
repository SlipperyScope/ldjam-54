using System;
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

    [Export]
    private Node2D Stumptown;

    [Export]
    private Camp Camp;

    private Double Time = 0d;
    private Double Next = 0f;

    private RandomSound2D GrowSound;
    private RandomSound2D Felled;

    public override void _EnterTree()
    {
        if (Stumptown is null) throw new NullReferenceException($"{nameof(Stumptown)} is not set");

        GrowSound = GetNodeOrNull<RandomSound2D>($"{nameof(GrowSound)}") ?? throw new NullReferenceException($"Could not find {nameof(RandomSound2D)} named {nameof(GrowSound)}");
        Felled = GetNodeOrNull<RandomSound2D>($"{nameof(Felled)}") ?? throw new NullReferenceException($"Could not find {nameof(RandomSound2D)} named {nameof(Felled)}");

        Next = SpawnInterval;
        this.Global().Won += Winning;
    }

    private void Winning(Object sender, EventArgs e)
    {
        SpawnInterval = 0f;
    }

    private void Camp_AreaEntered(Area2D area)
    {
        //GD.Print(area, area.GetParent());

        GD.PushWarning("Lol, you lose");
        GD.Print("💀💀 Lol, you lose 💀💀");
        SpawnInterval = 0f;
        Next = 0f;
        Camp.CampArea.AreaEntered -= Camp_AreaEntered;
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
            Camp.AllTreesDeadNow();
        }
    }

    public override void _PhysicsProcess(Double delta)
    {
        Time += delta;

        if (Time >= Next && Trees.Count > 0)
        {
            var seed = Trees.Values.ToList()[(Int32)(GD.Randi() % Trees.Count)].Spread();

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

    private Boolean StopSpawn = false;

    private void SpawnTree(Seed seed)
    {
        //GD.Print(Trees.Count);

        if (StopSpawn) return;

        var tree = seed.Scene.InstantiateOrNull<ITree>() ?? throw new InvalidCastException(nameof(seed));
        var name = GetRandomUniqueName();
        tree.TreeName = name;
        Trees.Add(name, tree);
        tree.Felled += OnTreeFelled;
        AddChild(tree.AsNode);
        tree.AsNode.GlobalTransform = seed.GlobalTransform;
        GrowSound.GlobalPosition = tree.AsNode.GlobalPosition;
        GrowSound.Play();
    }

    private String GetRandomUniqueName()
    {
        var unusedNames = SillyNames.Except(Trees.Keys).ToList();
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
                //throw new Exception("You fool, you absolute baffoon, you utterly incompetant single celled organism - you did not make enough name!");
                GD.PushWarning("You fool, you absolute baffoon, you utterly incompetant single celled organism - you did not make enough name!");
                StopSpawn = true;
                return null;
            }
        }
    }

    private List<String> SillyNames = new()
    {
        "Jerry", 
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
        "Norman","Adonis", "Astrid", "Beatrix", "Blaine", "Bodhi", "Briar", "Caspian", "Coraline", "Daphne", "Darius", "Delilah", "Dorian", "Elodie", "Emrys", "Esme", "Finnick", "Freya", "Gideon", "Harlow", "Hugo", "Imogen", "Iris", "Jasper", "Juno", "Kaius", "Lara", "Leander", "Lennox", "Lila", "Lorelei", "Magnus", "Maren", "Matteo", "Mira", "Nadia", "Nico", "Odette", "Orion", "Oscar", "Phoebe", "Remy", "Rhea", "Ronan", "Ruby", "Sasha", "Sawyer", "Seraphina", "Talia", "Thalia", "Zara"
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