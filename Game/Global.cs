using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Game;

/// <summary>
/// A global class for convenience. Use [node].Global() to get a reference
/// </summary>
public partial class Global : Node
{
    public DateTime startTime;
    public DateTime endTime;
    public bool isRunning = false;

    public Player player;

    /// <summary>
    /// Testing
    /// </summary>
    public void HelloWorld() => GD.Print("Hello World");

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
            player.targetTree = tree;
        }
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

