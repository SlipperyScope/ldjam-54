using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Game.NotTreesOrAxes.Menus;
public partial class ReturnPlacard : Control
{
    [Export]
    private Button Return;

    [Export]
    private Button Exit;

    [Export]
    private String ReturnScene;

    public override void _EnterTree()
    {
        Return.Pressed += ReturnPressed;
        Exit.Pressed += ExitPressed;
    }

    private void ExitPressed() => GetTree().Quit();

    private void ReturnPressed()
    {
        this.Global().Reset(ReturnScene);
    }
}
