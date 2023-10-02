using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Game.NotTreesOrAxes.Menus;
public partial class TitleScene : Node
{
    [Export]
    private Button Play;

    [Export]
    private Button Instructions;

    [Export]
    private Button Exit;

    [Export]
    private PackedScene PlayScene;

    [Export]

    private AcceptDialog InstructionsDialog;

    public override void _Ready()
    {
        foreach (var button in new []{Play, Instructions, Exit })
        {

        }

        Play.Pressed += OnPlay;
        Instructions.Pressed += OnInstructions;
        Exit.Pressed += OnExit;

        InstructionsDialog.Hide();
        InstructionsDialog.Confirmed += CloseDialog;
    }

    private void CloseDialog()
    {
        InstructionsDialog.Hide();
    }

    private void OnExit() => GetTree().Quit();

    //private void OnInstructions() => GetTree().CurrentScene = InstructionsScene?.InstantiateOrNull<Node2D>() ?? throw new ArgumentNullException("Instructions scene not set in inspector");
    private void OnInstructions()
    {
        InstructionsDialog.Show();
    }

    private void OnPlay() => GetTree().ChangeSceneToPacked(PlayScene ?? throw new ArgumentNullException("Play scene not set in inspector"));
}
