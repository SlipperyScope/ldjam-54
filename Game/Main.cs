using Game;
using Game.NotTreesOrAxes;
using Godot;
using System;
using System.Linq;

namespace Game;

public partial class Main : Node
{
    public override void _Ready()
    {
        this.Global().GameWin += GameWon;
        this.Global().GameLose += GameLose;
    }

    private void GameLose(Object sender, EventArgs e)
    {
        var board = GetNode<Deceased>("%DeceasedBoard");
        board.Visible = true;
        var log = this.Global().Logbook;
        var pad = log.Count is not 0 ? log.Max(e => e.Name.Length) : 0;
        log.Add(new("You".PadLeft(pad), "💀💀 allowed the forest to take you 💀💀", 1));
        board.Populate(log);
    }

    private void GameWon(Object sender, EventArgs e)
    {
        var board = GetNode<Deceased>("%DeceasedBoard");
        board.Visible = true;
        var log = this.Global().Logbook;
        var pad = log.Count is not 0 ? log.Max(e => e.Name.Length) : 0;
        log.Add(new("You".PadLeft(pad), "😶😐 dominated with confident finality 😑🫥", 1));
        board.Populate(log);
    }
}
