using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.GamJamScripts;
using Game.NotTreesOrAxes;
using Godot;

namespace Game;

/// <summary>
/// A global class for convenience. Use [node].Global() to get a reference
/// </summary>
public partial class Global : Node
{
    public event EventHandler Beat;
    private Double Time = 0d;
    private Double NextBeat = 0d;
    public override void _PhysicsProcess(Double delta)
    {
        Time += delta;
        if (Time >= NextBeat)
        {
            NextBeat += 0.5d;
            Beat?.Invoke(this,new());
        }
    }
    public DateTime startTime;
    public DateTime endTime;
    public bool isRunning = false;

    public Player player;
    public CardSelect cardSelect;

    public event EventHandler FireLit;
    public void LightFire() => FireLit?.Invoke(this, new());

    public event EventHandler GameWin;
    public void WinGame() => GameWin?.Invoke(this, new());

    public event EventHandler GameLose;

    public void LoseGame() => GameLose?.Invoke(this, new());

    /// <summary>
    /// Testing
    /// </summary>
    public void HelloWorld() => GD.Print("Hello World");

    private readonly Dictionary<String, Int32> MurderLog = new();

    public List<DeceasedData> Logbook => MurderLog.Select(kvp => new DeceasedData(kvp.Key, GetRandomDeathText(), kvp.Value)).ToList();

    private String GetRandomDeathText() => Messages[(Int32)(GD.Randi() % Messages.Count)];

    private List<String> Messages = new()
    {
        "had it's life cut short",
        "was smashed to a pulp",
        "made like a tree and died",
        "fell to it's death",
        "pondered but then was stumped",
        "got back to it's roots",
        "learned to do the splits",
        "ended up with split custody",
        "logged off for the last time",
        "didn't even have a stake in it",
        "got fired for being high",
        "had it's chops busting",
        "lumbered around too long",
        "was slowly chipped away",
        "got hacked apart... slowly",
        "was torn limb from limb",
        "looked out and sawdust",
        "was really torn up over it",
        "found it's final resting place",
        "got a bunch of splinters",
        "better beleaf in a higher power",
        "was axed for the route",
        "disembarked from this world",
        "had the wrong axecent",
        "made an end-user log",
        "was in the shady part of town",
        "was with seedy characters",
        "got caught up in a cord",
        "suffered permanent alderations",
        "had acute limbic dissociation",
        "had the life sapped from it"
    };

    /// <summary>
    /// Logs a murder in the murder log
    /// </summary>
    /// <param name="name">They're all named Jerry</param>
    public void LogMurder(String name)
    {
        if (MurderLog.ContainsKey(name) is true)
        {
            MurderLog[name] += 1;
        }
        else
        {
            MurderLog.Add(name, 1);
        }

        GD.Print($"{name} has been murdered {MurderLog[name]} time{(MurderLog[name] is not 1 ? "s" : "")}");
    }

    public void StartGame() {
        // Clear last game state (spawned trees, inventory, etc) here
        startTime = DateTime.Now;
        isRunning = true;
    }

    public void EndGame() {
        endTime = DateTime.Now;
        isRunning = false;
    }

    public TimeSpan gameDuration {
        get => isRunning ? DateTime.Now - startTime : endTime - startTime;
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    public void SetTargetTree(Game.Andrew.Tree tree)
    {
        if (player != null) {
            player.SetTarget(tree);
        }
    }

    public void SetTarget(ITargetable target)
    {
        if (player is not null && !player.fishing) //I know, bad, but game jam
        {
            player.targetTree = target as CanvasItem ?? throw new InvalidCastException();
        }
    }

    public void GoFishing() {
        if (player is not null && !player.chopping && !player.fishing) {
            player.fishIntent = true;
        }
    }

    public void CardSelectScreen() {
        cardSelect.Toggle();
        cardSelect.SetCards(new string[]{"oldBoot", "sunfish", "bullKelp"});
    }

    public void DismissCardSelectScreen() {
        cardSelect.Toggle();
    }
}

/// <summary>
/// Extensions for the node class
/// </summary>
public static partial class NodeExtensions
{
    /// <summary>
    /// Gets a reference to the global class
    /// </summary>
    /// <param name="node">Node in the same scene tree as the global class</param>
    /// <returns>The Global class instance</returns>
    public static Global Global(this Node node) => node.GetNode<Global>("/root/Global");
}

