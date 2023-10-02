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

    private void GameLose(Object sender, EventArgs e) => GameOver(false);
    private void GameWon(Object sender, EventArgs e) => GameOver(true);

    private void GameOver(Boolean won)
    {
        var board = GetNode<Deceased>("%DeceasedBoard");

        var jerry = new DeceasedData("Jerry", "Oh, how the mighty have fallen", 0);
        var self = new DeceasedData("You", won switch
        { true => "🌳🫷🏻 dominated with confident finality 🫸🏻🌲",
          false => "💀💀 allowed the forest to take you 💀💀"
        }, 1);

        var log = this.Global().Logbook.OrderBy(e => e.Deaths).ToList();
        log.Add(jerry);
        log.Add(self);

        var pad = log.Max(e => e.Name.Length);
        log = log.Select(e => e with { Name = e.Name.PadLeft(pad) }).ToList();

        board.Visible = true;
        board.Populate(log);
    }
}
