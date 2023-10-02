﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Trees;
using Godot;

namespace Game.NotTreesOrAxes;
public partial class Camp : Node2D
{
    public Area2D CampArea { get; private set; }
    public Area2D WinBubble { get; private set; }
    private AnimatedSprite2D Sprite;

    private List<ITree> Harm = new();

    [Export] // just for testing
    private Boolean Winning = false;

    [Export]
    private Single WinSpeed = 1f;

    /// <summary>
    /// Amount of damage the win bubble does to trees per beat
    /// </summary>
    [Export]
    private Single WinDamage = 1f;

    public override void _Ready()
    {
        CampArea = GetNodeOrNull<Area2D>($"%{nameof(CampArea)}") ?? throw new ArgumentNullException($"There is no {nameof(Area2D)} named {nameof(CampArea)}");
        WinBubble = GetNodeOrNull<Area2D>($"%{nameof(WinBubble)}") ?? throw new ArgumentNullException($"There is no {nameof(Area2D)} named {nameof(WinBubble)}");
        WinBubble.AreaEntered += Overlap;
        Sprite = GetNodeOrNull<AnimatedSprite2D>($"%{nameof(Sprite)}") ?? throw new ArgumentNullException($"There is no {nameof(AnimatedSprite2D)} named {nameof(Sprite)}");
        this.Global().Beat += OnBeat;
        this.Global().Won += Win;
    }

    private void Win(Object sender, EventArgs e)
    {
        LightFire();
    }

    private void OnBeat(Object sender, EventArgs e)
    {
        foreach(var tree in Harm.ToList())
        {
            tree.DoAHit((Int32)WinDamage);
        }
    }

    private void Overlap(Area2D area)
    {
        if (area.GetParent() is ITree tree)
        {
            tree.Felled += TreeKilled;
            Harm.Add(tree);
        }
    }

    private void TreeKilled(Object sender, Node2D e)
    {
        var tree = sender as ITree;
        GD.Print($"🔥🔥🔥🔥🔥🔥🔥🔥🔥 Get rekt {tree.TreeName} 🔥🔥🔥🔥🔥🔥🔥🔥🔥");
        Harm.Remove(tree);
        tree.Felled -= TreeKilled;
    }

    private void PrintWin() => GD.Print("🤣🤣 Congratulations, you won! 🤑🤑🤑🤑🤑 ");

    public void AllTreesDeadNow()
    {
        Winning = false;
        CallDeferred(nameof(PrintWin));        
    }

    public void LightFire()
    {
        Sprite.Frame = 1;
        Winning = true;
    }

    

    public override void _Process(Double delta)
    {

        if (Winning is true)
        {
            var scale = WinBubble.Scale.X + WinSpeed * (Single)delta;
            WinBubble.Scale = new(scale, scale);
        }
    }
}